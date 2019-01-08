using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Quartz;
using Quartz.Spi;
using SimpleInjector;
using SimpleInjector.Lifestyles;

namespace ProductProjectAzure.Jobs
{
    public class SimpleInjectorJobFactory : IJobFactory
    {
        private readonly Container container;
        private readonly Dictionary<Type, InstanceProducer> jobProducers;

        public SimpleInjectorJobFactory(
            Container container, params Assembly[] assemblies)
        {
            this.container = container;

            var transient = Lifestyle.Transient;
            jobProducers =
                container.GetTypesToRegister(typeof(IJob), assemblies).ToDictionary(
                    type => type,
                    type => transient.CreateProducer(typeof(IJob), type, container));
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler _)
        {
            var jobProducer = jobProducers[bundle.JobDetail.JobType];
            return new AsyncScopedJobDecorator(
                container, () => (IJob)jobProducer.GetInstance());
        }

        public void ReturnJob(IJob job)
        {
        }

        private sealed class AsyncScopedJobDecorator : IJob
        {
            private readonly Container container;
            private readonly Func<IJob> decorateeFactory;

            public AsyncScopedJobDecorator(
                Container container, Func<IJob> decorateeFactory)
            {
                this.container = container;
                this.decorateeFactory = decorateeFactory;
            }

            public async Task Execute(IJobExecutionContext context)
            {
                using (AsyncScopedLifestyle.BeginScope(container))
                {
                    var job = decorateeFactory();
                    await job.Execute(context);
                }
            }
        }
    }
}