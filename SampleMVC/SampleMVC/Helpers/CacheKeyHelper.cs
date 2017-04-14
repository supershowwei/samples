using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace SampleMVC.Helpers
{
    internal class CacheKeyHelper
    {
        private static readonly Regex NameRegex = new Regex(@"{(?<name>\w*)}");

        public static string GenerateKey(
            MethodInfo method,
            ParameterInfo[] parameters,
            object[] arguments,
            string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                input = string.Join(",", arguments.Select(x => x?.ToString() ?? string.Empty));
            }
            else
            {
                var argumentsWithName = GenerateArgumentsWithName(parameters, arguments);

                while (NameRegex.IsMatch(input))
                {
                    input = NameRegex.Replace(
                        input,
                        m =>
                            {
                                var name = m.Groups["name"].Value;

                                return argumentsWithName.ContainsKey(name) && argumentsWithName[name] != null
                                           ? argumentsWithName[name].ToString()
                                           : string.Empty;
                            });
                }
            }

            return $"{method.DeclaringType.Name}.{method.Name}({input})";
        }

        private static Dictionary<string, object> GenerateArgumentsWithName(ParameterInfo[] parameters, object[] args)
        {
            return parameters.Select((p, i) => new { p.Name, Argument = args[i] })
                .ToDictionary(x => x.Name, x => x.Argument);
        }
    }
}