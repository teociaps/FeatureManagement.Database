// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using System.Data;
using static Dapper.SqlMapper;

namespace FeatureManagement.Database.Dapper;

internal class GuidTypeHandler : TypeHandler<Guid>
{
    public override Guid Parse(object value)
    {
        if (value is string stringValue && Guid.TryParse(stringValue, out Guid guid))
        {
            return guid;
        }

        throw new DataException("Unable to convert database value to Guid.");
    }

    public override void SetValue(IDbDataParameter parameter, Guid value)
    {
        parameter.Value = value.ToString();
    }
}