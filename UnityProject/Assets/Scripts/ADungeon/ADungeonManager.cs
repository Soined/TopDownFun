using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ADungeonManager : MonoBehaviour
{
    [SerializeField]
    // After Sorting only contains rooms that are not endRooms
    private List<ARoom> allRooms = new List<ARoom>();

    private List<ARoom> endRooms = new List<ARoom>();

    [SerializeField]
    private ARoom startRoom;

    private List<ARoom> mainRouteRooms = new List<ARoom>();

    [SerializeField]
    private LayerMask roomCheckLayer;

    [SerializeField]
    private int minMainBody, maxMainBody;

    System.Random random = new System.Random();



    public void SpawnDungeon()
    {
        SortRooms();

        SpawnMainRoute();
    }

    private void SpawnMainRoute()
    {
        int MainRoomCount = random.Next(minMainBody, maxMainBody);

        mainRouteRooms.Add(startRoom);

    }

    private List<Direction> GetAllUnusedDirections(ARoom room)
    {
        List<Direction> directions = new List<Direction>();
        foreach(ADoor d in room.doors)
        {
            if (d.isConnected) continue;

            directions.Add(d.direction);
        }

        return directions;
    }
    /// <summary>
    /// Returns the Position of the door that is spawned next to this room
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>
    private ADoor GetAnyDoor(ARoom room)
    {
        return room.doors[random.Next(0, room.doors.Length)];
    }
    private ARoom GetNextMainRoom()
    {
        ARoom lastRoom = mainRouteRooms[mainRouteRooms.Count - 1];
        
        ADoor nextDoor = GetAnyDoor(lastRoom);

        List<ARoom> usableRooms = FilterRoomsForDirection(nextDoor.InverseDirection);



    }

    private ARoom GetRandomSuitableRoom(ref List<ARoom> usableRooms)
    {
        
        if(usableRooms.Count == 0)
        {
            return null;
        }

        ARoom nextRoom = usableRooms[random.Next(0, usableRooms.Count)];

        //next room possible

        usableRooms.Remove(nextRoom);

        GetRandomSuitableRoom(ref usableRooms);
    }
    private bool RoomIsSuitable(ARoom nextRoom, ADoor oldDoor)
    {
        Vector3 nextDoorPosition = oldDoor.GetConnectedDoorPosition();

        List<ADoor> suitableDoors = nextRoom.doors.Where(d => d.direction == oldDoor.InverseDirection).ToList();

        ARoom spawnedRoom = Instantiate(nextRoom, Vector3.zero, Quaternion.identity);

        ADoor suitableDoor = null;
        int suitableDoorsCount = suitableDoors.Count;
        for(int i= 0; i < suitableDoorsCount;i++)
        {
            ADoor nextDoor = suitableDoors[random.Next(0, suitableDoors.Count)];
            if (DoorIsSuitable(nextDoor, nextDoorPosition, spawnedRoom))
            {
                suitableDoor = nextDoor;
                break;
            }
        }

        if(suitableDoor == null)
        {
            Destroy(spawnedRoom);
            return false;
        }
        return true;
    }

    private bool DoorIsSuitable(ADoor nextDoor, Vector3 nextDoorPosition, ARoom nextRoom)
    {
        nextRoom.transform.position = nextDoorPosition + nextDoor.GetRelativeRoomPosition();

        Collider2D[] results = { };
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = roomCheckLayer;

        if (nextRoom.col.OverlapCollider(filter, results) > 0)
        {
            return false;
        }

        //TODO: Check if all doors of nextRoom are valid (=can spawn another room)
        //TODO: If at MainPath only check for endRooms at any doors that are not connected to MainPath


        return true;
        

        //int collidingCount = Physics2D.OverlapCollider(nextRoom.col, filter, results);

        //return !(collidingCount > 0);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction">Direction the Room must include</param>
    /// <returns></returns>
    private List<ARoom> FilterRoomsForDirection(Direction direction)
    {
        return allRooms.Where(r => r.GetAllDirections().Contains(direction)).ToList();
    }



    private void SortRooms()
    {
        endRooms = allRooms.Where(r => r.doors.Length == 1).ToList();
        allRooms.RemoveAll(r => r.doors.Length == 1);
    }
}
