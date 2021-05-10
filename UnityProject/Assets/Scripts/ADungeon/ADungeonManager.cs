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
    // Room that contains the end of the Level
    private ARoom finalRoom;

    [SerializeField]
    private int sidePathMaxLength = 3;

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
        foreach (ADoor d in room.doors)
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





    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentRoom"></param>
    /// <param name="roomCounter">Sollte 1 sein, wenn zum ersten mal gecalled</param>
    /// <param name="mainPath"></param>
    /// <param name="maxRoomsForCurrentPath"></param>
    /// <returns></returns>
    private bool SpawnNextRoom(ARoom currentRoom, int roomCounter, bool mainPath, int maxRoomsForCurrentPath)
    {
        //Endräume
        if (roomCounter == maxRoomsForCurrentPath - 1) //currentRoom == vorletzter Raum
        {
            for(int i = 0; i < currentRoom.doors.Length; i++)
            { 
                List<ARoom> usableRooms = FilterEndRoomsForDirection(currentRoom.doors[i].InverseDirection);

                if (usableRooms.Count == 0) //Only wenn keine Door in irgendeinem Raum mit InverseDirection zu door gefunden werden kann.
                {
                    return false;
                }
                foreach (ARoom room in usableRooms)
                {
                    CheckForNextRoom(room, usableRooms, roomCounter, mainPath, maxRoomsForCurrentPath);
                }
            }
        }
        else //Mittelräume
        {
            foreach (ADoor door in currentRoom.doors) //Alle Pfade von aktuellem Room öffnen
            {
                List<ARoom> usableRooms = FilterMiddleRoomsForDirection(door.InverseDirection);

                if (usableRooms.Count == 0) //Only wenn keine Door in irgendeinem Raum mit InverseDirection zu door gefunden werden kann.
                {
                    return false;
                }
                int usableRoomsCount = usableRooms.Count;
                for (int i = 0; i < usableRoomsCount; i++)
                {
                    if(CheckForNextRoom(ref usableRooms, roomCounter, mainPath, maxRoomsForCurrentPath))
                    {
                        //Der letzte Raum wurde gespawned
                        return true;
                    }
                }
                return false; //Mindestens eine der Türen vom aktuellen Raum kann nicht verbunden werden
            }
        }
    }

    bool CheckForNextRoom(ref List<ARoom> usableRooms, int roomCounter, bool mainPath, int maxRoomsForCurrentPath)
    {
        bool mainPathSet = false;
        foreach (ADoor nextDoor in potentialNextRoom.doors) //TODO: Randomize door
        {
            ARoom spawnedRoom = SpawnRoom(nextDoor, endRoom: false, ref usableRooms);
            if (spawnedRoom == null)
            {
                //Kein Raum konnte für die aktuelle Tür gefunden werden, also ist potentialNextRoom nicht verwendbar
                return false;
            }
            else
            {
                //Wenn hier, dann potentialNextRoom => currentRoom
                if (mainPath && !mainPathSet) //Wir bleiben auf MainPath
                {
                    mainPathSet = true;
                    SpawnNextRoom(spawnedRoom, roomCounter + 1, mainPath, maxRoomsForCurrentPath);
                }
                else if (mainPath && mainPathSet) //Wir verlassen MainPath
                {
                    SpawnNextRoom(spawnedRoom, 0, false, random.Next(0, sidePathMaxLength));
                }
                else //Wir sind nicht MainPath
                {
                    SpawnNextRoom(spawnedRoom, roomCounter + 1, false, maxRoomsForCurrentPath);
                }
            }
        }
        return true; //Für alle Türen dieses Raumes konnte ein passender Raum gefunden werden
    }
    /* MainPathFunktion() -> SpawnNextRoom() macht Raum in Liste -> SpawnNextRoom() [bis path fertig]
     * Wenn SpawnNextRoom() == false => Letzte Raum despawn und neu, ohne bereits despawntem Raum mit derselben Tür
     * 
     * 
     */
    /// <summary>
    /// 
    /// </summary>
    /// <param name="oldDoor"></param>
    /// <param name="currentPathCount"></param>
    /// <param name="mainPath"></param>
    /// <returns>false, wenn kein Raum an oldDoor verbunden werden kann</returns>
    ARoom SpawnRoom(ADoor oldDoor, bool endRoom, ref List<ARoom> usableRooms)
    {
        //List<ARoom> usableRooms = endRoom ? FilterEndRoomsForDirection(oldDoor.InverseDirection)
        //    : FilterMiddleRoomsForDirection(oldDoor.InverseDirection);



        ARoom nextRoom = usableRooms[random.Next(0, usableRooms.Count)];

        Vector3 nextDoorPosition = oldDoor.GetConnectedDoorPosition();

        List<ADoor> suitableDoors = nextRoom.doors.Where(d => d.direction == oldDoor.InverseDirection).ToList();

        ARoom spawnedRoom = Instantiate(nextRoom, Vector3.zero, Quaternion.identity);

        #region DoorLogik returned false, wenn keine Door von nextRoom an oldDoor angeknüpft werden kann
        ADoor suitableDoor = null;
        int suitableDoorsCount = suitableDoors.Count;
        for (int i = 0; i < suitableDoorsCount; i++)
        {
            ADoor nextDoor = suitableDoors[random.Next(0, suitableDoors.Count)];
            if (DoorIsSuitableToConnect(nextDoor, nextDoorPosition, spawnedRoom))
            {
                suitableDoor = nextDoor;
                break;
            }
        }

        if (suitableDoor == null)
        {
            Destroy(spawnedRoom);
            usableRooms.Remove(nextRoom);
            return null;
        }
        #endregion

        return spawnedRoom;
    }

    /// <summary>
    /// If true the room can spawn, connecting itself to the previous room with the given door
    /// </summary>
    /// <param name="nextDoor"></param>
    /// <param name="nextDoorPosition"></param>
    /// <param name="nextRoom"></param>
    /// <returns></returns>
    private bool DoorIsSuitableToConnect(ADoor nextDoor, Vector3 nextDoorPosition, ARoom nextRoom)
    {
        nextRoom.transform.position = nextDoorPosition + nextDoor.GetRelativeRoomPosition();

        Collider2D[] results = { };
        ContactFilter2D filter = new ContactFilter2D();
        filter.layerMask = roomCheckLayer;
        //int collidingCount = Physics2D.OverlapCollider(nextRoom.col, filter, results);
        if (nextRoom.col.OverlapCollider(filter, results) > 0)
        {
            return false;
        }

        //TODO: Check if all doors of nextRoom are valid (=can spawn another room)
        //TODO: If at MainPath only check for endRooms at any doors that are not connected to MainPath


        return true;




        //return !(collidingCount > 0);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <param name="direction">Direction the Room must include</param>
    /// <returns></returns>
    private List<ARoom> FilterMiddleRoomsForDirection(Direction direction)
    {
        return allRooms.Where(r => r.GetAllDirections().Contains(direction)).ToList();
    }
    private List<ARoom> FilterEndRoomsForDirection(Direction direction)
    {
        return endRooms.Where(r => r.GetAllDirections().Contains(direction)).ToList();
    }



    private void SortRooms()
    {
        endRooms = allRooms.Where(r => r.doors.Length == 1).ToList();
        allRooms.RemoveAll(r => r.doors.Length == 1);
    }
}
