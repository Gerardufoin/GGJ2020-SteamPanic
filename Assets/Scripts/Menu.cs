using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    private bool _go;

    private IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(1f);
        _go = true;
    }

    private void Start()
    {
        StartCoroutine(StartDelay());
    }

    private void Update()
    {
        if (_go && Input.anyKeyDown)
        {
            SceneManager.LoadSceneAsync(1);
        }
    }
}
