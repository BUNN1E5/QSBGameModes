using System.Collections.Generic;
using QSBGameModes.GameManagement;
using QSBGameModes.GameManagement.SharedSettings;
using Mirror;
using Newtonsoft.Json;
using QSB.Messaging;
using QSBGameModes.Menu;

namespace QSBGameModes.GameManagement.SharedSettings;

public class SharedSettingsMessage : QSBMessage{
    string GameType;
    bool Disable6thLocation;
    bool ActivateAllReturnPlatforms;
    bool AllowJoinWhileGameInProgress;
    
    public SharedSettingsMessage(SettingsPayload payload) { 
        GameType = payload.GameType;
        Disable6thLocation = payload.Disable6thLocation;
        ActivateAllReturnPlatforms = payload.ActivateAllReturnPlatforms;
        AllowJoinWhileGameInProgress = payload.AllowJoinWhileGameInProgress;
    }
    
    public override void Serialize(NetworkWriter writer){
        base.Serialize(writer);
        writer.Write(GameType);
        writer.Write(Disable6thLocation);
        writer.Write(ActivateAllReturnPlatforms);
        writer.Write(AllowJoinWhileGameInProgress);
    }

    public override void Deserialize(NetworkReader reader){
        base.Deserialize(reader);
        GameType = reader.Read<string>();
        Disable6thLocation = reader.Read<bool>();
        ActivateAllReturnPlatforms = reader.Read<bool>();
        AllowJoinWhileGameInProgress = reader.Read<bool>();
    }

    public override void OnReceiveRemote(){
        Utils.WriteLine("Recieved Settings");
        SharedSettings.settingsToShare = new SettingsPayload(){ //This looks so dumb lmao
            GameType = GameType,
            Disable6thLocation = Disable6thLocation,
            ActivateAllReturnPlatforms = ActivateAllReturnPlatforms,
            AllowJoinWhileGameInProgress = AllowJoinWhileGameInProgress
        };
        Utils.WriteLine(SharedSettings.settingsToShare.ToString());
        GameModeMenu.UpdateGUI();
    }
    
    public override void OnReceiveLocal() => OnReceiveRemote();
}