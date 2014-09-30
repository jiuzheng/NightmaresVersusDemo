using UnityEngine;
using System.Collections;

namespace MultiPlayer
{
	public class EnemyTargetNetwork : Photon.MonoBehaviour
	{
		Transform targetedPlayer;				// Reference to the player's position.
		PlayerHealthNetwork playerHealth;		// Reference to the player's health.

		void Start ()
		{
			FindNextTargetPlayer ();
		}
		
		void Update ()
		{
			if (targetedPlayer == null || playerHealth == null || playerHealth.currentHealth <= 0)
			{
				FindNextTargetPlayer ();
			}
		}

		public Transform TargetedPlayer 
		{
			get 
			{
				return targetedPlayer;
			}
		}

		public PlayerHealthNetwork PlayerHealth 
		{
			get 
			{
				return playerHealth;
			}
		}

		public void FindNextTargetPlayer ()
		{
			// Only the master client can update an enemy's target
			if (PhotonNetwork.isMasterClient)
			{
				int closestPlayerID = NetworkGameManager.GetClosestPlayerID (transform.position);
				if (closestPlayerID != -1) // -1 is the default value. Invalid player actorID
				{
					photonView.RPC ("setTarget", PhotonTargets.All, closestPlayerID);
				}
			}
		}

		// It won't work if we send GameObject or Transform over RPC since they are different across game clients
		// But Player actorID remains the same in the Photon room
		[RPC] void setTarget (int newTargetPlayerID)
		{
			Transform newTargetPlayer = NetworkGameManager.playersDict [newTargetPlayerID];
			if (newTargetPlayer != null) 
			{
				targetedPlayer = newTargetPlayer;
				playerHealth = newTargetPlayer.gameObject.GetComponent <PlayerHealthNetwork> ();
			}
		}

	}
}