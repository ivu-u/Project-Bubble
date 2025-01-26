using UnityEngine;

public class MainMenuButton : MenuButton {

    public event System.Action<MainMenuBType> OnPopEnd;

    [SerializeField] private MainMenuBType buttonType;

    protected override void DoPopBehavior() {
        OnPopEnd?.Invoke(buttonType);
        OnPopEnd = null;
    }
}