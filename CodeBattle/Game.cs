using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class Game
{
    /// <summary>
    /// Gets or sets the token of the game
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Is the game started?
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public GameStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the speed of the game (number of milliseconds in a time unit)
    /// </summary>
    public int Speed { get; set; }

    /// <summary>
    /// If status is {@link GameStatus#PLAYING}, indicates the time (in ms) until the game starts
    /// </summary>
    public int? CountDown { get; set; }

    /// <summary>
    /// Data of the current player
    /// </summary>
    public Player Me { get; set; }

    /// <summary>
    /// Data of the foe of the current player
    /// </summary>
    public Player Foe { get; set; }

    public enum GameStatus { WAITING, PLAYING, FINISHED }

    public class Player
    {
        /// <summary>
        /// Heal points remaining
        /// </summary>
        public long HealthPoints { get; set; }

        /// <summary>
        /// Armor remaining
        /// </summary>
        public long Armor { get; set; }

        /// <summary>
        /// isBehindShield
        /// </summary>
        public bool IsBehindShield { get; set; }

        /// <summary>
        /// Character chosen by the player
        /// </summary>
        public CharacterCharacteristic Character { get; set; }

        /// <summary>
        /// History of actions
        /// </summary>
        public List<History> History { get; set; }
    }

    public class CharacterCharacteristic
    {
        /// <summary>
        /// Armor of the character. All characters start with the same amount of heal points, but they have different armors.
        /// </summary>
        public long Armor { get; set; }

        /// <summary>
        /// Name of the character
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of the available actions for this character
        /// </summary>
        public List<Action> Actions { get; set; }
    }

    public class Action
    {

        /// <summary>
        /// Name of the action (to be used by the players to play)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the action (to be read by players to understand the action: players can't see real effects)
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Time before the player can't play again after this action (in time units, can't be null)
        /// </summary>
        public double CoolDown { get; set; }
    }

    public class History
    {
        public Action Action { get; set; }
        public int Age { get; set; }
        public int Id { get; set; }
    }
}
