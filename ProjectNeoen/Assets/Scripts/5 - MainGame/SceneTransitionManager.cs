using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransitionManager : MonoBehaviour
{
    [SerializeField] private NPCDialogueManager _npcDialogueManager;
    [SerializeField] private Image _fader;
    [SerializeField] private float _fadeSpeed;
    [SerializeField] private string _miniGame;
    [field: SerializeField] public bool Transitioning { get; private set; }
    public Camera _camera;
    public CinemachineVirtualCamera Cam;

    private void Awake()
    {
        StartCoroutine(FadeFromBlack());
    }

    public void ReturnToGame() //function to return to futureville 
    {
        _fader.gameObject.SetActive(true);
        StartCoroutine(FadeToBlack(false, _miniGame));
    }

    public void LoadMiniGame() //function to load mini game
    {
        _fader.gameObject.SetActive(true);
        StartCoroutine(FadeToBlack(true, _miniGame));
    }

    public void SetSceneName(string sceneName) //set the name of the scene to load
    {
        _miniGame = sceneName;
    }

    private IEnumerator FadeToBlack(bool loadingScene, string miniGame) //start fading to black and set game state
    {
        //_npcDialogueManager.EndDialogue();
        Transitioning = true;
        _fader.GetComponentInParent<Canvas>().sortingOrder = 100;
        Color fadeColor = _fader.color;
        float fadeAmount;

        while (_fader.color.a < 1)
        {
            fadeAmount = fadeColor.a + (_fadeSpeed * Time.deltaTime);
            fadeColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, fadeAmount);
            _fader.color = fadeColor;
            yield return null;
        }

        //Load/Unload Scene
        if (loadingScene)
        {
            SceneManager.LoadScene(miniGame, LoadSceneMode.Additive);
            GameManager.Instance.UpdateGameState(GameState.MiniGame); //change state to mini game
        }
        else
        {
            SceneManager.UnloadSceneAsync(miniGame);
            Cam.m_Lens.OrthographicSize = 7;
            GameManager.Instance.UpdateGameState(GameState.Futureville); //change state to main game
            
            _npcDialogueManager.TriggerNPCDialogue();
        }

        StartCoroutine(FadeFromBlack());
    }

    private IEnumerator FadeFromBlack() //fade overlay from black
    {
        Color fadeColor = _fader.color;
        float fadeAmount;

        while (_fader.color.a > 0)
        {
            fadeAmount = fadeColor.a - (_fadeSpeed * Time.deltaTime);
            fadeColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, fadeAmount);
            _fader.color = fadeColor;
            yield return null;
        }

        _fader.GetComponentInParent<Canvas>().sortingOrder = 0;
        Transitioning = false;
        _fader.gameObject.SetActive(false);
    }
}
