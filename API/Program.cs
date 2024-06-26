using API.Data;
using API.Extensions;
using API.Middlewares;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);

var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors(builder => builder.AllowAnyHeader().AllowAnyOrigin().
    WithOrigins("https://localhost:4200"));
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<DataContext>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(context);
}
catch (Exception ex)
{
    var logger = services.GetService<ILogger<Program>>();
    logger.LogError(ex, "Error ocurred during migration");
}
app.Run();