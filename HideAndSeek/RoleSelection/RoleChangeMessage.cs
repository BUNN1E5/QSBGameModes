using Mirror;
using QSB.Messaging;
using QSB.Player;

namespace HideAndSeek.HidersAndSeekersSelection
{
	//For when there is a change of player state, during the match
    internal class RoleChangeMessage : QSBMessage
	{
		private uint playerId;
		int newPlayerState;

		public RoleChangeMessage(uint playerId, PlayerState playerState)
		{
			this.playerId = playerId;
			newPlayerState = (int)playerState;
		}

		public override void Serialize(NetworkWriter writer)
		{
			base.Serialize(writer);
			writer.Write(playerId);
			writer.Write(newPlayerState);
		}

		public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize(reader);
			playerId = reader.Read<uint>();
			newPlayerState = reader.Read<int>();
		}

		public override void OnReceiveLocal() => OnReceiveRemote();

		public override void OnReceiveRemote()
        {
			if (QSBPlayerManager.PlayerExists(playerId))
			{
				var playerInfo = QSBPlayerManager.GetPlayer(playerId);
				PlayerManager.SetPlayerState(playerInfo, (PlayerState)newPlayerState);
			}
		}
	}
}
