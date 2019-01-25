using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

		float angle;
		float speed;
		float lifetime;
		int damage;

		float lifeCounter;

		SpriteRenderer spriteRenderer;
		ParticleSystem particles;
		Rigidbody2D rb;

	void Awake () {
		spriteRenderer = GetComponent<SpriteRenderer>();
		particles = GetComponent<ParticleSystem>();
		rb = GetComponent<Rigidbody2D>();
		particles.Stop();
	}

	public void Setup(float angle, float speed, float lifetime, int damage, float gravityScale, float mass) {
		this.angle = angle;
		this.speed = speed;
		this.lifetime = lifetime;
		this.damage = damage;
		if(lifetime > 0)
			Destroy(gameObject, lifetime);
		if(gravityScale > 0) {
			rb.gravityScale = gravityScale;
			rb.mass = mass;
			rb.AddForce(new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * speed);
		}
	}

	void Update() {
		if(rb.gravityScale == 0) {
			rb.velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * speed * Time.deltaTime;
		}
	}

	void OnCollisionEnter2D(Collision2D other) {
		if(other.gameObject.layer != gameObject.layer) {
			particles.Play();
			spriteRenderer.enabled = false;
			ShootTarget target = other.gameObject.GetComponent<ShootTarget>();
			if(target) {
				target.Damage(damage);
			}
			Destroy(gameObject, 0.15f);
		}
	}
}
