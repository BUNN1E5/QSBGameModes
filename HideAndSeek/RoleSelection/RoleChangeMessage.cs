using Mirror;
using QSB.Messaging;
using QSB.Player;

namespace HideAndSeek.RoleSelection
{
	//For when there is a change of player state, during the match
    internal class RoleChangeMessage : QSBMessage
	{
		private uint playerId;
		PlayerState playerState;

		public RoleChangeMessage(uint playerId, PlayerState playerState)
		{
			this.playerId = playerId;
			this.playerState = playerState;
		}

		public override void Serialize(NetworkWriter writer)
		{
			base.Serialize(writer);
			writer.Write(playerId);
			writer.Write(playerState);
		}

		public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize(reader);
			playerId = reader.Read<uint>();
			playerState = reader.Read<PlayerState>();
		}

		public override void OnReceiveLocal() => OnReceiveRemote();

		public override void OnReceiveRemote()
        {
			if (QSBPlayerManager.PlayerExists(playerId))
			{
				var playerInfo = QSBPlayerManager.GetPlayer(playerId);
				PlayerManager.SetPlayerState(playerInfo, playerState);
			}
		}
	}
}
