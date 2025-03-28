using QSBGameModes.GameManagement;
using Mirror;
using QSB.Messaging;
using QSB.Player;
using QSBGameModes.GameManagement.PlayerManagement;

namespace QSBGameModes.Messages;
public class PlayerSignalSizeMessage : QSBMessage
{
    uint playerId;
    float size;

    public PlayerSignalSizeMessage(GameModeInfo info, float size)
    {
        playerId = info.Info.PlayerId;
        this.size = size;
    }
    public override void Serialize(NetworkWriter writer)
    {
        base.Serialize(writer);
        writer.WriteUInt(playerId);
        writer.WriteFloat(size);
    }

    public override void Deserialize(NetworkReader reader)
    {
        base.Deserialize(reader);
        playerId = reader.ReadUInt();
        size = reader.ReadFloat();
    }

    public override void OnReceiveRemote()
    {
        PlayerManager.SetPlayerSignalSize(QSBPlayerManager.GetPlayer(playerId), size);
    }

    //public override void OnReceiveLocal() => OnReceiveRemote();

}