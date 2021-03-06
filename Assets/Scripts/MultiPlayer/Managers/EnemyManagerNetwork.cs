﻿using UnityEngine;

namespace MultiPlayer
{
    public class EnemyManagerNetwork : Photon.MonoBehaviour
    {
        public PlayerHealthNetwork playerHealth;    // Reference to the player's heatlh.
        public GameObject enemy;                	// The enemy prefab to be spawned.
        public float spawnTime = 3f;            	// How long between each spawn.
        public Transform[] spawnPoints;         	// An array of the spawn points this enemy can spawn from.


        void Start ()
        {
            // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
            InvokeRepeating ("Spawn", spawnTime, spawnTime);
        }


        void Spawn ()
        {

			if (!PhotonNetwork.isMasterClient)
			{
				// Let the master client do the spawn work
				return;
			}

			// TO DO: Maybe start spawning when our lightweight server signals game start?
            // If there is no player left in the room
            if(NetworkGameManager.playersDict.Count < 1)
            {
                // ... exit the function.
                return;
            }

            // Find a random index between zero and one less than the number of spawn points.
            int spawnPointIndex = Random.Range (0, spawnPoints.Length);

            // Create an instance of the enemy prefab at the randomly selected spawn point's position and rotation.
			PhotonNetwork.InstantiateSceneObject (enemy.name, spawnPoints[spawnPointIndex].position, spawnPoints[spawnPointIndex].rotation, 0, null);
        }
    }
}