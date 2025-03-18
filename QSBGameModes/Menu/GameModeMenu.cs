using OWML.Common;
using OWML.Utils;
using QSB;
using QSB.WorldSync;
using QSBGameModes.GameManagement;
using QSBGameModes.GameManagement.PlayerManagement;

namespace QSBGameModes.Menu;

public static class GameModeMenu
{
    private static SubmitAction _hostButton;
    private static SubmitAction _joinButton;

    public static void SetupPauseButton()
    {
        Utils.WriteLine("Adding button to menu", MessageType.Info);
        if (QSBCore.IsHost)
        {
            _hostButton =
                QSBGameModes.instance.ModHelper.MenuHelper.PauseMenuManager.MakeSimpleButton("READY UP", 0, false);
            _hostButton.OnSubmitAction += HostButtonHandler;
        }

        _joinButton =
            QSBGameModes.instance.ModHelper.MenuHelper.PauseMenuManager.MakeSimpleButton("JOIN GAME", 0, false);
        _joinButton.OnSubmitAction += JoinButtonHandler;

        UpdateGUI();
    }

    //Update GUI gets ran so often LMAO
    public static void UpdateGUI()
    {
        Utils.RunWhen(() => QSBWorldSync.AllObjectsReady,
            () => Utils.ModHelper.Events.Unity.FireInNUpdates(UpdateGUI_, 10));
    }

    private static void UpdateGUI_()
    {
        var currentPlayerState = PlayerManager.LocalPlayer == null
            ? GameManagement.PlayerManagement.PlayerState.None
            : PlayerManager.LocalPlayer.State;

        Utils.WriteLine($"Updating GUI, Current State is {GameManager.state.GetName()}");
        Utils.WriteLine($"Current Player state is {currentPlayerState}");

        if (QSBCore.IsHost)
        {
            UpdateHostButton();
        }

        UpdateJoinButton(currentPlayerState);
    }

    private static void UpdateHostButton()
    {
        if (GameManager.state == GameState.Stopped)
            ChangeButtonText(_hostButton, "START " + SharedSettings.settingsToShare.GameType);
        else
            ChangeButtonText(_hostButton, "STOP " + SharedSettings.settingsToShare.GameType);
    }

    private static void UpdateJoinButton(GameManagement.PlayerManagement.PlayerState currentPlayerState)
    {
        switch (currentPlayerState)
        {
            case GameManagement.PlayerManagement.PlayerState.Seeking:
            case GameManagement.PlayerManagement.PlayerState.Ready:
            case GameManagement.PlayerManagement.PlayerState.Hiding:
                if (GameManager.state != GameState.Stopped)
                    ChangeButtonText(_joinButton, "SPECTATE " + SharedSettings.settingsToShare.GameType);
                else
                    ChangeButtonText(_joinButton, "UNREADY UP FOR " + SharedSettings.settingsToShare.GameType);
                break;
            case GameManagement.PlayerManagement.PlayerState.None:
            case GameManagement.PlayerManagement.PlayerState.Spectating:
            default:
                if (GameManager.state != GameState.Stopped)
                    ChangeButtonText(_joinButton, "JOIN " + SharedSettings.settingsToShare.GameType);
                else
                    ChangeButtonText(_joinButton, "READY UP FOR " + SharedSettings.settingsToShare.GameType);
                break;
        }
    }

    // Unified handler for the host button
    private static void HostButtonHandler()
    {
        if (GameManager.state == GameState.Stopped) QSBGameModes.StartGameMode();
        else QSBGameModes.StopGameMode();
    }

    // Unified handler for the join button
    private static void JoinButtonHandler()
    {
        var currentPlayerState = PlayerManager.LocalPlayer == null
            ? GameManagement.PlayerManagement.PlayerState.None
            : PlayerManager.LocalPlayer.State;

        switch (currentPlayerState)
        {
            case GameManagement.PlayerManagement.PlayerState.Hiding:
            case GameManagement.PlayerManagement.PlayerState.Seeking:
            case GameManagement.PlayerManagement.PlayerState.Ready:
                QSBGameModes.LeaveGameMode();
                break;
            case GameManagement.PlayerManagement.PlayerState.None:
            case GameManagement.PlayerManagement.PlayerState.Spectating:
            default:
                QSBGameModes.JoinGameMode();
                break;
        }
    }

    private static void ChangeButtonText(SubmitAction button, string text)
    {
        QSBGameModes.instance.ModHelper.MenuHelper.PauseMenuManager.SetButtonText(button, text);
    }
}