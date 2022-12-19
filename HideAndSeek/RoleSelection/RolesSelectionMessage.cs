using Mirror;
using QSB.Messaging;
using QSB.Player;
using System.Collections.Generic;
using System.Linq;

namespace HideAndSeek.HidersAndSeekersSelection
{
	//For when the roles are selected, before the match begins
    public class RolesSelectionMessage : QSBMessage
    {
		private List<uint> seekers;

		public RolesSelectionMessage(uint[] seekers)
		{	
			this.seekers = seekers.ToList();
		}

		public override void Serialize(NetworkWriter writer)
		{
			base.Serialize(writer);
			writer.Write(seekers.ToArray());
		}

		public override void Deserialize(NetworkReader reader)
		{
			base.Deserialize(reader);
			seekers = reader.Read<uint[]>().ToList();
		}


		public override void OnReceiveLocal() => OnReceiveRemote();
		public override void OnReceiveRemote()
		{
			for(int i = 0; i< QSBPlayerManager.PlayerList.Count; i++)
			{
				var player = QSBPlayerManager.PlayerList[i];
				PlayerState playerState = PlayerState.Hiding;

				if (seekers.Contains(player.PlayerId)){
					playerState = PlayerState.Seeking;
				}

				PlayerManager.SetPlayerState(player, playerState);
			}
		}
	}
}
