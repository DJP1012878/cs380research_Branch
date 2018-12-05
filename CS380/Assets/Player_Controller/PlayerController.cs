using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    [Header("Player movement")]
    public float turn = 1;
    public float move_Accel = 1;
    public float move = 5;
    public float jump = 10;
    bool on_ground = true;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  void FixedUpdate()
  {
      //if A then turn left
      if (Input.GetKey(KeyCode.A))
      {
          gameObject.GetComponent<Transform>().Rotate(Vector3.down * Time.deltaTime * turn);
      }
      //if D then turn right
      else if (Input.GetKey(KeyCode.D))
      {
          gameObject.GetComponent<Transform>().Rotate(Vector3.up * Time.deltaTime * turn);
      }
        //if W then move forward
      float mag = Vector3.Magnitude(gameObject.GetComponent<Rigidbody>().velocity);
      if (Input.GetKey(KeyCode.W) && mag <= move && on_ground == true)
      {
          gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * move_Accel);
      }
      else if (mag > move)
      {
            int i = 0;
      }
      //if S then move back
      if (Input.GetKey(KeyCode.S) && mag <= move && on_ground == true)
      {
          gameObject.GetComponent<Rigidbody>().AddForce(-transform.forward * move_Accel);
        }
      //space to jump
      if (Input.GetKeyDown(KeyCode.Space) && on_ground == true)
      {
          gameObject.GetComponent<Rigidbody>().AddForce(transform.up * jump);
            on_ground = false;
      }
    }

    private void OnCollisionStay(Collision collision)
    {
        //if the player is on the ground then set the on_ground variable to true
        if(collision.collider.tag == "Ground")
        {
            on_ground = true;
        }
    }
}
