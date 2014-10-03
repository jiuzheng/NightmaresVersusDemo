using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MultiPlayer
{
	public class PvPTimeAttackGameOverManagerNetwork : AbstractGameOverManager 
	{

		public float tournamentTimeLimit = 60f;				// When the timer expires the tournament ends
		public float reviveDelay = 6f;						// In time attack mode, each player revives automatically when killed

		float reviveTimer = 0f;								// The count down timer for revival
		Text reviveText;									// Reference to the revive text child.
		Text gameTimerText;									// Reference to the game timer text.
		Animator anim;                          			// Reference to the animator component.
		
		// Use this for initialization
		void Awake () 
		{
			reviveText = GameObject.Find ("RevivalText").GetComponent <Text> ();
			gameTimerText = GameObject.Find ("GameTimerText").GetComponent <Text > ();
			anim = GetComponent <Animator> ();
		}
		
		// Update is called once per frame
		void Update () 
		{
			if (reviveTimer > 0f)
			{
				// If revival timer has a positive number on it, show it
				reviveText.enabled = true;
				reviveTimer -= Time.deltaTime;
				reviveText.text = "Revive in " + (int)reviveTimer +"s";
			}
			else
			{
				// else the player is still alive
				reviveText.enabled = false;
			}

			// Count down the time attack timer
			tournamentTimeLimit -= Time.deltaTime;
			gameTimerText.text = "Game End In " + (int)tournamentTimeLimit + "s";

			if (tournamentTimeLimit < 10f)
			{
				// If less than 10 secs, show red
				gameTimerText.color = Color.red;
			}

			if (tournamentTimeLimit <= 0f)
			{
				// Tell all clients game over
				gameTimerText.enabled = false;
				if (PhotonNetwork.isMasterClient)
				{
					photonView.RPC ("processGameOver", PhotonTargets.All);
				}
			}
		}

		override public void NotifyPlayerDead (int photonPlayerID)
		{
			// Tell all clients this
			if (PhotonNetwork.isMasterClient)
			{
				photonView.RPC ("revivePlayerWithDelay", PhotonTargets.All, photonPlayerID, this.reviveDelay);
			}
		}

		[RPC] void revivePlayerWithDelay (int photonPlayerID, float reviveDelay)
		{
			Transform player = NetworkGameManager.playersDict[photonPlayerID];
			PlayerHealthNetwork playerHealth = player.gameObject.GetComponent <PlayerHealthNetwork> ();
			if (playerHealth != null)
			{
				StartCoroutine(playerHealth.ReviveAfterSeconds (reviveDelay));
				reviveTimer = reviveDelay;
			}
		}

		[RPC] void processGameOver ()
		{
			// ... tell the animator the game is over.
			anim.SetTrigger ("GameOver");

			StartCoroutine (ProceedToEndScene ());
		}

		IEnumerator ProceedToEndScene ()
		{
			yield return new WaitForSeconds (5);
			Application.LoadLevel(3);
		}
	}
}
