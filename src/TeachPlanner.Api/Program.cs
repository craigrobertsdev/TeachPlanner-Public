using TeachPlanner.Api;
using TeachPlanner.Api.DependencyInjection;
using TeachPlanner.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

const string corsPolicyName = "web_client";

builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicyName, p =>
    {
        var webClient = builder.Configuration["Endpoints:WebClient"];
        if (webClient is not null)
        {
            p.WithOrigins(webClient).AllowAnyHeader().AllowAnyMethod();
        }
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
    app.UseWebAssemblyDebugging();
}

app.UseErrorHandlingMiddleware();

app.UseHttpsRedirection();
app.UseRouting();

// enable cors
app.UseCors(corsPolicyName);

app.UseAuthentication();
app.UseAuthorization();

app.MapApi();

app.Run();

// For integration test visibility
public partial class Program { }