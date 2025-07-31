using ExampleLibrary;

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
                Description = "This Api Microservice is for demonstration purposed only.",
                //TermsOfService = new Uri("https://blazorui20230314133145.azurewebsites.net/privacy-policy"),
                Contact = new OpenApiContact
                {
                    Name = "API Support",
                    Email = "lazer8701@gmail.com",
                   // Url = new Uri("https://blazorui20230314133145.azurewebsites.net")
                }
            });
            c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
        });
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowSpecificOrigin",
                build => build
                    .WithOrigins("https://localhost:7007", "https://localhost:5173") // Allow only this origin can be changed to allow multiple origins with a list of strings
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });


        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
        builder.Services.AddSingleton<IConfiguration>(configuration);

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


        builder.Services.AddScoped<IEmployee, Employee>();

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