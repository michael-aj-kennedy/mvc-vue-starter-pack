using StarterProject.Data.Configuration;
using StarterProject.Shared;
using Microsoft.Extensions.Configuration;
using SimpleInjector;
using StarterProject.Shared.Extensions;
using System.Data.Common;

namespace StarterProject.Data;

public class ComponentSetup : IComponentSetup
{
    private readonly Container _container;

    public ComponentSetup(Container container)
    {
        _container = container;
    }

    public void RegisterComponents()
    {
        var startupConfiguration = new ConfigurationBuilder().UseConfiguration().Build();
        var databaseSettings = startupConfiguration.Get<DatabaseSettings>() ?? new DatabaseSettings();

        if (!string.IsNullOrWhiteSpace(databaseSettings.ConnectionStrings.Database))
        {
            DbProviderFactories.RegisterFactory("System.Data.SqlClient", System.Data.SqlClient.SqlClientFactory.Instance);
            DbConnectionFactory.SetConnectionString(databaseSettings.ConnectionStrings.Database, "System.Data.SqlClient");
        }
    }
}
