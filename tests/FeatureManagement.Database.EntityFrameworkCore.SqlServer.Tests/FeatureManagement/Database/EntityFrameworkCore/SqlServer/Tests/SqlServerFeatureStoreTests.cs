// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.EntityFrameworkCore.Tests;

namespace FeatureManagement.Database.EntityFrameworkCore.SqlServer.Tests;

public sealed class SqlServerFeatureStoreTests(SqlServerIntegrationTestWebAppFactory factory)
    : EFCoreFeatureStoreTests<SqlServerIntegrationTestWebAppFactory>(factory)
{
}