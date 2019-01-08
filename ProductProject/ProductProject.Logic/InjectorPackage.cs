using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SimpleInjector;
using SimpleInjector.Packaging;
using Castle.DynamicProxy;
using ProductProject.DataAccess.Context;
using ProductProject.DataAccess.Common.Repositories;
using ProductProject.DataAccess.Repositories;
using ProductProject.Logic.Common.Services;
using ProductProject.Logic.Services;
using ProductProject.Logic.Validator;
using ProductProject.Logic.Logging;

namespace ProductProject.Logic
{
    public class InjectorPackage : IPackage
    {
        public void RegisterServices(Container container)
        {
            RegisterProductServices(container);
        }

        private void RegisterProductServices(Container container)
        {
            container.Register(() =>
                new ProxyGenerator().CreateInterfaceProxyWithTargetInterface<IProductService>(
                    container.GetInstance<ProductService>(),
                    container.GetInstance<ValidatorInterceptor>(),
                    container.GetInstance<Log4netInterceptor>()
                ));
            container.Register<ProductProjectContext>();

            //container.Register<IProductService, ProductService>();
            container.Register<IProductRerository, ProductDbRepository>();
            container.Register<ITransactionHistoryRepository, TransactionHistoryRepository>();
        }
    }
}
