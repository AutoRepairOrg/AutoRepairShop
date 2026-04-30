namespace AutoRepairShop.Tests.Controllers;

internal static class ControllerResultValueReader
{
    public static string? GetString(object? value, string propertyName)
    {
        return value?.GetType().GetProperty(propertyName)?.GetValue(value)?.ToString();
    }
}
