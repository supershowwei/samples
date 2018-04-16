using System.Collections.Generic;
using System.Reflection;

namespace SampleMVC.Containers
{
    public class UncacheKeyContainer
    {
        public static readonly UncacheKeyContainer Instance = new UncacheKeyContainer();

        private UncacheKeyContainer()
        {
            this.Keys = new Dictionary<MethodInfo, MethodInfo[]>();
        }

        public Dictionary<MethodInfo, MethodInfo[]> Keys { get; set; }

        public static void Register(MethodInfo invoked, params MethodInfo[] keys)
        {
            Instance.Keys.Add(invoked, keys);
        }
    }
}