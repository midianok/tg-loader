using Autofac;
using System;

namespace MultiLoader.Core
{
    public class DependencyResolver
    {
        public static T ResolveAndGet<T>(Action<TypeRegister> action)
        {
            var typeRegister = new TypeRegister();
            action(typeRegister);
            var builder = typeRegister.GetBuilder();
            using (var container = builder.Build())
                return container.Resolve<T>();
        }

        public class TypeRegister
        {
            private readonly ContainerBuilder _containerBuilder;

            public TypeRegister() => _containerBuilder = new ContainerBuilder();

            public TypeRegister RegisterType<TImplementer>()
            {
                _containerBuilder.RegisterType<TImplementer>();
                return this;
            }

            public TypeRegister RegisterType<TImplementer, TService>()
            {
                _containerBuilder.RegisterType<TImplementer>().As<TService>();
                return this;
            }

            public TypeRegister RegisterType<TImplementer, TService>(string constructorParameterName, object constructorParameterValue)
            {
                _containerBuilder.RegisterType<TImplementer>().As<TService>().WithParameter(constructorParameterName, constructorParameterValue).InstancePerLifetimeScope();
                return this;
            }

            public ContainerBuilder GetBuilder() => _containerBuilder;
        }
    }
}
