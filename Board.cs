namespace TerminalSnake;

/// <summary>
/// Describes the rectangular playable area. The wall is outside these bounds.
/// </summary>
public sealed class Board
{
    public int Width { get; }
    public int Height { get; }

    public Board(int width, int height)
    {
        if (width <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), "Board width must be positive.");
        }

        if (height <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), "Board height must be positive.");
        }

        Width = width;
        Height = height;
    }

    /// <summary>
    /// True only for cells inside the walls. Touching a wall is not contained.
    /// </summary>
    public bool Contains(Position position) =>
        position.X >= 0 &&
        position.X < Width &&
        position.Y >= 0 &&
        position.Y < Height;
}
