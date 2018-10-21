# Prerequistes

dotnetcore SDK 2.1 installed

# Compile ia-csharp

```
dotnet build
```

# Create a new game

```
dotnet run -p .\CodeBattle\ CREATE [GAME_NAME] [CHARACTER] [PLAYER_NAME] [VERSUS_PLAYER]
```
* GAME_NAME: Name of the new game
* CHARACTER: Type of character (WARRIOR, PALADIN, DRUID, SORCERER, ELF, TROLL)
* PLAYER_NAME: Your name
* VERSUS_PLAYER: If true, an other player must join your game, otherwise it's a server side IA

Example to play against a server side IA :
```
dotnet run -p CodeBattle CREATE youShouldNotPass SORCERER gandalf false
```
Example to play against another player :
```
dotnet run -p CodeBattle CREATE youShouldNotPass SORCERER gandalf true
```

# Join a created game as the second player

```
dotnet run -p CodeBattle JOIN [GAME_TOKEN] [CHARACTER] [PLAYER_NAME]
```
* GAME_TOKEN: Key of the game you want to join
* CHARACTER: Type of your character (WARRIOR, PALADIN, DRUID, SORCERER, ELF, TROLL)
* PLAYER_NAME: Your name

Example :
```
dotnet run -p CodeBattle JOIN vyhu5a SORCERER saroumane
```