using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastEngine : MonoBehaviour {

	[SerializeField] LayerMask platformMask;
	[SerializeField] float parallelInsetLen;
	[SerializeField] float perpendicularInsetLen;
	[SerializeField] float groundTestLen;

	[SerializeField] float gravity;
	[SerializeField] float xAccel;
	[SerializeField] float xMaxSpeed;

	Vector2 velocity;

	RaycastMoveDirection raycastDown;
	RaycastMoveDirection raycastLeft;
	RaycastMoveDirection raycastRight;
	RaycastMoveDirection raycastUp;

	RaycastCheckTouch groundDown;


	void Start () {
		raycastDown = new RaycastMoveDirection(
			platformMask, new Vector2(-0.2f, -0.3f), new Vector2(0.2f, -0.3f), Vector2.down,
			Vector2.right * perpendicularInsetLen, Vector2.up * parallelInsetLen
		);
		raycastLeft = new RaycastMoveDirection(
			platformMask, new Vector2(-0.2f, -0.3f), new Vector2(-0.2f, 0.3f), Vector2.left,
			Vector2.up * perpendicularInsetLen, Vector2.right * parallelInsetLen
		);
		raycastRight = new RaycastMoveDirection(
			platformMask, new Vector2(0.2f, -0.3f), new Vector2(0.2f, 0.3f), Vector2.right,
			Vector2.up * perpendicularInsetLen, Vector2.left * parallelInsetLen
		);
		raycastUp = new RaycastMoveDirection(
			platformMask, new Vector2(-0.2f, 0.3f), new Vector2(0.2f, 0.3f), Vector2.up,
			Vector2.right * perpendicularInsetLen, Vector2.down * parallelInsetLen
		);
		groundDown = new RaycastCheckTouch(
			platformMask, new Vector2(-0.2f, -0.3f), new Vector2(0.2f, -0.3f), Vector2.down,
			Vector2.right * perpendicularInsetLen, Vector2.up * parallelInsetLen, groundTestLen
		);
	}
	
	void FixedUpdate () {
		// Left and right
		float xInput = Input.GetAxisRaw("Horizontal");
		velocity.x += xAccel * Time.deltaTime * xInput;
		if(Mathf.Abs(velocity.x) > xMaxSpeed) {
			velocity.x = xMaxSpeed * Mathf.Sign(xInput);
		}

		// Contact
		if(!groundDown.Cast(transform.position)) {
			velocity.y -= gravity * Time.deltaTime;
		} else {
			velocity.y = 0;
		}

		// Move
		Vector2 move = Vector2.zero;
		if (velocity.x > 0) {
			move.x = raycastRight.Cast(transform.position, velocity.x * Time.deltaTime);
		} else if (velocity.x < 0) {
			move.x = -raycastLeft.Cast(transform.position, -velocity.x * Time.deltaTime);
		}

		if (velocity.y > 0) {
			move.y = raycastUp.Cast(transform.position, velocity.y * Time.deltaTime);
		} else if (velocity.y < 0) {
			move.y = -raycastDown.Cast(transform.position, -velocity.y * Time.deltaTime);
		}

		transform.Translate(move);
	}
}
