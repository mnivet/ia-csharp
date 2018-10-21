using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

public class Ia
{
    // data from main arguments
    private static Mode _mode;
    private static string _gameName;
    private static string _gameToken;
    private static bool _versusPlayer;
    private static Character _character;
    private static string _playerName;

    private const bool Speedy = false; // change this boolean to accelerate the game
    private const bool ConfirmExit = true; // change this boolean to remove the press key on exit

    private enum Character { WARRIOR, PALADIN, DRUID, SORCERER, TROLL, ELF }

    private static readonly string PlayerKey = Guid.NewGuid().ToString("N");

    private enum Mode { CREATE, JOIN }

    public static async Task Main(string[] args)
    {
        try
        {
            if (ParseArgs(args))
            {
                await Play();
            }
        }
        catch (Exception e)
        {
            Console.Out.WriteLine("Something goes wrong...");
            Console.Out.WriteLine(e.Message);
            Console.Out.WriteLine(e.StackTrace);
        }

        if (ConfirmExit)
        {
            Console.Out.WriteLine("Press a key to exit...");
            Console.ReadKey();
        }
    }

    public static bool ParseArgs(string[] args)
    {
        // check mode argument
        if (args.Length == 0 || (!Mode.CREATE.ToString().Equals(args[0]) && !Mode.JOIN.ToString().Equals(args[0])))
        {
            Console.Out.WriteLine("1st argument is required, and must be " + Mode.CREATE + " or " + Mode.JOIN);
            return false;
        }

        _mode = Enum.Parse<Mode>(args[0]);

        // check game name/token argument, and versus argument in case of creation mode
        switch (_mode)
        {
            case Mode.CREATE:
                if (args.Length < 2)
                {
                    Console.Out.WriteLine("2nd argument is required, and must be the game name");
                    return false;
                }
                _gameName = args[1];

                if (args.Length > 4 && args[4].ToLower().Equals("true"))
                {
                    _versusPlayer = true;
                }
                break;
            case Mode.JOIN:
                if (args.Length < 2)
                {
                    Console.Out.WriteLine("2nd argument is required, and must be the game token");
                    return false;
                }
                _gameToken = args[1];
                break;
        }

        // check character argument
        if (args.Length < 3)
        {
            Console.Out.WriteLine("3rd argument is required, and must be your character type");
            return false;
        }

        if (!Enum.TryParse(args[2], out _character))
        {
            Console.Out.WriteLine("3rd argument must be a character, not " + args[2]);
            return false;
        }

        // check player name argument
        if (args.Length < 4)
        {
            Console.Out.WriteLine("4th argument is required and must be your player name");
            return false;
        }
        _playerName = args[3];

        Console.Out.WriteLine("All yours arguments are OK.");
        Console.Out.WriteLine();
        return true;
    }

    public static async Task Play()
    {
        Game game;
        switch (_mode)
        {
            case Mode.CREATE:
                Console.Out.WriteLine("Creating the game...");
                game = await CodingGameClient.CreateGame(_gameName, Speedy, _versusPlayer);
                break;
            case Mode.JOIN:
                Console.Out.WriteLine("Joining the game...");
                game = await CodingGameClient.JoinGame(_gameToken, PlayerKey, _character.ToString(), _playerName);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (game.Status == Game.GameStatus.WAITING)
        {
            Console.Out.WriteLine("Waiting the foe to join the game...");
            while (game.Status == Game.GameStatus.WAITING)
            {
                Thread.Sleep(500);
                game = await CodingGameClient.JoinGame(game.Token, PlayerKey, _character.ToString(), _playerName);
            }
        }

        game = await CodingGameClient.GetGame(game.Token, PlayerKey);
        Console.Out.WriteLine("Waiting count down during " + game.CountDown + "ms...");
        Thread.Sleep(game.CountDown.GetValueOrDefault());

        var random = new Random();
        while (game.Status != Game.GameStatus.FINISHED)
        {
            // ******************************************
            // IMPLEMENT YOUR AI HERE
            // ******************************************

            // The following AI just randomly alternate with the 4 possible actions
            switch (random.Next(4))
            {
                case 0:
                    await CodingGameClient.PlayAndWaitCoolDown(game.Token, PlayerKey, "HIT");
                    break;
                case 1:
                    await CodingGameClient.PlayAndWaitCoolDown(game.Token, PlayerKey, "THRUST");
                    break;
                case 2:
                    await CodingGameClient.PlayAndWaitCoolDown(game.Token, PlayerKey, "HEAL");
                    break;
                case 3:
                    await CodingGameClient.PlayAndWaitCoolDown(game.Token, PlayerKey, "SHIELD");
                    Console.Out.WriteLine("Waiting for shield duration... (" + (long)game.Speed / 2 + "ms)");
                    Thread.Sleep(game.Speed / 2);
                    break;
            }

            game = await CodingGameClient.GetGame(game.Token, PlayerKey);
            Console.Out.WriteLine("Me: " + game.Me.HealthPoints + "pv, foe: " + game.Foe.HealthPoints + "pv.");
        }

        Console.Out.WriteLine(game.Me.HealthPoints > 0 ? "You WIN !" : "You lose.");
    }

}
