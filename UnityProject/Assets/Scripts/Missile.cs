using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    Vector2 direction = Vector2.zero;

    public float destroyTime = 4f;
    private float _destroyTime = 0f;

    private float speed = 0;

    private void Update()
    {
        Vector2 moveVector = direction * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x + moveVector.x,
                                         transform.position.y + moveVector.y,
                                         transform.position.z);

        _destroyTime += Time.deltaTime;
        if(_destroyTime >= destroyTime)
        {
            Destroy(gameObject);
        }
    }
    public void Setup(int damage, float speed, Vector2 direction,float scale = 1)
    {
        this.direction = direction.normalized;
        this.speed = speed;
    }
}
