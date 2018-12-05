using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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

  [Header("Stat Counters")]
  float Deaths_A = 1;
  float Deaths_B = 1;
  public float Item_A = 0;
  public float Item_B = 0;
  float Hazard_A_Enter = 1;
  float Hazard_B_Enter = 1;
  public float Item_A_Amount = 10;
  public float Item_B_Amount = 10;

  [Header("Stat Ratios")]
  public float Item_A_Left = 0;
  public float Item_B_Left = 0;
  public float AB_DeathRatio = 0.5f;
  public float BA_DeathRatio = 0.5f;

  [Header("World Items")]
  public LevelSystem LvlSys;
  public float[] RoomWeights;// = new float[(int)Room.RoomIndex];
  public GameObject spawn;
  public Target path;

  //respawn and item buffers
  bool respawn = false;
  bool cooldown = false;

  public void SetRoomWeights()
  {
    //calculate the death and item ratios
    AB_DeathRatio = ((Deaths_A) / ((Deaths_A + Deaths_B)));
    BA_DeathRatio = ((Deaths_B) / ((Deaths_A + Deaths_B)));
    Item_A_Left = 1.0f - (Item_A_Amount - Item_A) / Item_A_Amount;
    Item_B_Left = 1.0f - (Item_B_Amount - Item_B) / Item_B_Amount;

    //set the weights
    RoomWeights[(int)Room.HazardRoomA] = AB_DeathRatio;
    RoomWeights[(int)Room.HazardRoomB] = BA_DeathRatio;
    RoomWeights[(int)Room.ItemRoomA] = Item_A_Left;
    RoomWeights[(int)Room.ItemRoomB] = Item_B_Left;
  }

  void Start()
  {
    RoomWeights = new float[(int)Room.RoomIndex];
    //get the spawn
    spawn = GameObject.FindGameObjectWithTag("Spawn");
    //set the room weights
    RoomWeights[(int)Room.StartRoom] = 0.0f;
    RoomWeights[(int)Room.GoalRoom] = 0.0f;
    SetRoomWeights();
  }
	
	// Update is called once per frame
	void Update ()
  {
    //if respawn buffer is set
    if (respawn == true)
    {
        //recalculate the path
        path.ReCalculatePath();
        //since we only need the buffer to last one update then turn it false
        respawn = false;
    }
    if (cooldown == true)//if cooldown buffer is set
    {
        cooldown = false;//turn it false
    }
  }

  private void OnCollisionEnter(Collision collision)
  {
      //if hazard A , then increment hazard A and reset the room Weights
      if(collision.collider.tag == "HazardA" && respawn == false)
      {
        gameObject.GetComponent<Transform>().position = spawn.transform.position;
        ++Deaths_A;
        respawn = true;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

        AB_DeathRatio = ((Deaths_A) / ((Deaths_A + Deaths_B)));
        BA_DeathRatio = ((Deaths_B) / ((Deaths_A + Deaths_B)));
        RoomWeights[(int)Room.HazardRoomA] = AB_DeathRatio;
        RoomWeights[(int)Room.HazardRoomB] = BA_DeathRatio;
      }
      //if hazard B , then increment hazard B and reset the room Weights
      if (collision.collider.tag == "HazardB" && respawn == false)
      {
        gameObject.GetComponent<Transform>().position = spawn.transform.position;
        ++Deaths_B;
        respawn = true;
        gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;

        AB_DeathRatio = ((Deaths_A) / ((Deaths_A + Deaths_B)));
        BA_DeathRatio = ((Deaths_B) / ((Deaths_A + Deaths_B)));
        RoomWeights[(int)Room.HazardRoomA] = AB_DeathRatio;
        RoomWeights[(int)Room.HazardRoomB] = BA_DeathRatio;
      }

  }
  private void OnTriggerEnter(Collider other)
  {
      if (other.tag == "ItemA" && cooldown == false)//if itemA then increment itemA and destroy the object
      {
          cooldown = true;
          ++Item_A;
          Item_A_Left = 1.0f - (Item_A_Amount - Item_A) / Item_A_Amount;
          Destroy(other.gameObject);
          RoomWeights[(int)Room.ItemRoomA] = Item_A_Left;
      }
      if (other.tag == "ItemB" && cooldown == false)//if itemB then increment itemB and destroy the object
      {
          cooldown = true;
          ++Item_B;
          Item_B_Left = 1.0f - (Item_B_Amount - Item_B) / Item_B_Amount;
          Destroy(other.gameObject);
          RoomWeights[(int)Room.ItemRoomB] = Item_B_Left;
      }
      if (other.tag == "HazardAEnter" && cooldown == false)//if hazard A then increment hazard A
      {
          cooldown = true;
          ++Hazard_A_Enter;
          Destroy(other.gameObject);
      }
      if (other.tag == "HazardBEnter" && cooldown == false)//if hazard B then increment hazard B
      {
          cooldown = true;
          ++Hazard_B_Enter;
          Destroy(other.gameObject);
      }

      //if the player entered the goal then regenerate the room and recalculate the path
      if (other.tag == "Goal")
      {
          LvlSys.GenerateRooms(LvlSys.m_StartHeight, LvlSys.m_StartWidth, true);
          path.ReCalculatePath();
      }

      //recalculate the path weight
      path.ReWeight();
  }

  public float GetWeight(Room room)
  {
     //get the area cost
     return NavMesh.GetAreaCost((int)room);
  }

  public float GetCost(Room room)
  {
     //get the ratio cost
     Debug.Log("Requesting cost of room: " + room);
     return RoomWeights[(int)room];
  }
}
