// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FeatureManagement.Database.EntityFrameworkCore.Tests;

namespace FeatureManagement.Database.EntityFrameworkCore.PostgreSQL.Tests;

public sealed class PostgreSqlFeatureStoreWithCacheTests(PostgreSqlWithCacheIntegrationTestWebAppFactory factory)
    : EFCoreFeatureStoreTests<PostgreSqlWithCacheIntegrationTestWebAppFactory>(factory)
{
}