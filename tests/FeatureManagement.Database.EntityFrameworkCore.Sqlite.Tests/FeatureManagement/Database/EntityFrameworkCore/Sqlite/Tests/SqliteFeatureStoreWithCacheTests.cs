// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.EntityFrameworkCore.Tests;

namespace FeatureManagement.Database.EntityFrameworkCore.Sqlite.Tests;

public sealed class SqliteFeatureStoreWithCacheTests(SqliteWithCacheIntegrationTestWebAppFactory factory)
    : EFCoreFeatureStoreTests<SqliteWithCacheIntegrationTestWebAppFactory>(factory)
{
}