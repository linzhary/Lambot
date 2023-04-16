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

            services.AddDbContext<FastLearningDbContext>(opts =>
            {
                opts.UseSqlite("Data Source=./data/database/FastLearning.db");
            }, ServiceLifetime.Scoped);
            services.AddScoped<FastLearningRepository>();

            //添加OneBot适配器
            services.RegisterAdapter<OneBotAdapter>();

            //自动注册插件列表
            services.RegisterPlugins();
        }).Build();
await host.RunAsync();