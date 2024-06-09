// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database;
using FeatureManagement.Database.EntityFrameworkCore.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace FeatureManagement.Database.EntityFrameworkCore.Sqlite.Tests;

/// <summary>
/// Factory used to build migrations for Sqlite.
/// </summary>
public class SqliteTestDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
{
    public TestDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("FeatureManagement/Database/EntityFrameworkCore/appsettings.json", false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
        var connectionString = configuration.GetConnectionString("Sqlite");

        optionsBuilder.UseSqlite(connectionString,
            options => options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));

        return new TestDbContext(optionsBuilder.Options);
    }
}