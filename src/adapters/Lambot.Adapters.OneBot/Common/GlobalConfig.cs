using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lambot.Adapters.OneBot;

internal class GlobalConfig
{
    internal static readonly JsonSerializer JsonDeserializer = JsonSerializer.Create(new JsonSerializerSettings
    {
        ContractResolver = new CamelCasePropertyNamesContractResolver
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        },
        Converters = new JsonConverter[] { new StringToEnumJsonConverter() }
    });
}
