using UnityEngine;
using System.Collections;

namespace MultiPlayer
{
	public abstract class AbstractGameOverManager : Photon.MonoBehaviour
	{		
		void Awake () {}
		
		void Update () {}
		
		// Tell the GameOver manager that one player is dead. Behavior different in subclasses
		abstract public void NotifyPlayerDead (int photonPlayerID);
	}
}