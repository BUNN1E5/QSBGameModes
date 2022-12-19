using QSB;
using QSB.ClientServerStateSync;
using QSB.Messaging;
using QSB.Player;

namespace HideAndSeek.ArbitraryLocaltionRespawnMessage
{
    public class ArbitraryLocationPlayerRespawnMessage : QSBMessage
	{
		uint playerId;
		public ArbitraryLocationPlayerRespawnMessage(uint playerId) 
		{
			this.playerId = playerId;
		}

		public override void OnReceiveLocal() => OnReceiveRemote();

		public override void OnReceiveRemote()
		{
			if (playerId == QSBPlayerManager.LocalPlayerId)
			{
				RespawnManager.Instance.Respawn();
				ClientStateManager.Instance.OnRespawn();
			}

			RespawnManager.Instance.OnPlayerRespawn(QSBPlayerManager.GetPlayer(Data));
		}

		public static void ClientStateManagerOnRespawn() 
		{
			OWScene currentScene = QSBSceneManager.CurrentScene;
			if (currentScene == OWScene.SolarSystem)
			{
				DebugLog.DebugWrite("RESPAWN!");
				new ClientStateMessage(ClientState.AliveInSolarSystem).Send();
			}
			else
			{
				DebugLog.ToConsole($"Error - Player tried to respawn in scene {currentScene}", MessageType.Error);
			}
		}

		public static void ClientStateManagerOnRespawn()
		{
			OWScene currentScene = QSBSceneManager.CurrentScene;
			if (currentScene == OWScene.SolarSystem)
			{
				DebugLog.DebugWrite("RESPAWN!");
				new ClientStateMessage(ClientState.AliveInSolarSystem).Send();
			}
			else
			{
				DebugLog.ToConsole($"Error - Player tried to respawn in scene {currentScene}", MessageType.Error);
			}
		}
	}
}
