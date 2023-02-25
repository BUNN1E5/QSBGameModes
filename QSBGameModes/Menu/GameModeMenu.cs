using OWML.Common;
using QSB;
using QSBGameModes.GameManagement;
using QSBGameModes.GameManagement.PlayerManagement;
using UnityEngine.UI;

namespace QSBGameModes.Menu;

public static class GameModeMenu{
    public static Button menuButton;
    public static Text menuText;

    public static Button spectateButton;
    public static Text spectateText;

    public static void SetupPauseButton(){
        Utils.WriteLine("Adding button to menu", MessageType.Info);
        //Setup the Host button 
        //TODO :: MAKE BETTER GUI FOR SETTING UP GAME
        if (QSBCore.IsHost){ //TODO :: CHANGE ORDER OF HIDE AND SEEK INTERACT BUTTON
            menuButton = QSBCore.MenuApi.PauseMenu_MakeSimpleButton("START"); //HIDE AND SEEK INTERACT BUTTON
            menuText = menuButton.GetComponentInChildren<Text>();
            Button.ButtonClickedEvent c_event = new Button.ButtonClickedEvent();
            c_event.AddListener(QSBGameModes.StartHideAndSeek);
                
            menuButton.onClick = c_event;                
        } else {
            menuButton = QSBCore.MenuApi.PauseMenu_MakeSimpleButton("JOIN"); //HIDE AND SEEK INTERACT BUTTON
            menuText = menuButton.GetComponentInChildren<Text>();
            Button.ButtonClickedEvent c_event = new Button.ButtonClickedEvent();
            c_event.AddListener(QSBGameModes.JoinHideAndSeek);
                        
            menuButton.onClick = c_event;
        }
    }

    //TODO :: SETUP THE FUNCTIONALITY FOR THE SMART BUTTON
    public static void UpdateGUI(){
        if (QSBCore.IsHost){
            if (GameManager.state == GameState.Stopped){
                menuText.text = "START " + SharedSettings.settingsToShare.GameType;
            } else{
                menuText.text = "STOP " + SharedSettings.settingsToShare.GameType;
            }
        } else {
            if (GameManager.state != GameState.Stopped){
                if (PlayerManager.LocalPlayer.State == GameManagement.PlayerManagement.PlayerState.None){
                    menuText.text = "Join " + SharedSettings.settingsToShare.GameType;
                } else if(PlayerManager.LocalPlayer.State != GameManagement.PlayerManagement.PlayerState.None){
                    menuText.text = "Leave " + SharedSettings.settingsToShare.GameType;
                }
            }
            else{
                menuText.text = "READY UP FOR " + SharedSettings.settingsToShare.GameType;// + SharedSettings.settingsToShare.GameType;
            }
        }
    }
}