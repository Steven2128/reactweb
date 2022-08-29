using Api.Data;
using Api.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddDbContext<HouseDbContext>(o => 
    o.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
builder.Services.AddScoped<IHouseRepository, HouseRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(p => p.WithOrigins("http://localhost:3000")
    .AllowAnyHeader().AllowAnyMethod());

app.UseHttpsRedirection();

//List Houses
app.MapGet("/houses", (IHouseRepository repo) => repo.GetAll()).Produces<HouseDto>(StatusCodes.Status200OK);

//Detail House
app.MapGet("/houses/{houseId:int}", async (int houseId, IHouseRepository repo) => 
{
    var house = await repo.Get(houseId);
    if (house == null)
    {
        return Results.Problem($"House with ID {houseId} not found.", statusCode: 404);
    }
    return Results.Ok(house);
}).ProducesProblem(StatusCodes.Status404NotFound).Produces<HouseDetailDto>(StatusCodes.Status200OK);

//Create House
app.MapPost("/houses", async ([FromBody]HouseDetailDto dto, IHouseRepository repo) =>
{
    var newHouse = repo.Add(dto);
    return Results.Created($"/house/{newHouse.Id}", newHouse);
}).Produces<HouseDetailDto>(StatusCodes.Status201Created);

//Update House
app.MapPut("/houses", async ([FromBody] HouseDetailDto dto, IHouseRepository repo) =>
{
    if (await repo.Get(dto.Id) == null)
    {
        return Results.Problem($"House with ID {dto.Id} not found.", statusCode: 404);
    }
    var house = await repo.Update(dto);
    return Results.Ok(house);
}).Produces<HouseDetailDto>(StatusCodes.Status200OK);

//Delete House
app.MapDelete("/houses/{houseId:int}", async (int houseId, IHouseRepository repo) =>
{
    if (await repo.Get(houseId) == null)
    {
        return Results.Problem($"House with ID {houseId} not found.", statusCode: 404);
    }
    await repo.Delete(houseId);
    return Results.Ok();
}).Produces<HouseDetailDto>(StatusCodes.Status200OK);

app.Run();
