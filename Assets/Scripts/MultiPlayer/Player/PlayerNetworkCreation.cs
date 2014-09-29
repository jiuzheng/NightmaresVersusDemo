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
			else
			{
				tMesh.text = photonView.owner.name;
			}
		}

		void Update()
		{
			if (!photonView.isMine)
			{
				Vector3 pos = tMesh.transform.position - textMeshDirection;
				tMesh.transform.LookAt(pos);
			}
		}
		
		void OnPhotonInstantiate(PhotonMessageInfo info)
		{
			photonPlayerID = info.sender.ID;
			NetworkGameManager.AddPlayer(info.sender.ID, transform);
		}

		void OnDestroy()
		{	
			Debug.Log ("Player is destroyed");
			NetworkGameManager.RemovePlayer(photonPlayerID);
		}
	}
}


