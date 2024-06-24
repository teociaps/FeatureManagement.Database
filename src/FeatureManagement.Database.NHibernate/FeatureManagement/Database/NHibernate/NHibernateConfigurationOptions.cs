using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;

namespace FeatureManagement.Database.NHibernate;

/// <summary>
/// Options for configuring NHibernate.
/// </summary>
public sealed class NHibernateConfigurationOptions
{
    /// <summary>
    /// The database configurer, such as a connection string.
    /// </summary>
    public IPersistenceConfigurer DatabaseConfigurer { get; set; }

    /// <summary>
    /// The action used to configure NHibernate mapping.
    /// </summary>
    public Action<MappingConfiguration> ConfigureMapping { get; set; }

    /// <summary>
    /// The action for additional generic NHibernate configuration.
    /// </summary>
    public Action<Configuration> ConfigurationSetup { get; set; }
}
