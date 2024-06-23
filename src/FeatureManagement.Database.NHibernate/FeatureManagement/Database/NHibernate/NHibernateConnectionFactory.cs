// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FluentNHibernate.Cfg;
using NHibernate;
using NHibernate.Bytecode;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

namespace FeatureManagement.Database.NHibernate;

/// <summary>
/// Default implementation of <see cref="INHibernateConnectionFactory"/> for creating NHibernate session factories.
/// </summary>
public class NHibernateConnectionFactory : INHibernateConnectionFactory
{
    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">Thrown to indicate that the method should be extended and implemented.</exception>
    public virtual ISessionFactory CreateSessionFactory(string connectionString)
    {
        throw new NotImplementedException("Extend NHibernateConnectionFactory to implement this method.");
    }

    /// <inheritdoc/>
    public ISessionFactory CreateSessionFactory(string connectionString, DatabaseType databaseType)
    {
        return Fluently.Configure()
            .Database(DatabaseConfigurer.ConfigureDatabase(connectionString, databaseType))
            .Mappings(m => m.FluentMappings.AddFromAssemblyOf<FeatureMap>())
            .ExposeConfiguration(ConfigureSchema)
            .BuildSessionFactory();
    }

    /// <inheritdoc/>
    public ISessionFactory CreateSessionFactory(Action<NHibernateConfigurationOptions> configureOptions)
    {
        var options = new NHibernateConfigurationOptions()
        {
            ConfigureMapping = m => m.FluentMappings.AddFromAssemblyOf<FeatureMap>()
        };

        configureOptions.Invoke(options);

        return Fluently.Configure()
            .Database(options.DatabaseConfigurer)
            .Mappings(options.ConfigureMapping)
            .ExposeConfiguration(cfg =>
            {
                options.ConfigurationSetup.Invoke(cfg);
                ConfigureSchema(cfg);
            })
            .BuildSessionFactory();
    }

    private static void ConfigureSchema(Configuration cfg)
    {
        cfg.Proxy(p => p.Validation = false);
        cfg.Proxy(p => p.ProxyFactoryFactory<StaticProxyFactoryFactory>());
        new SchemaUpdate(cfg).Execute(false, true);
    }
}