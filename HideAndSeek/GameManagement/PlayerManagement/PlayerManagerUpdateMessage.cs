using System.Collections.Generic;
using System.Linq;
using Mirror;
using QSB.Messaging;
using QSB.Player;

namespace HideAndSeek.GameManagement.PlayerManagement;

public class PlayerManagerUpdateMessage : QSBMessage{

    private HideAndSeekInfo[] HideAndSeekInfos;

    public PlayerManagerUpdateMessage(HideAndSeekInfo[] HideAndSeekInfos){
        this.HideAndSeekInfos = HideAndSeekInfos;
    }

    public override void Serialize(NetworkWriter writer){
        base.Serialize(writer);
        writer.Write(HideAndSeekInfos);
    }

    public override void Deserialize(NetworkReader reader){
        base.Deserialize(reader);
        this.HideAndSeekInfos = reader.Read<HideAndSeekInfo[]>();
    }

    public override void OnReceiveLocal() => OnReceiveRemote();

    public override void OnReceiveRemote(){
        foreach (HideAndSeekInfo info in HideAndSeekInfos){
            PlayerManager.playerInfo[info.Info] = info;
            PlayerManager.SetupPlayer(info.Info);
        }
    }
}