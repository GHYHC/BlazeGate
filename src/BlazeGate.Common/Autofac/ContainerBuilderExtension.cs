using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BlazeGate.Common.Autofac
{
    public static class ContainerBuilderExtension
    {
        /// <summary>
        /// 批量注入扩展
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="assembly"></param>
        public static void BatchAutowired(this ContainerBuilder builder, params Assembly[] assemblies)
        {
            var transientType = typeof(ITransitDenpendency); //瞬时注入
            var singletonType = typeof(ISingletonDenpendency); //单例注入
            var scopeType = typeof(IScopeDenpendency); //单例注入

            #region 类型注入

            //瞬时注入
            builder.RegisterAssemblyTypes(assemblies).Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(transientType))
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerDependency();
            //单例注入
            builder.RegisterAssemblyTypes(assemblies).Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(singletonType))
               .AsSelf()
               .AsImplementedInterfaces()
               .SingleInstance();
            //生命周期注入
            builder.RegisterAssemblyTypes(assemblies).Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(scopeType))
               .AsSelf()
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();

            #endregion 类型注入

            #region 泛型注入

            //瞬时注入
            builder.RegisterAssemblyOpenGenericTypes(assemblies).Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(transientType))
                .AsSelf()
                .AsImplementedInterfaces()
                .InstancePerDependency();
            //单例注入
            builder.RegisterAssemblyOpenGenericTypes(assemblies).Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(singletonType))
               .AsSelf()
               .AsImplementedInterfaces()
               .SingleInstance();
            //生命周期注入
            builder.RegisterAssemblyOpenGenericTypes(assemblies).Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Contains(scopeType))
               .AsSelf()
               .AsImplementedInterfaces()
               .InstancePerLifetimeScope();

            #endregion 泛型注入
        }
    }
}