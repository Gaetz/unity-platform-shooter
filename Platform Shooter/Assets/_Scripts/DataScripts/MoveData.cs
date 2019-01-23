using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Move Data", menuName="Data/Character Move")]
public class MoveData : ScriptableObject {

	[Tooltip("Layer with which the player collides")]
	public LayerMask platformMask;

	[Tooltip("Gravity acceleration")]
	public float gravity;

	[Tooltip("Max fall speed")]
	public float maxFallSpeed;

	[Tooltip("Player's horizontal acceleration when pressing move buttons")]
	public float xAccel;

	[Tooltip("Player's horizontal decelleration when releasing move buttons")]
	public float xDecel;

	[Tooltip("Player's max horizontal speed")]
	public float xMaxSpeed;

	[Tooltip("Player's start speed when changing direction or when starting move from idle")]
	public float xSnapSpeed;

	[Tooltip("Jump speed when the player presses the jump button")]
	public float jumpStartSpeed;

	[Tooltip("Period the player can hold jump button to jump higher")]
	public float jumpMaxHoldPeriod;

	[Tooltip("While holding jump button, jump speed decreases. This is the minimum jump speed value.")]
	public float jumpMinSpeed;

	[Tooltip("Period the player is allowed to press jump button before the actual jump")]
	public float jumpInputLeewayPeriod;
}
