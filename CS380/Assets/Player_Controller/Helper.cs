using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helper : MonoBehaviour {

  public Arrow arrow;
  public Player_Stat_Traker stats;
  public List<Transform> points;
    bool Achange = false;
    bool Bchange = false;

	// Use this for initialization
	void Start () {
        //arrow.SetTarget(points[0]);
    }

	// Update is called once per frame
	void Update () {
		if(stats.AB_DeathRatio > stats.BA_DeathRatio && Achange == false)
        {
            Achange = true;
            Bchange = false;
            //arrow.SetTarget(points[1]);
        }
    else if(stats.AB_DeathRatio <= stats.BA_DeathRatio && Bchange == false)
        {
            Achange = false;
            Bchange = true;
            //arrow.SetTarget(points[0]);
        }
	}
}
