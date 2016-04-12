using UnityEngine;
using System.Collections;

public class input : MonoBehaviour {
    
    public float speed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    void Update()
    {
        float h = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        float v = Input.GetAxis("Vertical") * speed * Time.deltaTime;

        gameObject.transform.Translate(h, v, 0);
    }
}
