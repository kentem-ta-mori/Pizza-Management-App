using ContosoPizza.Interfaces;
using ContosoPizza.Repositories.Mocks;
using ContosoPizza.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IToppingRepository, MockToppingRepository>();
builder.Services.AddScoped<IBasePizzaRepository, MockBasePizzaRepository>();
// インメモリのモックリポジトリであるためシングルトンで注入
builder.Services.AddSingleton<IOrderedMenuRepository, MockOrderedMenuRepository>();
builder.Services.AddScoped<IPizzaSuggester, PizzaSuggester>();
builder.Services.AddScoped<IPizzaService, PizzaService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
