# Lambot （小羊）

## 基于.NET6实现的机器人框架

>使用方法参考 https://github.com/linzhary/Lambot/blob/main/Src/Lambot


    1. 支持DependencyInjection, 使用了 Microsoft.Extensions.DependencyInjection
    2. 支持Configuration, 使用了 Microsoft.Extensions.Configuration
    3. 支持LoggerFactory, 使用了 Microsoft.Extensions.Logging

基础用法
``` C#
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
```
