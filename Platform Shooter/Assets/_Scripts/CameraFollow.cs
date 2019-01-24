using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

	[Tooltip("Target of the camera (usually player)")]
	[SerializeField] Controller target;

	[Tooltip("Size of the leeway inside which the camera does not move when the player moves")]
	[SerializeField] Vector2 focusAreaSize;

	[Tooltip("Camera frustrum is offset toward the top of the level by this value")]
	[SerializeField] float verticalOffset;

	[Tooltip("How far away the camera will look in the direction of the player horizontal movement")]
	[SerializeField] float lookAheadDistX;

	[Tooltip("Time it takes to transition to look ahead position")]
	[SerializeField] float HorizontalSmoothTime;

	[Tooltip("Time it takes to transition when the player is jumping or falling")]
	[SerializeField] float verticalSmoothTime;
	
	FocusArea focusArea;

	float currentLookAheadX;
	float targetLookAheadX;
	float lookAheadDirX;
	float smoothLookVelocityX;
	float smoothVelocityY;
	bool lookAheadStopped;

	void Start() {
		focusArea = new FocusArea(target.Collider.bounds, focusAreaSize);
	}

	void LateUpdate() {
		focusArea.Update(target.Collider.bounds);
		Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;
		if(focusArea.velocity.x != 0) {
			lookAheadDirX = Mathf.Sign(focusArea.velocity.x);
			// Smooth change direction
			if (Mathf.Sign(target.PlayerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.PlayerInput.x != 0) {
				// Complete camera move
				lookAheadStopped = false;
				targetLookAheadX = lookAheadDirX * lookAheadDistX;
			}
			// Stopped camera move 
			else {
				if(!lookAheadStopped) {
					lookAheadStopped = true;
					targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDistX - currentLookAheadX) / 4f;
				}
			}
		}
		currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, HorizontalSmoothTime);

		focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalSmoothTime);
		focusPosition += Vector2.right * currentLookAheadX;
		transform.position = (Vector3)focusPosition + Vector3.forward * -10;
	}

	void OnDrawGizmos() {
		Gizmos.color = new Color(0, 0.5f, 0.6f, 0.2f);
		Gizmos.DrawCube(focusArea.centre, focusAreaSize);
	}

	struct FocusArea {
		public Vector2 centre;
		public Vector2 velocity;

		float left, right, top, bottom;

		public FocusArea(Bounds targetBounds, Vector2 size) {
			left = targetBounds.center.x - size.x / 2;
			right = targetBounds.center.x + size.x / 2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;
			centre = new Vector2((left + right) / 2, (bottom + top) / 2);
			velocity = Vector2.zero;
		}
		
		public void Update(Bounds targetBounds) {
			float shiftX = 0;
			if (targetBounds.min.x < left) {
				shiftX = targetBounds.min.x - left;
			} else if (targetBounds.max.x > right) {
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom) {
				shiftY = targetBounds.min.y - bottom;
			} else if (targetBounds.max.y > top) {
				shiftY = targetBounds.max.y - top;
			}
			bottom += shiftY;
			top += shiftY;

			centre = new Vector2((left + right) / 2, (bottom + top) / 2);
			velocity = new Vector2(shiftX, shiftY);
		}
	}
}

