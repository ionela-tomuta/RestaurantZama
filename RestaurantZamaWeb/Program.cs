using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using ZamaRestaurantWeb.Data;
using RestaurantZamaShared.Models;
using RestaurantZamaWeb.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configurare CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// DbContext configurations
builder.Services.AddDbContext<ZamaIdentityDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));
builder.Services.AddDbContext<ZamaDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RestaurantConnection")));

// Identity configuration
builder.Services.AddAuthentication();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ZamaIdentityDbContext>();

// Swagger configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Zama Restaurant API",
        Version = "v1",
        Description = "API pentru aplicația Restaurant Zama"
    });
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });
    options.OperationFilter<SecurityRequirementsOperationFilter>();
});

// JSON Serialization options
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

// Http Client Configuration
builder.Services.AddHttpClient();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Zama Restaurant API V1");
    });
}

// Enable CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();

// Exception Handling
app.UseExceptionHandler("/error");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Identity endpoints
app.MapIdentityApi<IdentityUser>();

// Controllers
app.MapControllers();

// Automatically apply migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ZamaDbContext>();
        var identityContext = services.GetRequiredService<ZamaIdentityDbContext>();
        
        context.Database.Migrate();
        identityContext.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();