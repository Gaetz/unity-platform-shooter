using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {

	[SerializeField] float speed;
	[SerializeField] Transform[] waypoints;

	int currentWp;
	int NextWp {
		get {
			int next = currentWp + 1;
			return next >= waypoints.Length ? 0 : next;
		}
	}

	void Start () {
		
	}
	
	void FixedUpdate () {
		Vector2 velocity = (waypoints[currentWp].position - transform.position).normalized * speed * Time.deltaTime;
		// Reach next node
		if((transform.position - waypoints[currentWp].position).magnitude <= speed * Time.deltaTime) {
			velocity = Vector2.ClampMagnitude(velocity, (waypoints[currentWp].position - transform.position).magnitude);
			currentWp = NextWp;
		}
		transform.Translate(velocity);
	
	}
}
