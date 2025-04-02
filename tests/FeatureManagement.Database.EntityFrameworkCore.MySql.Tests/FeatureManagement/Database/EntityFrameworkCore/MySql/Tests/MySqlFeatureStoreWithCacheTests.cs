// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.EntityFrameworkCore.Tests;

namespace FeatureManagement.Database.EntityFrameworkCore.MySql.Tests;

public sealed class MySqlFeatureStoreWithCacheTests(MySqlWithCacheIntegrationTestWebAppFactory factory)
    : EFCoreFeatureStoreTests<MySqlWithCacheIntegrationTestWebAppFactory>(factory)
{
}