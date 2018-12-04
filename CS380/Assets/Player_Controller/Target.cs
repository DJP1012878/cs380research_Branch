using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Target : MonoBehaviour {

    public Transform Player;
    public Vector3 goal;
    public Player_Stat_Traker stats;
    public Arrow arrow;
    NavMeshAgent agent;

    float radius = 15.0f;
    public Vector3[] Path;
    Vector3 CurrTarget;
    int index = 0;
    bool PathFound = false;
    bool moved = false;

    public float HazardMinCost = 1;
    public float ItemMinCost = 2;
    public float HazardMaxCost = 2;
    public float ItemMaxCost = 1;

    public void ReWeight()
    {
        NavMesh.SetAreaCost((int)Room.HazardRoomA, HazardMinCost + HazardMaxCost * stats.GetCost(Room.HazardRoomA));
        NavMesh.SetAreaCost((int)Room.HazardRoomB, HazardMinCost + HazardMaxCost * stats.GetCost(Room.HazardRoomB));
        NavMesh.SetAreaCost((int)Room.ItemRoomA, ItemMinCost + ItemMaxCost * stats.GetCost(Room.ItemRoomA));
        NavMesh.SetAreaCost((int)Room.ItemRoomB, ItemMinCost + ItemMaxCost * stats.GetCost(Room.ItemRoomB));
    }

    public void ReCalculatePath()
    {
        index = 0;
        NavMesh.SetAreaCost((int)Room.HazardRoomA, HazardMinCost + HazardMaxCost * stats.GetCost(Room.HazardRoomA));
        NavMesh.SetAreaCost((int)Room.HazardRoomB, HazardMinCost + HazardMaxCost * stats.GetCost(Room.HazardRoomB));
        NavMesh.SetAreaCost((int)Room.ItemRoomA, ItemMinCost + ItemMaxCost*stats.GetCost(Room.ItemRoomA));
        NavMesh.SetAreaCost((int)Room.ItemRoomB, ItemMinCost + ItemMaxCost*stats.GetCost(Room.ItemRoomB));
        agent.Warp(Player.position);
        agent.isStopped = true;
        //agent.SetDestination(goal);
        agent.SetDestination(goal);
        Path = agent.path.corners;
        PathFound = false;
    }

    public void SetPath(Vector3[] NewPath)
    {
        //Target = target;
        index = 0;
        Path = NewPath;
        CurrTarget = Path[index];
    }

    // Use this for initialization
    void Start () {
        agent = GetComponent<NavMeshAgent>();
        agent.Warp(Player.position);
        goal = GameObject.FindGameObjectWithTag("Goal").transform.position;
        NavMeshHit closestHit;

        if (NavMesh.SamplePosition(goal, out closestHit, 500f, NavMesh.AllAreas))
        {
            goal = closestHit.position;
        }
        else
        { Debug.LogError("Could not find position on NavMesh for Goal!"); }

        //agent.isStopped = true;
        agent.SetDestination(goal);
        // gameObject.transform.position = g;
        Path = agent.path.corners;
    }
	
	// Update is called once per frame
	void Update () {

        if ( index > 0  && index < Path.Length &&  Vector3.Distance(Player.position, Path[index - 1]) > radius && Vector3.Distance(Player.position, Path[index]) > radius)
        {
            ReCalculatePath();
        }
          

        bool CurrPathFound = agent.hasPath;
        if (CurrPathFound == true && PathFound == false)
        {
            SetPath(agent.path.corners);
            DrawPath();
            CurrTarget = Path[index];
            agent.Warp(CurrTarget);
            arrow.SetTarget(CurrTarget);
            moved = true;
        }
        if(moved == true)
        {
            moved = false;
        }
        PathFound = CurrPathFound;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && moved == false)
        {
            if(++index < Path.Length)
            {
                CurrTarget = Path[index];
                agent.Warp(CurrTarget);
                arrow.SetTarget(CurrTarget);
                moved = true;

            }
        }
    }

    public void DrawPath()
    {
        if (agent == null || agent.path == null)
            return;

        var line = GetComponent<LineRenderer>();
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;

        var path = agent.path;

        line.positionCount = path.corners.Length;
        Vector3 pos;
        for (int i = 0; i < path.corners.Length; i++)
        {
            pos = path.corners[i];
            pos.y += 0.3f;
            line.SetPosition(i, pos);
        }

    }
}
