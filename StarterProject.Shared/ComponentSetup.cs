using StarterProject.Shared.Configuration;
using Microsoft.Extensions.Configuration;
using SimpleInjector;
using SimpleInjector.Diagnostics;
using System.Reflection;
using StarterProject.Shared.Extensions;

namespace StarterProject.Shared;

public class ComponentSetup : IComponentSetup
{
    private readonly Container _container;
    
    public IApplicationSettings ApplicationSettings { get; private set; }

    public ComponentSetup(Container container)
    {
        _container = container;
    }

    public void RegisterComponents()
    {
        var startupConfiguration = new ConfigurationBuilder().UseConfiguration().Build();
        ApplicationSettings = startupConfiguration.Get<ApplicationSettings>() ?? new ApplicationSettings();

        _container.RegisterInstance<IApplicationSettings>(ApplicationSettings);
    }

    // example of setting up all items of a certain type under DI
    public void RegisterItemsOfType<T>(Assembly assembly)
    {
        var types = _container.GetTypesToRegister<T>(assembly);
        
        foreach (var type in types)
        {
            var registration =
                Lifestyle.Transient.CreateRegistration(type, _container);

            registration.SuppressDiagnosticWarning(
                DiagnosticType.DisposableTransientComponent,
                "Forms should be disposed by app code; not by the container.");

            _container.AddRegistration(type, registration);
        }
    }
}
