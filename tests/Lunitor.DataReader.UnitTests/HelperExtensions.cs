using System;

namespace Lunitor.DataReader.UnitTests
{
    public static class HelperExtensions
    {
        public static string GetParamName(this Type type, string methodName, int paramIndex)
        {
            string paramName = "";

            var methodInfo = type.GetMethod(methodName);

            if (methodInfo != null && methodInfo.GetParameters().Length > paramIndex)
                paramName = methodInfo.GetParameters()[paramIndex].Name;

            return paramName;
        }
    }
}
