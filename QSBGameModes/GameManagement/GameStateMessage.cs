using Mirror;
using QSB.Messaging;
using QSBGameModes.GameManagement.PlayerManagement;
using QSBGameModes.GameManagement.RoleSelection;
using QSBGameModes.Menu;

namespace QSBGameModes.GameManagement{
    public static partial class GameManager{
        public class GameStateMessage : QSBMessage{
            private GameState state;
            
            public GameStateMessage(GameState state){
                this.state = state;
            }

            public override void Serialize(NetworkWriter writer){
                base.Serialize(writer);
                writer.WriteFloat(GameManager.gameMode.gameStartTime);
                writer.WriteInt((int)state);
            }

            public override void Deserialize(NetworkReader reader){
                base.Deserialize(reader);
                GameManager.gameMode.gameStartTime = reader.ReadFloat();
                state = (GameState)reader.ReadInt();
            }

            public override void OnReceiveLocal() => OnReceiveRemote();

            public override void OnReceiveRemote(){
                _state = state;
                GameManager.gameMode.OnStateChange(state);
                GameModeMenu.UpdateGUI();
            }
        }
    }
}