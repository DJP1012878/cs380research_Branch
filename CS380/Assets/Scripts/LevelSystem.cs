using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum DummyEnum
{
  HazardA,
  HazardB,
  HazardC
}

public class LevelSystem : MonoBehaviour {
  [Header("Necessary Objects")]
  public GameObject StartRoom;
  public GameObject GoalRoom;
  public GameObject[] Rooms;
  public GameObject Wall;
  public GameObject PatherSystem;

  [Header("Room Variables")]
  public Vector3 m_RoomBaseSize;

  [Header("System Variables")]
  public bool m_GenerateOnStart;
  public int m_StartHeight;
  public int m_StartWidth;

  [Header("System Restrictions")]
  public int m_MinWidth;
  public int m_MinHeight;
  public int m_MinGoalOffset;

  [Header("Debug")]
  public List<Vector3> m_RoomPositions;
  public List<GameObject> m_SpawnedRooms;


  [Header("Stats")]
  public Player_Stat_Traker stats;
  public Target path;
  public bool SetSeed;
  public int seed;

  //Privates
  int p_RoomIndexOffset = 5;
  float p_TotalHeuristic;
  public List<int> p_SortedIndex;
  public List<GameObject> p_SortedRooms;   //Possible Usage to sort by heuristic cost

    private void Awake()
    {
        p_SortedRooms = new List<GameObject>();
        m_SpawnedRooms = new List<GameObject>();
    p_SortedIndex = new List<int>();

        if (m_GenerateOnStart)
            GenerateRooms(m_StartHeight, m_StartWidth);

        GetComponent<NavMeshSurface>().BuildNavMesh();
    }
    // Use this for initialization
    void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
    }
  
  //Returns success or failure
  public bool GenerateRooms(int height, int width, bool random = false)
  {
    Debug.Log("[LVGEN] Generating Rooms");

    if (SetSeed != true)
    {
       seed = (int)(Random.value * 100000);
    }
    Random.InitState(seed++);

    //Reset Lists
    m_RoomPositions.Clear();
    for (int i = 0; i < m_SpawnedRooms.Count; ++i)
      Destroy(m_SpawnedRooms[i]);
    m_SpawnedRooms.Clear();

    //Check that given parameters are valid
    if (height < m_MinHeight)
      height = m_MinHeight;
    if (width < m_MinWidth)
      width = m_MinWidth;

    //Sort Rooms
    SortRooms();

    //Find lowest left corner
    Vector3 anchor = Vector3.zero;
    anchor.x = -m_RoomBaseSize.x * width;
    anchor.z = -m_RoomBaseSize.z * height;

    //Set a start and goal
    Vector2 startIndex = 
      new Vector2(Random.Range(0, width), Random.Range(0, height));
    Vector2 goalIndex =
      new Vector2(Random.Range(0, width), Random.Range(0, height));
    int goalOffset =
      IndexDistance(startIndex, goalIndex);
    
    //Make sure start and goal are a minimum # of rooms apart
    while (goalOffset < m_MinGoalOffset)
    {
      goalIndex =
        new Vector2(Random.Range(0, width), Random.Range(0, height));
      goalOffset =
        IndexDistance(startIndex, goalIndex);
    }


    //Weighted Random Room Generation
    if (!random)
    {
      //Find maximum index distance between the goal and any index
      float maxIndexDistance = 0;
      //X Distance
      if (Mathf.Abs((int)goalIndex.x - (width - 1)) > Mathf.Abs((int)goalIndex.x - 0))
      {
        maxIndexDistance += Mathf.Abs((int)goalIndex.x - (width - 1));
        Debug.Log("Width: " + goalIndex + " value: " + Mathf.Abs((int)goalIndex.x - (width - 1)));
      }
      else
      {
        maxIndexDistance += Mathf.Abs((int)goalIndex.x - 0);
        Debug.Log("0: " + goalIndex + " value: " + Mathf.Abs((int)goalIndex.x - 0));
      }
      //Y Distance
      if (Mathf.Abs((int)goalIndex.y - (height - 1)) > Mathf.Abs((int)goalIndex.y - 0))
      {
        maxIndexDistance += Mathf.Abs((int)goalIndex.y - (height - 1));
        Debug.Log("Height: " + goalIndex + " value: " + Mathf.Abs((int)goalIndex.y - (height - 1)));
      }
      else
      {
        maxIndexDistance += Mathf.Abs((int)goalIndex.y - 0);
        Debug.Log("0: " + goalIndex + " value: " + Mathf.Abs((int)goalIndex.y - 0));
      }

      Debug.Log("[LVGEN] Max Index Distance from goal is: " + maxIndexDistance);

      //Create rooms
      for (int i = 0; i < height; ++i)
      {
        for (int j = 0; j < width; ++j)
        {
          GameObject spawnedRoom;
          int range = 0;
          //Set mask value
          float indexDistance =
            IndexDistance(new Vector2(j, i), goalIndex);
          //Debug.Log("Index Distance from: (" + j + ", " + i + ") is " + indexDistance);
          float distanceRatio = indexDistance / maxIndexDistance;
          float allowedHeuristic = p_TotalHeuristic * distanceRatio;
          float currentHeuristic = 0;

          while (currentHeuristic < allowedHeuristic)
          {
            //Debug.Log("Total Heuristic: " + p_TotalHeuristic +
            //  " Allowed Heuristic: " + allowedHeuristic +
            //  " Current Heurisitc: " + currentHeuristic);
            currentHeuristic += NavMesh.GetAreaCost(p_SortedIndex[range]);
            ++range;
          }

          if (currentHeuristic != allowedHeuristic)
            --range;

          //Range should always be at least 50% of the sorted rooms
          if (range < (p_SortedRooms.Count * .6f))
          {
            range = (int)(p_SortedRooms.Count * .6f);
          }

          Debug.Log("[LVGEN] Range set to: " + range +
                    " at Distance Ratio: " + distanceRatio +
                    " from index: " + new Vector2(j, i) +
                    " to goal index: " + goalIndex);

          bool spawn = false;
          bool goal = false;
          //Check if start or goal
          if (i == startIndex.y && j == startIndex.x)
          {
            spawnedRoom = Instantiate(StartRoom);
            spawn = true;
          }
          else if (i == goalIndex.y && j == goalIndex.x)
          {
            spawnedRoom = Instantiate(GoalRoom);
            goal = true;
          }
          else
            spawnedRoom =
              Instantiate(p_SortedRooms[Random.Range(0, range)]);

          Vector3 spawnPos = anchor;
          spawnPos.x += m_RoomBaseSize.x * j;
          spawnPos.z += m_RoomBaseSize.z * i;
          spawnedRoom.transform.position = spawnPos;

          if (spawn == true)
          {
            //if it is the startroom, then move the player and path agent there
            stats.gameObject.transform.position = spawnPos;
            stats.spawn = spawnedRoom;
            path.gameObject.GetComponent<NavMeshAgent>().Warp(spawnPos);
          }
          else if (goal == true)
          {
            path.goal = spawnPos;
            PatherSystem.GetComponent<PatherSystem>().Destination = spawnPos;
          }

          //Push into lists
          m_RoomPositions.Add(spawnPos);
          m_SpawnedRooms.Add(spawnedRoom);
        }
      }
    }
    //Completely random
    else
    {
      //Create rooms
      for (int i = 0; i < height; ++i)
      {
        for (int j = 0; j < width; ++j)
        {
          GameObject spawnedRoom;
          bool spawn = false;
          bool goal = false;
          //Check if start or goal
          if (i == startIndex.y && j == startIndex.x)
          {
            spawnedRoom = Instantiate(StartRoom);
            spawn = true;
          }
          else if (i == goalIndex.y && j == goalIndex.x)
          {
            spawnedRoom = Instantiate(GoalRoom);
            goal = true;
          }
          else
            spawnedRoom =
              Instantiate(Rooms[Random.Range(p_RoomIndexOffset, (int)Room.RoomIndex - 1)]);

          Vector3 spawnPos = anchor;
          spawnPos.x += m_RoomBaseSize.x * j;
          spawnPos.z += m_RoomBaseSize.z * i;
          spawnedRoom.transform.position = spawnPos;

          if (spawn == true)
          {
            //if it is the startroom, then move the player and path agent there
            stats.gameObject.transform.position = spawnPos;
            stats.spawn = spawnedRoom;
            path.gameObject.GetComponent<NavMeshAgent>().Warp(spawnPos);
          }
          else if (goal == true)
          {
            path.goal = spawnPos;
            PatherSystem.GetComponent<PatherSystem>().Destination = spawnPos;
          }

          //Push into lists
          m_RoomPositions.Add(spawnPos);
          m_SpawnedRooms.Add(spawnedRoom);
        }
      }
   }


    //Create border walls
    float zRot = 90.0f;
    anchor.y += m_RoomBaseSize.y * .5f;
    for (int i = 0; i < height; ++i)
    {
      GameObject spawnedWall;
      Vector3 spawnPos = anchor;
      spawnPos.x -= m_RoomBaseSize.x * .5f;
      spawnPos.z += m_RoomBaseSize.z * i;
      spawnedWall = Instantiate(Wall);
      spawnedWall.transform.position = spawnPos;
      spawnedWall.transform.eulerAngles =
        new Vector3(spawnedWall.transform.eulerAngles.x,
                    spawnedWall.transform.eulerAngles.y,
                    zRot);

      spawnPos = anchor;
      spawnPos.x += m_RoomBaseSize.x * width;
      spawnPos.x -= m_RoomBaseSize.x * .5f;
      spawnPos.z += m_RoomBaseSize.z * i;
      spawnedWall = Instantiate(Wall);
      spawnedWall.transform.position = spawnPos;
      spawnedWall.transform.eulerAngles =
        new Vector3(spawnedWall.transform.eulerAngles.x,
                    spawnedWall.transform.eulerAngles.y,
                    zRot);
    }
    zRot = 0;
    for (int i = 0; i < width; ++i)
    {
      GameObject spawnedWall;
      Vector3 spawnPos = anchor;
      spawnPos.x += m_RoomBaseSize.x * i;
      spawnPos.z -= m_RoomBaseSize.z * .5f;
      spawnedWall = Instantiate(Wall);
      spawnedWall.transform.position = spawnPos;
      spawnedWall.transform.eulerAngles =
        new Vector3(spawnedWall.transform.eulerAngles.x,
                    spawnedWall.transform.eulerAngles.y,
                    zRot);

      spawnPos = anchor;
      spawnPos.x += m_RoomBaseSize.x * i;
      spawnPos.z += m_RoomBaseSize.z * height;
      spawnPos.z -= m_RoomBaseSize.z * .5f;
      spawnedWall = Instantiate(Wall);
      spawnedWall.transform.position = spawnPos;
      spawnedWall.transform.eulerAngles =
        new Vector3(spawnedWall.transform.eulerAngles.x,
                    spawnedWall.transform.eulerAngles.y,
                    zRot);
    }

    //Create inner walls


    return true;
  }

  void SortRooms()
  {
    if (p_SortedRooms.Count != 0)
      p_SortedRooms.Clear();
    if (p_SortedIndex.Count != 0)
      p_SortedIndex.Clear();

    Debug.Log("[LVGEN] Sorting Rooms");

    //Find Total Heuristic
    p_TotalHeuristic = 0;
    for (int i = p_RoomIndexOffset; i < (int)Room.RoomIndex; ++i)
    {
      Debug.Log("Adding value: " + NavMesh.GetAreaCost(i) + 
        " at index " + i +
        " to TotalHeuristic.");
      p_TotalHeuristic += NavMesh.GetAreaCost(i);
    }

    Debug.Log("[LVGEN] Completed Calculating Total Heuristic");

    //Place rooms into Sorted
    //Push valid room indexes into sorted index list
    for (int i = p_RoomIndexOffset; i < (int)Room.RoomIndex; ++i)
    {
      Debug.Log("Index " + i +
        " returned NavMeshCost: " + NavMesh.GetAreaCost(i));
      p_SortedIndex.Add(i);
    }
    //Sort by heuristic cost
    p_SortedIndex.Sort(SortByNavMeshValue);


    //Create the sorted rooms list
    for (int i = 0;  i < p_SortedIndex.Count; ++i)
    {
      Debug.Log("Adding index: " + (p_SortedIndex[i] - p_RoomIndexOffset)
        + " to SortedRooms");
      p_SortedRooms.Add(Rooms[p_SortedIndex[i] - p_RoomIndexOffset]);
    }


}

  int IndexDistance(Vector2 index1, Vector2 index2)
  {
    return (int)(Mathf.Abs(index1.x - index2.x) + Mathf.Abs(index1.y - index2.y));
  }

  int SortByNavMeshValue(int index1, int index2)
  {
    return NavMesh.GetAreaCost(index2).CompareTo(
      NavMesh.GetAreaCost(index1));
  }

  public void BuildNextLevel()
  {
        //GenerateRooms(m_StartHeight, m_StartWidth);

        //GetComponent<NavMeshSurface>().BuildNavMesh();
        //stats.Restart();
    }
}
