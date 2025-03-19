using Mirror;
using QSB.Messaging;
using QSB.Player;

namespace QSBGameModes.Messages
{
    public class LocationWarpMessage : QSBMessage
    {
        private uint playerId;
        private int spawnLocation;
        public LocationWarpMessage(uint playerId, SpawnLocation spawnLocation)
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
                PlayerSpawner playerSpawner = UnityEngine.Object.FindObjectOfType<PlayerSpawner>();
                playerSpawner.DebugWarp(playerSpawner.GetSpawnPoint((SpawnLocation)spawnLocation));
            }
        }
    }
}
