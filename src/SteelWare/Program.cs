using SteelWare.Application.Extensions;
using SteelWare.Infrastructure.DataAccess.Extensions;
using SteelWare.Presentation.Http.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSteelWareHttpPresentation();
builder.Services.AddSteelWareApplication();
builder.Services.AddSteelWareDataAccess(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await app.Services.InitializeSteelWareDataAccessAsync();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

app.MapControllers();

app.Run();
