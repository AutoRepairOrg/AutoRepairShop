using System.Reflection;

namespace AutoRepairShop.Tests.Services;

internal static class TestObjectFactory
{
    public static T Create<T>(params (string PropertyName, object? Value)[] properties)
        where T : new()
    {
        var instance = new T();

        foreach (var (propertyName, value) in properties)
        {
            SetProperty(instance, propertyName, value);
        }

        return instance;
    }

    private static void SetProperty<T>(T instance, string propertyName, object? value)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        var property =
            typeof(T).GetProperty(propertyName, flags)
            ?? throw new InvalidOperationException(
                $"Property '{propertyName}' not found on type '{typeof(T).Name}'."
            );

        var setter =
            property.GetSetMethod(true)
            ?? throw new InvalidOperationException(
                $"Property '{propertyName}' on type '{typeof(T).Name}' does not have a setter."
            );
        setter.Invoke(instance, [value]);
    }
}
