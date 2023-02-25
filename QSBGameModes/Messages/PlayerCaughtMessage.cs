using Mirror;
using QSB.Messaging;
using QSB.Player;
using QSBGameModes.GameManagement.PlayerManagement;

namespace QSBGameModes.Messages;

public class PlayerCaughtMessage : QSBMessage{
    uint catcher;
    uint catchee;

    public PlayerCaughtMessage(GameModeInfo catcher, GameModeInfo catchee){
        this.catcher = catcher.Info.PlayerId;
        this.catchee = catchee.Info.PlayerId;
    }
    public override void Serialize(NetworkWriter writer){
        base.Serialize(writer);
        writer.WriteUInt(catcher);
        writer.WriteUInt(catchee);
    }

    public override void Deserialize(NetworkReader reader){
        base.Deserialize(reader);
        catcher = reader.ReadUInt();
        catchee = reader.ReadUInt();
    }

    public override void OnReceiveRemote(){
        //TODO :: Do something here?
    }
}