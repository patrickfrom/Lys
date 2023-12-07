namespace Lys;

public static class Program
{
    public static void Main()
    {
        using var window = new Window(800, 600, "Lys");
        window.Run();
    }
}