# Game Flow
1. **Player initializes a new game.** \
Based on the chosen game setting a new LightsOut game object is initialized. This contains the matrix itself and some other information that will be used throughout the game play through. This is called the ```GameState.cs```. \
This GameState is stored in memory and any moves are stored in this InMemory Cache. For distributed installations of this web-app, distributed caches would need to be used.
2. **Player toggles a cell** \
Each time a player toggles a cell, the in memory GameState will be overridden and the cache duration will be reset. This ensures that if the player is active, their state will not be lost, but if the player is AFK, the state is automatically removed from the InMemory Cache. \
With each action, the app checks if the game is either solved or the timer ran out and either way inserts a high score for the player.
3. **Player surrenders** \
If the user gives up and surrenders, a high score is inserted and the InMemoryCache game state is removed.

***P.S*** Server Side Rendering is used to visualize the game. A couple hundred lines of javascript (Bootstrap, FetchAPI, Vanilla ES6+) then just make the game interactive.

# Application Structure
The application is split up as follows:

```
📂 src
|_ 🕹 LightsOut.Web.dll
    API with Game Settings CRUD, Server Side Rendering of Game, HighScore listing

|_ 📂 lib
|___ LightsOut.GameLogic.dll
        All game logic and services that deal with game settings and highscores

|___ LightsOut.Infrastructure.dll
        Some infrastructure stuff, namely caching and a system clock implementation
```

The game is using a Matrix data structure that is based on the game setting that the player chooses when they initialize a new game.

There are multiple game settings that vary in complexity and each of these rank differently in the high scores table.

## Dotnet
The API is written in **dotnet 6.0** and the app registration is inline with the new code practices proposed by Microsoft. I still retained a separate ```Program.cs``` and ```Startup.cs``` because I find it neater and easier to maintain.

All other class libraries are written in ```netstardard 2.1```.

## SQL backend
Due to the nature of this project, I felt that SQLite was ideal. It's self contained in the app and requires little to no setup when starting the app for the first time.

***P.S*** The ```.sqlite``` file is not included in this project. A new file will be generated during the API startup.

During the API startup I have setup a ```BootstrapperService.cs``` service that resolves all ```IBootstrapper.cs``` implementations and runs the ```BootstrapAsync``` function. The ```DatabaseMigratorBootstrapper``` will run on startup. For migrations I am using the ```FluentMigrator``` Nuget package. All migrations are in the ```LightsOut.GameLogic.dll``` class library.

***P.S*** The BootStrapper Service runs on a completely different thread (as a Background Service) so it does not slow down the startup process. Once all Bootstrappers are done, the ```StopAsync``` function is called.


# Running the application
## Option 1 - Dotnet CLI

## Option 2 - Docker CLI