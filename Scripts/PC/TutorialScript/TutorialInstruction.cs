using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "TutorialInstruction", menuName = "createTutorial")]
public class TutorialInstruction : ScriptableObject {

	public string instructionForCharacter;

	[System.Serializable]
	public struct InstructionData{
		public string tutorialTitle;
		public string instructionText;
		public Sprite instructionSprite;
	}
	
 	public List<InstructionData> allInstruction;
}
