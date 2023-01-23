using System.Collections.Generic;
using HideAndSeek.GameManagement;
using Mirror;
using Newtonsoft.Json;
using QSB.Messaging;

namespace HideAndSeek.Messages;

public class SharedSettingsMessage : QSBMessage{
    private SettingsPayload payload;
    
    public SharedSettingsMessage(SettingsPayload payload) {
        this.payload = payload;
    }
    
    public override void Serialize(NetworkWriter writer){
        base.Serialize(writer);
        writer.Write(payload);
    }

    public override void Deserialize(NetworkReader reader){
        base.Deserialize(reader);
        payload = reader.Read<SettingsPayload>();
    }

    public override void OnReceiveRemote(){
        //if we are receiving a message we are not the host
        SharedSettings.settingsToShare = payload;
        SharedSettings.LoadSettings();
    }
    
    public override void OnReceiveLocal() => OnReceiveRemote();
}