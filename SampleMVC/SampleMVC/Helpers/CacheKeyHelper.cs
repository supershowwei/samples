using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SampleMVC.Helpers
{
    internal class CacheKeyHelper
    {
        public static string GenerateKey(MethodInfo method, object[] arguments, string input)
        {
            var key = string.Join(",", arguments.Select(x => x?.ToString() ?? string.Empty));

            return string.IsNullOrEmpty(input)
                       ? $"{method.DeclaringType.Name}.{method.Name}({key})"
                       : Regex.Replace(input, "{key}", key);
        }
    }
}