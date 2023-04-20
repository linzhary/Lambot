using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Lambot.Core.Adapter;

public interface IAdapter
{
    string AdapterName { get; }
    void OnConfigureService(IServiceCollection services);
    void OnBuild(WebApplication app);
}