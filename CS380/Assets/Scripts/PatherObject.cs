using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatherObject : MonoBehaviour {
  [Header("Variables")]
  public Vector3 Destination;

  GameObject PlayerObject;

	// Use this for initialization
	void Start () {
    PlayerObject = GameObject.FindGameObjectWithTag("Player");

    GetComponent<NavMeshAgent>().Warp(PlayerObject.transform.position);
  }
	
	// Update is called once per frame
	void Update () {
    GetComponent<NavMeshAgent>().SetDestination(Destination);
  }
}
