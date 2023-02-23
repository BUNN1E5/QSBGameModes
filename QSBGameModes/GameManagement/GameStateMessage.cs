using Mirror;
using QSB.Messaging;

namespace QSBGameModes.GameManagement{
    public static partial class GameManager{
        public class GameStateMessage : QSBMessage{
            private GameState state;
            public GameStateMessage(GameState state){
                this.state = state;
            }
            public override void Serialize(NetworkWriter writer){
                base.Serialize(writer);
                writer.WriteInt((int)state);
            }

            public override void Deserialize(NetworkReader reader){
                base.Deserialize(reader);
                state = (GameState)reader.ReadInt();
            }

            public override void OnReceiveLocal() => OnReceiveRemote();

            public override void OnReceiveRemote(){
                _state = state;
                GameManager.gameMode.OnStateChange(state);
            }
        }
    }
}