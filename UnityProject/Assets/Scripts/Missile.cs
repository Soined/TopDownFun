using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem BulletParticle;
    [SerializeField]
    private ParticleSystem DeathParticle;

    Vector2 direction = Vector2.zero;


    private int damage = 0;

    public float destroyTime = 4f;
    private float _destroyTime = 0f;

    private float speed = 0;

    private bool isDead = false;

    private void Update()
    {
        Vector2 moveVector = direction * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x + moveVector.x,
                                         transform.position.y + moveVector.y,
                                         transform.position.z);

        _destroyTime += Time.deltaTime;
        if (_destroyTime >= destroyTime && !isDead)
        {

            StartCoroutine(DeathCoroutine());
        }
    }
    public void Setup(int damage, float speed, Vector2 direction, float scale = 1)
    {
        this.damage = damage;
        this.direction = direction.normalized;
        this.speed = speed;
        DeathParticle.gameObject.SetActive(false);
        BulletParticle.gameObject.SetActive(true);
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.GetComponent<IDamageable>() is IDamageable damageable)
        {
            damageable.TakeDamage(damage);
            StartCoroutine(DeathCoroutine());
        }
    }

    private IEnumerator DeathCoroutine()
    {
        isDead = true;
        speed = 0;
        BulletParticle.gameObject.SetActive(false);
        DeathParticle.gameObject.SetActive(true);
        DeathParticle.Play();
        yield return new WaitForSeconds(DeathParticle.main.duration);
        Destroy(gameObject);
    }
}
