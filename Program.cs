using ExampleLibrary;
using WebApi.Middleware;
using static System.Collections.Specialized.BitVector32;

namespace WebApi;

public static class Program
{
    public static void Main(string[] args)
    {
        // Create the builder and services
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Web API",
                Description = "This Api is for demonstration purposed only.",
                //TermsOfService = new Uri("https://ctate-gbhta6fedebjfcf9.canadaeast-01.azurewebsites.net/privacy-policy"),
                Contact = new OpenApiContact
                {
                    Name = "API Support",
                    Email = "xXClearwaterXx@gmail.com",
                    Url = new Uri("https://ctate-gbhta6fedebjfcf9.canadaeast-01.azurewebsites.net/")
                }
            });
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
        });

        var configuration = builder.Configuration;
        var origins = configuration.GetSection("AllowedOrigins");
        var allowedOriginList = new List<string>();
        var i = 0;
        while (true)
        {
            var value = origins.GetSection(i.ToString()).Value;
            if (string.IsNullOrEmpty(value))
            {
                break;
            }
            allowedOriginList.Add(value);
            i++;
        }
        var allowedOrigins = allowedOriginList.ToArray();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                build => build
                    .WithOrigins(allowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });


        

        // Only reference the logic layer all Dependency Injection for this library is done here.
        builder.Services.AddExampleLibrary(
            configuration.GetConnectionString("DefaultConnection")
        );
        // Add services to the container.
        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseMiddleware<ModelStateValidationMiddleware>();

        // Enable CORS
        app.UseCors("AllowSpecificOrigin");

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        // Run the application.
        app.Run();
    }
}