using DG.Tweening;
using System.Collections;
using UnityEngine;

public class NPCEscalation : MonoBehaviour
{
    [Header("General Gameplay Variables")]
    [SerializeField] private CommunityChaosManager _communityChaosManager;
    [SerializeField] private AudioManager _audioManager;

    public bool _hittable { get; private set; }
    public enum NPCType { Default, Question, Opinion, Takeover };
    public NPCType _npcType { get; private set; }
    //private Image _image;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Tween _npcShake;
    public Coroutine _coroutine;

    [Header("Escalation States")]
    [SerializeField] private Sprite _default;
    [SerializeField] private Sprite _question;
    [SerializeField] private Sprite _opinion;
    [SerializeField] private Sprite _takeover;

    [Header("Lives")]
    [SerializeField] private int _questionLives;
    [SerializeField] private int _opinionLives;
    [SerializeField] private int _takeOverLives;
    private int _lives;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _hittable = false;
        _npcType = NPCType.Default;
    }

    public void SetReferences(AudioManager audioManager, CommunityChaosManager gameManager)
    {
        _communityChaosManager = gameManager;
        _audioManager = audioManager;
    }

    public void Activate(float timer)
    {
        _hittable = true;
        _coroutine = StartCoroutine(WaitChangeColour(timer));
    }

    private IEnumerator WaitChangeColour(float timer)
    {
        _npcType = NPCType.Question;
        _spriteRenderer.sprite = _question;
        _npcShake.Kill();
        _npcShake = transform.DOShakePosition(1, 0.05f, 2, 90).SetLoops(-1);
        _lives = _questionLives;
        _audioManager.PlayAudioClip("Question");

        yield return new WaitForSeconds(timer);

        _communityChaosManager.TriggerAdjacent(this);
        _npcType = NPCType.Opinion;
        _spriteRenderer.sprite = _opinion;
        _npcShake.Kill();
        _npcShake = transform.DOShakePosition(0.5f, 0.05f, 2, 90).SetLoops(-1);
        _lives = _opinionLives;
        _audioManager.PlayAudioClip("Opinion");

        yield return new WaitForSeconds(timer);

        _npcType = NPCType.Takeover;
        _spriteRenderer.sprite = _takeover;
        _npcShake.Kill();
        _npcShake = transform.DOShakePosition(0.25f, 0.05f, 2, 90).SetLoops(-1);
        _lives = _takeOverLives;
        _audioManager.PlayAudioClip("TakeOver");

        yield return new WaitForSeconds(timer);

        if (_hittable) //missed takeover
        {
            _communityChaosManager.FullTakeOverTrigger();
        }
    }

    private void ResetLives()
    {
        switch (_npcType)
        {
            case NPCType.Question:
                _lives = _questionLives;
                break;

            case NPCType.Opinion:
                _lives = _opinionLives;
                break;

            case NPCType.Takeover:
                _lives = _takeOverLives;
                break;

            default:
                break;
        }
    }

    public void OnMouseDown()
    {
        if (_hittable)
        {
            PulseNPC();
            if (_lives >= 1) _lives--;
            else ResetNPC();
        }
    }

    public void EnergyDrinkPowerUp(int question, int opinion, int takeover)
    {
        _questionLives = question;
        _opinionLives = opinion;
        _takeOverLives = takeover;
        ResetLives();
    }

    public void FullTakeover()
    {
        _hittable = false;
        _npcType = NPCType.Takeover;
        _spriteRenderer.sprite = _takeover;
        _npcShake.Kill();
        _npcShake = transform.DOShakePosition(0.25f, 0.05f, 2, 90).SetLoops(-1);
    }

    public void ResetNPC()
    {
        if (_coroutine != null)  StopCoroutine(_coroutine);
        _communityChaosManager.DeEscalateNPC(this);
        _hittable = false;
        _npcType = NPCType.Default;
        _spriteRenderer.sprite = _default;
        _npcShake.Kill();
        transform.localPosition = Vector3.zero;
    }

    public void PulseNPC()
    {
        Vector3 origianlScale = Vector3.one;
        transform.localScale = origianlScale;
        transform.DOScale(1.2f, 0.1f).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            transform.DOScale(origianlScale, 0.1f).SetEase(Ease.InOutSine);
        });
    }
}
