using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feet : MonoBehaviour
{
    public bool OnGround = true;

    private Player _player;

    void Start()
    {
        _player = GetComponentInParent<Player>();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        OnGround = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnGround = false;
    }
}
