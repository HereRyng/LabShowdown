using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 0;
    private Rigidbody2D rb;
    [SerializeField] private int damage;
    [SerializeField] private int lifeTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        Move();
        Destroy(gameObject, lifeTime);
    }

    private void Move()
    {
        rb.velocity = transform.right * speed;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.GetComponent<StatsController>()?.TakeDamage(damage);
        if (collision != null) 
        {
          
            //Destroy(gameObject);
        }
        
        
    }
}