using UnityEngine;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviour {

	public Button EsacapeButton;
	public GameObject EscapeMenuList;

	public bool isShowingList;

	private void Start() {
		ShowHideEscapeMenuList(false);
	}

	public void ShowHideEscapeMenuList(bool show){
		EsacapeButton.gameObject.SetActive(!show);
		EscapeMenuList.SetActive(show);
		isShowingList = show;
	}

	public void BackToMenu(){
		GameManagerPC.Instance.ResetValueAndGoToMenu();
	}

	public void QuitGame(){
		Application.Quit();
	}

	private void Update() {
		if(Input.GetKeyDown(KeyCode.Escape)){
			if(isShowingList) ShowHideEscapeMenuList(false);
			else ShowHideEscapeMenuList(true);
		}
	}



}
