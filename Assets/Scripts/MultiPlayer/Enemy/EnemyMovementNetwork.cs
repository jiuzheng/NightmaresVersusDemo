using UnityEngine;
using System.Collections;

namespace MultiPlayer
{
    public class EnemyMovementNetwork : Photon.MonoBehaviour
    {
		EnemyTargetNetwork enemyTarget;     // Reference to the target player.
        EnemyHealthNetwork enemyHealth;     // Reference to this enemy's health.
        NavMeshAgent nav;               	// Reference to the nav mesh agent.
		
        void Awake ()
        {
            // Set up the references.
			enemyTarget = GetComponent <EnemyTargetNetwork> ();
            enemyHealth = GetComponent <EnemyHealthNetwork> ();
            nav = GetComponent <NavMeshAgent> ();
        }

		void Start ()
		{

		}
		
		void LateUpdate ()
        {			
			// If the enemy and the player have health left...
			if(/* enemyHealth.currentHealth > 0 && */ enemyTarget.TargetedPlayer != null && enemyTarget.PlayerHealth.currentHealth > 0)
            {
                // ... set the destination of the nav mesh agent to the player.
				nav.enabled = true;
				nav.SetDestination (enemyTarget.TargetedPlayer.position);
            }
            // Otherwise...
            else
            {
                // ... disable the nav mesh agent.
                nav.enabled = false;
            }
        }

		void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.isWriting)
			{
				stream.SendNext(transform.position);
				stream.SendNext(transform.rotation);
			}
			else
			{
				transform.position = (Vector3)stream.ReceiveNext();
				transform.rotation = (Quaternion)stream.ReceiveNext();
			}
		}
    }
}