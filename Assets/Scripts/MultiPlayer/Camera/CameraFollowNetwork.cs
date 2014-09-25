using UnityEngine;
using System.Collections;

namespace MultiPlayer
{
	public class CameraFollowNetwork : MonoBehaviour
	{
		public Transform target;            // The position that that camera will be following.
		public float smoothing = 5f;        // The speed with which the camera will be following.
		
		
		Vector3 offset;                     // The initial offset from the target.
		
		
		void Start ()
		{
			// Calculate the initial offset.
			GameObject playerSpawnPoint = GameObject.Find ("PlayerSpawnPoint");
			offset = transform.position - playerSpawnPoint.transform.position;

			// Get our local player and track camera will track on this
			FindLocalPlayer ();
		}
		
		
		void FixedUpdate ()
		{
			if (target != null) 
			{
				// Create a postion the camera is aiming for based on the offset from the target.
				Vector3 targetCamPos = target.position + offset;
				
				// Smoothly interpolate between the camera's current position and it's target position.
				transform.position = Vector3.Lerp (transform.position, targetCamPos, smoothing * Time.deltaTime);
			}
			else
			{
				// JChen: Local player is created in Awake() medthod in NetworkGameManager
				// Theoretically the player should exist before Camera's Start() method is called
				// So I wonder if this is redundant???
				FindLocalPlayer ();
			}
		}

		void FindLocalPlayer ()
		{
			target = NetworkGameManager.localPlayer;
		}
	}
}
