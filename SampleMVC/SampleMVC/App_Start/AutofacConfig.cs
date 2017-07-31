using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Autofac;
using Autofac.Extras.DynamicProxy;
using Autofac.Integration.Mvc;
using Castle.DynamicProxy;
using SampleMVC.Aspects;
using SampleMVC.Interceptors;

namespace SampleMVC
{
    public class AutofacConfig
    {
        public static void Register()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<ExceptionInterceptor>();
            builder.RegisterType<CacheReaderAspect>();
            builder.RegisterType<CacheWriterAspect>();
            builder.RegisterType<CacheWritingPublisherAspect>();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.RegisterAssemblyTypes(Assembly.Load("SampleMVC.Physical"))
                .Where(x => !x.Name.EndsWith("Config", StringComparison.OrdinalIgnoreCase))
                .AsImplementedInterfaces()
                .EnableInterfaceInterceptors(
                    new ProxyGenerationOptions
                        {
                            Selector =
                                new OrderingInterceptors(
                                    typeof(CacheWriterAspect),
                                    typeof(CacheWritingPublisherAspect))
                        });

            builder.RegisterAssemblyTypes(Assembly.Load("SampleMVC.Logic"))
                .AsImplementedInterfaces()
                .EnableInterfaceInterceptors();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }

    internal class OrderingInterceptors : IInterceptorSelector
    {
        private readonly Type[] excludeds;

        public OrderingInterceptors(params Type[] excludeds)
        {
            this.excludeds = excludeds;
        }

        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            return interceptors.Where(x => this.excludeds.All(t => x.GetType() != t)).OrderBy(
                x =>
                    {
                        var order = x.GetType().GetProperty("Order");

                        return order == null ? int.MaxValue : (int)order.GetValue(x);
                    }).ToArray();
        }
    }
}