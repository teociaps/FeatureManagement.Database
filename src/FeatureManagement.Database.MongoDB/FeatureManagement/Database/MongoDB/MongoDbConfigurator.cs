// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using MongoDB.Bson.Serialization.Serializers;

namespace FeatureManagement.Database.MongoDB;

/// <summary>
/// Provides methods for configuring MongoDB class mappings.
/// </summary>
internal static class MongoDBConfigurator
{
    /// <summary>
    /// Registers class maps for collections.
    /// </summary>
    internal static void RegisterClassMaps()
    {
        BsonClassMap.RegisterClassMap<Feature>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(f => f.Id)
              .SetSerializer(new GuidSerializer(BsonType.String));
            cm.MapMember(f => f.Name).SetIsRequired(true);
            cm.MapMember(f => f.RequirementType).SetIsRequired(true);
            cm.MapMember(f => f.Settings);
        });

        BsonClassMap.RegisterClassMap<FeatureSettings>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(fs => fs.Id)
              .SetSerializer(new GuidSerializer(BsonType.String))
              .SetIdGenerator(CombGuidGenerator.Instance);
            cm.MapMember(fs => fs.FilterType).SetIsRequired(true);
            cm.MapMember(fs => fs.Parameters).SetIsRequired(true);
            cm.MapMember(fs => fs.FeatureId).SetSerializer(new GuidSerializer(BsonType.String));
        });
    }
}