using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Room : MonoBehaviour
{ 
    public bool openLeft;
    public bool openRight;
    public bool openUp;
    public bool openDown;
    public bool isStart;
    public bool isEnd;
    public bool allRoomsPlaced;
    public bool rightSpawned;
    public bool leftSpawned;
    public bool upSpawned;
    public bool downSpawned;
    public List<BoxCollider2D> allDoorColliders = new List<BoxCollider2D>();
    public Door[] allDoors;


    void Awake()
    {

        allDoors = gameObject.GetComponentsInChildren<Door>();
        allDoors.ToList().ForEach(d => allDoorColliders.Add(d.GetComponent<BoxCollider2D>()));
    }

    void Update()
    {
        
    }
}
