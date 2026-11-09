using TerminalSnake;

namespace TerminalSnake.Tests;

public class SnakeTests
{
    [Fact]
    public void Constructor_PlacesHeadAtStartAndBuildsBodyBehindIt()
    {
        var snake = new Snake(new Position(5, 5), initialLength: 3, Direction.Right);

        Assert.Equal(new Position(5, 5), snake.Head);
        Assert.Equal(
            new[] { new Position(5, 5), new Position(4, 5), new Position(3, 5) },
            snake.Body);
    }

    [Fact]
    public void Move_WithoutGrowing_AdvancesHeadAndPreservesLength()
    {
        var snake = new Snake(new Position(5, 5), initialLength: 3, Direction.Right);

        snake.Move(grow: false);

        Assert.Equal(new Position(6, 5), snake.Head);
        Assert.Equal(3, snake.Body.Count);
        Assert.DoesNotContain(new Position(3, 5), snake.Body);
    }

    [Fact]
    public void Move_WithGrowing_AddsOneSegment()
    {
        var snake = new Snake(new Position(5, 5), initialLength: 3, Direction.Right);

        snake.Move(grow: true);

        Assert.Equal(new Position(6, 5), snake.Head);
        Assert.Equal(4, snake.Body.Count);
    }

    [Fact]
    public void Move_AppliesQueuedTurn()
    {
        var snake = new Snake(new Position(5, 5), initialLength: 3, Direction.Right);
        Assert.True(snake.TryQueueTurn(Direction.Down));

        snake.Move(grow: false);

        Assert.Equal(Direction.Down, snake.Direction);
        Assert.Equal(new Position(5, 6), snake.Head);
    }

    [Fact]
    public void TryQueueTurn_RejectsImmediateReversal()
    {
        var snake = new Snake(new Position(5, 5), initialLength: 3, Direction.Right);

        Assert.False(snake.TryQueueTurn(Direction.Left));
        Assert.Equal(Direction.Right, snake.Direction);
    }

    [Fact]
    public void WouldCollide_TreatsVacatingTailAsSafeOnlyWhenNotGrowing()
    {
        var snake = new Snake(new Position(5, 5), initialLength: 3, Direction.Right);
        var tail = new Position(3, 5);

        Assert.False(snake.WouldCollide(tail, isGrowing: false));
        Assert.True(snake.WouldCollide(tail, isGrowing: true));
    }
}
