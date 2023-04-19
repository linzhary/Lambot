# Lambot （小羊）

## 基于 ASP.NET Core Web 主机实现的机器人框架

>使用方法参考 [https://github.com/linzhary/Lambot/tree/main/src/Lambot.Template]

基础用法
``` C#
using Lambot.Adapters.OneBot;
using Lambot.Core;
using Lambot.Core.Adapter;
using Lambot.Core.Plugin;
using Lambot.Template.Plugins.FastLearning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = LambotApplication.CreateBuilder(args);

Directory.CreateDirectory("./data/database");
Directory.CreateDirectory("./data/images");

builder.Services.AddDbContextPool<FastLearningDbContext>(opts =>
{
    opts.UseSqlite("Data Source=./data/database/FastLearning.db");
});
builder.Services.AddScoped<FastLearningRepository>();

//添加OneBot适配器
builder.Services.RegisterAdapter<OneBotAdapter>();

//自动注册插件列表
builder.Services.RegisterPlugins();

await builder.Build().RunAsync("http://127.0.0.1:9527");
```
