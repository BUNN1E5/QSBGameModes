using System;
using Mirror;
using OWML.Common;
using QSB.Messaging;
using QSBGameModes.Menu;

namespace QSBGameModes.GameManagement{
    public static partial class GameManager{
        public class GameStateMessage : QSBMessage{
            private GameState newState;
            
            public GameStateMessage(GameState newState){
                this.newState = newState;
                GameManager.gameMode.stateTime = System.DateTime.Now.Millisecond / 1000f;
            }

            public override void Serialize(NetworkWriter writer){
                base.Serialize(writer);
                writer.WriteFloat(GameManager.gameMode.stateTime);
                writer.WriteInt((int)newState);
            }

            public override void Deserialize(NetworkReader reader){
                base.Deserialize(reader);
                GameManager.gameMode.stateTime = reader.ReadFloat();
                newState = (GameState)reader.ReadInt();
            }

            public override void OnReceiveLocal() => OnReceiveRemote();

            public override void OnReceiveRemote(){
                _state = newState;
                Utils.WriteLine($"Start time set to {GameManager.gameMode.stateTime:0.##}", MessageType.Debug);
                Utils.WriteLine($"Current time is {System.DateTime.Now.Millisecond / 1000f:0.##}", MessageType.Debug);
                Utils.WriteLine("Game State set to " + _state, MessageType.Debug);
                GameManager.gameMode.OnStateChange(newState);
                GameModeMenu.UpdateGUI();
            }
        }
    }
}