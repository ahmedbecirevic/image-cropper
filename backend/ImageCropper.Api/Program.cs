using ImageCropper.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddApplicationServices();
builder.Services.AddAuthentication(builder.Configuration);
builder.Services.AddSwaggerDocumentation();
builder.Services.AddCorsPolicy();

var app = builder.Build();

// Configure the HTTP request pipeline
app.UseSwaggerDocumentation(app.Environment);
app.UseHttpsRedirection();
app.UseCorsPolicy(app.Environment);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Initialize database
await app.RunDatabaseMigrationsAsync();

app.Run();