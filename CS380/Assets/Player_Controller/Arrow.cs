using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {
    //public Transform Target;
    //GameObject goal;
    Vector3 Target;

    public void SetTarget(Vector3 NewTarget)
    {
        Target = NewTarget;
    }

    // Use this for initialization
    void Start()
    {
        //goal = GameObject.FindGameObjectWithTag("Goal");
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 pos = Target;
        pos.y = gameObject.GetComponent<Transform>().position.y;
        gameObject.GetComponent<Transform>().LookAt(pos, Vector3.up);
        //else if( Vector3.Distance(transform.position, Path[index+1]) <= radius)
        //{
        //    ++index;
        //    ++index;
        //    if (index >= Path.Length)
        //    {
        //        //GOAL REACHED
        //    }
        //    else
        //    {
        //        CurrTarget = Path[index];
        //    }
        //}
    }
}
