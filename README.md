<div align="center">

# 🐍 Terminal Snake

### A flicker-free, pure C# Snake game built for the terminal.

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
[![.NET CI](https://github.com/Zaiim23/terminal-snake/actions/workflows/dotnet.yml/badge.svg)](https://github.com/Zaiim23/terminal-snake/actions/workflows/dotnet.yml)
![Dependencies](https://img.shields.io/badge/dependencies-none-success)
![Platform](https://img.shields.io/badge/platform-console-lightgrey)

</div>

---

## 📖 Introduction

**Terminal Snake** is a modern take on the arcade classic, built from scratch with C# and the .NET console. There is no game engine, no third-party library, and no hidden framework — just clean C# and the Base Class Library.

Behind the simple, nostalgic gameplay is a carefully designed game loop: **diff-based rendering** updates only the cells that changed to eliminate flicker, **asynchronous input polling** keeps the snake moving without waiting for key presses, and a small set of focused classes keeps game rules cleanly separated from console presentation.

## ✨ Features

- 🖥️ **Flicker-free rendering** — Uses `Console.SetCursorPosition()` and frame diffing instead of clearing the screen every tick.
- ⌨️ **Non-blocking input** — Polls `Console.KeyAvailable` while a `Stopwatch` and `Task.Delay` drive the fixed 100 ms game tick.
- 🧠 **Smart turn buffering** — Queues up to two turns and prevents 180° reversals, even during rapid key sequences.
- 💥 **Accurate collision detection** — Handles wall collisions and self-collisions precisely, including the “vacating tail” case.
- 🍎 **Fair food spawning** — Food always appears on a random free cell and never inside the snake.
- 🎨 **Colored console visuals** — Green snake, red food, gray walls, and a clearly visible score.
- 🏁 **Complete game-over flow** — Displays the final score and supports instant restart or quit.
- 🧩 **Clean OOP architecture** — Game state, rules, input, and rendering each have a dedicated responsibility.
- 📦 **Zero third-party dependencies** — Pure C# using only the .NET Base Class Library.
- 🖥️ **Full playfield border** — A stable, colored border frames the entire playable area.

## ✅ Prerequisites

- **.NET 8 SDK** — [Download .NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- A terminal that supports console colors. **Windows Terminal** is recommended on Windows.
- No additional packages or game engines are required.

Verify your installation:

```bash
dotnet --version
```

The output should start with `8.`.

## 🚀 How to Run

1. **Clone the repository**

   ```bash
   git clone https://github.com/zaiim23/terminal-snake.git
   ```

   > If you are using a fork or a different remote, replace the URL with your repository’s URL.

2. **Navigate into the project folder**

   ```bash
   cd terminal-snake
   ```

3. **Start the game**

   ```bash
   dotnet run
   ```

That’s it! `dotnet run` restores and builds the project automatically before launching the game.

## 🎮 Controls

| Input | Action |
| :--- | :--- |
| **Arrow keys** ⬆️ ⬇️ ⬅️ ➡️ | Steer the snake |
| **R** | Restart after Game Over |
| **Q** | Quit from the Game Over screen |

> The snake cannot reverse directly into itself. Pressing the opposite direction is safely ignored.

## 🏗️ Project Architecture

| File | Responsibility |
| :--- | :--- |
| `Program.cs` | Application entry point. Configures the console and restores the cursor and colors on exit. |
| `Game.cs` | Orchestrates the game loop, fixed tick timing, keyboard polling, scoring, collisions, and restart flow. |
| `Snake.cs` | Owns the snake body, current direction, buffered turns, movement, and self-collision checks. |
| `Food.cs` | Stores the food position and selects a random free cell for the next spawn. |
| `Board.cs` | Defines the playable area’s dimensions and wall boundaries. |
| `Position.cs` | Immutable value type representing an `(X, Y)` cell and calculating movement offsets. |
| `Direction.cs` | Direction enum plus the opposite-direction check used to prevent reversals. |
| `ConsoleRenderer.cs` | Draws the colored playfield, HUD, score, game-over screen, and performs diff-based updates. |
| `TerminalSnake.csproj` | Project configuration targeting .NET 8 with nullable reference types enabled. |
| `TerminalSnake.Tests/` | xUnit test project covering snake movement and food spawning rules. |
| `TerminalSnake.sln` | Solution file used by the CI workflow to build and test both projects. |

## 🧪 Testing

The game rules are covered by an xUnit test project. Run the full suite from the repository root:

```bash
dotnet test TerminalSnake.sln
```

The tests cover:

- Head and body placement during construction
- Movement with and without growth
- Queued turns and rejection of 180° reversals
- Tail-vacating collision behaviour
- Food spawning on free cells, never on occupied cells, and the full-board case

The same build and test commands run automatically on every push through GitHub Actions.

## 📄 License

Released under the MIT License. See [LICENSE](LICENSE) for the full text.

---

<div align="center">

**Built with C# and a little nostalgia. 🐍**

</div>
