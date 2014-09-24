using UnityEngine;
using System.Collections;

namespace MultiPlayer 
{
	public class PlayerNetworking : Photon.MonoBehaviour 
	{
		
		void Awake()
		{
			Debug.Log("SPAWN PLAYER");
		}
		void Start ()   
		{		    	
			transform.SendMessage ("IsLocalPlayer", photonView.isMine, SendMessageOptions.DontRequireReceiver);
			
			if (photonView.isMine)
			{
				Camera.main.SendMessage("SetLocalPlayerTransform", transform, SendMessageOptions.DontRequireReceiver);
				
			}		
		}
		
		
		void Update()
		{
			//		if (!photonView.isMine)
			//		{
			//			Vector3 pos = tMesh.transform.position - new Vector3(-10, 5, 10);// Camera.main.transform.position;
			//			// pos = tMesh.transform.position + pos;
			//			tMesh.transform.LookAt(pos);
			//		}
		}
		
		void OnPhotonInstantiate(PhotonMessageInfo info)
		{       
			NetworkGameManager.AddPlayer(transform);
		}
		void OnDestroy()
		{	
			NetworkGameManager.RemovePlayer(transform);
		}
	}
}


