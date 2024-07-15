using Lambot.Adapters.OneBot;
using Lambot.Core;
using Lambot.Core.Plugin;
using Lambot.Template.Plugins.FastLearning;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Template.Plugins
{
    [PluginInfo(Name = "吃什么",Enabled = false)]
    public class WhatToEatPlugin : PluginBase
    {
        private readonly HashSet<long> _allowedGroups = new();
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly OneBotClient _oneBotClient;
        private readonly string _imgPath = Path.Combine(Directory.GetCurrentDirectory(), "data", "whattoeat", "images");
        private readonly Random _random = new((int)DateTimeOffset.Now.ToUnixTimeMilliseconds());
        public WhatToEatPlugin(
            IConfiguration configuration,
            OneBotClient oneBotClient,
            IHttpClientFactory httpClientFactory)
        {
            configuration.GetSection("AllowedGroups").GetChildren().ForEach(item => { _allowedGroups.Add(Convert.ToInt64(item.Value)); });
            _oneBotClient = oneBotClient;
            if (!Directory.Exists(_imgPath))
            {
                Directory.CreateDirectory(_imgPath);
            }
            _httpClientFactory = httpClientFactory;
        }

        //判断群号是否在白名单
        private bool CheckGroupPermission(long group_id)
        {
            return _allowedGroups.Contains(group_id);
        }

        [OnGroupMessage]
        [OnRegex(@"^(今天|[早中午晚][上饭餐午]|夜宵)吃(什么|啥|点啥)")]
        public async Task<Message?> GetFoodAsync(GroupMessageEvent evt)
        {
            if (!CheckGroupPermission(evt.GroupId)) return null;
            var fileNames = Directory.GetFiles(_imgPath, "*.jpg");
            if (fileNames.Length < 1) return (Message)"我还不知道有什么能吃的呢~";
            var randomIndex = _random.Next(0, fileNames.Length);
            var fileName = fileNames[randomIndex];
            var foodName = Path.GetFileNameWithoutExtension(fileName);
            var fileBytes = await File.ReadAllBytesAsync(fileName);
            var fileBase64 = Convert.ToBase64String(fileBytes);
            var message = new Message();
            message.Segments.Add(new TextMessageSeg($"吃{foodName}!{Environment.NewLine}"));
            message.Segments.Add(new ImageMessageSeg(Path.GetFileNameWithoutExtension(fileName), $"base64://{fileBase64}"));
            return message;
        }

        [OnGroupMessage]
        [OnStartWith(@"#加菜 ")]
        public async Task<string?> AddFoodAsync(GroupMessageEvent evt)
        {
            if (!CheckGroupPermission(evt.GroupId)) return null;
            if (evt.Message is null) return "不知道说的啥玩意";
            if (evt.Message.Segments.ElementAtOrDefault(0) is not TextMessageSeg seg_0)
            {
                return "不知道说的啥玩意";
            }
            var fileName = seg_0.Text.Split(" ")[1].Trim();
            var filePath = Path.Combine(_imgPath, $"{fileName}.jpg");
            if (File.Exists(filePath)) return "这个菜已经有了~";
            var imageSegment = evt.Message.Segments.ElementAtOrDefault(1);
            if (imageSegment is not ImageMessageSeg)
            {
                await _oneBotClient.SendGroupMessageAsync(evt.GroupId, "你倒是给图啊!");
                var args = await _oneBotClient.WaitGroupMessageAsync(evt.GroupId, evt.UserId);
                imageSegment = args.Segments.ElementAtOrDefault(0);
            }
            if (imageSegment is not ImageMessageSeg seg_1)
            {
                return "你给的图不对!";
            }
            var fileUrl = seg_1.Url;
            using var client = _httpClientFactory.CreateClient();
            var fileBytes = await client.GetByteArrayAsync(fileUrl);
            await File.WriteAllBytesAsync(filePath, fileBytes);
            return "添加成功~";
        }

        [OnGroupMessage]
        [OnFullMatch("怎么加菜")]
        public string? Help() => "发送 #加菜 [菜名][图片]";
    }
}
