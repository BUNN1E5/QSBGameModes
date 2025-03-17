using OWML.Common;
using OWML.Utils;
using QSB;
using QSB.WorldSync;
using QSBGameModes.GameManagement;
using QSBGameModes.GameManagement.PlayerManagement;
using UnityEngine.Events;
using UnityEngine.UI;

namespace QSBGameModes.Menu;

public static class GameModeMenu{
    
    private static SubmitAction HostButton;
    private static SubmitAction JoinButton;

    public static void SetupPauseButton(){
        Utils.WriteLine("Adding button to menu", MessageType.Info);
        if (QSBCore.IsHost){
            HostButton = QSBGameModes.instance.ModHelper.MenuHelper.PauseMenuManager.MakeSimpleButton("READY UP", 0, false);
            SetPauseButtonAction(HostButton, QSBGameModes.StartGameMode);
        }
        
        JoinButton = QSBGameModes.instance.ModHelper.MenuHelper.PauseMenuManager.MakeSimpleButton("JOIN GAME", 0, false);
        SetPauseButtonAction(JoinButton, QSBGameModes.JoinGameMode);

        UpdateGUI();
    }

    //Update GUI gets ran so often LMAO
    public static void UpdateGUI(){
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

        
        if (QSBCore.IsHost){
            Utils.WriteLine($"We are host!");
            if (GameManager.state == GameState.Stopped){
                ChangeButtonText(HostButton, "START " + SharedSettings.settingsToShare.GameType);
                SetPauseButtonAction(HostButton, QSBGameModes.StartGameMode);
                Utils.WriteLine($"Setting StartGameMode to Start");
            } else{
                ChangeButtonText(HostButton, "STOP " + SharedSettings.settingsToShare.GameType);
                SetPauseButtonAction(HostButton, QSBGameModes.StopGameMode);
                Utils.WriteLine($"Setting StartGameMode to Stop");
            }
        }
        
        if (GameManager.state != GameState.Stopped){
            switch (currentPlayerState){
                case GameManagement.PlayerManagement.PlayerState.Seeking:
                case GameManagement.PlayerManagement.PlayerState.Ready:
                case GameManagement.PlayerManagement.PlayerState.Hiding:
                    ChangeButtonText(JoinButton, "SPECTATE " + SharedSettings.settingsToShare.GameType);
                    SetPauseButtonAction(JoinButton, QSBGameModes.LeaveGameMode);
                    Utils.WriteLine($"Setting Leave Button");
                    break;
                
                case GameManagement.PlayerManagement.PlayerState.None:
                case GameManagement.PlayerManagement.PlayerState.Spectating:
                    ChangeButtonText(JoinButton, "JOIN " + SharedSettings.settingsToShare.GameType);
                    SetPauseButtonAction(JoinButton, QSBGameModes.JoinGameMode);
                    Utils.WriteLine($"Setting Join Button");
                    break;
            }
        } else {
            if (currentPlayerState is GameManagement.PlayerManagement.PlayerState.None 
                or GameManagement.PlayerManagement.PlayerState.Spectating){
                ChangeButtonText(JoinButton, 
                    "READY UP FOR " +
                    SharedSettings.settingsToShare.GameType); // + SharedSettings.settingsToShare.GameType;
                SetPauseButtonAction(JoinButton, QSBGameModes.JoinGameMode);
                Utils.WriteLine($"Setting Ready Up Button");

            } else {
                ChangeButtonText(JoinButton, 
                    "UNREADY UP FOR " +
                    SharedSettings.settingsToShare.GameType); // + SharedSettings.settingsToShare.GameType;
                SetPauseButtonAction(JoinButton, QSBGameModes.LeaveGameMode);
                Utils.WriteLine($"Setting Unready Up Button");
            }
        }
    }

    public static void SetPauseButtonAction(SubmitAction button, SubmitAction.SubmitActionEvent action){
        button.OnSubmitAction += action;
    }
    
    private static void ChangeButtonText(SubmitAction button, string text){
        QSBGameModes.instance.ModHelper.MenuHelper.PauseMenuManager.SetButtonText(button, text);
    }
}