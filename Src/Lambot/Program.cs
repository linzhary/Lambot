using Lambot.Adapters.OneBot;
using Lambot.Core;
using Lambot.Plugin;

var builder = LambotHost.CreateBuilder();

builder.RegisterAdapter<OneBotAdapter>();
builder.RegisterPlugins();

builder.Build().Run("localhost:62912");


