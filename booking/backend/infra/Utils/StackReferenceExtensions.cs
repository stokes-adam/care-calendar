using Pulumi;

namespace infra;

public static class StackReferenceExtensions
{
    public static Output<T> Output<T>(this StackReference stack, string name)
    {
        return stack.RequireOutput(name).Apply(x => (T)x);
    }
}