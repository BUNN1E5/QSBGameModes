using Mirror;
using QSB.Messaging;
using QSBGameModes.GameManagement.PlayerManagement;
using QSBGameModes.Menu;

namespace QSBGameModes.GameManagement;

public class SharedSettingsMessage : QSBMessage{
    private string GameType;
    private int StartingSeekers;
    private float PreroundTime;
    private bool AddPlayerSignals;
    private bool Disable6thLocation;
    private bool ActivateAllReturnPlatforms;
    private bool AllowJoinWhileGameInProgress;
    private bool KillHidersOnCatch;
    
    public SharedSettingsMessage(SettingsPayload payload) { 
        GameType = payload.GameType;
        StartingSeekers = payload.StartingSeekers;
        PreroundTime = payload.PreroundTime;
        AddPlayerSignals = payload.AddPlayerSignals;
        Disable6thLocation = payload.Disable6thLocation;
        ActivateAllReturnPlatforms = payload.ActivateAllReturnPlatforms;
        AllowJoinWhileGameInProgress = payload.AllowJoinWhileGameInProgress;
        KillHidersOnCatch = payload.KillHidersOnCatch;
    }
    
    public override void Serialize(NetworkWriter writer){
        base.Serialize(writer);
        writer.Write(GameType);
        writer.Write(StartingSeekers);
        writer.Write(PreroundTime);
        writer.Write(AddPlayerSignals);
        writer.Write(Disable6thLocation);
        writer.Write(ActivateAllReturnPlatforms);
        writer.Write(AllowJoinWhileGameInProgress);
        writer.Write(KillHidersOnCatch);
    }

    public override void Deserialize(NetworkReader reader){
        base.Deserialize(reader);
        GameType = reader.Read<string>();
        StartingSeekers = reader.Read<int>();
        PreroundTime = reader.Read<float>();
        AddPlayerSignals = reader.Read<bool>();
        Disable6thLocation = reader.Read<bool>();
        ActivateAllReturnPlatforms = reader.Read<bool>();
        AllowJoinWhileGameInProgress = reader.Read<bool>();
        KillHidersOnCatch = reader.Read<bool>();
    }

    public override void OnReceiveLocal() => OnReceiveRemote();

    public override void OnReceiveRemote(){
        Utils.WriteLine("Recieved Settings");
        SharedSettings.settingsToShare = new SettingsPayload(){ //This looks so dumb lmao
            GameType = GameType,
            StartingSeekers = StartingSeekers,
            PreroundTime = PreroundTime,
            AddPlayerSignals = AddPlayerSignals,
            Disable6thLocation = Disable6thLocation,
            ActivateAllReturnPlatforms = ActivateAllReturnPlatforms,
            AllowJoinWhileGameInProgress = AllowJoinWhileGameInProgress,
            KillHidersOnCatch = KillHidersOnCatch
        };
        SharedSettings.UpdateSettings();
        SharedSettings.receivedSettings = true; 
        GameModeMenu.UpdateGUI();
        PlayerManager.OnSettingsChange();
    }
    
    //Dont do this cause then we get caught in an infinite loop of updating the settings
    //public override void OnReceiveLocal() => OnReceiveRemote();
}