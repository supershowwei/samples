namespace SamplesForm.Aspects
{
    [MulticastAttributeUsage(MulticastTargets.Method, TargetMemberAttributes = MulticastAttributes.Public,
         Inheritance = MulticastInheritance.Multicast)]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    [PSerializable]
    internal class TaskRunnerAspectAttribute : MethodInterceptionAspect
    {
        public override void OnInvoke(MethodInterceptionArgs args)
        {
            var instance = (IJob)args.Instance;

            instance.GetType().GetProperty("Task").SetValue(instance, Task.Run(() => { base.OnInvoke(args); }));

            instance.Task.ContinueWith(
                task =>
                {
                    if (args.Arguments.First() != null)
                    {
                        ((Action)args.Arguments.First()).Invoke();
                    }
                },
                TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}