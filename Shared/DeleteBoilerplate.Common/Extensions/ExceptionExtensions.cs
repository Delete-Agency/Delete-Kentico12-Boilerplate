using System;
using System.Text;

namespace DeleteBoilerplate.Common.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetAllMessages(this Exception exception)
        {
            var currentException = exception;
            var str = new StringBuilder();

            while (currentException != null)
            {
                str.AppendLine(currentException.Message);
                currentException = currentException.InnerException;
            }
            return str.ToString();
        }

    }
}
