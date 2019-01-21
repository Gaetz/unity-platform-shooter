using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastMoveDirection {

	LayerMask layerMask;
	Vector2 direction;
	Vector2[] offsetPoints;
	float addLength;

	int rayCount = 4;
	int raySpacing;

	public RaycastMoveDirection(LayerMask layerMask, Vector2 start, Vector2 end, Vector2 direction, Vector2 perpendicularInset, Vector2 parallelInset) {
		this.layerMask = layerMask;
		this.direction = direction;
		offsetPoints = new Vector2[] {
			start + parallelInset + perpendicularInset,
			end + parallelInset - perpendicularInset
		};
		addLength = parallelInset.magnitude;
	}

	private RaycastHit2D Raycast(Vector2 start, Vector2 direction, float length, LayerMask layerMask) {
		Debug.DrawLine(start, start + direction * length, Color.blue);
		return Physics2D.Raycast(start, direction, length, layerMask);
	}

	public float Cast(Vector2 origin, float distance) {
		float minDistance = distance;
		foreach(var offset in offsetPoints) {
			RaycastHit2D hit = Raycast(origin + offset, direction, distance + addLength, layerMask);
			if(hit.collider != null) {
				// Ignore collision if going in the right direction for a ThroughPlatform
				// else collide
				ThroughPlatform tp = hit.collider.GetComponent<ThroughPlatform>();
				if(tp == null || tp.permitDirection != direction) {
					minDistance = Mathf.Min(minDistance, hit.distance - addLength);
				}
			}
		}
		return minDistance;
	}
}
