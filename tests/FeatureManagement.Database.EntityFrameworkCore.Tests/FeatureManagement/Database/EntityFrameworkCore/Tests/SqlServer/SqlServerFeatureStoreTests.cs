// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

namespace FeatureManagement.Database.EntityFrameworkCore.Tests.SqlServer;

public sealed class SqlServerFeatureStoreTests(SqlServerIntegrationTestWebAppFactory factory)
    : EFCoreFeatureStoreTests<SqlServerIntegrationTestWebAppFactory>(factory)
{
}