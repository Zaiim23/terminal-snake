namespace TerminalSnake;

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public static class DirectionExtensions
{
    /// <summary>
    /// Returns true when two directions point in exactly opposite ways.
    /// </summary>
    public static bool IsOpposite(this Direction direction, Direction other) =>
        (direction == Direction.Up && other == Direction.Down) ||
        (direction == Direction.Down && other == Direction.Up) ||
        (direction == Direction.Left && other == Direction.Right) ||
        (direction == Direction.Right && other == Direction.Left);
}
