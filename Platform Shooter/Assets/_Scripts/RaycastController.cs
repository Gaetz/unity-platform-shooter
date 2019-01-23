using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastController : MonoBehaviour {

	enum JumpState {
		None = 0, Holding
	}

	[SerializeField] MoveData moveData;

	Vector2 velocity;
	Vector2 move;

	const float parallelInsetLen = 0.05f;
	const float perpendicularInsetLen = 0.05f;
	const float groundMargin = 0.05f;

	RaycastVerticalDirection raycastDown;
	RaycastHorizontalDirection raycastLeft;
	RaycastHorizontalDirection raycastRight;
	RaycastVerticalDirection raycastUp;

	RaycastCheckTouch groundDown;

	float jumpStartTimer;
	float jumpHoldTimer;
	bool jumpInputDown;
	JumpState jumpState;

	bool lastGrounded;

	Animator animator;
	SpriteRenderer spriteRenderer;
	BoxCollider2D boxCollider;
	Rect box;

	public RaycastState State { get; set; }
	RaycastState state;

	[SerializeField] AudioSource jumpSfx, landSfx, startMoveSfx;

	void Start () {
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		boxCollider = GetComponent<BoxCollider2D>();
		state = new RaycastState();

		raycastDown = new RaycastVerticalDirection(moveData.platformMask, Vector2.down, DataAccess.Instance.EngineMoveData.margin, state);
		raycastLeft = new RaycastHorizontalDirection(moveData.platformMask, Vector2.left, DataAccess.Instance.EngineMoveData.margin, state);
		raycastRight = new RaycastHorizontalDirection(moveData.platformMask, Vector2.right, DataAccess.Instance.EngineMoveData.margin, state);
		raycastUp = new RaycastVerticalDirection(moveData.platformMask, Vector2.up, DataAccess.Instance.EngineMoveData.margin, state);

		groundDown = new RaycastCheckTouch(
			moveData.platformMask, new Vector2(-0.2f, -0.3f), new Vector2(0.2f, -0.3f), Vector2.down,
			Vector2.right * perpendicularInsetLen, Vector2.up * parallelInsetLen, groundMargin
		);

	}

	void Update() {
		jumpStartTimer -= Time.deltaTime;
		bool jumpButton = Input.GetButton("Jump");
		if(jumpButton && !jumpInputDown) {
			jumpStartTimer = moveData.jumpInputLeewayPeriod;
		}
		jumpInputDown = jumpButton;
	}
	
	void FixedUpdate () {
		box = new Rect(
			boxCollider.bounds.min.x,
			boxCollider.bounds.min.y,
			boxCollider.bounds.size.x,
			boxCollider.bounds.size.y
		);
		raycastDown.UpdateOrigin(box);
		raycastUp.UpdateOrigin(box);
		raycastLeft.UpdateOrigin(box);
		raycastRight.UpdateOrigin(box);

		// Jump
		state.Grounded = groundDown.Cast(transform.position);
		if (state.Grounded && !lastGrounded) {
			landSfx.Play();
			state.Falling = false;
		}
		lastGrounded = state.Grounded;

		switch(jumpState) {
			case JumpState.None:
				if (state.Grounded && jumpStartTimer > 0) {
					jumpStartTimer = 0;
					jumpHoldTimer = 0;
					jumpState = JumpState.Holding;
					velocity.y = moveData.jumpStartSpeed;
					jumpSfx.Play();
				}
				break;
			case JumpState.Holding:
				jumpHoldTimer += Time.deltaTime;
				if(!jumpInputDown || jumpHoldTimer >= moveData.jumpMaxHoldPeriod) {
					jumpState = JumpState.None;
					velocity.y = Mathf.Lerp(moveData.jumpMinSpeed, moveData.jumpStartSpeed, jumpHoldTimer / moveData.jumpMaxHoldPeriod);
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
				velocity.x = moveData.xSnapSpeed * wantedDirection;
				startMoveSfx.Play();
			}	else {
				velocity.x = Mathf.MoveTowards(velocity.x, moveData.xMaxSpeed * wantedDirection, moveData.xAccel * Time.deltaTime);
			}
		} else {
			velocity.x = Mathf.MoveTowards(velocity.x, 0, moveData.xDecel * Time.deltaTime);
		}


		// Gravity
		if(jumpState == JumpState.None) {
			velocity.y = Mathf.Max(velocity.y - moveData.gravity, -moveData.maxFallSpeed); // Negative, so we use max
		}
		if (velocity.y < 0  && !state.Grounded) {
			state.Falling = true;
		}


		// Move
		move = Vector2.zero;
		Vector2 wantedMove = velocity * Time.deltaTime;


		if (velocity.y > 0) {
			raycastUp.Cast(transform.position, wantedMove.y, ref move);
		} else if (velocity.y < 0) {
			raycastDown.Cast(transform.position, -wantedMove.y, ref move, wantedMove.x);
		}

		if (velocity.x > 0) {
			raycastRight.Cast(transform.position, wantedMove.x, ref move);
		} else if (velocity.x < 0) {
			raycastLeft.Cast(transform.position, -wantedMove.x, ref move);
		}

		if(move.y >= 0) {
			state.Descending = false;
			state.Falling = false;
		}
		

		if (!Mathf.Approximately(wantedMove.x, move.x)) {
			velocity.x = 0;
		}
		if (!Mathf.Approximately(wantedMove.y, move.y)) {
			velocity.y = 0;
		}


		// Animations 
		if(jumpState == JumpState.Holding) {
			animator.Play("Jump");
		} else {
			if (state.Grounded) {
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

	void LateUpdate() {
		transform.Translate(move);
	}
}
