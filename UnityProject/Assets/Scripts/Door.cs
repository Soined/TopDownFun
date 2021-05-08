using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public enum OpenDirection
    {
        Right,
        Left,
        Up,
        Down
    }
    public Vector2 directionVector;
    public OpenDirection openDirection;

    public void Start()
    {
        directionVector = DirectionVector();
    }

    public Vector2 DirectionVector()
    {
        switch (openDirection)
        {
            case OpenDirection.Right:
                return Vector2.right;
            case OpenDirection.Left:
                return Vector2.left;
            case OpenDirection.Up:
                return Vector2.up;
            case OpenDirection.Down:
                return Vector2.down;
            default:
                return Vector2.zero;

        }
    }
}
