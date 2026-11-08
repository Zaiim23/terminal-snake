namespace TerminalSnake;

/// <summary>
/// Owns the snake's body, current direction, and queued turns.
/// The head is always at index 0 and the tail is at the end.
/// </summary>
public sealed class Snake
{
    // Two buffered turns make quick, deliberate inputs feel responsive
    // without allowing the player to queue up a large backlog of moves.
    private const int MaxQueuedTurns = 2;

    private readonly List<Position> _body = new();
    private readonly Queue<Direction> _pendingTurns = new();
    private Direction _lastRequestedDirection;

    public Snake(Position startHead, int initialLength, Direction initialDirection)
    {
        if (initialLength < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(initialLength), "The snake must have at least one segment.");
        }

        Direction = initialDirection;
        _lastRequestedDirection = initialDirection;

        int stepX = initialDirection switch
        {
            Direction.Left => -1,
            Direction.Right => 1,
            _ => 0
        };

        int stepY = initialDirection switch
        {
            Direction.Up => -1,
            Direction.Down => 1,
            _ => 0
        };

        for (int i = 0; i < initialLength; i++)
        {
            _body.Add(new Position(startHead.X - stepX * i, startHead.Y - stepY * i));
        }
    }

    public IReadOnlyList<Position> Body => _body;

    public Position Head => _body[0];

    /// <summary>The direction used for the most recent movement.</summary>
    public Direction Direction { get; private set; }

    /// <summary>The direction the snake will use on its next movement.</summary>
    public Direction NextDirection => _pendingTurns.Count > 0 ? _pendingTurns.Peek() : Direction;

    /// <summary>The cell the head will occupy after the next movement.</summary>
    public Position NextHead => Head.Offset(NextDirection);

    /// <summary>
    /// Queues a turn if it is not a 180-degree reversal or a duplicate.
    /// Checking against the last queued direction (not only the current one)
    /// prevents a rapid Up-then-Down or Right-then-Left reversal.
    /// </summary>
    public bool TryQueueTurn(Direction direction)
    {
        if (_pendingTurns.Count >= MaxQueuedTurns)
        {
            return false;
        }

        if (direction == _lastRequestedDirection ||
            direction.IsOpposite(_lastRequestedDirection))
        {
            return false;
        }

        _pendingTurns.Enqueue(direction);
        _lastRequestedDirection = direction;
        return true;
    }

    /// <summary>
    /// Checks whether moving the head to <paramref name="position"/> would hit
    /// the body. When the snake is not growing, the tail vacates its cell
    /// during the move, so that cell is safe to enter.
    /// </summary>
    public bool WouldCollide(Position position, bool isGrowing)
    {
        int segmentsToCheck = isGrowing ? _body.Count : _body.Count - 1;

        for (int i = 0; i < segmentsToCheck; i++)
        {
            if (_body[i] == position)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Moves one cell in the next direction. The tail is removed unless the
    /// snake is growing, which makes the snake one segment longer.
    /// </summary>
    public void Move(bool grow)
    {
        if (_pendingTurns.TryDequeue(out Direction nextDirection))
        {
            Direction = nextDirection;
        }

        Position newHead = Head.Offset(Direction);
        _body.Insert(0, newHead);

        if (!grow)
        {
            _body.RemoveAt(_body.Count - 1);
        }
    }
}
