using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.AI;

public class randomSystem_03 : MonoBehaviour
{
    //output 
    public List<vertex> OpenVertex = new List<vertex>();
    public List<path> OpenPath = new List<path>();
    //input 
    public NavMeshSurface surface;
    public List<room> SorceRoom = new List<room>();
    public List<room> SpecialRoom = new List<room>();
    public List<path> paths = new List<path>();//all paths
    public List<vertex> vertices = new List<vertex>();//all vertices
    public List<pathFriend_Anti> pathFriend = new List<pathFriend_Anti>();
    public List<pathFriend_Anti> pathAnti = new List<pathFriend_Anti>();
    public room BossRoom;
    /**/
    public int Num_StageRoom = 0;//max 5!!!
    public SpawnPoint spawnPoint;
    public EnemySpawnSystem enemySpawnSystem;
    public WarpGateSystem WrapGateToTheNextLevel;
    public string nextMap = "";
    //inFunction
    room selectedRoom;
    List<room> rooms = new List<room>();//all rooms (count = num_StageRoom)
    List<vertex> doors = new List<vertex>();// all opened doors
    List<vertex> NotConnectedDoors = new List<vertex>();
    List<vertex> UnUsedVertices = new List<vertex>();//pick a new room pos from here
    bool setUpPathSuccess = false;
    //for BFS
    List<vertex> dontUse = new List<vertex>();
    List<vertex> empty = new List<vertex>();

    [Tooltip("As percent")]
    public int specailRoomAppearChance = 10;


    // FOR CAMERA FOLLOW WHEN ENTER ROOM
    public CinemachineVirtualCamera mainCam;
    public CinemachineVirtualCamera roomCam;

    void Start()
    {
        setup_vertex();
        int errorCount = 0;
        while (!setUpPathSuccess && errorCount < 100)
        {
            Reset_all();
            setUpRoom(Num_StageRoom);
            setUpPath();
            errorCount++;
        }

        build();
        surface.BuildNavMesh();

        if (WrapGateToTheNextLevel == null) Debug.LogError("No WarpGate This Scene");
        else WrapGateToTheNextLevel.gameObject.SetActive(false);
    }

    private void Reset_all()
    {
        OpenPath = new List<path>();
        OpenVertex = new List<vertex>();
        foreach (room r in rooms)
        {
            DestroyImmediate(r.gameObject);
            //Destroy(r.gameObject);
        }
        NotConnectedDoors = new List<vertex>();
        UnUsedVertices = new List<vertex>();
        //Reset vertex
        foreach (vertex v in vertices)
        {
            UnUsedVertices.Add(v);
            v.Reset();
        }
        //Reset door
        foreach (vertex d in doors)
        {
            d.pass = null;
            d.connection = new List<vertex>();
        }
        rooms = new List<room>();
        doors = new List<vertex>();
    }


    void setup_vertex()
    {
        foreach (vertex v in vertices)
        {
            UnUsedVertices.Add(v);
            v.setup(v.input, v.inputd);
        }
    }

    void setUpRoom(int n) //create a room and move to the posible vertex
    {
        for (int i = 0; i < n; i++)
        {
            //random vertex
            vertex v = randomVertex(UnUsedVertices);
            room r = new room();
            //create a room
            if (i == n - 1)
            {
                r = Instantiate(BossRoom);
                r.status = RoomStatus.bossRoom;
            }
            else if (i == 0)
            {
                r = Instantiate(SorceRoom[0]);
                r.status = RoomStatus.startRoom;
                //r.status = RoomStatus.clearedRoom;

            }
            else
            {
                int randomChanceRoom = Random.Range(0, 100);
                if (randomChanceRoom < specailRoomAppearChance)
                {
                    selectedRoom = SpecialRoom[Random.Range(0, SpecialRoom.Count)];
                }
                else
                {
                    selectedRoom = SorceRoom[Random.Range(0, SorceRoom.Count)];
                }
                r = Instantiate(selectedRoom);
            }
            //r.virtualRoom = SorceRoom_Sample;
            r.transform.SetParent(transform);

            //move to the vertex
            r.transform.position = v.transform.position;

            //random open door in r
            //door 0 = top , 1 = right, 2 = buttom, 3 = left 
            List<int> tempDoor = new List<int>();
            foreach (int d in v.canOpenDoors){ 
                tempDoor.Add(d);
            }
            for (int j = 0; j < 2; j++){
                int index = Random.Range(0, 20);
                if(tempDoor.Contains(index)&& tempDoor.Count > 2)
                    tempDoor.Remove(index);
            }
            foreach (int d in tempDoor){
                vertex door = r.doors[d];
                door.parent = r;
                doors.Add(door);
                NotConnectedDoors.Add(door);
                r.OpenedDoors.Add(door);
            }
            // for (int j = 0; j < 10; j++)
            // {
            //     int index = v.canOpenDoors[Random.Range(0, v.canOpenDoors.Length - 1)];
            //     vertex door = r.doors[index];
            //     print(r + " " + v + " get " + door);
            //     if (!r.OpenedDoors.Contains(door))
            //     {
            //         door.parent = r;
            //         doors.Add(door);
            //         NotConnectedDoors.Add(door);
            //         r.OpenedDoors.Add(door);
            //     }
            // }
            //change w.connection (remove vertex and add door/if posible)
            foreach (vertex w in v.connection)
            {
                w.connection.Remove(v);
                vertex theDoor = r.doors[w.door_connection[v]];
                if (r.OpenedDoors.Contains(theDoor))
                {
                    w.connection.Add(theDoor);
                    theDoor.connection.Add(w);
                    foreach (path p in w.paths_connection)
                    {
                        if (p.connection.Contains(v))
                        {
                            OpenPath.Add(p);
                        }
                    }
                }
            }

            for (int j = 0; j < 4; j++)
            {
                if (r.OpenedDoors.Contains(r.doors[j]))
                {
                    //close wall 
                    r.walls[j].gameObject.SetActive(false);
                }
                else
                {
                    //close door
                    r.doors[j].gameObject.SetActive(false);
                }
            }
            r.name = "room" + i;

            if (i == 0) spawnPoint.transform.position = r.transform.position;
            //add r to rooms
            rooms.Add(r);
            r.randomFrom = this;

        }
    }

    vertex randomVertex(List<vertex> pickFromHere)
    {
        if (pickFromHere.Count == 0)
        {
            return null;
        }
        vertex ans = pickFromHere[Random.Range(0, pickFromHere.Count - 1)];
        pickFromHere.Remove(ans);
        foreach (vertex v in ans.connection)
        {
            pickFromHere.Remove(v);
        }
        return ans;
    }

    void setUpPath()
    {
        while (NotConnectedDoors.Count > 0)
        {
            List<vertex> CantMatch = new List<vertex>();

            vertex s = NotConnectedDoors[Random.Range(0, NotConnectedDoors.Count - 1)];
            vertex t = randomDoor(s, CantMatch);

            int errorCount = 0;
            while (!BFS(s, t, empty) && errorCount <= 8)
            {
                CantMatch.Add(t);
                resetvisited();
                t = randomDoor(s, CantMatch);
                errorCount++;
                if (errorCount == 8)
                {
                    setUpPathSuccess = false;
                    return;
                }
            }

            CantMatch = new List<vertex>();
            NotConnectedDoors.Remove(s);
            // if (NotConnectedDoors.Contains(t))
            // {
            //     NotConnectedDoors.Remove(t);
            // }
            savepath(t, s);
            resetvisited();
        }
        setUpPathSuccess = true;
    }

    vertex randomDoor(vertex s, List<vertex> CantPick)
    {
        List<vertex> pickable = new List<vertex>();
        foreach (vertex d in doors)
        {
            if (d.parent != s.parent && !CantPick.Contains(d))
            {
                pickable.Add(d);
            }
        }
        if (pickable.Count == 0)
        {
            return null;
        }
        vertex t = pickable[Random.Range(0, pickable.Count - 1)];
        return t;
    }

    bool BFS(vertex s, vertex t, List<vertex> notAvailable)
    {
        Queue<vertex> next = new Queue<vertex>();
        next.Enqueue(s);
        s.visited = true;
        while (next.Count != 0)
        {
            vertex v = next.Dequeue();
            foreach (vertex w in v.connection)
            {
                if (w == t)
                {
                    w.pass = v;
                    return true;
                }
                else if (!w.visited && !notAvailable.Contains(w))
                {
                    next.Enqueue(w);
                    w.visited = true;
                    w.pass = v;
                }
            }
        }
        return false;
    }

    bool savepath(vertex D, vertex d)
    {
        vertex temp = D;
        while (temp != d)
        {
            if (temp == null || temp.pass == null) return false;
            if (!OpenVertex.Contains(temp))
            {
                OpenVertex.Add(temp);
            }
            foreach (path p in temp.paths_connection)
            {
                if (p.connection.Contains(temp) && p.connection.Contains(temp.pass))
                {
                    if (!OpenPath.Contains(p))
                    {
                        OpenPath.Add(p);
                    }
                }
            }
            temp = temp.pass;
        }
        OpenVertex.Add(d);
        return true;
    }

    void resetvisited()
    {
        foreach (vertex v in vertices)
        {
            v.visited = false;
            v.pass = null;
        }
    }

    void build()
    {
        foreach (path p in paths)
        {
            if (!OpenPath.Contains(p))
            {
                p.gameObject.SetActive(false);
            }
        }
        foreach (vertex v in vertices)
        {
            if (!OpenVertex.Contains(v))
            {
                v.gameObject.SetActive(false);//close vertex that not be used
            }
        }
        foreach (pathFriend_Anti pf in pathFriend)
        {
            if (!OpenVertex.Contains(pf.pair))
            {
                pf.gameObject.SetActive(false);
            }
        }
        foreach (pathFriend_Anti pa in pathAnti)
        {
            if (OpenPath.Contains(pa.pair_path))
            {
                pa.gameObject.SetActive(false);
            }
        }
    }

    public bool OnAllRoomClear()
    {
        foreach (room r in rooms)
        {
            if (r.status != RoomStatus.clearedRoom)
            {
                return false;
            }
        }

        return true;
    }

}

// Manage Each Room
public enum RoomStatus
{
    startRoom, bossRoom, normalRoom, healRoom, clearedRoom
}
