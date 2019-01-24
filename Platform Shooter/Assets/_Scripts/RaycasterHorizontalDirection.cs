using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycasterHorizontalDirection : RaycasterMoveDirection {

	float maxSlopeAngle;
	float noSlowSlopeAngle;

	public RaycasterHorizontalDirection(LayerMask layerMask, Vector2 direction, float margin, RaycastState state)
	: base(layerMask, direction, margin, state)
	{
		maxSlopeAngle = DataAccess.Instance.EngineMoveData.MaxSlopeClimb;
		noSlowSlopeAngle = DataAccess.Instance.EngineMoveData.NoSlowSlopeClimp;
	}

	public override void Cast(Vector2 origin, float distance, ref Vector2 move) {
		float minDistance = distance;
		float slopeAngle = 0;
		foreach(var offset in offsetPoints) {
			RaycastHit2D hit = Raycast(origin + offset, direction, distance + margin, layerMask);
			if(hit.collider != null) {
				slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
				minDistance = Mathf.Min(minDistance, hit.distance - margin);
			}
		}
		if (slopeAngle > 0 && slopeAngle < maxSlopeAngle) {
			if(state.Descending) state.Descending = false;
			state.Climbing = true;
			ClimbSlope(ref move, slopeAngle, distance, slopeAngle >= noSlowSlopeAngle);
		} else {
			state.Climbing = false;
			move.x = minDistance * direction.x;
		}
	}

	void ClimbSlope(ref Vector2 move, float slopeAngle, float distance, bool slow) {
		float climbVelocityX = Mathf.Cos(Mathf.Deg2Rad * slopeAngle) * distance;
		float climbVelocityY = Mathf.Sin(Mathf.Deg2Rad * slopeAngle) * distance;
		float bonusSpeedMult = slow? 1 : distance / climbVelocityX;
		if(move.y <= climbVelocityY) {
			move.x = climbVelocityX * direction.x * bonusSpeedMult;
			move.y = climbVelocityY * bonusSpeedMult;
		}
	}

}
