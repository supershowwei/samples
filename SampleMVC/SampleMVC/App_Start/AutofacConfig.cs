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
            builder.RegisterType<CacheWritingTriggerAspect>();

            builder.RegisterControllers(Assembly.GetExecutingAssembly());

            builder.RegisterAssemblyTypes(Assembly.Load("SampleMVC.Physical"))
                .Where(x => !x.Name.EndsWith("Config", StringComparison.OrdinalIgnoreCase))
                .AsImplementedInterfaces()
                .EnableInterfaceInterceptors(new ProxyGenerationOptions { Selector = new InterceptorsForPhysical() });

            builder.RegisterAssemblyTypes(Assembly.Load("SampleMVC.Logic"))
                .AsImplementedInterfaces()
                .EnableInterfaceInterceptors();

            var container = builder.Build();

            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }

    internal class InterceptorsForPhysical : IInterceptorSelector
    {
        public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
        {
            // 排除特定 Interceptor、按照 Order 排序。
            var excludedInterceptors = new[] { typeof(CacheWriterAspect), typeof(CacheWritingTriggerAspect) };

            return interceptors.Where(x => excludedInterceptors.All(t => x.GetType() != t)).OrderBy(
                x =>
                    {
                        var order = x.GetType().GetProperty("Order");

                        return order == null ? int.MaxValue : (int)order.GetValue(x);
                    }).ToArray();
        }
    }
}