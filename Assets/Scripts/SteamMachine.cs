using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SteamMachine : MonoBehaviour
{
    private const float LIFE_INFLUENCE = 1.5f;
    private const int PIPES_NUMBER = 6;
    private const float GAME_OVER_STEAM_DURATION = 2f;
    private const float STEAM_GEN_MODIF = 0.5f;

    public int LeakingPipes;
    [SerializeField]
    private Transform _indicator = null;
    [SerializeField]
    private Vector2 _indicatorMinMax = new Vector2(-75, 75);
    [SerializeField]
    private TextMeshProUGUI _scoreUI = null;
    [SerializeField]
    private TextMeshProUGUI _steamUI = null;
    [SerializeField]
    private Image _steamScreenUI = null;
    [SerializeField]
    private TextMeshProUGUI _finalScoreUI = null;
    [SerializeField]
    private TextMeshProUGUI _finalScoreValueUI = null;
    [SerializeField]
    private TextMeshProUGUI _clickToContinueUI = null;

    private float _steam;
    private bool _returnMenu;
    private bool _gameOver;
    [HideInInspector]
    public bool Running
    {
        get
        {
            return !_gameOver;
        }
    }
    public int Steam
    {
        get
        {
            return Mathf.FloorToInt(_steam);
        }
        set
        {
            _steam = value;
        }
    }
    private float _score;
    public int Score
    {
        get
        {
            return Mathf.FloorToInt(_score);
        }
    }

    private int _steamGen = 0;
    private float _maxLife = 100f;
    private float _life;
    public float LifeRatio
    {
        get
        {
            return _life / _maxLife;
        }
    }

    private void Awake()
    {
        _steamScreenUI.material.SetFloat("_Progress", 0f);
        _finalScoreUI.gameObject.SetActive(false);
        _finalScoreValueUI.gameObject.SetActive(false);
        _clickToContinueUI.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        _steamScreenUI.material.SetFloat("_Progress", 0f);
    }

    private void Start()
    {
        _life = _maxLife;
    }

    private IEnumerator GameOverAnimation()
    {
        FindObjectOfType<BGMManager>().SetPitch(1f);
        float timer = 0f;
        while (timer < GAME_OVER_STEAM_DURATION)
        {
            _steamScreenUI.material.SetFloat("_Progress", Mathf.Lerp(0f, 1f, timer / GAME_OVER_STEAM_DURATION));
            timer += Time.deltaTime;
            yield return null;
        }
        foreach (Pipe pipe in FindObjectsOfType<Pipe>())
        {
            pipe.Mute();
        }
        yield return new WaitForSeconds(1f);
        _finalScoreUI.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        _finalScoreValueUI.text = "0";
        _finalScoreValueUI.gameObject.SetActive(true);
        timer = 0f;
        while (timer < GAME_OVER_STEAM_DURATION)
        {
            _finalScoreValueUI.text = Mathf.FloorToInt(_score * Mathf.Lerp(0f, 1f, timer / GAME_OVER_STEAM_DURATION)).ToString();
            timer += Time.deltaTime;
            yield return null;
        }
        _finalScoreValueUI.text = Score.ToString();
        yield return new WaitForSeconds(1f);
        _clickToContinueUI.gameObject.SetActive(true);
        _returnMenu = true;
    }

    private void Update()
    {
        if (_returnMenu && Input.anyKeyDown)
        {
            SceneManager.LoadSceneAsync(0);
        }

        if (Running)
        {
            _life = Mathf.Max(0, Mathf.Min(_maxLife, _life + (LIFE_INFLUENCE + _steamGen * STEAM_GEN_MODIF) * Time.deltaTime - LeakingPipes * LIFE_INFLUENCE * Time.deltaTime));
            _indicator.rotation = Quaternion.Euler(0f, 0f, _indicatorMinMax.y - _life / _maxLife * (_indicatorMinMax.y - _indicatorMinMax.x));
            if (_life <= 0)
            {
                _gameOver = true;
                StartCoroutine(GameOverAnimation());
                return;
            }
            _steam += (PIPES_NUMBER - LeakingPipes) * (1 + _steamGen * STEAM_GEN_MODIF) * Time.deltaTime;
            _steamUI.text = Mathf.FloorToInt(_steam).ToString();
            _score += (PIPES_NUMBER - LeakingPipes) * (1 + _steamGen * STEAM_GEN_MODIF) * Time.deltaTime;
            _scoreUI.text = Score.ToString();
        }
    }

    public void UpgradeSteamGen()
    {
        _steamGen++;
    }
}
