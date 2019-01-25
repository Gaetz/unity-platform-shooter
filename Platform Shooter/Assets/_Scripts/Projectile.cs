using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

		float angle;
		float speed;
		float lifetime;

		float lifeCounter;

		BoxCollider2D collide;
		SpriteRenderer spriteRenderer;
		ParticleSystem particles;
		Rigidbody2D rbody;

	void Start () {
		collide = GetComponent<BoxCollider2D>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		particles = GetComponent<ParticleSystem>();
		rbody = GetComponent<Rigidbody2D>();
		particles.Stop();
	}

	public void Setup(float angle, float speed, float lifetime) {
		this.angle = angle;
		this.speed = speed;
		this.lifetime = lifetime;
		if(lifetime > 0)
			Destroy(gameObject, lifetime);
	}

	void Update() {
		rbody.velocity = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)) * speed * Time.deltaTime;
	}

	void OnCollisionEnter2D(Collision2D other) {
		particles.Play();
		spriteRenderer.enabled = false;
		Destroy(gameObject, 0.15f);
	}
}
