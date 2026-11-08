namespace TerminalSnake;

/// <summary>
/// Owns the food's position and chooses a new random free cell.
/// </summary>
public sealed class Food
{
    private readonly Random _random;

    public Food(Random random)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random));
    }

    /// <summary>Null means there is no free cell left on the board.</summary>
    public Position? Position { get; private set; }

    /// <summary>
    /// Places food on a uniformly random empty cell. Building the list of free
    /// cells guarantees termination and also handles a completely full board.
    /// </summary>
    public bool TrySpawn(Board board, IReadOnlyCollection<Position> occupiedCells)
    {
        ArgumentNullException.ThrowIfNull(board);
        ArgumentNullException.ThrowIfNull(occupiedCells);

        var occupied = new HashSet<Position>(occupiedCells);
        var freeCells = new List<Position>();

        for (int y = 0; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                var candidate = new Position(x, y);
                if (!occupied.Contains(candidate))
                {
                    freeCells.Add(candidate);
                }
            }
        }

        if (freeCells.Count == 0)
        {
            Position = null;
            return false;
        }

        Position = freeCells[_random.Next(freeCells.Count)];
        return true;
    }
}
