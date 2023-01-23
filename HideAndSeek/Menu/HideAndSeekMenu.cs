using HideAndSeek.GameManagement;
using HideAndSeek.GameManagement.PlayerManagement;
using QSB;
using QSB.Player;
using QSB.WorldSync;
using UnityEngine.UI;

namespace HideAndSeek.Menu;

public static class HideAndSeekMenu{
    private static Button menuButton;
    private static Text menuText;

    private static Button spectateButton;
    private static Text spectateText;

    public static void SetupPauseButton(){
        Utils.ModHelper.Events.Unity.FireOnNextUpdate(() => {
            Utils.WriteLine("Adding button to menu");
            //Setup the Host button 
            //TODO :: MAKE BETTER GUI FOR SETTING UP GAME
            if (QSBCore.IsHost){ //TODO :: CHANGE ORDER OF HIDE AND SEEK INTERACT BUTTON
                menuButton = QSBCore.MenuApi.PauseMenu_MakeSimpleButton("START " + SharedSettings.settingsToShare.GameType); //HIDE AND SEEK INTERACT BUTTON
                menuText = menuButton.GetComponentInChildren<Text>();
                Button.ButtonClickedEvent c_event = new Button.ButtonClickedEvent();
                c_event.AddListener(HideAndSeek.StartHideAndSeek);
                
                menuButton.onClick = c_event;                
            } else {
                menuButton = QSBCore.MenuApi.PauseMenu_MakeSimpleButton("JOIN " + SharedSettings.settingsToShare.GameType); //HIDE AND SEEK INTERACT BUTTON
                menuText = menuButton.GetComponentInChildren<Text>();
                Button.ButtonClickedEvent c_event = new Button.ButtonClickedEvent();
                c_event.AddListener(HideAndSeek.JoinHideAndSeek);
                        
                menuButton.onClick = c_event;       
            }
        });
    }

    //TODO :: SETUP THE FUNCTIONALITY FOR THE SMART BUTTON
    public static void UpdateGUI(){
        if (QSBCore.IsHost){
            if (GameManager.state == GameState.Stopped){
                menuText.text = "Start " + SharedSettings.settingsToShare.GameType;
            } else{
                menuText.text = "Stop " + SharedSettings.settingsToShare.GameType;
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
                menuText.text = "Ready Up For " + SharedSettings.settingsToShare.GameType;
            }
        }
    }
}