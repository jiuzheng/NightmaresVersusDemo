using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace MultiPlayer
{
    public class PvPScoreManagerNetwork : AbstractScoreManager
    {
		Text text;

        void Awake ()
        {
			// Reset the scoreDict.
			scoreDict = new Dictionary<int, int> ();
			
			// Photon player actorID.
			localPlayerID = PhotonNetwork.player.ID;

			// Set the reference.
			text = GetComponent <Text> ();
        }


        void Update ()
        {
			text.text = "Score: " + scoreDict[localPlayerID];
			string s = "";
			foreach (KeyValuePair <int, int> entry in scoreDict)
			{
				s += " Player " + entry.Key + ": " + entry.Value;
			}

			text.text = s;
        }

		// Add Player should set up references on the new player's health bar, text label etc.
		override public void AddPlayer (int photonPlayerID)
		{
			scoreDict[photonPlayerID] = 0;

			if (PhotonNetwork.isMasterClient)
			{	
				photonView.RPC ("updateScoreDict", PhotonTargets.Others, scoreDict);
			}
		}

		// Should be an RPC call made only by the master client. Updates the score dict
		override public void AddScore (int score, int photonPlayerID)
		{
			if (PhotonNetwork.isMasterClient)
			{
				scoreDict[photonPlayerID] += score;

				photonView.RPC ("updateScoreDict", PhotonTargets.Others, scoreDict);
			}
		}
    }
}