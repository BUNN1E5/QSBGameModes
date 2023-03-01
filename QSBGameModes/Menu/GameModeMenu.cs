using OWML.Common;
using QSB;
using QSBGameModes.GameManagement;
using QSBGameModes.GameManagement.PlayerManagement;
using UnityEngine.Events;
using UnityEngine.UI;

namespace QSBGameModes.Menu;

public static class GameModeMenu{
    public static Button menuButton;
    public static Text menuText;
    public static Button.ButtonClickedEvent clickedEvent;

    public static Button spectateButton;
    public static Text spectateText;

    public static void SetupPauseButton(){
        Utils.WriteLine("Adding button to menu", MessageType.Info);
        //Setup the Host button 
        //TODO :: MAKE BETTER GUI FOR SETTING UP GAME
        if (QSBCore.IsHost){ //TODO :: CHANGE ORDER OF HIDE AND SEEK INTERACT BUTTON
            menuButton = QSBCore.MenuApi.PauseMenu_MakeSimpleButton("START"); //HIDE AND SEEK INTERACT BUTTON
            menuText = menuButton.GetComponentInChildren<Text>();
            SetPauseButtonAction(QSBGameModes.StartGameMode);
        } else {
            menuButton = QSBCore.MenuApi.PauseMenu_MakeSimpleButton("JOIN"); //HIDE AND SEEK INTERACT BUTTON
            menuText = menuButton.GetComponentInChildren<Text>();
            SetPauseButtonAction(QSBGameModes.JoinGameMode);
        }

        UpdateGUI();
    }

    private static GameState lastState = GameState.Starting;
    public static void UpdateGUI(){
        Utils.ModHelper.Events.Unity.RunWhen(() => lastState != GameManager.state, UpdateGUI_);
    }

    //TODO :: SETUP THE FUNCTIONALITY FOR THE SMART BUTTON
    private static void UpdateGUI_(){
        lastState = GameManager.state;
        Utils.WriteLine("Updating GUI");
        
        if (QSBCore.IsHost){
            if (GameManager.state == GameState.Stopped){
                menuText.text = "START " + SharedSettings.settingsToShare.GameType;
                SetPauseButtonAction(QSBGameModes.StartGameMode);
            } else{
                menuText.text = "STOP " + SharedSettings.settingsToShare.GameType;
                SetPauseButtonAction(QSBGameModes.StopGameMode);
            }
        } else {
            if (GameManager.state != GameState.Stopped){
                switch (PlayerManager.LocalPlayer.State){
                    case GameManagement.PlayerManagement.PlayerState.Seeking:
                    case GameManagement.PlayerManagement.PlayerState.Ready:
                    case GameManagement.PlayerManagement.PlayerState.Hiding:
                        menuText.text = "LEAVE " + SharedSettings.settingsToShare.GameType;
                        SetPauseButtonAction(QSBGameModes.LeaveGameMode);
                        break;
                    
                    case GameManagement.PlayerManagement.PlayerState.None:
                    case GameManagement.PlayerManagement.PlayerState.Spectating:
                        menuText.text = "JOIN " + SharedSettings.settingsToShare.GameType;
                        SetPauseButtonAction(QSBGameModes.JoinGameMode);
                        break;
                }
            }
            else{
                if (PlayerManager.LocalPlayer.State == GameManagement.PlayerManagement.PlayerState.None){
                    menuText.text ="READY UP FOR " + SharedSettings.settingsToShare.GameType; // + SharedSettings.settingsToShare.GameType;
                    SetPauseButtonAction(QSBGameModes.JoinGameMode);
                } else {
                    menuText.text ="UNREADY UP FOR " + SharedSettings.settingsToShare.GameType; // + SharedSettings.settingsToShare.GameType;
                    SetPauseButtonAction(QSBGameModes.LeaveGameMode);
                }
            }
        }
    }

    public static void SetPauseButtonAction(UnityAction action){
        if (clickedEvent == null){
            clickedEvent = new Button.ButtonClickedEvent();
            menuButton.onClick = clickedEvent;
        }
        clickedEvent.RemoveAllListeners();
        clickedEvent.AddListener(action);
        clickedEvent.AddListener(UpdateGUI);
    }
}