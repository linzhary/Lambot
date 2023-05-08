using Lambot.Adapters.OneBot;
using Lambot.Template.Plugins.FastLearning.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Lambot.Template.Plugins.FastLearning;

public class FastLearningRepository
{
    private static long UnionId(long group_id, long? user_id)
    {
        return (group_id << 16) | (user_id ?? 0);
    }

    private static string TrimMessage(string raw_msg)
    {
        return raw_msg.Trim(' ', '\r', '\n').Replace("\r", string.Empty);
    }


    private static bool CheckQuestion(string question)
    {
        if (string.IsNullOrWhiteSpace(question)) return false;
        var trimed_str = question.Trim(".*+?()[]{,}".ToArray());
        return !string.IsNullOrEmpty(trimed_str);
    }

    private static async Task DownloadImageAsync(string fileName, string url)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return;
        if (string.IsNullOrWhiteSpace(url)) return;

        var file = ResolveImage(fileName);
        if (!File.Exists(file))
        {
            try
            {
                using var client = new HttpClient();
                File.WriteAllBytes(file, await client.GetByteArrayAsync(url));
            }
            catch
            {

            }
        }
    }
    private static string ResolveImage(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName)) return fileName;
        return "file://" + Path.GetFullPath($"./data/images/{fileName}").Replace('\\', '/');
    }

    private static string TrimCQImage(Message message, bool download = true)
    {
        message.Segments.ForEach(async seg =>
        {
            if (seg is not ImageMessageSeg img_seg) return;
            if (string.IsNullOrWhiteSpace(img_seg.File)) return;
            if (download && img_seg.Url is not null)
            {
                await DownloadImageAsync(img_seg.File, img_seg.Url);
            }
            img_seg.Url = null;
        });
        return (string)message;
    }

    private static string ParseCQImage(Message message)
    {
        message.Segments.ForEach(seg =>
        {
            if (seg is not ImageMessageSeg img_seg) return;
            img_seg.File = ResolveImage(img_seg.File);
        });
        return (string)message;
    }

    private readonly ConcurrentDictionary<string, ConcurrentDictionary<long, string>> _cache = new();

    private readonly FastLearningDbContext _dbContext;

    public FastLearningRepository(FastLearningDbContext dbContext)
    {
        _dbContext = dbContext;
        var records = dbContext.Records.ToList();
        records.ForEach(async item =>
        {
            await AddAsync(item.Question, item.Answer, item.GroupId, item.UserId);
        });
    }

    private static (string, string) BeforAdd(string question, string answer)
    {
        question = TrimMessage(question);
        question = TrimCQImage((Message)question);

        answer = TrimMessage(answer);
        answer = TrimCQImage((Message)answer);
        return (question, answer);
    }

    public async Task<string> AddAsync(string question, string answer, long group_id, long user_id)
    {
        if (!CheckQuestion(question)) return "暂不支持的添加方式~";
        (question, answer) = BeforAdd(question, answer);
        var unionId = UnionId(group_id, user_id);

        var answers = _cache.GetOrAdd(question, k => new());
        answers.AddOrUpdate(unionId, answer, (k, v) => answer);
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
        return "我已经记住了~";
    }

    private static string BeforMatch(string question)
    {
        question = TrimMessage(question);
        question = TrimCQImage((Message)question, false);
        return question;
    }

    public string? MatchText(string question, long group_id, long user_id)
    {
        question = BeforMatch(question);
        var answers = _cache.GetValueOrDefault(question);
        return MatchAnswer(answers, group_id, user_id);
    }

    public string? MatchRegex(string question, long group_id, long user_id)
    {
        foreach (var item in _cache)
        {
            var pattern = $"^{item.Key}$";
            var match = Regex.Match(question, pattern);
            if (!match.Success) continue;
            var answer = MatchAnswer(item.Value, group_id, user_id);
            if (answer != default)
            {
                return match.Result(answer);
            }
        }
        return default;
    }

    private static string? MatchAnswer(IDictionary<long, string>? answers, long group_id, long user_id)
    {
        if (answers is null) return default;
        var union_id = UnionId(group_id, user_id);
        string? answer;
        if (!answers.TryGetValue(union_id, out answer))
        {
            union_id = UnionId(group_id, 0);
            answers.TryGetValue(union_id, out answer);
        }
        return answer is null ? answer : ParseCQImage((Message)answer);
    }

    public async Task<string> DelAsync(string question, long group_id, long user_id)
    {
        question = BeforMatch(question);
        var answers = _cache.GetValueOrDefault(question);
        if (answers is null) return "没有找到这个问题~";
        var union_id = UnionId(group_id, user_id);
        var answer = answers.GetValueOrDefault(union_id);
        if (answer == default)
        {
            return "没有找到这个问题~";
        }
        answers.Remove(union_id, out var _);
        await _dbContext.Records
        .Where(x => x.Question == question)
        .Where(x => x.GroupId == group_id)
        .Where(x => x.UserId == user_id)
        .ExecuteDeleteAsync();
        return $"我不再回答{ParseCQImage((Message)answer)}了~";
    }

    public async Task<List<FastLearningRecord>> ListAsync(long group_id, long user_id)
    {
        var result = await _dbContext.Records
        .Where(x => x.GroupId == group_id)
        .Where(x => x.UserId == user_id)
        .ToListAsync();
        result.ForEach(item =>
        {
            item.Question = ParseCQImage((Message)item.Question)!;
            item.Answer = ParseCQImage((Message)item.Answer)!;
        });
        return result;
    }
}