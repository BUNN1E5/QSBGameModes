using System.Collections.Generic;
using System.Linq;
using HideAndSeek.GameManagement.PlayerManagement;
using Mirror;
using QSB.Messaging;
using QSB.Player;

namespace HideAndSeek.GameManagement.RoleSelection
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
			writer.Write(seekers.ToArray());
			writer.Write(hiders.ToArray());
			writer.Write(spectators.ToArray());
		}

		public override void Deserialize(NetworkReader reader){
			base.Deserialize(reader);
			seekers = reader.Read<uint[]>().ToList();
			hiders = reader.Read<uint[]>().ToList();
			spectators = reader.Read<uint[]>().ToList();
		}
		
		public override void OnReceiveLocal() => OnReceiveRemote();
		public override void OnReceiveRemote(){
			foreach (uint seeker in seekers){
				if (QSBPlayerManager.PlayerExists(seeker))
					return;
				new RoleChangeMessage(seeker, (uint)PlayerManagement.PlayerState.Seeking).Send();
			}

			foreach (uint hider in hiders){
				if (QSBPlayerManager.PlayerExists(hider))
					return;
				new RoleChangeMessage(hider, (uint)PlayerManagement.PlayerState.Seeking).Send();
			}
			
			foreach (uint spectator in spectators){
				if (QSBPlayerManager.PlayerExists(spectator))
					return;
				new RoleChangeMessage(spectator, (uint)PlayerManagement.PlayerState.Seeking).Send();
			}
		}
	}
}
