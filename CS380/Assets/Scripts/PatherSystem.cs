using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatherSystem : MonoBehaviour {
  [Header("Necessary Objects")]
  public GameObject PatherSafe;
  public GameObject PatherDanger;
  public Vector3 Destination;

  GameObject PlayerObject;

	// Use this for initialization
	void Start () {
    PlayerObject = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown(KeyCode.P))
    {
      Debug.Log("[Pather] Spawning Safe Pather");
      GameObject pather = Instantiate(PatherSafe);
      pather.GetComponent<PatherObject>().Destination = Destination;
    }
	}
}
