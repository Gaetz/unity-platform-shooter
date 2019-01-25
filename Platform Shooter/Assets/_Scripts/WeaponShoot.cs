using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Shooter { Player, Enemy };

public class WeaponShoot : MonoBehaviour {

	[SerializeField] Projectile projectile;
  [SerializeField] WeaponData weaponData;
	[SerializeField] Shooter shooter;

	float cooldownCounter;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		cooldownCounter += Time.deltaTime;
		if(Input.GetButton("Fire1") && cooldownCounter > weaponData.cooldown) {
			Vector3 projectilePosition = transform.position;
				projectilePosition.x += weaponData.horizontalOffset;
			projectilePosition.y += weaponData.verticalOffset;
			for(int i = 0; i < weaponData.projectileNumber; i++) {
				projectilePosition.y += Random.Range(-weaponData.verticalSpread, weaponData.verticalSpread);
				Projectile p = Instantiate(projectile, projectilePosition, Quaternion.identity);
				float angle = weaponData.defaultAngle + Random.Range(-weaponData.angularSpread, weaponData.angularSpread);
				p.Setup(angle, weaponData.projectileSpeed, weaponData.projectileLifetime, weaponData.projectileDamage, weaponData.gravityScale, weaponData.projectileMass, shooter);
			}
			cooldownCounter = 0;
		}
	}
}
