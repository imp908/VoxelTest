using UnityEngine;
using System.Collections;

public class PlayerMoovement : MonoBehaviour {

    GameObject player;
    public float speed;

    private bool jumping = false;
    private bool inAir= false;
    private float jumpHeight;

	// Use this for initialization
	void Start () {
       
        player=GameObject.Find("Player");
        jumpHeight = 10;
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetKey(KeyCode.UpArrow)) { player.rigidbody.AddForce(new Vector3(-1 *  speed, 0, 0)); }
        if (Input.GetKey(KeyCode.DownArrow)) { player.rigidbody.AddForce(new Vector3(1 *  speed, 0, 0)); }

        if (Input.GetKey(KeyCode.LeftArrow)) { player.rigidbody.AddForce(new Vector3(0, 0, -1 *  speed)); }
        if (Input.GetKey(KeyCode.RightArrow)) { player.rigidbody.AddForce(new Vector3(0, 0, 1 * speed) ); }

        if (Input.GetKey(KeyCode.Space) ) {   
           
            player.rigidbody.AddForce(Vector3.up*jumpHeight*speed);            
        }
          
	}

   
}
