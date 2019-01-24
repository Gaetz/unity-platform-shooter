using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour {

	[Tooltip("Link to dialog text")]
	[SerializeField] Text dialogText;

	[Tooltip("Link to dialog canvas")]
	[SerializeField] GameObject dialogGUI;

	[Tooltip("Link to dialog background box")]
	[SerializeField] Transform dialogBoxGUI;

	[Tooltip("Default time between letter")]
	[SerializeField] float letterDelay = 0.025f;

	[Tooltip("Time between letter when dialog input (Return by default) is pressed")]
	[SerializeField] float letterMultiplier = 0.005f;

	[Tooltip("Dialog input key")]
	[SerializeField] KeyCode dialogInput = KeyCode.Return;
	public string[] dialogLines;

	bool isLetterMultiplied = false;
	bool dialogActive = false;
	bool dialogEnded = false;
	public bool outOfRange = true;

	[SerializeField] AudioClip audioClip;
	AudioSource audioSource;

	void Start() {
		audioSource = GetComponent<AudioSource>();
		dialogText.text = "";		
		dialogGUI.SetActive(false);
	}

	public void EnterRangeOfNPC() {
		outOfRange = false;
		dialogGUI.SetActive(true);
		if(dialogActive) {
			dialogGUI.SetActive(false);
		}
	}

	public void TriggerDialog() {
		outOfRange = false;
		dialogBoxGUI.gameObject.SetActive(true);
		if(!dialogActive) {
			dialogActive = true;
			StartCoroutine(StartDialog());
		}
		StartDialog();
	}

	IEnumerator StartDialog() {
		if (!outOfRange) {
			int dialogLength = dialogLines.Length;
			int currentDialogIndex = 0;
			while (currentDialogIndex < dialogLength  || !isLetterMultiplied) {
				if (!isLetterMultiplied) {
					isLetterMultiplied = true;
					StartCoroutine(DisplayString(dialogLines[currentDialogIndex++]));
					if(currentDialogIndex >= dialogLength) {
						dialogEnded = true;
					}
				}
				yield return 0;
			}
			while(true) {
				if(Input.GetKeyDown(dialogInput) && dialogEnded == false) {
					break;
				}
				yield return 0;
			}
			dialogEnded = false;
			dialogActive = false;
			DropDialog();
		}
	}

	IEnumerator DisplayString(string stringToDisplay) {
		if(!outOfRange) {
			int stringLength = stringToDisplay.Length;
			int currentCharacterIndex = 0;
			dialogText.text = "";

			while (currentCharacterIndex < stringLength) {
				dialogText.text += stringToDisplay[currentCharacterIndex];
				currentCharacterIndex++;
				if (currentCharacterIndex < stringLength) {
					if (Input.GetKey(dialogInput)) {
						yield return new WaitForSeconds(letterDelay * letterMultiplier);
						if(audioClip) audioSource.PlayOneShot(audioClip, 0.5f);
					} else {
						yield return new WaitForSeconds(letterDelay);
						if(audioClip) audioSource.PlayOneShot(audioClip, 0.5f);
					}
				} else {
					dialogEnded = false;
					break;
				} 
			}

			while (true) {
				if (Input.GetKeyDown(dialogInput)) { 
					break;
				}
				yield return 0;
			}

			dialogEnded = false;
			isLetterMultiplied = false;
			dialogText.text = "";
		}
	}

	public void DropDialog() {
		dialogGUI.SetActive(false);
		dialogBoxGUI.gameObject.SetActive(false);
	}

	public void OutOfRange() {
		outOfRange = true;
		if(outOfRange) {
			isLetterMultiplied = false;
			dialogActive = false;
			StopAllCoroutines();
			DropDialog();
		}
	}
	
}
