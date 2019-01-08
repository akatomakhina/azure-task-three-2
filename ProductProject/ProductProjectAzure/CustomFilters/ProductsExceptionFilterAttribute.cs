using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using log4net;
using ProductProject.Logic.Common.Exceptions;

namespace ProductProjectAzure.CustomFilters
{
    public class ProductsExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private ILog _logger;

        public ProductsExceptionFilterAttribute()
        {
            _logger = LogManager.GetLogger(
                System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public override void OnException(HttpActionExecutedContext context)
        {
            if (context.Exception.GetType() == Type.GetType("RequestedResourceNotFoundException"))
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            else if (context.Exception.GetType() == Type.GetType("RequestedResourceHasConflictException"))
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.Conflict);
            }
            else if (context.Exception.GetType() == Type.GetType("UriFormatException"))
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            else if (context.Exception.GetType() == Type.GetType("ProductServiceException"))
            {
                var exception = (ProductServiceException)context.Exception;
                switch (exception.ErrorCode)
                {
                    case ErrorType.ValidationException:
                        _logger.Warn(exception);
                        context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        break;
                    case ErrorType.BadRequest:
                        _logger.Warn(exception);
                        context.Response = new HttpResponseMessage(HttpStatusCode.BadRequest);
                        break;
                    default:
                        _logger.Error(exception);
                        context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                        break;
                }
            }
            else if (context.Exception is Exception)
            {
                context.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
    }
}