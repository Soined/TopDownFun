using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ARoom : MonoBehaviour
{
    public ADoor[] doors;
    public Collider2D col;

    private void Start()
    {
        col = GetComponent<Collider2D>();
    }

    public List<Direction> GetAllDirections()
    {
        return doors.Select(d => d.direction).ToList();
    }
}
