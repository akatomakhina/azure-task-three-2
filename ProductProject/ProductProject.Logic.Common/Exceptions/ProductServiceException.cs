using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ProductProject.Logic.Common.Exceptions
{
    [Serializable]
    public class ProductServiceException : Exception
    {
        public ErrorType ErrorCode { get; }

        public ProductServiceException(ErrorType errorCodeType)
        {
            this.ErrorCode = errorCodeType;
        }

        public ProductServiceException(string message, ErrorType errorCodeType)
            : base(message)
        {
            this.ErrorCode = errorCodeType;
        }

        public ProductServiceException(string message, Exception inner
            , ErrorType errorCodeType)
            : base(message, inner)
        {
            this.ErrorCode = errorCodeType;
        }

        protected ProductServiceException(SerializationInfo info, StreamingContext context
            , ErrorType errorCodeType)
            : base(info, context)
        {
            this.ErrorCode = errorCodeType;
        }
    }

    public enum ErrorType
    {
        ValidationException,
        BadRequest
    }
}
