using Osint.Application;
using Osint.Infrastructure;
using Osint.Logging;
using Osint.Mapper;
using Osint.Validator;

var builder = WebApplication.CreateBuilder(args);

// Sin AddPersistence/connection string y sin AddSecurity/JWT por ahora —
// decisión registrada en .claude/skill-decisions.md (ver _plan/plan-trabajo.md §0).
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddMapper();
builder.Services.AddValidator();
builder.Services.AddAppLogging();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
        policy.WithOrigins(allowedOrigins).AllowAnyHeader().AllowAnyMethod());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.OpenApiInfo
    {
        Title = "Osint Dashboard API",
        Version = "v1",
        Description = "Orquestador de búsquedas OSINT (PhoneInfoga, Holehe, Maigret, theHarvester, SpiderFoot). " +
                      "Modo básico (POST /api/search) y modo avanzado multi-dato (POST /api/search/advanced)."
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
if (!app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCors("CorsPolicy");
app.MapControllers();
app.Run();
