using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace ImageCropper.Api.Extensions;

public class SwaggerFileOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var hasFileParameter = context.MethodInfo.GetParameters()
            .Any(p => p.ParameterType == typeof(IFormFile) ||
                      p.ParameterType == typeof(IFormFile[]) ||
                      p.ParameterType.GetProperties().Any(prop =>
                          prop.PropertyType == typeof(IFormFile) ||
                          prop.PropertyType == typeof(IFormFile[])));

        if (hasFileParameter)
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new()
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = new Dictionary<string, OpenApiSchema>
                            {
                                ["image"] = new()
                                {
                                    Type = "string",
                                    Format = "binary",
                                    Description = "PNG image file"
                                },
                                ["logoImage"] = new()
                                {
                                    Type = "string",
                                    Format = "binary",
                                    Description = "PNG logo image file"
                                }
                            }
                        }
                    }
                }
            };
    }
}