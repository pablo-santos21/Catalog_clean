using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text.Json.Serialization;
using Catalog.Infrastructure.DependencyInjection;
using Catalog.API.Filters;


var builder = WebApplication.CreateBuilder(args);

//Conf Controllers e JSON
builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ApiExceptionFilter));
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

//Conf Cors
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:xxxx").AllowAnyMethod();
        })
);

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        Description = "Bearer JWT",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
      {
        {
          new OpenApiSecurityScheme
          {
            Reference = new OpenApiReference
              {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
              },
            },
            new string[] {}
          }
        });

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);

    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Tesouros da Terra",
        Description = "Documentação da API do nosso catalogo store.",
        Contact = new OpenApiContact
        {
            Name = "Pablo Santos",
            Email = "contato@pablosantos.dev",
            Url = new Uri("https://pablosantos.dev")
        }
    });
});


//Services
builder.Services.AddConfigureInfrastructure(builder.Configuration)
                .AddConfigureIdentity()
                .AddJwtConfigure(builder.Configuration)
                .AddConfigureRateLimiting(builder.Configuration)
                .AddConfigureAutoMapper()
                .AddRepositories();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    //app.ConfigureExceptionHandler();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseCors();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
