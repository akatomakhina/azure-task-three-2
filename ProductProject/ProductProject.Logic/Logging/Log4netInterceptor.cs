using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using log4net;

namespace ProductProject.Logic.Logging
{
    public class Log4netInterceptor : IAsyncInterceptor
    {
        public void InterceptAsynchronous(IInvocation invocation)
        {
            string targetTypeName = invocation.TargetType.FullName;
            ILog logger = LogManager.GetLogger(targetTypeName);
            try
            {
                CompareAndLogArguments(invocation, targetTypeName, logger);
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw new Exception();
            }
        }

        public void InterceptAsynchronous<TResult>(IInvocation invocation)
        {
            string targetTypeName = invocation.TargetType.FullName;
            ILog logger = LogManager.GetLogger(targetTypeName);
            try
            {
                string invocationDesc = CompareAndLogArguments(invocation, targetTypeName, logger);

                ((Task<TResult>)invocation.ReturnValue)
                    .ContinueWith(task =>
                    {
                        if (task.Status == TaskStatus.RanToCompletion)
                        {
                            if (logger.IsDebugEnabled)
                            {
                                StringBuilder sbResult = new StringBuilder();
                                AppendObject(sbResult, task.Result);
                                logger.Debug("Result of " + invocationDesc + " is: " + sbResult);
                            }
                        }
                    });

            }
            catch (Exception e)
            {
                logger.Error(e);
                throw new Exception();
            }
        }

        public void InterceptSynchronous(IInvocation invocation)
        {
            string targetTypeName = invocation.TargetType.FullName;
            ILog logger = LogManager.GetLogger(targetTypeName);
            try
            {
                string invocationDesc = CompareAndLogArguments(invocation, targetTypeName, logger);

                if (logger.IsDebugEnabled)
                {
                    StringBuilder sbResult = new StringBuilder();
                    AppendObject(sbResult, invocation.ReturnValue);
                    logger.Debug("Result of " + invocationDesc + " is: " + sbResult);
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw new Exception();
            }
        }

        private static void AppendObject(StringBuilder sb, object obj)
        {
            if (Equals(obj, null))
            {
                sb.Append("NULL");
            }
            else
                if (obj.GetType() == Type.GetType("IEnumerable"))
            {
                IEnumerable en = (IEnumerable)obj;
                sb.Append("[");
                IEnumerator enumerator = en.GetEnumerator();
                bool isFirstObject = true;
                while (enumerator.MoveNext())
                {
                    if (!isFirstObject)
                    {
                        sb.Append(", ");
                    }

                    AppendObject(sb, enumerator.Current);

                    if (isFirstObject)
                    {
                        isFirstObject = false;
                    }
                }

                sb.Append("]");
            }
            else
            {
                Type objType = obj.GetType();
                if (objType.IsPrimitive || obj is DateTime)
                {
                    sb.Append(obj);
                }
                else
                {
                    sb.Append("{");
                    sb.Append(obj);
                    sb.Append("}");
                }
            }
        }


        private static string CompareAndLogArguments(IInvocation invocation, string targetTypeName, ILog logger)
        {
            StringBuilder sb;
            if (logger.IsDebugEnabled)
            {
                sb = new StringBuilder(targetTypeName)
                    .Append(".")
                    .Append(invocation.Method)
                    .Append("(");
                for (int i = 0; i < invocation.Arguments.Length; i++)
                {
                    object argument = invocation.Arguments[i];
                    if (i > 0)
                    {
                        sb.Append(", ");
                    }

                    AppendObject(sb, argument);
                }

                sb.Append(")");
                logger.Debug(sb);
            }
            else
            {
                sb = new StringBuilder();
            }

            invocation.Proceed();
            return sb.ToString();
        }
    }
}
