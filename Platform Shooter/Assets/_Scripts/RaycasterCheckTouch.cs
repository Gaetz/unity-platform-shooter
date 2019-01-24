using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycasterCheckTouch : Raycaster {

	public RaycasterCheckTouch(LayerMask layerMask, Vector2 direction, float margin, RaycastState state) 
	: base(layerMask, direction, margin, state, RaycastOrigin.CENTER_LEFT)
	{
		offsetPoints = new Vector2[] {
			raycastStart.centerLeft,
			raycastStart.centerRight
		};
	}

	private RaycastHit2D Raycast(Vector2 start, Vector2 direction, float length, LayerMask layerMask) {
		//Debug.DrawLine(start, start + direction * length, Color.red);
		return Physics2D.Raycast(start, direction, length, layerMask);
	}

	public float Cast(Vector2 origin, Rect rect) {
		MovingPlatform hitMovingPlatform = null;
		float minDistance = float.PositiveInfinity;
		foreach(var offset in offsetPoints) {
			RaycastHit2D hit = Raycast(origin + offset, direction, rect.size.y / 2 + margin, layerMask);
			if(hit.collider != null) {
				hitMovingPlatform = hit.collider.GetComponent<MovingPlatform>();
				float realDistance = hit.distance - rect.size.y / 2;
				if(realDistance < minDistance) {
					minDistance = realDistance;
					if(Mathf.Approximately(minDistance, 0)) {
						minDistance = 0;
					}
				}
			}
		}
		if(hitMovingPlatform)
			state.MovingPlatform = hitMovingPlatform;
		return minDistance;
	}
}
