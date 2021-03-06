﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace MultiPlayer
{
    public class PlayerHealthNetwork : Photon.MonoBehaviour
    {
        public int startingHealth = 100;                            // The amount of health the player starts the game with.
        public int currentHealth;                                   // The current health the player has.
        public Slider healthSlider;                                 // Reference to the UI's health bar.
        public Image damageImage;                                   // Reference to an image to flash on the screen on being hurt.
		public AudioClip hurtClip;                                  // The audio clip to play when the player is hurt.
        public AudioClip deathClip;                                 // The audio clip to play when the player dies.
        public float flashSpeed = 5f;                               // The speed the damageImage will fade at.
        public Color flashColour = new Color(1f, 0f, 0f, 0.1f);     // The colour the damageImage is set to, to flash.


        Animator anim;                                              // Reference to the Animator component.
        AudioSource playerAudio;                                    // Reference to the AudioSource component.
		AbstractGameOverManager gameOverManager;    				// Reference to the Game Over manager. Different between Scenes
        PlayerMovementNetwork playerMovement;                       // Reference to the player's movement.
        PlayerShootingNetwork playerShooting;                       // Reference to the PlayerShooting script.
        bool isDead;                                                // Whether the player is dead.
        bool damaged;                                               // True when the player gets damaged.


        void Awake ()
        {
            // Setting up the references.
            anim = GetComponent <Animator> ();
            playerAudio = GetComponent <AudioSource> ();
            playerMovement = GetComponent <PlayerMovementNetwork> ();
            playerShooting = GetComponentInChildren <PlayerShootingNetwork> ();
			gameOverManager = GameObject.Find("HUDCanvas").GetComponent <AbstractGameOverManager> ();

            // Set the initial health of the player.
            currentHealth = startingHealth;
        }

		//----------------------------------------------------------------
		// TO DO : Make individual health bars for each player
		//----------------------------------------------------------------
		void Start ()
		{
			if (photonView.isMine)
			{
				healthSlider = GameObject.Find ("HealthSlider").GetComponent <Slider> ();
				damageImage = GameObject.Find ("DamageImage").GetComponent <Image> ();
			}
		}


        void Update ()
        {
			// Don't show damage flash when it's not me
			if (!photonView.isMine)
			{
				return;
			}

            // If the player has just been damaged...
            if(damaged)
            {
                // ... set the colour of the damageImage to the flash colour.
                damageImage.color = flashColour;
            }
            // Otherwise...
            else
            {
                // ... transition the colour back to clear.
                damageImage.color = Color.Lerp (damageImage.color, Color.clear, flashSpeed * Time.deltaTime);
            }

            // Reset the damaged flag.
            damaged = false;
        }


        public void TakeDamage (int amount)
        {
            // Set the damaged flag so the screen will flash.
            damaged = true;

			// Play the hurt sound effect.
			playerAudio.Play ();

			// Only the master client should update health and call death
			if (PhotonNetwork.isMasterClient)
			{
				// Reduce the current health by the damage amount.
				photonView.RPC("SetHealth", PhotonTargets.All, currentHealth - amount);
				
				// If the player has lost all it's health and the death flag hasn't been set yet...
				if(currentHealth <= 0 && !isDead)
				{
					Debug.Log ("Player " + photonView.owner.ID + " is Dead");
					// ... it should die.
					photonView.RPC("Death", PhotonTargets.All);
				}
			}
        }

		public IEnumerator ReviveAfterSeconds (float seconds)
		{
			if (PhotonNetwork.isMasterClient)
			{
				yield return new WaitForSeconds(seconds);
				photonView.RPC ("ResetPlayer", PhotonTargets.All);
			}
		}

		[RPC] void SetHealth (int newValue)
		{
			// Updates the current health
			currentHealth = newValue;

			if (healthSlider != null) 
			{
				// Set the health bar's value to the current health.
				healthSlider.value = currentHealth;
			}
		}

        [RPC] void Death ()
        {
            // Set the death flag so this function won't be called while the player is dead.
            isDead = true;

            // Turn off any remaining shooting effects.
            playerShooting.DisableEffects ();

            // Tell the animator that the player is dead.
            anim.SetTrigger ("Die");

            // Set the audiosource to play the death clip and play it (this will stop the hurt sound from playing).
            playerAudio.clip = deathClip;
            playerAudio.Play ();

            // Turn off the movement and shooting scripts.
            playerMovement.enabled = false;
            playerShooting.enabled = false;

			gameOverManager.NotifyPlayerDead (photonView.owner.ID);
        }

		[RPC] void ResetPlayer ()
		{
			// Refill health
			SetHealth (startingHealth);

			isDead = false;

			// Tell the animator that the player is revived.
			anim.SetTrigger ("Revive");

			// Set the audiosource back to the hurt sound
			playerAudio.clip = hurtClip;

			// Turn on the movement and shooting scripts.
			playerMovement.enabled = true;
			playerShooting.enabled = true;
		}
    }
}