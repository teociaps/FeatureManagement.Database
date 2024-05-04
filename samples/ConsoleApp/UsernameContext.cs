// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using ConsoleApp.FeatureFilters;

namespace ConsoleApp;

internal class UsernameContext : IUsernameContext
{
    public required string Username { get; set; }
}