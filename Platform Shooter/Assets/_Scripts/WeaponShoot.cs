using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShoot : MonoBehaviour {

	[SerializeField] Projectile projectile;
  [SerializeField] float cooldown;

	float cooldownCounter;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		cooldownCounter += Time.deltaTime;
		if(Input.GetButton("Fire1") && cooldownCounter > cooldown) {
			Projectile p = Instantiate(projectile, transform.position, Quaternion.identity);
			p.Setup(0, 500, 5);
			cooldownCounter = 0;
		}
	}
}
