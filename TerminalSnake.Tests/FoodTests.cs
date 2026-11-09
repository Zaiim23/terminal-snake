using TerminalSnake;

namespace TerminalSnake.Tests;

public class FoodTests
{
    [Fact]
    public void TrySpawn_PlacesFoodOnAFreeCellInsideTheBoard()
    {
        var board = new Board(width: 4, height: 4);
        var food = new Food(new Random(1234));
        var occupied = new[]
        {
            new Position(0, 0),
            new Position(1, 0),
            new Position(2, 0)
        };

        bool spawned = food.TrySpawn(board, occupied);

        Assert.True(spawned);
        Assert.NotNull(food.Position);
        Assert.True(board.Contains(food.Position.Value));
        Assert.DoesNotContain(food.Position.Value, occupied);
    }

    [Fact]
    public void TrySpawn_NeverChoosesAnOccupiedCell()
    {
        var board = new Board(width: 3, height: 3);
        var food = new Food(new Random(42));
        var occupied = new[] { new Position(1, 1) };

        for (int i = 0; i < 100; i++)
        {
            Assert.True(food.TrySpawn(board, occupied));
            Assert.NotEqual(new Position(1, 1), food.Position);
        }
    }

    [Fact]
    public void TrySpawn_ReturnsFalseAndClearsPositionWhenBoardIsFull()
    {
        var board = new Board(width: 2, height: 2);
        var food = new Food(new Random(7));
        var occupied = new[]
        {
            new Position(0, 0),
            new Position(1, 0),
            new Position(0, 1),
            new Position(1, 1)
        };

        bool spawned = food.TrySpawn(board, occupied);

        Assert.False(spawned);
        Assert.Null(food.Position);
    }
}
