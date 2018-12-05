using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Target : MonoBehaviour {

    [Header("World Objects")]
    public Transform Player;
    public Vector3 goal;
    public Player_Stat_Traker stats;
    public Arrow arrow;
    NavMeshAgent agent;

    [Header("Path")]
    public Vector3[] Path;
    float radius = 15.0f;
    Vector3 CurrTarget;
    int index = 0;
    bool PathFound = false;
    bool moved = false;

    [Header("Area Costs")]
    public float HazardMinCost = 1;
    public float ItemMinCost = 2;
    public float HazardMaxCost = 2;
    public float ItemMaxCost = 1;

    public void ReWeight() //recalculate the weights of the areas
    {
        NavMesh.SetAreaCost((int)Room.HazardRoomA, HazardMinCost + HazardMaxCost * stats.GetCost(Room.HazardRoomA));
        NavMesh.SetAreaCost((int)Room.HazardRoomB, HazardMinCost + HazardMaxCost * stats.GetCost(Room.HazardRoomB));
        NavMesh.SetAreaCost((int)Room.ItemRoomA, ItemMinCost + ItemMaxCost * stats.GetCost(Room.ItemRoomA));
        NavMesh.SetAreaCost((int)Room.ItemRoomB, ItemMinCost + ItemMaxCost * stats.GetCost(Room.ItemRoomB));
    }

    public void ReCalculatePath() //recalculate the weights then recalculate the path
    {
        index = 0;
        ReWeight();
        agent.Warp(Player.position);
        agent.isStopped = true;
        agent.SetDestination(goal);
        Path = agent.path.corners;
        PathFound = false;
    }

    public void SetPath(Vector3[] NewPath)//manually set the path
    {
        index = 0;
        Path = NewPath;
        CurrTarget = Path[index];
    }

    // Use this for initialization
    void Start () {
        //warp the beacon to the player
        agent = GetComponent<NavMeshAgent>();
        agent.Warp(Player.position);
        //get the goal
        goal = GameObject.FindGameObjectWithTag("Goal").transform.position;

        NavMeshHit closestHit; // find the closest point on the navmesh to the goal
        if (NavMesh.SamplePosition(goal, out closestHit, 500f, NavMesh.AllAreas))
        {
            goal = closestHit.position;
        }
        else
        { Debug.LogError("Could not find position on NavMesh for Goal!"); }
        
        //set the destingation and recalculate the path
        agent.SetDestination(goal);
        Path = agent.path.corners;
    }
	
	// Update is called once per frame
	void Update () {

        //if the player isn't within rang of the current node or the previous node then recalculate the path
        if ( index > 0  && index < Path.Length &&  Vector3.Distance(Player.position, Path[index - 1]) > radius && Vector3.Distance(Player.position, Path[index]) > radius)
        {
            ReCalculatePath();
        }
          
        //refresh the path check
        bool CurrPathFound = agent.hasPath;
        if (CurrPathFound == true && PathFound == false)//if a path was just found this update
        {
            //reset the path and draw it
            SetPath(agent.path.corners);
            DrawPath();
            CurrTarget = Path[index];//set the current target
            agent.Warp(CurrTarget);//warp to the current target
            arrow.SetTarget(CurrTarget);
        }
        if(moved == true)
        {
            moved = false;
        }
        PathFound = CurrPathFound;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player" && moved == false)//if the player has entered the beacon's trigger
        {
            if(++index < Path.Length)//increment to the next beacon
            {
                CurrTarget = Path[index];
                agent.Warp(CurrTarget);
                arrow.SetTarget(CurrTarget);
                moved = true;

            }
        }
    }

    public void DrawPath()//draw the path
    {
        var line = GetComponent<LineRenderer>();//get the line
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;

        var path = agent.path;//get the path

        //move through the path and add each node
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
