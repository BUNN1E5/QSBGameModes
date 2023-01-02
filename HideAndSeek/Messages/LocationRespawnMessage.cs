using Mirror;
using QSB.ClientServerStateSync;
using QSB.Messaging;
using QSB.Patches;
using QSB.Player;
using QSB.RespawnSync;

namespace HideAndSeek.Messages
{
    public class LocationRespawnMessage : QSBMessage
	{
		private uint playerId;
		private int spawnLocation;
		public LocationRespawnMessage(uint playerId, SpawnLocation spawnLocation) 
		{
			this.playerId = playerId;
			this.spawnLocation = (int)spawnLocation;
		}
		public override void Serialize(NetworkWriter writer)
		{
			base.Serialize(writer);
			writer.Write(playerId);
			writer.Write(spawnLocation);
		}

		public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize(reader);
			playerId = reader.Read<uint>();
			spawnLocation = reader.Read<int>();
		}

		public override void OnReceiveLocal() => OnReceiveRemote();

		public override void OnReceiveRemote()
		{
			if (playerId == QSBPlayerManager.LocalPlayerId)
			{
				Respawn((SpawnLocation)spawnLocation);
				ClientStateManager.Instance.OnRespawn();
			}

			RespawnManager.Instance.OnPlayerRespawn(QSBPlayerManager.GetPlayer(playerId));
		}

		public static void Respawn(SpawnLocation spawnLocation)
		{
			MapController mapController = UnityEngine.Object.FindObjectOfType<MapController>();
			QSBPatchManager.DoUnpatchType(QSBPatchTypes.RespawnTime);
			PlayerSpawner playerSpawner = UnityEngine.Object.FindObjectOfType<PlayerSpawner>();
			
			//TODO :: MAKE THE PLAYER SPAWN WHERE THEY DIED
			playerSpawner.DebugWarp(playerSpawner.GetSpawnPoint(spawnLocation));
			mapController.ExitMapView();
			PlayerCameraEffectController cameraEffectController = Locator.GetPlayerCamera().GetComponent<PlayerCameraEffectController>();
			cameraEffectController.OpenEyes(1f, false);
		}
	}
}
