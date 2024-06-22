// Copyright (c) Matteo Ciapparelli.
// Licensed under the MIT license.

using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Bytecode;
using NHibernate.Tool.hbm2ddl;

namespace FeatureManagement.Database.NHibernate;

public class SqlServerConnectionFactory : INHibernateConnectionFactory
{
    public ISessionFactory CreateSessionFactory(string connectionString)
    {
        return Fluently.Configure()
            .Database(MsSqlConfiguration.MsSql2012
                .ConnectionString(connectionString)
                .ShowSql()
            )
            .Mappings(m =>
                m.FluentMappings
                    .AddFromAssemblyOf<FeatureMap>()
            )
            .ExposeConfiguration(cfg =>
            {
                cfg.Proxy(x => x.Validation = false);
                cfg.Proxy(x => x.ProxyFactoryFactory<StaticProxyFactoryFactory>());
                new SchemaUpdate(cfg).Execute(false, true);
            })
            .BuildSessionFactory();
    }
}