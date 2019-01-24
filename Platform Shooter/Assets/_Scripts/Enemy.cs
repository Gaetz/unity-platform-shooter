using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

	enum EnemyState { Approching, RandomMove }

	[SerializeField] float speed;

	bool randomMove;
	PolygonCollider2D collide;
	Rigidbody2D rbody;
	Transform player;
	EnemyState state;

	float counter;

	// Use this for initialization
	void Start () {
		collide = GetComponent<PolygonCollider2D>();
		rbody = GetComponent<Rigidbody2D>();
		player = GameObject.FindGameObjectWithTag("Player").transform;
		state = EnemyState.Approching;
	}
	
	// Update is called once per frame
	void Update () {
		counter += Time.deltaTime;
		switch(state) {

			case EnemyState.Approching:
				Vector3 between = player.transform.position - transform.position;
				if(between.magnitude > speed) {
					transform.Translate(between * speed * Time.deltaTime / between.magnitude);
				}
			break;

			case EnemyState.RandomMove:
			
			break;
		}
	}
}
