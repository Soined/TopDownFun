using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    Vector2 direction = Vector2.zero;

    private float speed = 0;

    private void Update()
    {
        Vector2 moveVector = direction * speed * Time.deltaTime;
        transform.position = new Vector3(transform.position.x + moveVector.x,
                                         transform.position.y + moveVector.y,
                                         transform.position.z);
    }
    public void Setup(int damage, float speed, Vector2 direction,float scale = 1)
    {
        this.direction = direction.normalized;
        this.speed = speed;
    }
}
