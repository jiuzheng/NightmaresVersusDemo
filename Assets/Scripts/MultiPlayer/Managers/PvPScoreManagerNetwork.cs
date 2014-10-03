using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace MultiPlayer
{
    public class PvPScoreManagerNetwork : AbstractScoreManager
    {
		public Text myScoretext;
		public Text otherPlayerScoreText;

        void Awake ()
        {
			// Reset the scoreDict.
			scoreDict = new Dictionary<int, int> ();
			
			// Photon player actorID.
			localPlayerID = PhotonNetwork.player.ID;
        }


        void Update ()
        {

        }

		// Add Player should set up references on the new player's health bar, text label etc.
		override public void AddPlayer (int photonPlayerID)
		{
			// Create new entry in score dict
			scoreDict[photonPlayerID] = 0;

			if (PhotonNetwork.isMasterClient)
			{	
				// Ask all other players sync the score dict
				photonView.RPC ("updateScoreDict", PhotonTargets.Others, scoreDict);
			}

			updateScoreLabels ();
		}

		// Should be an RPC call made only by the master client. Updates the score dict
		override public void AddScore (int score, int photonPlayerID)
		{
			if (PhotonNetwork.isMasterClient)
			{
				// Updates the score only when I am master client
				scoreDict[photonPlayerID] += score;

				// Ask all other players sync the score dict
				photonView.RPC ("updateScoreDict", PhotonTargets.Others, scoreDict);
			}

			updateScoreLabels ();
		}

		void updateScoreLabels ()
		{
			// Updates local player's score text label
			myScoretext.text = "Your Score: " + scoreDict [localPlayerID];

			string otherPlayerScoreString = "";
			foreach (PhotonPlayer player in PhotonNetwork.otherPlayers)
			{
				// I am being lazy here...
				otherPlayerScoreString += player.name + ": " + scoreDict[player.ID] + "\n"; 
			}

			// Updates other players' score text label
			otherPlayerScoreText.text = otherPlayerScoreString;
		}
    }
}