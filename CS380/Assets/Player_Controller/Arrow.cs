using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {

    Vector3 Target;

    //set the target that the arrow should be facing
    public void SetTarget(Vector3 NewTarget)
    {
        Target = NewTarget;
    }
    
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Target; //get the posisition of the target
        pos.y = gameObject.GetComponent<Transform>().position.y; //set the y position to the position of the arrow
        gameObject.GetComponent<Transform>().LookAt(pos, Vector3.up); //look at the the given position
    }
}
