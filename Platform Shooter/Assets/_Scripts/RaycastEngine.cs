using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastEngine : MonoBehaviour {

	enum JumpState {
		None = 0, Holding
	}

	[Tooltip("Layer with which the player collides")]
	[SerializeField] LayerMask platformMask;

	[Tooltip("Gravity acceleration")]
	[SerializeField] float gravity;

	[Tooltip("Player's horizontal acceleration when pressing move buttons")]
	[SerializeField] float xAccel;

	[Tooltip("Player's horizontal decelleration when releasing move buttons")]
	[SerializeField] float xDecel;

	[Tooltip("Player's max horizontal speed")]
	[SerializeField] float xMaxSpeed;

	[Tooltip("Player's start speed when changing direction or when starting move from idle")]
	[SerializeField] float xSnapSpeed;

	Vector2 velocity;
	float parallelInsetLen = 0.05f;
	float perpendicularInsetLen = 0.05f;
	float groundTestLen = 0.05f;

	RaycastMoveDirection raycastDown;
	RaycastMoveDirection raycastLeft;
	RaycastMoveDirection raycastRight;
	RaycastMoveDirection raycastUp;

	RaycastCheckTouch groundDown;

	[Tooltip("Jump speed when the player presses the jump button")]
	[SerializeField] float jumpStartSpeed;

	[Tooltip("Period the player can hold jump button to jump higher")]
	[SerializeField] float jumpMaxHoldPeriod;

	[Tooltip("While holding jump button, jump speed decreases. This is the minimum jump speed value.")]
	[SerializeField] float jumpMinSpeed;

	[Tooltip("Period the player is allowed to press jump button before the actual jump")]
	[SerializeField] float jumpInputLeewayPeriod;


	float jumpStartTimer;
	float jumpHoldTimer;
	bool jumpInputDown;
	JumpState jumpState;


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

	void Update() {
		jumpStartTimer -= Time.deltaTime;
		bool jumpButton = Input.GetButton("Jump");
		if(jumpButton && !jumpInputDown) {
			jumpStartTimer = jumpInputLeewayPeriod;
		}
		jumpInputDown = jumpButton;
	}
	
	void FixedUpdate () {

		// Jump
		switch(jumpState) {
			case JumpState.None:
				if (groundDown.Cast(transform.position) && jumpStartTimer > 0) {
					jumpStartTimer = 0;
					jumpHoldTimer = 0;
					jumpState = JumpState.Holding;
					velocity.y = jumpStartSpeed;
				}
				break;
			case JumpState.Holding:
				jumpHoldTimer += Time.deltaTime;
				if(!jumpInputDown || jumpHoldTimer >= jumpMaxHoldPeriod) {
					jumpState = JumpState.None;
					velocity.y = Mathf.Lerp(jumpMinSpeed, jumpStartSpeed, jumpHoldTimer/jumpMaxHoldPeriod);
				}
				break;
		}

		// Left and right
		float xInput = Input.GetAxisRaw("Horizontal");
		int wantedDirection = GetSign(xInput);
		int velocityDirection = GetSign(velocity.x);

		// Handle direction change
		if (wantedDirection != 0) {
			if (wantedDirection != velocityDirection) {
				velocity.x = xSnapSpeed * wantedDirection;
			}	else {
				velocity.x = Mathf.MoveTowards(velocity.x, xMaxSpeed * wantedDirection, xAccel * Time.deltaTime);
			}
		} else {
			velocity.x = Mathf.MoveTowards(velocity.x, 0, xDecel * Time.deltaTime);
		}


		// Gravity
		if(jumpState == JumpState.None) {
			velocity.y -= gravity * Time.deltaTime;
		}

		// Move
		Vector2 move = Vector2.zero;
		Vector2 wantedMove = velocity * Time.deltaTime;

		if (velocity.x > 0) {
			move.x = raycastRight.Cast(transform.position, wantedMove.x);
		} else if (velocity.x < 0) {
			move.x = -raycastLeft.Cast(transform.position, -wantedMove.x);
		}

		if (velocity.y > 0) {
			move.y = raycastUp.Cast(transform.position, wantedMove.y);
		} else if (velocity.y < 0) {
			move.y = -raycastDown.Cast(transform.position, -wantedMove.y);
		}

		if (!Mathf.Approximately(wantedMove.x, move.x)) {
			velocity.x = 0;
		}
		if (!Mathf.Approximately(wantedMove.y, move.y)) {
			velocity.y = 0;
		}

		transform.Translate(move);
	}

	int GetSign(float v) {
		if (Mathf.Approximately(v, 0)) {
			return 0;
		} else if (v > 0) {
			return 1;
		} else {
			return -1;
		}
	}
}
