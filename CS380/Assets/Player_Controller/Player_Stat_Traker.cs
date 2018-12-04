using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Room
{
    Walkable = 0,
    NotWalkable = 1,
    Jump = 2,
    StartRoom = 3,
    GoalRoom = 4,
    ItemRoomA = 5,
    ItemRoomB = 6,
    EmptyRoom = 7,
    HazardRoomA = 8,
    HazardRoomB = 9,
    RoomIndex = 10
};

public class Player_Stat_Traker : MonoBehaviour {

  float Deaths_A = 1;
  float Deaths_B = 1;
  public float Item_A = 0;
  public float Item_B = 0;
  float Hazard_A_Enter = 1;
  float Hazard_B_Enter = 1;
  public float Item_A_Amount = 10;
  public float Item_B_Amount = 10;
  bool respawn = false;
    bool cooldown = false;
  public float Item_A_Left = 0;
  public float Item_B_Left = 0;
  public float AB_DeathRatio = 0.5f;
  public float BA_DeathRatio = 0.5f;
  public float PathRefreshTime;
  float DT;
    public LevelSystem LvlSys;

    public float[] RoomWeights;

    public GameObject spawn;
    public Target path;
    // Use this for initialization
    void Start()
    {
        spawn = GameObject.FindGameObjectWithTag("Spawn");
        //gameObject.transform.position = spawn.transform.position;

        RoomWeights = new float[(int)Room.RoomIndex];

        AB_DeathRatio = ((Deaths_A) / ((Deaths_A + Deaths_B)));
        BA_DeathRatio = ((Deaths_B) / ((Deaths_A + Deaths_B)));
        Item_A_Left = 1.0f - (Item_A_Amount - Item_A) / Item_A_Amount;
        Item_B_Left = 1.0f - (Item_B_Amount - Item_B) / Item_B_Amount;

        RoomWeights[(int)Room.StartRoom] = 0.0f;
        RoomWeights[(int)Room.HazardRoomA] = AB_DeathRatio;
        RoomWeights[(int)Room.HazardRoomB] = BA_DeathRatio;
        RoomWeights[(int)Room.ItemRoomA] = Item_A_Left;
        RoomWeights[(int)Room.ItemRoomB] = Item_B_Left;
        RoomWeights[(int)Room.GoalRoom] = 0.0f;

        DT = PathRefreshTime;
        //NavMeshPath path = null;
        //NavMesh.CalculatePath(spawn.transform.position, g, NavMesh.AllAreas, path);
        //GetComponent<NavMeshAgent>().SetPath(path);
        //Debug.LogError("the number of nodes are: " + path.corners);
    }
	
	// Update is called once per frame
	void Update () {
            if (respawn == true)
            {
                path.ReCalculatePath();
                respawn = false;
            }
            if (cooldown == true)
            {
                cooldown = false;
            }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag == "HazardA" && respawn == false)
        {
            gameObject.GetComponent<Transform>().position = spawn.transform.position;
            ++Deaths_A;
            respawn = true;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

            AB_DeathRatio = ((Deaths_A) / ((Deaths_A  + Deaths_B)));
            BA_DeathRatio = ((Deaths_B) / ((Deaths_A  + Deaths_B)));
            RoomWeights[(int)Room.HazardRoomA] = AB_DeathRatio;
            RoomWeights[(int)Room.HazardRoomB] = BA_DeathRatio;
        }
        if (collision.collider.tag == "HazardB" && respawn == false)
        {
            gameObject.GetComponent<Transform>().position = spawn.transform.position;
            ++Deaths_B;
            respawn = true;
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            BA_DeathRatio = (Deaths_B / (Deaths_A + Deaths_B));
            AB_DeathRatio = (Deaths_A / (Deaths_A + Deaths_B));
            RoomWeights[(int)Room.HazardRoomA] = AB_DeathRatio;
            RoomWeights[(int)Room.HazardRoomB] = BA_DeathRatio;
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ItemA" && cooldown == false)
        {
            cooldown = true;
            ++Item_A;
            Item_A_Left = 1.0f - (Item_A_Amount - Item_A) / Item_A_Amount;
            Object.Destroy(other.gameObject);
            RoomWeights[(int)Room.ItemRoomA] = Item_A_Left;
        }
        if (other.tag == "ItemB" && cooldown == false)
        {
            cooldown = true;
            ++Item_B;
            Item_B_Left = 1.0f - (Item_B_Amount - Item_B) / Item_B_Amount;
            Object.Destroy(other.gameObject);
            RoomWeights[(int)Room.ItemRoomB] = Item_B_Left;
        }
        if (other.tag == "HazardAEnter" && cooldown == false)
        {
            cooldown = true;
            ++Hazard_A_Enter;
            Object.Destroy(other.gameObject);
            RoomWeights[(int)Room.HazardRoomA] = AB_DeathRatio;
        }
        if (other.tag == "HazardBEnter" && cooldown == false)
        {
            cooldown = true;
            ++Hazard_B_Enter;
            Object.Destroy(other.gameObject);
            RoomWeights[(int)Room.HazardRoomB] = BA_DeathRatio;
        }

        if (other.tag == "Goal")
        {
            LvlSys.GenerateRooms(LvlSys.m_StartHeight, LvlSys.m_StartWidth, true);
            path.ReCalculatePath();
        }

        path.ReWeight();
    }

    public float GetCost(Room room)
    {
        return RoomWeights[(int)room];
    }
}
