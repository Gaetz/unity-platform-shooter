using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RaycastMoveDirection {

	protected RaycastState state;
	protected LayerMask layerMask;
	protected Vector2 direction;
	protected Vector2[] offsetPoints;
	protected float margin;

	
	protected int hRayCount;
	protected int vRayCount;

	protected float horizontalRaySpacing;
	protected float verticalRaySpacing;
	protected RaycastOrigins raycastOrigins;


	public RaycastMoveDirection(LayerMask layerMask, Vector2 direction, float margin, RaycastState state) 
	{
		this.layerMask = layerMask;
		this.direction = direction;
		this.margin = margin;
		this.state = state;
		hRayCount = DataAccess.Instance.EngineMoveData.horizontalRays;
		vRayCount = DataAccess.Instance.EngineMoveData.verticalRays;
	}
	
	public void UpdateOrigin(Rect bounds) {
		ComputeRaySpacing(bounds);
		offsetPoints = ComputeOffsets(direction);
	}

	protected void ComputeRaySpacing(Rect colliderBounds) {
		raycastOrigins.bottomLeft = new Vector2(-colliderBounds.size.x / 2, -colliderBounds.size.y / 2 + margin);
		raycastOrigins.bottomRight = new Vector2(colliderBounds.size.x / 2, -colliderBounds.size.y / 2 + margin);
		raycastOrigins.topLeft = new Vector2(-colliderBounds.size.x / 2, colliderBounds.size.y / 2 - margin);
		raycastOrigins.topRight = new Vector2(colliderBounds.size.x / 2, colliderBounds.size.y / 2- margin);

		hRayCount = Mathf.Clamp(hRayCount, 2, int.MaxValue);
		vRayCount = Mathf.Clamp(vRayCount, 2, int.MaxValue);
		horizontalRaySpacing = (colliderBounds.size.y - margin) / (hRayCount - 1); // Top horizontal ray still in bounding box
		verticalRaySpacing = colliderBounds.size.x / (vRayCount - 1);	
	}


	protected Vector2[] ComputeOffsets(Vector2 direction) {
		int rayCount = 0;
		if(direction.x == 0) {
			rayCount = vRayCount;
		} else {
			rayCount = hRayCount;
		}
		Vector2[] offs = new Vector2[rayCount];
		for(int i = 0; i < rayCount; i++) {
			offs[i] = ComputeRayOrigin(direction, i);
		}
		return offs;
	}


	protected Vector2 ComputeRayOrigin(Vector2 direction, int rayId) {
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


	protected RaycastHit2D Raycast(Vector2 start, Vector2 direction, float length, LayerMask layerMask) {
		Debug.DrawLine(start, start + direction * length, Color.blue);
		return Physics2D.Raycast(start, direction, length, layerMask);
	}


	public abstract void Cast(Vector2 origin, float distance, ref Vector2 move);

	protected struct RaycastOrigins {
		public Vector2 topLeft, topRight, bottomLeft, bottomRight;
	}
}
