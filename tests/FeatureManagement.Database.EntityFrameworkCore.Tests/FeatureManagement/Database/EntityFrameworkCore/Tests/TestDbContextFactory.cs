// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace FeatureManagement.Database.EntityFrameworkCore.Tests;

/// <summary>
/// Factory used to build migrations.
/// </summary>
public class TestDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
{
    public TestDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("FeatureManagement/Database/EntityFrameworkCore/appsettings.json", false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        optionsBuilder.UseSqlServer(connectionString);

        return new TestDbContext(optionsBuilder.Options);
    }
}