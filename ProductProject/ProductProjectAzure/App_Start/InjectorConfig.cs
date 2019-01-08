using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Quartz.Impl;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;
using SimpleInjector.Lifestyles;
using AutoMapper;
using FluentValidation;
using FluentValidation.WebApi;
using log4net;
using System.Reflection;
using System.Web.Http;
using ProductProjectAzure.Jobs;

namespace ProductProjectAzure.App_Start
{
    public static class InjectorConfig
    {
        public static void Configure(HttpConfiguration config)
        {
            var container = new Container();
            container.Options.DefaultLifestyle = new AsyncScopedLifestyle();

            container.RegisterPackages(GetAssemblies());

            MapperSetup(container);
            ValidationSetup(config, container);
            LoggerSetup(container);
            QuartzFactory(container);

            container.RegisterWebApiControllers(config);
            container.Verify(VerificationOption.VerifyOnly);
            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(container);
        }

        private static Assembly[] GetAssemblies()
        {
            var a = AppDomain.CurrentDomain.GetAssemblies().Where(asm =>
               asm.FullName.StartsWith("ProductProject", StringComparison.OrdinalIgnoreCase)).ToArray();
            foreach (var b in a)
            {
                var c = b.FullName;
                //Console.WriteLine(b.FullName);
            }
            return a;
        }

        private static void MapperSetup(Container container)
        {
            Mapper.Initialize(config => config.AddProfiles(GetAssemblies()));
            Mapper.AssertConfigurationIsValid();
            var mapper = Mapper.Configuration.CreateMapper(container.GetInstance);
            container.RegisterInstance(typeof(IMapper), mapper);
        }

        private static void ValidationSetup(HttpConfiguration config, Container container)
        {
            FluentValidationModelValidatorProvider.Configure(config, provider => provider.ValidatorFactory = new SimpleInjectorFactory(container));
            AssemblyScanner.FindValidatorsInAssemblies(GetAssemblies()).ForEach(result =>
                container.Register(result.InterfaceType, result.ValidatorType));
        }

        private static void LoggerSetup(Container container)
        {
            container.RegisterInstance<ILog>(LogManager.GetLogger("Logger sample"));
        }

        private static void QuartzFactory(Container container)
        {
            var factory = new StdSchedulerFactory();
            var sheduler = factory.GetScheduler().Result;
            sheduler.JobFactory = new SimpleInjectorJobFactory(container, Assembly.GetExecutingAssembly());
        }
    }

    public class SimpleInjectorFactory : IValidatorFactory
    {
        private readonly Container _container;

        public SimpleInjectorFactory(Container container)
        {
            _container = container;
        }

        public IValidator<T> GetValidator<T>()
        {
            try
            {
                return (IValidator<T>)_container.GetInstance(typeof(T));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IValidator GetValidator(Type type)
        {
            try
            {
                return (IValidator)_container.GetInstance(type);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}