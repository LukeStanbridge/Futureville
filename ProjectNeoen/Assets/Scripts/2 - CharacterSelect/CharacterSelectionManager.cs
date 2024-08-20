using System.Collections;
using TMPro;
using UnityEngine;

public class CharacterSelectionManager : MonoBehaviour
{
    [SerializeField] private GameObject _startMenu;

    [Header("Character Selection GameObject References")]
    [SerializeField] private GameObject _characterSelection;
    [SerializeField] private GameObject _personas;
    [SerializeField] private GameObject _personaContinueButton;
    [SerializeField] private GameObject _avatars;
    [SerializeField] private GameObject _avatarContinueButton;
    [SerializeField] private TextMeshProUGUI _selectionText;

    [field: SerializeField] public Persona PlayerPersona { get; private set; }
    [field: SerializeField] public Avatar PlayerAvatar { get; private set; }
    [field: SerializeField] public string PlayerAvatarName { get; private set; }
    [field: SerializeField] public bool PlaySpecialAnimation { get; private set; }
    [field: SerializeField] public RuntimeAnimatorController AnimatorController { get; private set; }
    [field: SerializeField] public RuntimeAnimatorController AnimatorControllerRight { get; private set; }

    [Header("Developer Testing")]
    public RuntimeAnimatorController AnimatorControllerDefault;
    public Avatar PlayerAvatarDefault;
    public Persona PlayerPersonaDefault;

    private void Awake()
    {
        _personaContinueButton.SetActive(false);
        _avatarContinueButton.SetActive(false);
    }

    public void OpenCharacterSelection() //open character selection
    {
        _characterSelection.SetActive(true);
        _avatars.SetActive(false);
        _avatarContinueButton.SetActive(false);
    }

    public void SetPlayerPersona(Persona persona) //function to access and set the player persona on click
    {
        if (PlayerPersona == persona) return;
        if (PlayerPersona != null) PlayerPersona.GetComponent<Persona>().DeselectPersona();
        PlayerPersona = persona;
        if (!_personaContinueButton.activeSelf) _personaContinueButton.SetActive(true);
    }

    public void PersonaContinue() //continue button that takes player to avatar selection
    {
        if (PlayerPersona == null) return;

        //activate avatar UI
        _avatars.SetActive(true);
        _selectionText.text = "Choose an avatar";

        //deactivate persona UI
        _personas.SetActive(false);
        _personaContinueButton.SetActive(false);
    }

    public void SetPlayerAvatar(Avatar avatar, RuntimeAnimatorController controller, RuntimeAnimatorController controllerR)  //function to access and set the player avatar on click
    {
        if (PlayerAvatar == avatar) return;
        if (PlayerAvatar != null) PlayerAvatar.DeselectAvatar();
        PlayerAvatar = avatar;
        AnimatorController = controller;
        if (controllerR != null) AnimatorControllerRight = controllerR;
        PlayerAvatarName = avatar.gameObject.name;
        if (!_avatarContinueButton.activeSelf) _avatarContinueButton.SetActive(true);
    }

    public void AvatarContinue() //continue button that takes player to the main game and update's the game state
    {
        if (PlayerAvatar == null) return;

        _characterSelection.SetActive(false);
        _startMenu.SetActive(false);
        GameManager.Instance.UpdateGameState(GameState.WorldZoom);
        //GameManager.Instance.UpdateGameState(GameState.Futureville);
    }
}
