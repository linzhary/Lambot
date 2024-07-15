using Lambot.Adapters.OneBot;
using Lambot.Core.Adapter;
using Lambot.Core.Hosting;
using Lambot.Core.Plugin;
using Lambot.Template.Database;
using Lambot.Template.Plugins.FastLearning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = LambotApplication.CreateBuilder(args);

Directory.CreateDirectory("./data/database");
Directory.CreateDirectory("./data/images");

builder.Services.AddDbContextPool<LambotDbContext>(opts =>
{
    opts.UseSqlite("Data Source=./data/database/FastLearning.db");
});
builder.Services.AddScoped<FastLearningRepository>();

//添加OneBot适配器
builder.Services.RegisterAdapter<OneBotAdapter>();

//自动注册插件列表
builder.Services.RegisterPlugins();

await builder.Build().RunAsync("http://[*]:9527");