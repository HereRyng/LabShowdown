using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 0;

    [SerializeField] private int bulletType;

    private Rigidbody2D rb;

    private int damage= 10;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        Move();
    }

    private void Move()
    {
        rb.velocity = transform.right * speed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Agregar condicional
        collision.GetComponent<StatsController>()?.TakeDamage(damage);
        switch (bulletType)
        {
            case 1:
                collision.GetComponent<BuffsController>().Ignite();
                break;
            case 2:
                collision.GetComponent<BuffsController>().Frozeen();
                break;
        }
    }
}
