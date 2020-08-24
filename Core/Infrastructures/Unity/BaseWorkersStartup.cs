using System;
using Unity;
using Unity.Lifetime;

namespace TGS.Infrastructures.Unity
{
    public abstract class BaseFactory
    {
        protected BaseFactory()
        {
            Container = new UnityContainer();
            RegisterDefaultImplementations();
        }

        public IUnityContainer Container { get; }

        private void RegisterDefaultImplementations()
        {
            Container.RegisterFactory<Func<string>>("Prefix", c => new Func<string>(() => LoggingPrefix));
            SetupDefaultImplementations(Container);
        }

        protected abstract void SetupDefaultImplementations(IUnityContainer container);
        protected abstract string LoggingPrefix { get; }

        protected static ContainerControlledLifetimeManager ReuseWithinContainer =>
            new ContainerControlledLifetimeManager();

        protected static PerResolveLifetimeManager ReuseWithinResolve => new PerResolveLifetimeManager();
        protected static TransientLifetimeManager NewEachTime => new TransientLifetimeManager();
        protected static HierarchicalLifetimeManager Hierarchical => new HierarchicalLifetimeManager();
    }
}