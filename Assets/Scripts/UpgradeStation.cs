using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class UpgradeStation : MonoBehaviour
{
    public Player.e_Upgrades Upgrade;
    public int DefaultPrice;
    public TextMeshPro Modifier;
    public TextMeshPro Price;
    public UnityEvent Callback;

    private SteamMachine _machine;
    private int _currentPrice;
    private int _level;

    void Start()
    {
        _currentPrice = DefaultPrice;
        _level = 0;
        _machine = FindObjectOfType<SteamMachine>();
        Price.text = _currentPrice.ToString() + "C";
        Modifier.text = "+" + _level;
    }

    public bool Buy()
    {
        if (_machine.Steam > _currentPrice)
        {
            _machine.Steam -= _currentPrice;
            _currentPrice *= 2;
            _level++;
            Callback.Invoke();
            Price.text = _currentPrice.ToString() + "C";
            Modifier.text = "+" + _level;
            return true;
        }
        return false;
    }
}
