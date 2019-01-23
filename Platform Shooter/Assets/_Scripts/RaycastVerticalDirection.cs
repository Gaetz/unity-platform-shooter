using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastVerticalDirection : RaycastMoveDirection {

	float maxSlopeDescendingAngle;

	public RaycastVerticalDirection(LayerMask layerMask, Vector2 direction, float margin, RaycastState state)
	: base(layerMask, direction, margin, state) {
			maxSlopeDescendingAngle = DataAccess.Instance.EngineMoveData.MaxSlopeDescend;
		}

	public override void Cast(Vector2 origin, float distance, ref Vector2 move) {
		Cast(origin, distance, ref move, 0);
	}

	public void Cast(Vector2 origin, float distance, ref Vector2 move, float horizDistance) {
		float minDistance = distance;
		float slopeAngle = 0;
		bool slopeEnd = false;
		RaycastHit2D slopeHit = new RaycastHit2D();
		foreach(var offset in offsetPoints) {
			RaycastHit2D hit = Raycast(origin + offset, direction, distance + margin, layerMask);
			if(hit.collider != null) {
				// Ignore collision if going in the right direction for a ThroughPlatform
				// else collide
				ThroughPlatform tp = hit.collider.GetComponent<ThroughPlatform>();
				if(tp == null || tp.permitDirection != direction) {
					minDistance = Mathf.Min(minDistance, hit.distance - margin);
					// Get slope angle data
					float	slopeHitAngle = Vector2.Angle(hit.normal, Vector2.up);
					// If a raycast hit a flat surface, the slope is ending
					if(Mathf.Approximately(slopeHitAngle, 0)) {
						slopeEnd = true;
					} 
					if(direction.y == -1 && slopeHitAngle > slopeAngle && horizDistance != 0) {
						slopeAngle = slopeHitAngle;
						slopeHit = hit;
					}
				}
			}
		}
		
		// Descending slope
		if (slopeAngle > 0 && slopeAngle < maxSlopeDescendingAngle) {
			float rayDirectionX = Mathf.Sign(horizDistance);
			if (Mathf.Sign(slopeHit.normal.x) == rayDirectionX) {
				float yMove = Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(horizDistance);
				if(slopeHit.distance - DataAccess.Instance.EngineMoveData.margin <= yMove) {
					float moveDistance = Mathf.Abs(horizDistance);
					float descendVelocityY = Mathf.Sin(Mathf.Deg2Rad * slopeAngle) * moveDistance;
					float descendVelocityX = Mathf.Cos(Mathf.Deg2Rad * slopeAngle) * moveDistance;
					float horizontalSpeedMultiplier = moveDistance / descendVelocityX;
					move.x = descendVelocityX * Mathf.Sign(horizDistance) * horizontalSpeedMultiplier;
					move.y -= slopeEnd ? minDistance : descendVelocityY * horizontalSpeedMultiplier;
					state.Descending = true;
					state.Falling = false;
				}
			}
		} 
		// Normal move
		else {
			move.y = minDistance * direction.y;
			state.Descending = false;
		}
	}
}
