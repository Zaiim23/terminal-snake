namespace TerminalSnake;

/// <summary>
/// All console drawing lives here. Rendering is diff-based: after the initial
/// frame, only cells whose symbol or color changed are written. Combined with
/// Console.SetCursorPosition, this avoids the flicker caused by Console.Clear.
/// </summary>
public sealed class ConsoleRenderer
{
    private const int BoardLeft = 2;
    private const int BoardTop = 3;
    private const int HeaderRow = 0;
    private const int ScoreRow = 1;
    private const int HelpRow = 2;

    private readonly Board _board;
    private (char Symbol, ConsoleColor Color)[,]? _lastFrame;
    private int _lastScore = int.MinValue;

    public ConsoleRenderer(Board board)
    {
        _board = board ?? throw new ArgumentNullException(nameof(board));
    }

    /// <summary>
    /// Clears the screen once, draws the static chrome, and invalidates the
    /// previous frame so the next Render call paints every playfield cell.
    /// </summary>
    public void Initialize()
    {
        EnsureMinimumWindowSize(
            BoardLeft + _board.Width + 3,
            BoardTop + _board.Height + 3);

        Console.CursorVisible = false;
        Console.Clear();

        _lastFrame = null;
        _lastScore = int.MinValue;

        DrawStaticUi();
    }

    /// <summary>
    /// Draws only the cells that changed since the previous frame.
    /// </summary>
    public void Render(Snake snake, Food food, int score)
    {
        var snakeCells = new HashSet<Position>(snake.Body);
        _lastFrame ??= new (char Symbol, ConsoleColor Color)[_board.Width, _board.Height];

        for (int y = 0; y < _board.Height; y++)
        {
            for (int x = 0; x < _board.Width; x++)
            {
                var position = new Position(x, y);

                (char Symbol, ConsoleColor Color) cell;
                if (position == snake.Head)
                {
                    cell = ('@', ConsoleColor.Green);
                }
                else if (snakeCells.Contains(position))
                {
                    cell = ('o', ConsoleColor.DarkGreen);
                }
                else if (food.Position == position)
                {
                    cell = ('*', ConsoleColor.Red);
                }
                else
                {
                    cell = (' ', ConsoleColor.Gray);
                }

                if (_lastFrame[x, y] != cell)
                {
                    WriteAt(BoardLeft + 1 + x, BoardTop + 1 + y, cell.Symbol, cell.Color);
                    _lastFrame[x, y] = cell;
                }
            }
        }

        if (score != _lastScore)
        {
            WriteAt(
                BoardLeft,
                ScoreRow,
                $"Score: {score}".PadRight(_board.Width),
                ConsoleColor.Yellow);

            _lastScore = score;
        }

        Console.ResetColor();
    }

    public void ShowGameOver(int score, bool playerWon)
    {
        Console.Clear();
        _lastFrame = null;
        _lastScore = int.MinValue;

        int messageTop = Math.Max(0, BoardTop + _board.Height / 2 - 2);

        WriteAt(
            BoardLeft,
            messageTop,
            playerWon ? "YOU WIN!" : "GAME OVER",
            playerWon ? ConsoleColor.Green : ConsoleColor.Red);

        WriteAt(
            BoardLeft,
            messageTop + 2,
            $"Final score: {score}",
            ConsoleColor.Yellow);

        WriteAt(
            BoardLeft,
            messageTop + 4,
            "Press R to restart or Q to quit",
            ConsoleColor.Gray);

        Console.ResetColor();
    }

    /// <summary>Blocks until the player chooses restart (true) or quit (false).</summary>
    public bool AskRestartOrQuit()
    {
        while (true)
        {
            ConsoleKey key = Console.ReadKey(intercept: true).Key;

            if (key == ConsoleKey.R)
            {
                return true;
            }

            if (key == ConsoleKey.Q)
            {
                return false;
            }
        }
    }

    public void ShowExitMessage()
    {
        Console.Clear();
        WriteAt(0, 0, "Thanks for playing Terminal Snake!", ConsoleColor.Gray);
        Console.ResetColor();
    }

    private void DrawStaticUi()
    {
        WriteAt(BoardLeft, HeaderRow, "TERMINAL SNAKE", ConsoleColor.Green);
        WriteAt(BoardLeft, HelpRow, "Arrow keys: steer", ConsoleColor.DarkGray);

        string horizontalBorder = "+" + new string('-', _board.Width) + "+";

        WriteAt(BoardLeft, BoardTop, horizontalBorder, ConsoleColor.Gray);

        for (int y = 0; y < _board.Height; y++)
        {
            WriteAt(BoardLeft, BoardTop + 1 + y, '|', ConsoleColor.Gray);
            WriteAt(BoardLeft + _board.Width + 1, BoardTop + 1 + y, '|', ConsoleColor.Gray);
        }

        WriteAt(
            BoardLeft,
            BoardTop + _board.Height + 1,
            horizontalBorder,
            ConsoleColor.Gray);

        Console.ResetColor();
    }

    private static void WriteAt(int left, int top, char symbol, ConsoleColor color)
    {
        Console.SetCursorPosition(left, top);
        Console.ForegroundColor = color;
        Console.Write(symbol);
    }

    private static void WriteAt(int left, int top, string text, ConsoleColor color)
    {
        Console.SetCursorPosition(left, top);
        Console.ForegroundColor = color;
        Console.Write(text);
    }

    private static void EnsureMinimumWindowSize(int minimumWidth, int minimumHeight)
    {
        try
        {
            if (Console.WindowWidth >= minimumWidth && Console.WindowHeight >= minimumHeight)
            {
                return;
            }

            int targetWidth = Math.Max(Console.WindowWidth, minimumWidth);
            int targetHeight = Math.Max(Console.WindowHeight, minimumHeight);

            if (OperatingSystem.IsWindows() &&
                (targetWidth > Console.BufferWidth || targetHeight > Console.BufferHeight))
            {
                Console.SetBufferSize(
                    Math.Max(targetWidth, Console.BufferWidth),
                    Math.Max(targetHeight, Console.BufferHeight));
            }

            Console.SetWindowSize(targetWidth, targetHeight);
        }
        catch (Exception exception) when (
            exception is IOException or
            ArgumentOutOfRangeException or
            PlatformNotSupportedException)
        {
            // Some terminals do not allow resizing. If the window is large
            // enough already, this catch is never reached; otherwise the user
            // can simply enlarge the terminal.
        }
    }
}
