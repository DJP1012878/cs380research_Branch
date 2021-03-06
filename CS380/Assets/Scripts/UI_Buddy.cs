﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class UI_Buddy : MonoBehaviour {

    [Header("World Objects")]
    public Player_Stat_Traker stats;
    public LevelSystem Level;
    public Target Path;
    public GameObject arrow;
    public Renderer Beacon;
    public LineRenderer String;
    public Camera player;

    [Header("UI")]
    public Text Item_A_Counter;
    public Text Item_B_Counter;
    public Text Item_A_Weight;
    public Text Item_B_Weight;
    public Text Hazard_A_Weight;
    public Text Hazard_B_Weight;

    bool rebuild = false;

  public void Play()
  {
    if(player.enabled == true)
    {
        player.enabled = false;
    }
    else
    {
        player.enabled = true;
    }
  }

  public void RebuildLevel()
  {
     rebuild = true;
  }

  public void HazardRatio(float r)
  {
    if(r == 1) { r = 0.9999999f; }//i have no idea why, but if the ratio contains a whole number, then it has a percent chance of failing the level generation
    stats.RoomWeights[(int)Room.HazardRoomA] = 1 - r;
    stats.RoomWeights[(int)Room.HazardRoomB] = r;
    Path.ReWeight();
  }

  public void ItemARatio(float r)
  {
    if (r == 1) { r = 0.9999999f; }//i have no idea why, but if the ratio contains a whole number, then it has a percent chance of failing the level generation
    stats.RoomWeights[(int)Room.ItemRoomA] = r;
    Path.ReWeight();
  }

  public void ItemBRatio(float r)
  {
    if (r == 1) { r = 0.9999999f; } //i have no idea why, but if the ratio contains a whole number, then it has a percent chance of failing the level generation}
    stats.RoomWeights[(int)Room.ItemRoomB] = r;
    Path.ReWeight();
  }

  public void RePath()
  {
    Path.ReCalculatePath();
  }

  // Use this for initialization
  public void ToggleArrow()
  {
        if(arrow.active == true)
        {
            arrow.SetActive(false);
        }
        else
        {
            arrow.SetActive(true);
        }

  }

  public void ToggleBeacon()
    {
        if (Beacon.enabled == true)
        {
            Beacon.enabled = false;
        }
        else
        {
            Beacon.enabled = true;
        }
    }

  public void ToggleString()
    {
        if (String.enabled == true)
        {
            String.enabled = false;
        }
        else
        {
            String.enabled = true;
        }
    }

	void Start () {
        string tex = "Item A: " + stats.Item_A  + "/" + stats.Item_A_Amount.ToString();
        Item_A_Counter.text = tex;
        tex = "Item B: " + stats.Item_B.ToString() + "/" + stats.Item_B_Amount.ToString();
        Item_B_Counter.text = tex;

        tex = "Item A Weight: " + NavMesh.GetAreaCost((int)Room.ItemRoomA);
        Item_A_Weight.text = tex;
        tex = "Item B Weight: " + NavMesh.GetAreaCost((int)Room.ItemRoomB);
        Item_B_Weight.text = tex;
        tex = "Hazard A Weight: " + NavMesh.GetAreaCost((int)Room.HazardRoomA);
        Hazard_A_Weight.text = tex;
        tex = "Hazard B Weight: " + NavMesh.GetAreaCost((int)Room.HazardRoomB);
        Hazard_B_Weight.text = tex;
    }
	
	// Update is called once per frame
	void Update () {
        string tex = "Item A: " + stats.Item_A + "/" + stats.Item_A_Amount.ToString();
        Item_A_Counter.text = tex;
        tex = "Item B: " + stats.Item_B.ToString() + "/" + stats.Item_B_Amount.ToString();
        Item_B_Counter.text = tex;

        tex = "Item A Weight: " + Mathf.Round(NavMesh.GetAreaCost((int)Room.ItemRoomA) * 100.0f) / 100.0f;
        Item_A_Weight.text = tex;
        tex = "Item B Weight: " + Mathf.Round(NavMesh.GetAreaCost((int)Room.ItemRoomB) * 100.0f) / 100.0f;
        Item_B_Weight.text = tex;
        tex = "Hazard A Weight: " + Mathf.Round(NavMesh.GetAreaCost((int)Room.HazardRoomA) * 100.0f) / 100.0f;
        Hazard_A_Weight.text = tex;
        tex = "Hazard B Weight: " + Mathf.Round(NavMesh.GetAreaCost((int)Room.HazardRoomB) * 100.0f) / 100.0f;
        Hazard_B_Weight.text = tex;

        if(rebuild == true)
        {
            Level.GenerateRooms(Level.m_StartHeight, Level.m_StartWidth);
            Path.ReCalculatePath();
            rebuild = false;
        }
    }
}
