using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastMoveDirection {

	LayerMask layerMask;
	Vector2 direction;
	Vector2[] offsetPoints;
	Bounds colliderBounds;
	float addLength;

	
	int rayCount = 4;
	float horizontalRaySpacing;
	float verticalRaySpacing;
	RaycastOrigins raycastOrigins;


	public RaycastMoveDirection(LayerMask layerMask, Vector2 start, Vector2 end, Vector2 direction, 
		Bounds colliderBounds, float skinLen) 
	{
		this.layerMask = layerMask;
		this.direction = direction;
		addLength = skinLen;
		this.colliderBounds = colliderBounds;
		ComputeRaySpacing();
		offsetPoints = ComputeOffsets(direction);
	}

	void ComputeRaySpacing() {
		colliderBounds.Expand(addLength * -2);
		raycastOrigins.bottomLeft = new Vector2(-colliderBounds.size.x / 2, -colliderBounds.size.y/2);
		raycastOrigins.bottomRight = new Vector2(colliderBounds.size.x / 2, -colliderBounds.size.y/2);
		raycastOrigins.topLeft = new Vector2(-colliderBounds.size.x / 2, colliderBounds.size.y/2);
		raycastOrigins.topRight = new Vector2(colliderBounds.size.x / 2, colliderBounds.size.y/2);

		rayCount = Mathf.Clamp(rayCount, 2, int.MaxValue);
		horizontalRaySpacing = colliderBounds.size.y / (rayCount - 1);
		verticalRaySpacing = colliderBounds.size.x / (rayCount - 1);	
	}


	Vector2[] ComputeOffsets(Vector2 direction) {
		Vector2[] offs = new Vector2[rayCount];
		for(int i = 0; i < rayCount; i++) {
			offs[i] = ComputeRayOrigin(direction, i);
		}
		return offs;
	}


	Vector2 ComputeRayOrigin(Vector2 direction, int rayId) {
		Vector2 rayOrigin = new Vector2();
		if (direction == Vector2.up) {
			rayOrigin = raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * rayId);
		} else if (direction == Vector2.down) {
			rayOrigin = raycastOrigins.bottomLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * rayId);
		} else if (direction == Vector2.left) {
			rayOrigin = raycastOrigins.bottomLeft;
			rayOrigin += Vector2.up * (horizontalRaySpacing * rayId);
		} else if (direction == Vector2.right) {
			rayOrigin = raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * rayId);
		}
		return rayOrigin;
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


	struct RaycastOrigins {
		public Vector2 topLeft, topRight, bottomLeft, bottomRight;
	}
}
