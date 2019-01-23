using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastCheckTouch {

	LayerMask layerMask;
	Vector2 direction;
	Vector2[] offsetPoints;
	float raycastLen;
	RaycastState state;

	public RaycastCheckTouch(LayerMask layerMask, Vector2 start, Vector2 end, 
			Vector2 direction, Vector2 perpendicularInset, Vector2 parallelInset, float checkLength, RaycastState state) 
	{
		this.layerMask = layerMask;
		this.direction = direction;
		offsetPoints = new Vector2[] {
			start + parallelInset + perpendicularInset,
			end + parallelInset - perpendicularInset
		};
		raycastLen = parallelInset.magnitude + checkLength;
		this.state = state;
	}

	private RaycastHit2D Raycast(Vector2 start, Vector2 direction, float length, LayerMask layerMask) {
		Debug.DrawLine(start, start + direction * length, Color.red);
		return Physics2D.Raycast(start, direction, length, layerMask);
	}

	public bool Cast(Vector2 origin) {
		MovingPlatform hitMovingPlatform = null;
		bool hitSomething = false;
		foreach(var offset in offsetPoints) {
			RaycastHit2D hit = Raycast(origin + offset, direction, raycastLen, layerMask);
			if(hit.collider != null) {
				hitMovingPlatform = hit.collider.GetComponent<MovingPlatform>();
				hitSomething =  true;
			}
		}
		if(hitMovingPlatform)
			state.MovingPlatform = hitMovingPlatform;
		return hitSomething;
	}
}
