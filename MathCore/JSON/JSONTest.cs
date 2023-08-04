namespace MathCore.JSON;

internal static class JSONTest
{
    public static JSONObjectCreator<T> GetJSON<T>(this T obj) => new(obj);
}