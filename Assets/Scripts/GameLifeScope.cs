using VContainer;
using VContainer.Unity;

public class GameLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<NetworkMessagesService>(Lifetime.Singleton);
        builder.RegisterComponentInHierarchy<MyNetworkManager>();
    }
}