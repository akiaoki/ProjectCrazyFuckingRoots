using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class RootAgent : BuildableEntity
{
    public DecalProjector Decal;
    public float AttackRadius;
    public float AttackDamage;
    public float AttackDelay;
    public int MaxTargets = 3;

    public bool RegisterOnStart = true;

    private int _enemyLayer = 0;
    private float _attackTimeout = 0.0f;
    
    private GameUIController _gameUIController;
    private BuildController _buildController;

    private void Awake()
    {
        _collidersBuffer = new Collider[MaxTargets];
        _enemyLayer = LayerMask.NameToLayer("Enemy");
        _gameUIController = FindObjectOfType<GameUIController>();
        _buildController = FindObjectOfType<BuildController>();
    }

    private void Start()
    {
        OnBecomeDead += Die;
        var size = Decal.size;
        size.x = AttackRadius * 2.0f;
        size.y = AttackRadius * 2.0f;
        Decal.size = size;
        _gameUIController.SpawnHealthBar(this, Vector3.up * 2.0f);
        if (RegisterOnStart)
            _buildController.RegisterAgent(this);
    }

    private Collider[] _collidersBuffer;
    
    protected override void Update()
    {
        UpdateAttack();
        
        base.Update();
    }

    private void UpdateAttack()
    {
        if (_attackTimeout > 0.0f)
        {
            _attackTimeout -= Time.deltaTime;
            return;
        }

        _attackTimeout = AttackDelay;
        
        var targetCount = Physics.OverlapSphereNonAlloc(transform.position, AttackRadius, _collidersBuffer, 1 << _enemyLayer);
        for (var i = 0; i < targetCount; i++)
        {
            var c = _collidersBuffer[i];
            
            c.gameObject.SendMessage("TakeDamage", AttackDamage);
            // ToDo TakeDamage to enemy
            //AttackDamage
        }
    }

    private void Die(object sender, EventArgs args)
    {
        _buildController.UnregisterAgent(this);
        Destroy(gameObject);
    }
}
