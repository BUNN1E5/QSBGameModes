using System.Collections.Generic;
using System.Linq;
using QSBGameModes.GameManagement.PlayerManagement;
using Mirror;
using QSB.Messaging;
using QSB.Player;

namespace QSBGameModes.GameManagement.RoleSelection
{
	//For when the roles are selected, before the match begins
    public class RolesSelectionMessage : QSBMessage
    {
		private List<uint> seekers;
		private List<uint> hiders;
		private List<uint> spectators;

		public RolesSelectionMessage(uint[] seekers, uint[] hiders, uint[] spectators){
			this.seekers = seekers.ToList();
			this.hiders = hiders.ToList();
			this.spectators = spectators.ToList();
		}

		public override void Serialize(NetworkWriter writer){
			base.Serialize(writer);
			writer.WriteArray(seekers.ToArray());
			writer.WriteArray(hiders.ToArray());
			writer.WriteArray(spectators.ToArray());
		}

		public override void Deserialize(NetworkReader reader){
			base.Deserialize(reader);
			seekers = reader.ReadArray<uint>().ToList();
			hiders = reader.ReadArray<uint>().ToList();
			spectators = reader.ReadArray<uint>().ToList();
		}
		
		public override void OnReceiveLocal() => OnReceiveRemote();
		public override void OnReceiveRemote(){
			foreach (uint seeker in seekers){
				if (QSBPlayerManager.PlayerExists(seeker))
					new RoleChangeMessage(seeker, PlayerManagement.PlayerState.Seeking).Send();
			}

			foreach (uint hider in hiders){
				if (QSBPlayerManager.PlayerExists(hider))
					new RoleChangeMessage(hider, PlayerManagement.PlayerState.Seeking).Send();
			}
			
			foreach (uint spectator in spectators){
				if (QSBPlayerManager.PlayerExists(spectator))
					new RoleChangeMessage(spectator, PlayerManagement.PlayerState.Seeking).Send();
			}
		}
	}
}
