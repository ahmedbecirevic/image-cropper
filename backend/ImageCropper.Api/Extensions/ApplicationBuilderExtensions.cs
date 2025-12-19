using ImageCropper.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace ImageCropper.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static void UseSwaggerDocumentation(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (!env.IsDevelopment()) return;
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Image Cropper API v1");
            c.RoutePrefix = "swagger";
            c.DocumentTitle = "Image Cropper API Documentation";
        });
    }

    public static void UseCorsPolicy(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseCors(env.IsDevelopment() ? "Development" : "AllowAll");
    }

    public static async Task RunDatabaseMigrationsAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync();
    }
}