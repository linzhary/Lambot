using Microsoft.Extensions.DependencyInjection;

namespace Lambot.Core;

public interface IAdapter
{
    string AdapterName { get; }

    IServiceCollection ConfigureService(IServiceCollection services);
}