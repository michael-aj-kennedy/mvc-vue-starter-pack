using StarterProject.Shared;
using SimpleInjector;

namespace StarterProject.Business;

public class ComponentSetup : IComponentSetup
{
    private readonly Container _container;

    public ComponentSetup(Container container)
    {
        _container = container;
    }

    public void RegisterComponents()
    {
        var dataComponentSetup = new Data.ComponentSetup(_container);
        dataComponentSetup.RegisterComponents();
    }
}
