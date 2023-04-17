using Lambot.Adapters.OneBot;
using Lambot.Template.Plugins.FastLearning.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Lambot.Template.Plugins.FastLearning;

public class FastLearningRepository
{
    //private static readonly bool _debug = false;

    private static long UnionId(long group_id, long? user_id)
    {
        return (group_id << 16) | (user_id ?? 0);
    }

    private static string TrimMessage(string raw_msg)
    {
        return raw_msg.Trim(' ', '\r', '\n').Replace("\r", string.Empty);
    }

    private static async Task<string> DownloadImageAsync(string fileName, string url)
    {
        var file = Path.Combine("./data/images", fileName);
        if (!File.Exists(file))
        {
            try
            {
                using var client = new HttpClient();
                File.WriteAllBytes(file, await client.GetByteArrayAsync(url));
            }
            catch
            {
                return null;
            }
        }
        return file;
    }

    private static string TrimCQImage(string raw_msg, bool resolve_file, bool download = true)
    {
        var message = Message.Parse(raw_msg);
        var index = 0;
        message.Segments.ForEach(async seg =>
        {
            if (seg is not ImageMessageSeg) return;
            var img_seg = seg as ImageMessageSeg;
            if (string.IsNullOrWhiteSpace(img_seg.File)) return;
            if (download)
            {
                if (img_seg.Url is null) return;
                var file = await DownloadImageAsync(img_seg.File, img_seg.Url);
                if (file is null && resolve_file)
                {
                    message.Segments[index] = new TextMessageSeg { Text = $"[FileNotFound:{img_seg.File}]" };
                }
                else if (resolve_file)
                {
                    img_seg.File = $"file:///{Path.GetFullPath(file).Replace('\\', '/')}";
                    //if (_debug)
                    //{
                    //    img_seg.File = img_seg.File.Replace("D:/Workspace/Lambot/src/Lambot.Template/bin/Debug/net6.0/data/images", "home/bot");
                    //}
                }
            }
            img_seg.Url = null;
        });
        return message.ToString();
    }

    private readonly ConcurrentDictionary<string, Dictionary<long, string>> _cache = new();

    private readonly FastLearningDbContext _dbContext;

    public FastLearningRepository(FastLearningDbContext dbContext)
    {
        _dbContext = dbContext;
        var records = dbContext.Records.ToList();
        records.ForEach(async item =>
        {
            await Add(item.Question, item.Answer, item.GroupId, item.UserId);
        });
    }

    private static (string, string) BeforAdd(string question, string answer)
    {
        question = TrimMessage(question);
        question = TrimCQImage(question, false);

        answer = TrimMessage(answer);
        answer = TrimCQImage(answer, true);
        return (question, answer);
    }

    public async Task Add(string question, string answer, long group_id, long user_id)
    {
        (question, answer) = BeforAdd(question, answer);
        var unionId = UnionId(group_id, user_id);

        var answers = _cache.GetOrAdd(question, k => new());
        answers.TryAdd(unionId, answer);
        var entity = await _dbContext.Records
            .Where(x => x.Question == question)
            .Where(x => x.GroupId == group_id)
            .Where(x => x.UserId == user_id)
            .FirstOrDefaultAsync();
        if (entity == default)
        {
            entity = new FastLearningRecord
            {
                Question = question,
                Answer = answer,
                GroupId = group_id,
                UserId = user_id,
                Time = DateTimeOffset.UtcNow,
            };
            await _dbContext.AddAsync(entity);
        }
        else
        {
            entity.Answer = answer;
        }
        await _dbContext.SaveChangesAsync();
    }

    private static string BeforMatch(string question)
    {
        question = TrimMessage(question);
        question = TrimCQImage(question, false, false);
        return question;
    }

    public (bool, string) MatchText(string question, long group_id, long user_id)
    {
        question = BeforMatch(question);
        var answers = _cache.GetValueOrDefault(question);
        var answer = MatchAnswer(answers, group_id, user_id);
        return (answer != default, answer);
    }

    public (bool, string) MatchRegex(string question, long group_id, long user_id)
    {
        foreach (var item in _cache)
        {
            var pattern = $"^{item.Key}$";
            var match = Regex.Match(question, pattern);
            if (!match.Success) continue;
            var answer = MatchAnswer(item.Value, group_id, user_id);
            if (answer != default)
            {
                return (true, match.Result(answer));
            }
        }
        return (false, null);
    }

    public static string MatchAnswer(Dictionary<long, string> answers, long group_id, long user_id)
    {
        if (answers is null) return default;
        var union_id = UnionId(group_id, user_id);
        var answer = answers.GetValueOrDefault(union_id);
        if (answer == default)
        {
            union_id = UnionId(group_id, 0);
            answer = answers.GetValueOrDefault(union_id);
        }
        return answer;
    }
}