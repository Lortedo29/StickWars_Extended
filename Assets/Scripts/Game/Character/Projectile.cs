﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Projectile : MonoBehaviour
{
    #region Fields
    public static readonly float LIFETIME = 10f;
    public static readonly float STICKED_LIFETIME = 1f;

    [SerializeField] private ProjectileData _data;
    [SerializeField] private AudioSource _hitProjectileAudio;

    [HideInInspector] public int damage = 10;
    [HideInInspector] public Entity sender;

    private Vector3 _direction = Vector3.right;

    private Rigidbody2D _rb;
    #endregion

    #region Properties
    public Vector3 Direction
    {
        get
        {
            return _direction;
        }

        set
        {
            _direction = value.normalized;
            GetComponentInChildren<SpriteRenderer>().flipX = _direction.x > 0 ? false : true;
        }
    }
    #endregion

    #region Methods
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        Destroy(gameObject, LIFETIME);
    }

    void Update()
    {
        transform.position += _direction * _data.Speed * Time.deltaTime;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var entity = other.GetComponent<Entity>();

        if (entity != null && entity != sender)
        {
            entity.GetDamage(damage, sender);

            _hitProjectileAudio.transform.parent = null;
            _hitProjectileAudio.Play();

            Destroy(gameObject);
        }
    }
    #endregion
}
