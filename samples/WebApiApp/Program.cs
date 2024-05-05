// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database;
using Microsoft.Extensions.Options;
using WebApiApp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Set up feature management
builder.Services.AddDatabaseFeatureManagement<FeatureStore>(useCache: false); //set 'true' to use cache (with default values)

//builder.Services.AddOptions<FeatureCacheOptions>()
//            .Bind(builder.Configuration.GetSection($"FeatureManagement:{FeatureCacheOptions.Name}"));

var app = builder.Build();

//var options = app.Services.GetRequiredService<IOptions<FeatureCacheOptions>>().Value;

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