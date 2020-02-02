using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    private AudioSource _audio;
    private float _time;
    private float _desiredPitch = 1f;

    private void Awake()
    {
        if (FindObjectsOfType<BGMManager>().Length > 1)
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);
        _audio = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += RestorePitch;
    }

    private void Update()
    {
        if (_audio.pitch > _desiredPitch)
        {
            _audio.pitch = Mathf.Max(_desiredPitch, _audio.pitch - 0.05f * Time.deltaTime);
        }
        if (_audio.pitch < _desiredPitch)
        {
            _audio.pitch = Mathf.Min(_desiredPitch, _audio.pitch + 0.05f * Time.deltaTime);
        }
    }

    private void RestorePitch(Scene scene, LoadSceneMode mode)
    {
        _audio.pitch = 1;
    }

    public void SetPitch(float value)
    {
        _desiredPitch = value;
    }
}
