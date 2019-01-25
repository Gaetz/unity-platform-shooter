using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {

	void OnDrawGizmos()
	{
	#if UNITY_EDITOR
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(transform.position, 0.05f);
			Gizmos.DrawLine(transform.position - new Vector3(-0.1f, 0, 0), transform.position - new Vector3(0.1f, 0, 0));
			Gizmos.DrawLine(transform.position - new Vector3(0, -0.1f, 0), transform.position - new Vector3(0, 0.1f, 0));
			Gizmos.color = Color.white;
	#endif
	}

}
