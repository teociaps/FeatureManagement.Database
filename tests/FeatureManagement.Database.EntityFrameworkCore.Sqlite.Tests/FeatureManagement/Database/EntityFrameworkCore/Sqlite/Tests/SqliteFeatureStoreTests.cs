// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.EntityFrameworkCore.Tests;

namespace FeatureManagement.Database.EntityFrameworkCore.Sqlite.Tests;

public sealed class SqliteFeatureStoreTests(SqliteIntegrationTestWebAppFactory factory)
    : EFCoreFeatureStoreTests<SqliteIntegrationTestWebAppFactory>(factory)
{
}