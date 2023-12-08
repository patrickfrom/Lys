namespace Lys;

public static class Program
{
    public static void Main()
    {
        using var window = new Window(1280, 800, "Lys");
        window.Run();
    }
}