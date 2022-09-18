namespace ConsoleTest;

internal static class Test
{
    public static int Run(int[] array)
    {
        if (array is [.., 1, 2, 3, _])
            return 1;
        return 0;
    }
}