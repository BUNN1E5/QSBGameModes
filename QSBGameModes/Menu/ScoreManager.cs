using System.Collections.Generic;
using QSB;

namespace QSBGameModes;

public class ScoreManager
{
    //TODO :: DO Score stuff

    public static string CatcherName = "Catchers";
    public static string CatcheeName = "Catchees";



    public static Dictionary<GameModeInfo, float> scores = new();

    public static void InitScoreMenu()
    {
        //QSB.HUD.MultiplayerHUDManager.Instance.

        //Should I do a custom GUI?
    }

    public static void SetupScoreMenuGUI()
    {
    }

    public static void CloseScoreMenu()
    {

    }

    public static void OpenScoreMenu()
    {

    }

    public static void UpdateScore(GameModeInfo info, float score)
    {

    }
}