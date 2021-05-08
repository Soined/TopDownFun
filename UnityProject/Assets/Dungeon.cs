using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dungeon : MonoBehaviour
{
    [SerializeField] private Room[] AllRooms;
    [SerializeField] int seed;
    [SerializeField] private int maxRooms;

    public List<Room> rooms = new List<Room>();

    private RoomTemplate roomTemplates;

    private float roomLength;
    private float roomHeight;

    private bool spawningRooms;
    private bool firstInstansiated;
    private bool canSpawnNewRooms = true;

    private System.Random random;
    void Start()
    {
        random = new System.Random(seed);
        roomTemplates = GameObject.Find("RoomTemplate").GetComponent<RoomTemplate>();
    }

    void Update()
    {
        if (spawningRooms)
        {
            SpawnAllDungeonRooms();       
        }
    }

    public void StartDungeon()
    {
        if (!firstInstansiated)
        {
            Room room;
            room = Instantiate(AllRooms[random.Next(0,AllRooms.Length)], new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
            rooms.Add(room);
            roomLength = room.GetComponent<SpriteRenderer>().bounds.size.x;
            roomHeight = room.GetComponent<SpriteRenderer>().bounds.size.y;
            firstInstansiated = true;
        }
        spawningRooms = true;

    }

    private void TestForNeighbourInDirection(bool hasOpeningInDirection, ref bool hasSpawnedInDirection, Room targetRoom, int xModifier , int yModifier, Room[] availableRooms)
    {
        if (!canSpawnNewRooms) return;
       
        if (hasOpeningInDirection && !hasSpawnedInDirection && !targetRoom.allRoomsPlaced)
        {
            bool spawn = true;
            Vector3 tr = targetRoom.transform.position + new Vector3(targetRoom.transform.position.x + roomLength * xModifier, targetRoom.transform.position.y + roomHeight * yModifier, targetRoom.transform.position.z);
            /*foreach (var room in rooms)
            {
                if (Vector3.Distance(tr, room.transform.position) <= 1f)
                {
                    spawn = false;
                    //hasSpawnedInDirection = true;
                }
            }*/
            if (spawn)
            {

                //Debug.Log($"{targetRoom.name} wants to spawn at:{xModifier} {yModifier}");
                Room newRoom = Instantiate(availableRooms[random.Next(0, availableRooms.Length)], new Vector3(targetRoom.transform.position.x + roomLength * xModifier, targetRoom.transform.position.y + roomHeight *  yModifier, targetRoom.transform.position.z), new Quaternion(0, 0, 0, 0));
                rooms.Add(newRoom);
                SetSpawnedDirection(xModifier, yModifier, newRoom);
                CheckForNearbyOpenings(newRoom);
                canSpawnNewRooms = rooms.Count >= maxRooms ? false : true;
                hasSpawnedInDirection = true;
            }
        }
        else
        {
            hasSpawnedInDirection = true;
        }
    }

    public void CheckForNearbyOpenings(Room newSpawnedRoom)
    {
        Debug.Log(newSpawnedRoom.allDoorColliders.Count);
        foreach (var collider2D in newSpawnedRoom.allDoorColliders)
        {
            collider2D.enabled = false;
        }
        foreach(Door door in newSpawnedRoom.allDoors)
        {
            RaycastHit2D hitDoor = Physics2D.Raycast(door.transform.position, door.directionVector * 0.5f, LayerMask.GetMask("Door"));
            if(hitDoor.collider != null)
            {
                Room hitRoom = hitDoor.collider.gameObject.GetComponentInParent<Room>();
                switch (door.openDirection)
                {
                    case Door.OpenDirection.Right:
                        hitRoom.leftSpawned = true;
                        break;
                    case Door.OpenDirection.Left:
                        hitRoom.rightSpawned = true;
                        break;
                    case Door.OpenDirection.Up:
                        hitRoom.downSpawned = true;
                        break;
                    case Door.OpenDirection.Down:
                        hitRoom.upSpawned = true;
                        break;
                }
            }
        }
        foreach (var collider2D in newSpawnedRoom.allDoorColliders)
        {
            collider2D.enabled = true;
        }
    }

    public void SetSpawnedDirection(int xModifier, int yModifier, Room newSpawnedRoom)
    {
        Vector2 directionVector = new Vector2(xModifier, yModifier);
        switch (directionVector)
        {
            case Vector2 right when right.Equals(new Vector2(1, 0)):
                newSpawnedRoom.leftSpawned = true;
                break;
            case Vector2 left when left.Equals(new Vector2(-1, 0)):
                newSpawnedRoom.rightSpawned = true;
                break;
            case Vector2 up when up.Equals(new Vector2(0, 1)):
                newSpawnedRoom.downSpawned = true;
                break;
            case Vector2 down when down.Equals(new Vector2(0, -1)):
                newSpawnedRoom.upSpawned = true;
                break;
            default:
                Debug.LogWarning("I dont know these direction");
                break;
        }
    }

    public void SpawnAllDungeonRooms()
    {
        int a = rooms.Count;
            for(int i = 0; i < a; i++)
            {
                TestForNeighbourInDirection(rooms[i].openRight, ref rooms[i].rightSpawned, rooms[i], 1, 0, roomTemplates.leftRooms);
                TestForNeighbourInDirection(rooms[i].openLeft, ref rooms[i].leftSpawned, rooms[i], -1, 0, roomTemplates.rightRooms);
                TestForNeighbourInDirection(rooms[i].openUp, ref rooms[i].upSpawned, rooms[i],0,1, roomTemplates.downRooms);
                TestForNeighbourInDirection(rooms[i].openDown, ref rooms[i].downSpawned, rooms[i],0,-1, roomTemplates.upRooms);
            if (!canSpawnNewRooms) return;
                if (rooms[i].rightSpawned && rooms[i].leftSpawned && rooms[i].upSpawned && rooms[i].downSpawned)
                {
                    rooms[i].allRoomsPlaced = true;
                }
                
            }
        spawningRooms = false;
    }
}
