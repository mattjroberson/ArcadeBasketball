using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopWatch : MonoBehaviour {

    float time;
    float position;
    bool check;

    public Rigidbody2D player;

    // Use this for initialization
    void Start () {
        check = false;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        check = true;

		if (Input.GetKeyDown("right")) {
            time = Time.time;
            Debug.Log("Stopwatch Started");
        }

        if (Time.time - time > 1) {
            if (player.position.x == position) {
                if (check == true) {
                    Debug.Log(Time.time - time);
                    this.enabled = false;
                }
                else check = true;
            }
        }

        position = player.position.x;
    }
}
