﻿// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using ConsoleApp;
using ConsoleApp.FeatureFilters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

// Setup configuration
IConfiguration configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

// Setup application services + feature management
IServiceCollection services = new ServiceCollection();

services.AddSingleton(configuration)
        .AddDatabaseFeatureManagement<FeatureStore>() //set 'true' to use cache
        .AddFeatureFilter<UsernameFilter>();

// Get the feature manager from application services
using (ServiceProvider serviceProvider = services.BuildServiceProvider())
{
    IFeatureManager featureManager = serviceProvider.GetRequiredService<IFeatureManager>();

    var usernames = new List<string>()
    {
        "Paul",
        "Lara",
        "Matthew"
    };

    // Mimic work items in a task-driven console application
    foreach (var username in usernames)
    {
        // Check if feature enabled
        var usernameContext = new UsernameContext
        {
            Username = username
        };

        bool enabled = await featureManager.IsEnabledAsync(Features.Beta, usernameContext);

        // Output results
        Console.WriteLine($"The {Features.Beta} feature is {(enabled ? "enabled" : "disabled")} for the user '{username}'.");
    }
}