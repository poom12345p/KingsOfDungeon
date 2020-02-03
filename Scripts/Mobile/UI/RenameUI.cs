using UnityEngine;
using UnityEngine.UI;

public class RenameUI : MonoBehaviour
{
    public InputField renameInput;
    public UIMobile mainUi;

    public void CloseRenamePanle()
    {
        gameObject.SetActive(false);
    }

    public void Rename()
    {
        mainUi.Rename(renameInput.text);
        CloseRenamePanle();
    }
}
