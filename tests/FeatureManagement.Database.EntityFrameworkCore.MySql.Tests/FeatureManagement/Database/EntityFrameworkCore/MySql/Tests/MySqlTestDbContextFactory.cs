// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database;
using FeatureManagement.Database.EntityFrameworkCore.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace FeatureManagement.Database.EntityFrameworkCore.MySql.Tests;

/// <summary>
/// Factory used to build migrations for MySql.
/// </summary>
public class MySqlTestDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
{
    public TestDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("FeatureManagement/Database/EntityFrameworkCore/appsettings.json", false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
        var connectionString = configuration.GetConnectionString("MySql");

        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
            options => options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));

        return new TestDbContext(optionsBuilder.Options);
    }
}