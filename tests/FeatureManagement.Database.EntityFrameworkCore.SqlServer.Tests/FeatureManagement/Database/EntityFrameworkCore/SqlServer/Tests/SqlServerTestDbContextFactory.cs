// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database;
using FeatureManagement.Database.EntityFrameworkCore.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace FeatureManagement.Database.EntityFrameworkCore.SqlServer.Tests;

/// <summary>
/// Factory used to build migrations for SQL Server.
/// </summary>
public class SqlServerTestDbContextFactory : IDesignTimeDbContextFactory<TestDbContext>
{
    public TestDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("FeatureManagement/Database/EntityFrameworkCore/appsettings.json", false)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<TestDbContext>();
        var connectionString = configuration.GetConnectionString("SqlServer");

        optionsBuilder.UseSqlServer(connectionString,
            options => options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName));

        return new TestDbContext(optionsBuilder.Options);
    }
}