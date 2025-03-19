using Mirror;
using QSB.Messaging;
using QSB.Player;
using QSBGameModes.GameManagement.PlayerManagement;
using QSBGameModes.Menu;

namespace QSBGameModes.GameManagement.RoleSelection
{
    //For when there is a change of player state, during the match
    internal class RoleChangeMessage : QSBMessage
    {
        private uint playerId;
        PlayerManagement.PlayerState playerState;

        public RoleChangeMessage(PlayerInfo player, PlayerManagement.PlayerState playerState)
        {
            this.playerId = player.PlayerId;
            this.playerState = playerState;
        }

        public RoleChangeMessage(uint playerId, PlayerManagement.PlayerState playerState)
        {
            this.playerId = playerId;
            this.playerState = playerState;
        }

        public override void Serialize(NetworkWriter writer)
        {
            base.Serialize(writer);
            writer.WriteUInt(playerId);
            writer.WriteInt((int)playerState);
        }

        public override void Deserialize(NetworkReader reader)
        {
            base.Deserialize(reader);
            playerId = reader.ReadUInt();
            playerState = (PlayerManagement.PlayerState)reader.ReadInt();
        }

        public override void OnReceiveLocal() => OnReceiveRemote();

        public override void OnReceiveRemote()
        {
            if (QSBPlayerManager.PlayerExists(playerId))
            {
                Utils.WriteLine($"Received RoleChangeMessage with role {(PlayerManagement.PlayerState)playerState} for {playerId}");
                var playerInfo = QSBPlayerManager.GetPlayer(playerId);
                PlayerManager.SetPlayerState(playerInfo, playerState);
                GameModeMenu.UpdateGUI();
            }
        }
    }
}
