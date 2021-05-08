using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private Room OneDoor;
    [SerializeField] private Room TwoDoors;
    [SerializeField] private Room ThreeDoors;
    [SerializeField] private Room FourDoors;
    public int rows;
    public int colums;
    private List<Room> allRooms = new List<Room>();

    void Start()
    {
    }

    void Update()
    {
        
    }


    public void SpawnRoom()
    {
        Instantiate(OneDoor, new Vector3(0, 0, 0), new Quaternion(0,0,0,0));
    }
}
