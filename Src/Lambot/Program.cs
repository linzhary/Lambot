using Lambot.Adapters.OneBot;
using Lambot.Core;
using Lambot.Plugin;

var builder = LambotHost.CreateBuilder();

//添加OneBot适配器
builder.RegisterAdapter<OneBotAdapter>();
//自动注册插件列表
builder.RegisterPlugins();
//连接Websocket服务
builder.Build().Run("localhost:62912");