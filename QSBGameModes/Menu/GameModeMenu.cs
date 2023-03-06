using OWML.Common;
using QSB;
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
    
    public static Button.ButtonClickedEvent clickedEvent;

    public static void SetupPauseButton(){
        Utils.WriteLine("Adding button to menu", MessageType.Info);
        if (QSBCore.IsHost){
            HostButton = QSBCore.MenuApi.PauseMenu_MakeSimpleButton("READY UP"); //HIDE AND SEEK INTERACT BUTTON
            hostText = HostButton.GetComponentInChildren<Text>();
            SetPauseButtonAction(HostButton, QSBGameModes.StartGameMode);
            return;
        }
        
        ClientButton = QSBCore.MenuApi.PauseMenu_MakeSimpleButton("JOIN"); //HIDE AND SEEK INTERACT BUTTON
        clientText = ClientButton.GetComponentInChildren<Text>();
        SetPauseButtonAction(ClientButton, QSBGameModes.JoinGameMode);
    }

    public static void UpdateGUI(){
        Utils.ModHelper.Events.Unity.FireInNUpdates(UpdateGUI_, 10);
    }

    private static void UpdateGUI_(){
        Utils.WriteLine("Updating GUI");
        
        if (QSBCore.IsHost){
            if (GameManager.state == GameState.Stopped){
                hostText.text = "START " + SharedSettings.settingsToShare.GameType;
                SetPauseButtonAction(HostButton, QSBGameModes.StartGameMode);
            } else{
                hostText.text = "STOP " + SharedSettings.settingsToShare.GameType;
                SetPauseButtonAction(HostButton, QSBGameModes.StopGameMode);
            }

            return;
        }
        
        if (GameManager.state != GameState.Stopped){
            switch (PlayerManager.LocalPlayer.State){
                case GameManagement.PlayerManagement.PlayerState.Seeking:
                case GameManagement.PlayerManagement.PlayerState.Ready:
                case GameManagement.PlayerManagement.PlayerState.Hiding:
                    clientText.text = "SPECTATE " + SharedSettings.settingsToShare.GameType;
                    SetPauseButtonAction(ClientButton, QSBGameModes.LeaveGameMode);
                    break;
                
                case GameManagement.PlayerManagement.PlayerState.None:
                case GameManagement.PlayerManagement.PlayerState.Spectating:
                    clientText.text = "JOIN " + SharedSettings.settingsToShare.GameType;
                    SetPauseButtonAction(ClientButton, QSBGameModes.JoinGameMode);
                    break;
            }
        } else {
            if (PlayerManager.LocalPlayer.State == GameManagement.PlayerManagement.PlayerState.None){
                clientText.text =
                    "READY UP FOR " +
                    SharedSettings.settingsToShare.GameType; // + SharedSettings.settingsToShare.GameType;
                SetPauseButtonAction(ClientButton, QSBGameModes.JoinGameMode);
            }
            else{
                clientText.text =
                    "UNREADY UP FOR " +
                    SharedSettings.settingsToShare.GameType; // + SharedSettings.settingsToShare.GameType;
                SetPauseButtonAction(ClientButton, QSBGameModes.LeaveGameMode);
            }
        }
    }

    public static void SetPauseButtonAction(Button button, UnityAction action){
        if (clickedEvent == null){
            clickedEvent = new Button.ButtonClickedEvent();
            button.onClick = clickedEvent;
        }
        clickedEvent.RemoveAllListeners();
        clickedEvent.AddListener(action);
        clickedEvent.AddListener(UpdateGUI);
    }
}