using UnityEngine;
using System.Collections;

namespace MultiPlayer 
{
	public class PlayerNetworkCreation : Photon.MonoBehaviour 
	{
		private int photonPlayerID;
		private TextMesh tMesh;
		private Vector3 textMeshDirection = new Vector3(0, 0, -10);

		void Awake()
		{
			Debug.Log("SPAWN PLAYER");
			tMesh = gameObject.GetComponentInChildren<TextMesh>();
		}

		void Start ()   
		{	
			if ( photonView.isMine)
			{
				tMesh.gameObject.SetActive(false);
			}
			else // Show the Text Mesh (remote player name)
			{
				tMesh.text = photonView.owner.name;
			}
		}

		void Update()
		{
			// Text mesh always faces the player
			if (!photonView.isMine)
			{
				Vector3 pos = tMesh.transform.position - textMeshDirection;
				tMesh.transform.LookAt(pos);
			}
		}

		// When a player joins a room, add the player to the static Dictionary
		void OnPhotonInstantiate(PhotonMessageInfo info)
		{
			photonPlayerID = info.sender.ID;
			NetworkGameManager.AddPlayer(info.sender.ID, transform);
		}

		// Remove a player from static Dictionary upon leave
		void OnDestroy()
		{	
			Debug.Log ("Player is destroyed");
			NetworkGameManager.RemovePlayer(photonPlayerID);
		}
	}
}


