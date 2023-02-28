using System;
using Mirror;
using OWML.Common;
using QSB.Messaging;
using QSBGameModes.GameManagement.PlayerManagement;
using QSBGameModes.GameManagement.RoleSelection;
using QSBGameModes.Menu;
using UnityEngine;

namespace QSBGameModes.GameManagement{
    public static partial class GameManager{
        public class GameStateMessage : QSBMessage{
            private GameState newState;
            
            public GameStateMessage(GameState newState){
                this.newState = newState;
                GameManager.gameMode.gameStartTime = Time.time;
            }

            public override void Serialize(NetworkWriter writer){
                base.Serialize(writer);
                writer.WriteFloat(GameManager.gameMode.gameStartTime);
                writer.WriteInt((int)newState);
            }

            public override void Deserialize(NetworkReader reader){
                base.Deserialize(reader);
                GameManager.gameMode.gameStartTime = reader.ReadFloat();
                newState = (GameState)reader.ReadInt();
            }

            public override void OnReceiveLocal() => OnReceiveRemote();

            public override void OnReceiveRemote(){
                _state = newState;
                Utils.WriteLine("GameStateMessage :: " + String.Format("Start time set to {0:0.##}", GameManager.gameMode.gameStartTime), MessageType.Debug);
                Utils.WriteLine("GameStateMessage :: " + String.Format("Current time is {0:0.##}", Time.time), MessageType.Debug);
                Utils.WriteLine("GameStateMessage :: " + "Game State set to " + _state, MessageType.Debug);
                GameManager.gameMode.OnStateChange(newState);
                GameModeMenu.UpdateGUI();
            }
        }
    }
}