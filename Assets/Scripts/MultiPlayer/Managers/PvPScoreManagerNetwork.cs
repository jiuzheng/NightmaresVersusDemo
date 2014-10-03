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
			scoreDict[photonPlayerID] = 0;

			if (PhotonNetwork.isMasterClient)
			{	
				photonView.RPC ("updateScoreDict", PhotonTargets.Others, scoreDict);
			}

			updateScoreLabels ();
		}

		// Should be an RPC call made only by the master client. Updates the score dict
		override public void AddScore (int score, int photonPlayerID)
		{
			if (PhotonNetwork.isMasterClient)
			{
				scoreDict[photonPlayerID] += score;

				photonView.RPC ("updateScoreDict", PhotonTargets.Others, scoreDict);
			}

			updateScoreLabels ();
		}

		void updateScoreLabels ()
		{
			myScoretext.text = "Your Score: " + scoreDict [PhotonNetwork.player.ID];

			string otherPlayerScoreString = "";
			foreach (PhotonPlayer player in PhotonNetwork.otherPlayers)
			{
				otherPlayerScoreString += player.name + ": " + scoreDict[player.ID];
			}
			
			otherPlayerScoreText.text = otherPlayerScoreString;
		}
    }
}