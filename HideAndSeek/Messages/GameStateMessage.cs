using HideAndSeek.GameManagement;
using Mirror;
using QSB.Messaging;

namespace HideAndSeek.Messages{
    public class GameStateMessage : QSBMessage{
        private GameState state;
        public GameStateMessage(GameState state){
            this.state = state;
        }
        public override void Serialize(NetworkWriter writer){
            base.Serialize(writer);
            writer.Write(state);
        }

        public override void Deserialize(NetworkReader reader){
            base.Deserialize(reader);
            state = reader.Read<GameState>();
        }

        public override void OnReceiveLocal() => OnReceiveRemote();

        public override void OnReceiveRemote(){
            GameManager.state = state;
        }
    }
}