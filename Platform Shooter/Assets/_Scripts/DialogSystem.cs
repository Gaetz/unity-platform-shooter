using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogSystem : MonoBehaviour {

	[SerializeField] Text dialogText;
	[SerializeField] GameObject dialogGUI;
	[SerializeField] Transform dialogBoxGUI;

	[SerializeField] float letterDelay = 0.1f;
	[SerializeField] float letterMultiplier = 0.5f;
	[SerializeField] KeyCode dialogInput = KeyCode.Return;
	public string[] dialogLines;

	public bool IsLetterMultiplied { get { return isLetterMultiplied; } set { isLetterMultiplied = value; } }
	public bool isLetterMultiplied = false;

	public bool DialogActive { get { return dialogActive; } set { dialogActive = value; } }
	public bool dialogActive = false;

	public bool DialogEnded { get { return dialogEnded; } set { dialogEnded = value; } }
	public bool dialogEnded = false;

	//public bool OutOfRange { get { return outOfRange; } set { outOfRange = value; } }
	public bool outOfRange = true;

	[SerializeField] AudioClip audioClip;
	AudioSource audioSource;

	void Start() {
		audioSource = GetComponent<AudioSource>();
		dialogText.text = "";
	}
	void Update() {

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
			while (currentDialogIndex < dialogLength /* || !isLetterMultiplied*/) {
				//if (!isLetterMultiplied) {
				//	isLetterMultiplied = true;
					StartCoroutine(DisplayString(dialogLines[currentDialogIndex++]));
					if(currentDialogIndex >= dialogLength) {
						dialogEnded = true;
					}
				//}
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
