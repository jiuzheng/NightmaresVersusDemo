using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace MultiPlayer
{
    public abstract class AbstractScoreManager : Photon.MonoBehaviour
    {
		protected Dictionary <int, int> scoreDict;    // Key: photon player actorID. Value: corresponding score.
		protected int localPlayerID;                  // Local player's Photon player actorID.

		void Awake () {}

		void Update () {}

		// Sync the score dictionary over network
		[RPC] protected void updateScoreDict (Dictionary <int, int> newDict)
		{
			scoreDict = newDict;
		}

		// Add Player should set up references on the new player's health bar, text label etc.
		abstract public void AddPlayer (int photonPlayerID);

		// Should make an RPC call made only by the master client. Updates the score dict
		abstract public void AddScore (int score, int photonPlayerID);
    }
}