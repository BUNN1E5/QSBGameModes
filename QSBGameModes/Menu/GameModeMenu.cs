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
    public static Button HostButton;
    public static Button ClientButton;
    
    public static Text hostText;
    public static Text clientText;

    public static void SetupPauseButton(){
        Utils.WriteLine("Adding button to menu", MessageType.Info);
        if (QSBCore.IsHost){
            HostButton = QSBCore.MenuApi.PauseMenu_MakeSimpleButton("READY UP"); //HIDE AND SEEK INTERACT BUTTON
            hostText = HostButton.GetComponentInChildren<Text>();
            SetPauseButtonAction(HostButton, QSBGameModes.StartGameMode);
        }
        
        ClientButton = QSBCore.MenuApi.PauseMenu_MakeSimpleButton("JOIN"); //HIDE AND SEEK INTERACT BUTTON
        clientText = ClientButton.GetComponentInChildren<Text>();
        SetPauseButtonAction(ClientButton, QSBGameModes.JoinGameMode);

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
                hostText.text = "START " + SharedSettings.settingsToShare.GameType;
                SetPauseButtonAction(HostButton, QSBGameModes.StartGameMode);
                Utils.WriteLine($"Setting StartGameMode to Start");
            } else{
                hostText.text = "STOP " + SharedSettings.settingsToShare.GameType;
                SetPauseButtonAction(HostButton, QSBGameModes.StopGameMode);
                Utils.WriteLine($"Setting StartGameMode to Stop");
            }
        }
        
        if (GameManager.state != GameState.Stopped){
            switch (currentPlayerState){
                case GameManagement.PlayerManagement.PlayerState.Seeking:
                case GameManagement.PlayerManagement.PlayerState.Ready:
                case GameManagement.PlayerManagement.PlayerState.Hiding:
                    clientText.text = "SPECTATE " + SharedSettings.settingsToShare.GameType;
                    SetPauseButtonAction(ClientButton, QSBGameModes.LeaveGameMode);
                    Utils.WriteLine($"Setting Leave Button");
                    break;
                
                case GameManagement.PlayerManagement.PlayerState.None:
                case GameManagement.PlayerManagement.PlayerState.Spectating:
                    clientText.text = "JOIN " + SharedSettings.settingsToShare.GameType;
                    SetPauseButtonAction(ClientButton, QSBGameModes.JoinGameMode);
                    Utils.WriteLine($"Setting Join Button");
                    break;
            }
        } else {
            if (currentPlayerState is GameManagement.PlayerManagement.PlayerState.None 
                or GameManagement.PlayerManagement.PlayerState.Spectating){
                clientText.text =
                    "READY UP FOR " +
                    SharedSettings.settingsToShare.GameType; // + SharedSettings.settingsToShare.GameType;
                SetPauseButtonAction(ClientButton, QSBGameModes.JoinGameMode);
                Utils.WriteLine($"Setting Ready Up Button");

            } else {
                clientText.text =
                    "UNREADY UP FOR " +
                    SharedSettings.settingsToShare.GameType; // + SharedSettings.settingsToShare.GameType;
                SetPauseButtonAction(ClientButton, QSBGameModes.LeaveGameMode);
                Utils.WriteLine($"Setting Unready Up Button");
            }
        }
    }

    public static void SetPauseButtonAction(Button button, UnityAction action){
        if (button.onClick == null){
            button.onClick = new Button.ButtonClickedEvent();;
        }
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(action);
        button.onClick.AddListener(UpdateGUI);
    }
}