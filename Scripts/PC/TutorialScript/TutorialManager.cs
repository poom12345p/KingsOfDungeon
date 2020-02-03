using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

	public static TutorialManager Instance;

	public List<TutorialInstruction> allTutorial;

	public TutorialInstruction currentTutorial;
	public TutorialInstruction sharedTutorial;

	public Text instructionText;
	public Text TitleText;
	public Image instructionImage;

	public int currentIndex;
	public int currentSharedIndex;


	private void Awake()
    {
        // Singleton logic:
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
	}

	public TutorialInstruction SetTutorialFor(string character){

		foreach(TutorialInstruction tut in allTutorial){
			if(tut.instructionForCharacter == character){
				currentTutorial = tut;
				ChangeToSharedTutorial(0);
				return tut;
			}
		}
		Debug.LogError("Invalid character please check");
		return null;
	}

	public void ShowtutorialInstruction(Sprite instructionSprite, string title, string instruction){

		if(instructionSprite != null){
			instructionImage.sprite = instructionSprite;
			instructionImage.gameObject.SetActive(true);
			instructionText.text = "";
		}
		else
		{
			instructionImage.gameObject.SetActive(false);
			instructionText.text = instruction;
		}
		TitleText.text = title;
	}

	public void ChangeToCharacterTutorial(int change){
		currentIndex = change;
		ShowtutorialInstruction(currentTutorial.allInstruction[currentIndex].instructionSprite,
		 currentTutorial.allInstruction[currentIndex].tutorialTitle,
		 currentTutorial.allInstruction[currentIndex].instructionText
		);
	}

	public void ChangeToCharacterTutorialWithTitle(string title){
		int index = 0;

		foreach (var tutorial in currentTutorial.allInstruction)
		{
			if(tutorial.tutorialTitle.ToLower() == title.ToLower()){
				break;
			}
			index++;
		}

		ChangeToCharacterTutorial(index);
	}

	public void ChangeToSharedTutorial(int change)
	{
		currentSharedIndex = change;
		ShowtutorialInstruction(sharedTutorial.allInstruction[currentSharedIndex].instructionSprite,
		 sharedTutorial.allInstruction[currentSharedIndex].tutorialTitle,
		 sharedTutorial.allInstruction[currentSharedIndex].instructionText
		);
	}

	public void ChangeToSharedTutorialWithTitle(string title)
	{
		int index = 0;

		foreach (var tutorial in sharedTutorial.allInstruction)
		{
			if(tutorial.tutorialTitle.ToLower() == title.ToLower()){
				break;
			}
			index++;
		}

		ChangeToSharedTutorial(index);
	}
}
