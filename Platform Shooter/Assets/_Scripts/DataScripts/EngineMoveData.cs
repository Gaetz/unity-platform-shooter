using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Engine Move Data", menuName="Data/Engine Move Data (don't use)")]
public class EngineMoveData : ScriptableObject {

	[Tooltip("Max slope angle a player can climb, in degrees")]
	public int MaxSlopeClimb;

	[Tooltip("Max slope angle a player can climb without slowing, in degrees")]
	public int NoSlowSlopeClimp;

	[Tooltip("Max slope angle a player can descend sliding, in degrees")]
	public float MaxSlopeDescend;

	[Tooltip("Number of horizontal rays")]
	public int horizontalRays;

	[Tooltip("Number of vertical rays")]
	public int verticalRays;

	[Tooltip("Rays origin margin inside the collider")]
	public float margin;
}
