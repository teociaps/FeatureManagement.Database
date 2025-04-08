// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database;
using FeatureManagement.Database.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Set up feature management
builder.Services.AddDatabaseFeatureManagement<FeatureStore>()
    .ConfigureDbContext<FeatureManagementDbContext>(options => options.UseInMemoryDatabase("testDb"));

var app = builder.Build();

// Seed with feature
using var scope = app.Services.CreateScope();
var dbContext = scope.ServiceProvider.GetRequiredService<FeatureManagementDbContext>();
if (!await dbContext.Features.AnyAsync())
{
    dbContext.Features.Add(new Feature
    {
        Name = WebApiApp.Features.Weather,
        Settings = [new FeatureSettings { FilterType = FeatureFilterType.Percentage, Parameters = """{ "Value": 50 }""" }]
    });
    await dbContext.SaveChangesAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await app.RunAsync();