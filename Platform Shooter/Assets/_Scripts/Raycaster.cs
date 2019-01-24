using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Raycaster {
	
	protected Vector2 direction;
	public enum RaycastOrigin { DEFAULT, TOP_LEFT, TOP_RIGHT, BOTTOM_LEFT, BOTTOM_RIGHT, CENTER_LEFT, CENTER_RIGHT }
	protected RaycastOrigin origin;
	protected float horizontalRaySpacing;
	protected float verticalRaySpacing;
	protected RaycastStart raycastStart;

	protected int hRayCount;
	protected int vRayCount;

	protected Vector2[] offsetPoints;
	protected float margin;
	protected RaycastState state;
	protected LayerMask layerMask;

	public Raycaster(LayerMask layerMask, Vector2 direction, float margin, RaycastState state, RaycastOrigin overrideOrigin = RaycastOrigin.DEFAULT) 
	{
		this.layerMask = layerMask;
		this.direction = direction;
		this.margin = margin;
		this.state = state;
		hRayCount = DataAccess.Instance.EngineMoveData.horizontalRays;
		vRayCount = DataAccess.Instance.EngineMoveData.verticalRays;
		if(overrideOrigin != RaycastOrigin.DEFAULT) {
			origin = overrideOrigin;
		} else {
			if (direction == Vector2.up) {
				origin = RaycastOrigin.TOP_LEFT;
			} else if (direction == Vector2.down) {
				origin = RaycastOrigin.BOTTOM_LEFT;
			} else if (direction == Vector2.left) {
				origin = RaycastOrigin.BOTTOM_LEFT;
			} else if (direction == Vector2.right) {
				origin = RaycastOrigin.BOTTOM_RIGHT;
			}
		}
	}

	public void UpdateOrigin(Rect bounds) {
		ComputeRaySpacing(bounds);
		offsetPoints = ComputeOffsets();
	}

	protected void ComputeRaySpacing(Rect colliderBounds) {
		raycastStart.bottomLeft = new Vector2(-colliderBounds.size.x / 2, -colliderBounds.size.y / 2 + margin);
		raycastStart.bottomRight = new Vector2(colliderBounds.size.x / 2, -colliderBounds.size.y / 2 + margin);
		raycastStart.topLeft = new Vector2(-colliderBounds.size.x / 2, colliderBounds.size.y / 2 - margin);
		raycastStart.topRight = new Vector2(colliderBounds.size.x / 2, colliderBounds.size.y / 2- margin);
		raycastStart.centerLeft = new Vector2(-colliderBounds.size.x / 2, 0);
		raycastStart.centerRight = new Vector2(colliderBounds.size.x / 2, 0);

		hRayCount = Mathf.Clamp(hRayCount, 2, int.MaxValue);
		vRayCount = Mathf.Clamp(vRayCount, 2, int.MaxValue);
		horizontalRaySpacing = (colliderBounds.size.y - margin) / (hRayCount - 1); // Top horizontal ray still in bounding box
		verticalRaySpacing = colliderBounds.size.x / (vRayCount - 1);	
	}


	protected Vector2[] ComputeOffsets() {
		int rayCount = 0;
		if(direction.x == 0) {
			rayCount = vRayCount;
		} else {
			rayCount = hRayCount;
		}
		Vector2[] offs = new Vector2[rayCount];
		for(int i = 0; i < rayCount; i++) {
			offs[i] = ComputeRayOrigin(i);
		}
		return offs;
	}


	protected Vector2 ComputeRayOrigin(int rayId) {
		Vector2 rayOrigin = new Vector2();
		switch(origin) {
			case RaycastOrigin.BOTTOM_LEFT:
				if (direction == Vector2.down) {
					rayOrigin = raycastStart.bottomLeft;
					rayOrigin += Vector2.right * (verticalRaySpacing * rayId);
				} else if (direction == Vector2.left) {
					rayOrigin = raycastStart.bottomLeft;
					rayOrigin += Vector2.up * (horizontalRaySpacing * rayId);
				}
				break;
			case RaycastOrigin.BOTTOM_RIGHT:
				if (direction == Vector2.right) {
					rayOrigin = raycastStart.bottomRight;
					rayOrigin += Vector2.up * (horizontalRaySpacing * rayId);
				}
				break;
			case RaycastOrigin.TOP_LEFT:
				if (direction == Vector2.up) {
					rayOrigin = raycastStart.topLeft;
					rayOrigin += Vector2.right * (verticalRaySpacing * rayId);
				}
				break;
			case RaycastOrigin.CENTER_LEFT:
				if (direction == Vector2.down) {
					rayOrigin = raycastStart.centerLeft;
					rayOrigin += Vector2.right * (verticalRaySpacing * rayId);
				}
				break;
		}
		return rayOrigin;
	}

	protected struct RaycastStart {
		public Vector2 topLeft, topRight, bottomLeft, bottomRight, centerLeft, centerRight;
	}

}
