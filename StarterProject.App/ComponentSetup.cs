using StarterProject.App.Helpers;
using StarterProject.Shared;
using SimpleInjector;

namespace StarterProject.App;

public class ComponentSetup : IComponentSetup
{
    private readonly Container _container;

    public ComponentSetup(Container container)
    {
        _container = container;
    }

    public void RegisterComponents()
    {
        // shared components
        var sharedComponentSetup = new Shared.ComponentSetup(_container);
        sharedComponentSetup.RegisterComponents();       

        // local components
        var appSettings = sharedComponentSetup.ApplicationSettings;

        // register business components
        var businessComponentSetup = new Business.ComponentSetup(_container);
        businessComponentSetup.RegisterComponents();
    }
}
