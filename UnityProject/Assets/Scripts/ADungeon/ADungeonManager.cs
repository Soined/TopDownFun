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

    //List<Box> AllColliderPos = new List<Box>();

    Dictionary<ARoom, List<Box>> AllColliderPos = new Dictionary<ARoom, List<Box>>();

    public int randomSeed = 0;

    System.Random random;

    public GameObject spawnIndicator, spawnIndicatorDoor;


    public void Start()
    {
        randomSeed = UnityEngine.Random.Range(0, 1000);
        random = new System.Random(randomSeed);

        Physics2D.autoSyncTransforms = true;

        startRoom.isMainPath = true;

        AddColliderToList(startRoom, startRoom.GetComponents<BoxCollider2D>());

        SortRooms();

        CreateDungeon();
    }
    /// <summary>
    /// Returns the Position of the door that is spawned next to this room
    /// </summary>
    /// <param name="room"></param>
    /// <returns></returns>

    private void CreateDungeon()
    {
        if (!SpawnNextRoom(startRoom, 0, true, random.Next(minMainBody, maxMainBody)))
        {
            Debug.Log($"Dungeon spawn failed");
        }
        else
        {
            Debug.Log($"Dungeon spawn successfull");
        }
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
        if (roomCounter >= maxRoomsForCurrentPath - 1) //currentRoom == vorletzter Raum
        {
            bool mainPathSet = false;
            foreach (ADoor currentDoor in currentRoom.doors.Where(d => d.connectedDoor == null))
            {
                List<ARoom> useableEndRooms = FilterEndRoomsForDirection(currentDoor.InverseDirection);

                List<ARoom> useableMiddleRooms = FilterMiddleRoomsForDirection(currentDoor.InverseDirection);

                if (useableEndRooms.Count == 0) //Only wenn keine Door in irgendeinem Raum mit InverseDirection zu door gefunden werden kann.
                {
                    Debug.Log($"No Connecting room found for: {currentRoom.name}");
                    return false;
                }
                int usableRoomsCount = useableEndRooms.Count;
                bool roomFound = false;
                for (int i = 0; i < usableRoomsCount; i++)
                {
                    if (mainPath && mainPathSet)
                    {
                        if (CheckForNextRoom(ref useableMiddleRooms, currentDoor, roomCounter, mainPath, maxRoomsForCurrentPath, ref mainPathSet))
                        {
                            roomFound = true;
                            break;
                        }
                    }

                    if (CheckForEndRoom(ref useableEndRooms, currentDoor, mainPath, ref mainPathSet))
                    {
                        roomFound = true; //Endraum wurde gespawned => CurrentPath fertig und funktioniert
                        //Debug.Log($"MainPath: {mainPath && !mainPathSet}, finished generating");
                        break;
                    }
                }
                if (!roomFound) return false; //für eine Tür in currentRoom konnte kein Endraum gefunden werden
            }
            return true; //Alle Türen haben einen möglichen Raum gefunden
        }
        else //Mittelräume
        {
            bool mainPathSet = false;
            foreach (ADoor currentDoor in currentRoom.doors.Where(d => d.connectedDoor == null)) //Alle Pfade von aktuellem Room öffnen
            {
                List<ARoom> useableRooms = FilterMiddleRoomsForDirection(currentDoor.InverseDirection);

                if (useableRooms.Count == 0) //Only wenn keine Door in irgendeinem Raum mit InverseDirection zu door gefunden werden kann.
                {
                    Debug.Log($"No Connecting room found for: {currentRoom.name}");
                    return false;
                }
                int usableRoomsCount = useableRooms.Count;
                bool roomFound = false;
                for (int i = 0; i < usableRoomsCount; i++)
                {
                    if (CheckForNextRoom(ref useableRooms, currentDoor, roomCounter, mainPath, maxRoomsForCurrentPath, ref mainPathSet))
                    {
                        roomFound = true;
                        break;
                    }
                }
                if (!roomFound)
                {
                    return false;
                }
            }
            return true;
        }
    }
    bool CheckForEndRoom(ref List<ARoom> useableEndRooms, ADoor oldDoor, bool mainPath, ref bool mainPathSet)
    {
        ARoom spawnedRoom;

        if (mainPath && !mainPathSet)
        {
            spawnedRoom = SpawnRoom(oldDoor, finalRoom); //Getting spawnedRoom out of the coroutine by hacking
            mainPathSet = true;
        }
        else
        {
            spawnedRoom = SpawnRoom(oldDoor, ref useableEndRooms, out ARoom nextRoom);
        }

        if (spawnedRoom == null)
        {
            return false;
        }
        return true;
    }


    bool CheckForNextRoom(ref List<ARoom> useableRooms, ADoor oldDoor, int roomCounter, bool mainPath, int maxRoomsForCurrentPath, ref bool mainPathSet)
    {
        ARoom spawnedRoom = SpawnRoom(oldDoor, ref useableRooms, out ARoom currentRoomPrefab);
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
                spawnedRoom.isMainPath = true;
                mainPathSet = true;
                if (!OpenLoopForNextRoom(ref useableRooms, spawnedRoom, currentRoomPrefab, oldDoor, roomCounter, maxRoomsForCurrentPath, mainPath, ref mainPathSet)) return false;
            }
            else if (mainPath && mainPathSet) //Wir verlassen MainPath
            {
                //Debug.Log($"Sidepath created");
                //if (!OpenLoopForNextRoom(ref useableRooms, spawnedRoom, nextRoom, oldDoor, 0, random.Next(0, sidePathMaxLength), mainPath: false, ref mainPathSet)) return false;
                if (!OpenLoopForNextRoom(ref useableRooms, spawnedRoom, currentRoomPrefab, oldDoor, 0, sidePathMaxLength, mainPath: false, ref mainPathSet)) return false;
            }
            else //Wir sind nicht MainPath
            {
                //Debug.Log($"Sidepath added");
                if (!OpenLoopForNextRoom(ref useableRooms, spawnedRoom, currentRoomPrefab, oldDoor, roomCounter, maxRoomsForCurrentPath, mainPath: false, ref mainPathSet)) return false;
            }
        }
        return true; //Für alle Türen dieses Raumes konnte ein passender Raum gefunden werden
    }
    bool OpenLoopForNextRoom(ref List<ARoom> useableRooms, ARoom spawnedRoom, ARoom currentRoomPrefab, ADoor oldDoor, int roomCounter, int maxRoomsForCurrentPath, bool mainPath, ref bool mainPathSet)
    {
        if (!SpawnNextRoom(spawnedRoom, roomCounter + 1, mainPath, maxRoomsForCurrentPath)) //Wir haben keinen Raum gefunden, der an diesen passt
        {
            useableRooms.Remove(currentRoomPrefab);
            DeleteRoom(spawnedRoom, oldDoor, ref mainPathSet);
            if (!CheckForNextRoom(ref useableRooms, oldDoor, roomCounter, mainPath, maxRoomsForCurrentPath, ref mainPathSet)) //Checken einen anderen Raum in useableRooms
            {
                return false;
            }
        }
        return true;
    }

    private void DeleteRoom(ARoom toDeleteRoom, ADoor oldDoor, ref bool mainPathSet)
    {
        Debug.Log($"removed connection from {oldDoor.name} in Room {oldDoor.Room.name}");
        Debug.Log($"current toDeleteRoom: {toDeleteRoom.name}");

        oldDoor.connectedDoor.connectedDoor = null;
        oldDoor.connectedDoor = null;

        foreach (ADoor door in toDeleteRoom.doors)
        {
            if (door.connectedDoor == null) continue;

            DeleteRoom(door.connectedDoor.Room, door, ref mainPathSet);
        }

        //Destroy(toDeleteRoom.gameObject);
        if (toDeleteRoom.isMainPath) mainPathSet = false;

        AllColliderPos.Remove(toDeleteRoom);
        toDeleteRoom.gameObject.SetActive(false);
    }

    /// <summary>
    /// Finden falls möglich für eine Tür einen möglichen Raum, welcher nicht mit anderen Räumen kollidiert
    /// </summary>
    /// <param name="oldDoor"></param>
    /// <param name="currentPathCount"></param>
    /// <param name="mainPath"></param>
    /// <returns>false, wenn kein Raum an oldDoor verbunden werden kann</returns>
    ARoom SpawnRoom(ADoor oldDoor, ARoom RoomPrefab)
    {
        if (oldDoor.connectedDoor != null)
        {
            Debug.LogWarning($"Door{oldDoor} from {oldDoor.Room} already connected to {oldDoor.connectedDoor}");
            return null;
        }

        Vector3 nextDoorPosition = oldDoor.GetConnectedDoorPosition();

        ARoom spawnedRoom = Instantiate(RoomPrefab, Vector3.zero, Quaternion.identity);

        List<ADoor> suitableDoors = spawnedRoom.doors.Where(d => d.direction == oldDoor.InverseDirection).ToList();


        #region DoorLogik returned false, wenn keine Door von nextRoom an oldDoor angeknüpft werden kann
        ADoor suitableDoor = null;
        int suitableDoorsCount = suitableDoors.Count;
        for (int i = 0; i < suitableDoorsCount; i++)
        {
            ADoor nextDoor = suitableDoors[random.Next(0, suitableDoors.Count)];
            if (CheckCollisionForConnectingDoor(nextDoor, nextDoorPosition, spawnedRoom))
            {
                suitableDoor = nextDoor;
                break;
            }
        }

        if (suitableDoor == null)
        {
            Debug.Log($"SpawnRoom failed for door: {oldDoor} at Room: {oldDoor.Room} for new Room {spawnedRoom}");
            Destroy(spawnedRoom.gameObject);
            return null;
        }
        #endregion
        AddColliderToList(spawnedRoom, spawnedRoom.GetComponents<BoxCollider2D>());
        oldDoor.connectedDoor = suitableDoor;
        spawnedRoom.firstConnectedDoor = suitableDoor;
        suitableDoor.connectedDoor = oldDoor;
        return spawnedRoom;
    }

    //Hilfsfunktion für SpawnRoom()
    ARoom SpawnRoom(ADoor oldDoor, ref List<ARoom> useableRooms, out ARoom nextRoom)
    {
        nextRoom = null;
        if (useableRooms.Count == 0) return null;
        nextRoom = useableRooms[random.Next(0, useableRooms.Count)];
        ARoom spawnedRoom = SpawnRoom(oldDoor, nextRoom);
        if (spawnedRoom == null)
        {
            useableRooms.Remove(nextRoom); //Wir entfernen das Prefab aus useableRooms, wenn es nicht instantiated werden konnte
        }
        return spawnedRoom;
    }

    private void AddColliderToList(ARoom room, BoxCollider2D[] colliders)
    {
        List<Box> allBoxesForRoom = new List<Box>();
        colliders.ToList().ForEach(c => AddColliderToList(c, ref allBoxesForRoom));
        AllColliderPos.Add(room, allBoxesForRoom);
    }
    private void AddColliderToList(BoxCollider2D collider, ref List<Box> allBoxes)
    {
        Vector2 topLeftPoint = (Vector2)collider.bounds.center + new Vector2(-collider.bounds.extents.x, collider.bounds.extents.y);
        Vector2 bottomRightPoint = (Vector2)collider.bounds.center + new Vector2(collider.bounds.extents.x, -collider.bounds.extents.y);

        allBoxes.Add(new Box(topLeftPoint, bottomRightPoint));
    }

    /// <summary>
    /// If true the room can spawn, connecting itself to the previous room with the given door
    /// Also moves the room to its position if no collider stop this
    /// </summary>
    /// <param name="connectingDoor">Door in nextRoom that will be connected to our previous room</param>
    /// <param name="nextDoorPosition">Position that connectingDoor is supposed to appear</param>
    /// <param name="spawnedRoom"></param>
    /// <returns></returns>
    private bool CheckCollisionForConnectingDoor(ADoor connectingDoor, Vector3 nextDoorPosition, ARoom spawnedRoom)
    {
        spawnedRoom.transform.position = nextDoorPosition - connectingDoor.LocalPosition;


        //int collidingCount = Physics2D.OverlapCollider(nextRoom.col, filter, results);
        BoxCollider2D[] spawnedRoomColliders = spawnedRoom.GetComponents<BoxCollider2D>();

        foreach (BoxCollider2D col in spawnedRoomColliders)
        {
            Vector2 topLeftCol = (Vector2)col.bounds.center + new Vector2(-col.bounds.extents.x, col.bounds.extents.y);
            Vector2 bottomRightCol = (Vector2)col.bounds.center + new Vector2(col.bounds.extents.x, -col.bounds.extents.y);
            foreach (List<Box> boxes in AllColliderPos.Values)
            {
                foreach (Box box in boxes)
                {
                    if (CheckIfLinesIntersect(topLeftCol.x, bottomRightCol.x, box.topLeft.x, box.bottomRight.x)//Col.x-line does not intersect with box.x-line
                        && CheckIfLinesIntersect(topLeftCol.y, bottomRightCol.y, box.topLeft.y, box.bottomRight.y))//Col.y-line does not intersect with box.y-line
                    {
                        //If we reach this point, the col and the box do intersect
                        //Debug.Log($"intersection existing Col: ({box.topLeft}/{box.bottomRight}) with spawning: ({topLeftCol}/{bottomRightCol})");
                        return false;
                    }
                    //If we reach this point, we prove that col does not intersect with the current box
                }
            }
        }
        Instantiate(spawnIndicator, nextDoorPosition - connectingDoor.LocalPosition, Quaternion.identity);
        Instantiate(spawnIndicatorDoor, nextDoorPosition, Quaternion.identity);

        return true;

        //TODO: Check if all doors of nextRoom are valid (=can spawn another room)
        //TODO: If at MainPath only check for endRooms at any doors that are not connected to MainPath
    }

    private bool CheckIfLinesIntersect(float line1point1, float line1point2, float line2point1, float line2point2)
    {
        if (line1point1 < line2point1 && line1point1 < line2point2)
        {
            if (line1point2 < line2point1 && line1point2 < line2point2)
            {
                return false;
            }
        }
        else if ((line1point1 > line2point1 && line1point1 > line2point2))
        {
            if ((line1point2 > line2point1 && line1point2 > line2point2))
            {
                return false;
            }
        }
        return true;


        //return ((line1point1 < line2point1 && line1point1 < line2point2) && (line1point2 < line2point1 && line1point2 < line2point2)
        //    || (line1point1 > line2point1 && line1point1 > line2point2) && (line1point2 > line2point1 && line1point2 > line2point2));
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


    //private List<Direction> GetAllUnusedDirections(ARoom room)
    //{
    //    List<Direction> directions = new List<Direction>();
    //    foreach (ADoor d in room.doors)
    //    {
    //        if (d.isConnected) continue;

    //        directions.Add(d.direction);
    //    }

    //    return directions;
    //}

    //private ADoor GetAnyDoor(ARoom room)
    //{
    //    return room.doors[random.Next(0, room.doors.Length)];
    //}
}

public struct Box
{
    public Vector2 topLeft;
    public Vector2 bottomRight;

    public Box(Vector2 topLeft, Vector2 bottomRight)
    {
        this.topLeft = topLeft;
        this.bottomRight = bottomRight;
    }
}
