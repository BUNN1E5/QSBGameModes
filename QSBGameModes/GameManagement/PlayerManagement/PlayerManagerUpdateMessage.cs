using System.Collections.Generic;
using System.Linq;
using Mirror;
using QSB.Messaging;
using QSB.Player;

namespace QSBGameModes.GameManagement.PlayerManagement;
public class PlayerManagerUpdateMessage : QSBMessage{

    private uint[] playerIds;
    private PlayerState[] playerStates;

    public PlayerManagerUpdateMessage(params HideAndSeekInfo[] HideAndSeekInfos){
        playerIds = new uint[HideAndSeekInfos.Length];
        playerStates = new PlayerState[HideAndSeekInfos.Length];
        for (int i = 0; i < HideAndSeekInfos.Length; i++){
            playerIds[i] = HideAndSeekInfos[i].Info.PlayerId;
            playerStates[i] = HideAndSeekInfos[i].State;
        }
    }

    public override void Serialize(NetworkWriter writer){
        base.Serialize(writer);
        writer.WriteInt(playerIds.Length);
        for (int i = 0; i < playerIds.Length; i++){
            writer.WriteUInt(playerIds[i]);
            writer.WriteInt((int)playerStates[i]);
        }
    }

    public override void Deserialize(NetworkReader reader){
        base.Deserialize(reader);
        int lengh = reader.ReadInt();
        playerIds = new uint[lengh];
        playerStates = new PlayerState[lengh];
        for (int i = 0; i < playerIds.Length; i++){
            playerIds[i] = reader.ReadUInt();
            playerStates[i] = (PlayerState)reader.ReadInt();
        }
    }

    public override void OnReceiveLocal() => OnReceiveRemote();

    public override void OnReceiveRemote(){
        for (int i = 0; i < playerIds.Length; i++) {
            PlayerInfo info = QSBPlayerManager.GetPlayer(playerIds[i]);
            HideAndSeekInfo hideAndSeekInfo = new() { Info = info, State = playerStates[i] };
            PlayerManager.playerInfo[info] = hideAndSeekInfo;
            PlayerManager.SetupPlayer(info);
        }
    }
}