using System.Diagnostics;

namespace TerminalSnake;

/// <summary>
/// Coordinates input, game rules, timing, and rendering. The snake itself
/// knows nothing about the console, which keeps the model testable.
/// </summary>
public sealed class Game
{
    private const int BoardWidth = 40;
    private const int BoardHeight = 18;
    private const int InitialSnakeLength = 4;
    private const int TickMilliseconds = 100;
    private const int InputPollMilliseconds = 10;
    private const int PointsPerFood = 10;

    private readonly Board _board;
    private readonly ConsoleRenderer _renderer;
    private readonly Random _random;
    private readonly Food _food;

    private Snake _snake = null!;
    private int _score;

    public Game()
    {
        _board = new Board(BoardWidth, BoardHeight);
        _renderer = new ConsoleRenderer(_board);
        _random = new Random();
        _food = new Food(_random);
    }

    public async Task RunAsync()
    {
        _renderer.Initialize();

        while (await PlayRoundAsync())
        {
            _renderer.Initialize();
        }

        _renderer.ShowExitMessage();
    }

    private async Task<bool> PlayRoundAsync()
    {
        StartNewRound();

        while (true)
        {
            await WaitForNextTickAsync();

            Position nextHead = _snake.NextHead;
            bool willEat = _food.Position == nextHead;

            if (!_board.Contains(nextHead) || _snake.WouldCollide(nextHead, willEat))
            {
                _renderer.ShowGameOver(_score, playerWon: false);
                return _renderer.AskRestartOrQuit();
            }

            _snake.Move(willEat);

            if (willEat)
            {
                _score += PointsPerFood;

                // If no free cell exists, the board is full: a win.
                if (!_food.TrySpawn(_board, _snake.Body))
                {
                    _renderer.Render(_snake, _food, _score);
                    _renderer.ShowGameOver(_score, playerWon: true);
                    return _renderer.AskRestartOrQuit();
                }
            }

            _renderer.Render(_snake, _food, _score);
        }
    }

    private void StartNewRound()
    {
        _score = 0;

        _snake = new Snake(
            new Position(_board.Width / 2, _board.Height / 2),
            InitialSnakeLength,
            Direction.Right);

        if (!_food.TrySpawn(_board, _snake.Body))
        {
            throw new InvalidOperationException("The board is too small for the initial snake.");
        }

        _renderer.Render(_snake, _food, _score);
    }

    /// <summary>
    /// Waits for the next 100 ms tick while polling the keyboard. The snake
    /// keeps moving on schedule even if no key is pressed, and key input does
    /// not block the game loop.
    /// </summary>
    private async Task WaitForNextTickAsync()
    {
        var stopwatch = Stopwatch.StartNew();

        while (stopwatch.ElapsedMilliseconds < TickMilliseconds)
        {
            ProcessInput();

            int remaining = TickMilliseconds - (int)stopwatch.ElapsedMilliseconds;
            if (remaining > 0)
            {
                await Task.Delay(Math.Min(InputPollMilliseconds, remaining));
            }
        }

        ProcessInput();
    }

    private void ProcessInput()
    {
        try
        {
            // Drain everything currently buffered. Ignored keys are consumed
            // too, so a burst of presses cannot build up a stale backlog.
            while (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(intercept: true).Key;

                Direction? direction = key switch
                {
                    ConsoleKey.UpArrow => Direction.Up,
                    ConsoleKey.DownArrow => Direction.Down,
                    ConsoleKey.LeftArrow => Direction.Left,
                    ConsoleKey.RightArrow => Direction.Right,
                    _ => null
                };

                if (direction.HasValue)
                {
                    _snake.TryQueueTurn(direction.Value);
                }
            }
        }
        catch (InvalidOperationException)
        {
            // Console input is unavailable (for example, redirected stdin).
            // The game can still run and advance on its own.
        }
    }
}
