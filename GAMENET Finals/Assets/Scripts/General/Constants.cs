public class Constants
{
    public const string PLAYER_READY = "isPlayerReady";
    public const string PLAYER_SELECTION_NUMBER = "playerSelectionNumber";
    public const string PLAYER_ORDER = "playerOrder";

    public const string PLAYER_MOVEMENT_RIGHT = "d";
    public const string PLAYER_MOVEMENT_LEFT = "a";
    public const string PLAYER_MOVEMENT_JUMP = "space";
    public const int PLAYER_MOVEMENT_DASH = 0;

    public const int PLAY_PANEL = 0;
    public const int LOGIN_PANEL = 1;
    public const int CONNECTING_PANEL = 2;
    public const int GAMEMODE_PANEL = 3;
    public const int LOBBY_PANEL = 4;

    public const byte StartGameEventCode = 1;
    public const byte InitializeEventCode = 2;
    public const byte AddScoreEventCode = 3;
    public const byte UpdateScoreEventCode = 4;
    public const byte WinEventCode = 5;


    public const int PLAYER_STATE_GROUNDED = 0;
}
