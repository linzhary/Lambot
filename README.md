# Lambot （小羊）

## 基于 [Microsoft.Extensions.Hosting 7.0.1] 泛型主机实现的机器人框架

>使用方法参考 https://github.com/linzhary/Lambot/blob/main/Src/Lambot.Template

基础用法
``` C#
using Lambot.Adapters.OneBot;
using Lambot.Core;
using Lambot.Template.Plugins.FastLearning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = LambotHost.CreateDefaultBuilder(args)
        .ConfigureServices(services =>
        {
            Directory.CreateDirectory("./data/database");
            Directory.CreateDirectory("./data/images");

            services.AddDbContextPool<FastLearningDbContext>(opts =>
            {
                opts.UseSqlite("Data Source=./data/database/FastLearning.db");
            });
            services.AddScoped<FastLearningRepository>();

            //添加OneBot适配器
            services.RegisterAdapter<OneBotAdapter>();

            //自动注册插件列表
            services.RegisterPlugins();
        }).Build();
await host.RunAsync();
```
