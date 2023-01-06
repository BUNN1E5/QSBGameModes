using HideAndSeek.GameManagement;
using Mirror;
using QSB.Messaging;
using QSB.Player;

namespace HideAndSeek.Messages;

public class PlayerSignalSizeMessage : QSBMessage{
    HideAndSeekInfo info;
    float size;
    
    public PlayerSignalSizeMessage(HideAndSeekInfo info, float size){
        this.info = info;
        this.size = size;
    }
    public override void Serialize(NetworkWriter writer){
        base.Serialize(writer);
        writer.Write(info);
        writer.Write(size);
    }

    public override void Deserialize(NetworkReader reader){
        base.Deserialize(reader);
        info = reader.Read<HideAndSeekInfo>();
        size = reader.Read<float>();
    }

    public override void OnReceiveRemote(){
        PlayerManager.SetPlayerSignalSize(info, size);
    }
    
    //public override void OnReceiveLocal() => OnReceiveRemote();

}