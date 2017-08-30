using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SampleMVC.Helpers
{
    internal class CacheKeyHelper
    {
        public static string GenerateKey(MethodInfo method, object[] arguments, string input)
        {
            var key = string.Join(",", arguments.Select(x => x != null ? GetArgumentValue(x) : string.Empty));

            return string.IsNullOrEmpty(input)
                       ? $"{method.DeclaringType.Name}.{method.Name}({key})"
                       : Regex.Replace(input, "{key}", key);
        }

        private static string GetArgumentValue(object arg)
        {
            var value = arg.ToString();

            if (value.Equals(arg.GetType().FullName, StringComparison.OrdinalIgnoreCase))
            {
                value = string.Format(
                    "{{{0}}}",
                    string.Join(
                        ",",
                        arg.GetType().GetProperties().Select(
                            p =>
                            {
                                var argValue = p.GetValue(arg);

                                return argValue?.ToString() ?? string.Empty;
                            })));
            }

            return value;
        }
    }
}