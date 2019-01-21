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
	const float parallelInsetLen = 0.05f;
	const float perpendicularInsetLen = 0.05f;
	const float groundTestLen = 0.05f;
	const float skinLen = 0.0005f;

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
	bool lastGrounded;

	Animator animator;
	SpriteRenderer spriteRenderer;
	BoxCollider2D boxCollider;

	[SerializeField] AudioSource jumpSfx, landSfx, startMoveSfx;

	void Start () {
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		boxCollider = GetComponent<BoxCollider2D>();
		raycastDown = new RaycastMoveDirection(
			platformMask, new Vector2(-0.2f, -0.3f), new Vector2(0.2f, -0.3f), Vector2.down,
			boxCollider.bounds, skinLen
		);
		raycastLeft = new RaycastMoveDirection(
			platformMask, new Vector2(-0.2f, -0.3f), new Vector2(-0.2f, 0.3f), Vector2.left,
			boxCollider.bounds, skinLen
		);
		raycastRight = new RaycastMoveDirection(
			platformMask, new Vector2(0.2f, -0.3f), new Vector2(0.2f, 0.3f), Vector2.right,
			boxCollider.bounds, skinLen
		);
		raycastUp = new RaycastMoveDirection(
			platformMask, new Vector2(-0.2f, 0.3f), new Vector2(0.2f, 0.3f), Vector2.up,
			boxCollider.bounds, skinLen
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
		bool grounded = groundDown.Cast(transform.position);
		if (grounded && !lastGrounded) {
			landSfx.Play();
		}
		lastGrounded = grounded;

		switch(jumpState) {
			case JumpState.None:
				if (grounded && jumpStartTimer > 0) {
					jumpStartTimer = 0;
					jumpHoldTimer = 0;
					jumpState = JumpState.Holding;
					velocity.y = jumpStartSpeed;
					jumpSfx.Play();
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
				startMoveSfx.Play();
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

		// Animations 
		if(jumpState == JumpState.Holding) {
			animator.Play("Jump");
		} else {
			if (grounded) {
				if(wantedDirection == 0) {
					animator.Play("Idle");
				} else {
					animator.Play("Move");
				}
			} else {
				if (velocity.y < 0) {
					animator.Play("Fall");
				} else {
					animator.Play("Jump");
				}
			}
		}
		if (wantedDirection != 0) {
			spriteRenderer.flipX = wantedDirection < 0;
		}
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
