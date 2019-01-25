using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Data", menuName="Data/Weapon")]
public class WeaponData : ScriptableObject {

	[Tooltip("Cooldown between projectile shots")]
	public float cooldown;
	
	[Tooltip("Default angle of the shoots")]
	public float defaultAngle;

	[Tooltip("Random angle around the default angle")]
	public float angularSpread;

	[Tooltip("Spreading of projectile on the vertical axis (player is middle axe)")]
	public float verticalSpread;

	[Tooltip("Offset of middle axe (positive is up)")]
	public float verticalOffset;

	[Tooltip("Distance to the player (positive is right)")]
	public float horizontalOffset;

	[Tooltip("Number of projectile per shot")]
	public int projectileNumber;
	
	[Tooltip("Life time of a projectile once shoot")]
	public float projectileLifetime;

	[Tooltip("Speed of projectile")]
	public float projectileSpeed;

	[Tooltip("Damage per projectile")]
	public int projectileDamage;

	[Tooltip("Activate gravity on projectile if more than 0 (better with small speeds)")]
	public float gravityScale;

	[Tooltip("If gravity is enabled, mass of the projectile")]
	public float projectileMass;
}
