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
internal static class MongoDbConfigurator
{
    /// <summary>
    /// Registers class maps for collections.
    /// </summary>
    internal static void RegisterClassMaps()
    {
        BsonClassMap.RegisterClassMap<Feature>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(c => c.Id)
              .SetSerializer(new GuidSerializer(BsonType.String));
            cm.MapMember(c => c.Name).SetIsRequired(true);
            cm.MapMember(c => c.RequirementType).SetIsRequired(true);
            cm.MapMember(c => c.Settings);
        });

        BsonClassMap.RegisterClassMap<FeatureSettings>(cm =>
        {
            cm.AutoMap();
            cm.MapIdMember(c => c.Id)
              .SetSerializer(new GuidSerializer(BsonType.String))
              .SetIdGenerator(CombGuidGenerator.Instance);
            cm.MapMember(c => c.FilterType).SetIsRequired(true);
            cm.MapMember(c => c.Parameters).SetIsRequired(true);
            cm.MapMember(c => c.FeatureId).SetSerializer(new GuidSerializer(BsonType.String));
        });
    }
}