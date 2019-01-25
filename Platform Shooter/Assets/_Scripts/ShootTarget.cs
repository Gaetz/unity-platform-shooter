using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootTarget : MonoBehaviour {

	[SerializeField] int hp;

	public void Damage(int damage) {
		hp -= damage;
		Bleed();
		if (hp < 0) {
			Destroy();
		}
	}

	void Bleed() {

	}

	void Destroy() {
		Destroy(gameObject);
	}
}
