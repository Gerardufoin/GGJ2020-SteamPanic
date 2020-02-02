using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    private float BASE_DAMAGE_PER_SEC = 1f;
    private int MAX_DAMAGE_STATE = 4;
    private float DAMAGE_INCREASE_PER_SECOND = 0.0003f;

    public float BaseMultiplier = 1f;
    public float MaxLife = 20f;

    private Animator _animator;
    private SteamMachine _machine;
    private AudioSource _audio;
    private float _life;
    private float _difficultyMultiplier = 1f;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();
    }

    void Start()
    {
        _machine = FindObjectOfType<SteamMachine>();
        _life = MaxLife - Random.Range(0f, MaxLife * 0.3f * BaseMultiplier);
    }

    void Update()
    {
        if (_life > 0f)
        {
            _life = Mathf.Max(0, _life - BASE_DAMAGE_PER_SEC * BaseMultiplier * _difficultyMultiplier * Time.deltaTime);
            if (_life == 0)
            {
                _machine.LeakingPipes++;
                _audio.Play();
            }
            _animator.SetInteger("Damage", (_life > 0 ? Mathf.CeilToInt(MAX_DAMAGE_STATE - _life / (MaxLife / MAX_DAMAGE_STATE)) - 1 : MAX_DAMAGE_STATE));
        }
        BaseMultiplier += DAMAGE_INCREASE_PER_SECOND * Time.deltaTime;
    }

    public void Fix(float amount)
    {
        if (_life == 0 && amount > 0)
        {
            _machine.LeakingPipes--;
            _audio.Stop();
        }
        _life = Mathf.Min(MaxLife, _life + amount);
    }

    public void Mute()
    {
        _audio.mute = true;
    }
}
