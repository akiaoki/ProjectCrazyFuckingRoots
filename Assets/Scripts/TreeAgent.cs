using System;
using System.Collections;
using System.Collections.Generic;
using Models;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TreeAgent : BuildableEntity
{
    public DecalProjector Decal;
    public float BuildingRange;

    public bool ShowHealthBar = true;
    public bool RegisterOnStart = true;
    
    private float ManaTimeout;

    private GameController _gameController;
    private GameUIController _gameUIController;
    private BuildController _buildController;
    private BalanceController _balanceController;

    private void Awake()
    {
        _gameController = FindObjectOfType<GameController>();
        _gameUIController = FindObjectOfType<GameUIController>();
        _buildController = FindObjectOfType<BuildController>();
        _balanceController = FindObjectOfType<BalanceController>();
    }

    private void Start()
    {
        OnBecomeDead += Die;
        var size = Decal.size;
        size.x = BuildingRange * 2.0f;
        size.y = BuildingRange * 2.0f;
        Decal.size = size;
        
        if (ShowHealthBar)
            _gameUIController.SpawnHealthBar(this, Vector3.up * 5.0f);
        
        if (RegisterOnStart)
            _buildController.RegisterAgent(this);
    }

    protected override void Update()
    {
        base.Update();

        if (_gameController.State != GameState.Defending)
        {
            ManaTimeout = _balanceController.TreeManaRateSeconds;
            return;
        }
        
        if (ManaTimeout > 0.0f)
        {
            ManaTimeout -= Time.deltaTime;
            return;
        }

        ManaTimeout = _balanceController.TreeManaRateSeconds;

        _gameController.Mana += _balanceController.TreeManaValue;
        _gameUIController.SpawnFloatingText($"+{_balanceController.TreeManaValue}", transform.position + Vector3.up * 6.0f);
    }

    private void Die(object sender, EventArgs args)
    {
        _buildController.UnregisterAgent(this);
        Destroy(gameObject);
    }
}
