using System.Collections.Generic;
using System.Linq;
using QSBGameModes.GameManagement.PlayerManagement;
using QSB.Player;

namespace QSBGameModes.GameManagement.RoleSelection
{
    public class RoleSelector
    {
        static HashSet<uint> lastSeletedSeekers = new();

        public static void Reset()
        {
            lastSeletedSeekers.Clear();
        }

        public static HashSet<uint> SelectRoles(HashSet<GameModeInfo> players, int numberOfSeekers, int seed = -1, bool tryToNotRepeatPreviousSeekers = false)
        {
            System.Random rnd;
            if (seed > 0)
            {
                rnd = new(seed);
            }
            else
            {
                rnd = new();
            }


            HashSet<uint> seekers = new();
            while (seekers.Count < numberOfSeekers && seekers.Count < players.Count)
            {
                int newPosition = rnd.Next(0, players.Count);
                var playerId = players.ElementAt(newPosition).Info.PlayerId;

                if (!tryToNotRepeatPreviousSeekers || //If we don't care about repeating, just add it
                    (tryToNotRepeatPreviousSeekers && //If we do care, then we check to see if playerInfo was a seeker
                    (!lastSeletedSeekers.Contains(playerId) || (players.Count - lastSeletedSeekers.Count - seekers.Count <= 0))))
                {
                    //But in the situation where we don't have enough players (players.Count - lastSeekers.Count - seekers.Count <= 0) that weren't seekers, accept players that were:
                    //players.Count - lastSeekers.Count -> amount of non repeating seekers
                    //seekers.Count -> amount of current selected seekers
                    //players.Count - lastSeekers.Count - seekers.Count -> amount of non repeating seekers left to be selected, if it is <= 0, then we must use repeating seekers
                    seekers.Add(playerId);
                }
            }

            lastSeletedSeekers = seekers;
            return seekers;
        }
    }
}
