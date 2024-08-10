using TeachPlanner.Api;
using TeachPlanner.Api.Extensions.DependencyInjection;
using TeachPlanner.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Services
    .AddInfrastructure(builder.Configuration)
    .AddApplication();

builder.Services.AddCors(options =>
{
    options.AddPolicy("wasm",
      p =>
      {
          p.AllowAnyHeader();
          p.AllowAnyMethod();
          p.AllowAnyHeader();
          p.AllowAnyOrigin();
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
app.UseCors("wasm");

app.UseAuthentication();
app.UseAuthorization();

app.MapApi();

app.Run();
