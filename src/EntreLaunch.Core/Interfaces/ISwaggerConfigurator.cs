using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace EntreLaunch.Interfaces;

public interface ISwaggerConfigurator
{
    void ConfigureSwagger(SwaggerGenOptions options, OpenApiInfo settings);
}
