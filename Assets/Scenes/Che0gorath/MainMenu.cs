using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    [SerializeField] private MainMenuButton[] buttons;
    [SerializeField] private float buttonPopDelay;

    void Awake() {
        foreach (MainMenuButton menuButton in buttons) {
            menuButton.OnPopEnd += MenuButton_OnPopEnd;
        }
    }

    private void MenuButton_OnPopEnd(MainMenuBType type) {
        switch (type) {
            case MainMenuBType.Start:
                StartCoroutine(PopRemainingButtons());
                break;
            case MainMenuBType.Credits:
                break;
            case MainMenuBType.End:
                Application.Quit();
                break;
        }
        
    }

    private IEnumerator PopRemainingButtons() {
        IEnumerable<MenuButton> activeButtons = buttons.Where((button) => !button.IsPopped);
        foreach (MenuButton button in activeButtons) {
            button.PopButton();
            yield return new WaitForSeconds(buttonPopDelay);
        }
    }

    public void StartGame() {

    }

    public void ExitGame() {

    }
}
