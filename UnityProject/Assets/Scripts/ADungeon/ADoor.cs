using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADoor : MonoBehaviour
{
    public Direction direction;

    public ADoor connectedDoor = null;

    public ARoom Room { get => GetComponentInParent<ARoom>(); }

    public Vector3 LocalPosition { get => transform.localPosition; }

    public Direction InverseDirection { get
        {
            switch (direction)
            {
                case Direction.Up:
                    return Direction.Down;
                case Direction.Down:
                    return Direction.Up;
                case Direction.Left:
                    return Direction.Right;
                case Direction.Right:
                    return Direction.Left;
                default:
                    return Direction.Down;
            }
        } }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Nächster Raum
    }

    public Vector3 GetConnectedDoorPosition()
    {
        switch(direction)
        {
            case Direction.Left:
                return new Vector2(transform.position.x, transform.position.y) + Vector2.left * 10f;
            case Direction.Right:
                return new Vector2(transform.position.x, transform.position.y) + Vector2.right * 10f;
            case Direction.Up:
                return new Vector2(transform.position.x, transform.position.y) + Vector2.up * 10f;
            case Direction.Down:
                return new Vector2(transform.position.x, transform.position.y) + Vector2.down * 10f;
            default:
                return Vector2.zero;
        }
    }
    /// <summary>
    /// Vector3 of this door to the center of its room
    /// </summary>
    /// <returns></returns>


}
public enum Direction
{
    Left,
    Right,
    Up,
    Down
}