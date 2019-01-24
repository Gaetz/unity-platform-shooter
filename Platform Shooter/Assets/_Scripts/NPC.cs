using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour {
	[SerializeField] float dialogAbovePosition;
	[SerializeField] Transform ChatBackground;
	//[SerializeField] Transform NpcCharacter;
	[SerializeField][TextArea(5, 10)] string[] sentences;
	DialogSystem dialogSystem;
	

	// Use this for initialization
	void Start () {
		dialogSystem = FindObjectOfType<DialogSystem>();
				//enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
		pos.y += dialogAbovePosition;
		ChatBackground.position = pos;
	}

	public void OnTriggerStay2D(Collider2D other) {
		if(other.gameObject.tag == "Player") {
			dialogSystem.EnterRangeOfNPC();
			enabled = true;
			dialogSystem.dialogLines = sentences;
			dialogSystem.TriggerDialog();
		}
	}

	public void OnTriggerExit2D(Collider2D other) {
		dialogSystem.OutOfRange();
		enabled = false;
	}
}
