using System.Collections.Generic;
using System.Linq;
using QSBGameModes.GameManagement.PlayerManagement;
using Mirror;
using QSB.Messaging;
using QSB.Player;

namespace QSBGameModes.GameManagement.RoleSelection
{
	//For when the roles are selected, before the match begins
    public class RolesSelecedMessage : QSBMessage
    {
	    private bool isSelected;
		public RolesSelecedMessage(bool isSelected)
		{
			this.isSelected = isSelected;
		}

		public override void Serialize(NetworkWriter writer){
			base.Serialize(writer);
			writer.WriteBool(isSelected);
		}

		public override void Deserialize(NetworkReader reader){
			base.Deserialize(reader);
		}
		
		public override void OnReceiveLocal() => OnReceiveRemote();
		public override void OnReceiveRemote(){
			foreach (uint seeker in seekers){
				if (QSBPlayerManager.PlayerExists(seeker))
					new RoleChangeMessage(seeker, PlayerManagement.PlayerState.Seeking).Send();
			}

			foreach (uint hider in hiders){
				if (QSBPlayerManager.PlayerExists(hider))
					new RoleChangeMessage(hider, PlayerManagement.PlayerState.Hiding).Send();
			}
			
			foreach (uint spectator in spectators){
				if (QSBPlayerManager.PlayerExists(spectator))
					new RoleChangeMessage(spectator, PlayerManagement.PlayerState.Spectating).Send();
			}
		}
	}
}
