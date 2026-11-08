namespace TerminalSnake;

public static class Program
{
    public static async Task Main()
    {
        try
        {
            Console.Title = "Terminal Snake";

            var game = new Game();
            await game.RunAsync();
        }
        finally
        {
            Console.ResetColor();

            try
            {
                Console.CursorVisible = true;
            }
            catch (IOException)
            {
                // The console is no longer available; nothing to restore.
            }
            catch (PlatformNotSupportedException)
            {
                // This terminal does not expose a cursor visibility setting.
            }
        }
    }
}
