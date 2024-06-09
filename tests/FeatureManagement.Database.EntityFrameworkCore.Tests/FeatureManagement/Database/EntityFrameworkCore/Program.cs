// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    EnvironmentName = Environments.Staging
});

var app = builder.Build();

await app.RunAsync();

public partial class Program
{
    protected Program()
    { }
}