using UnityEngine;
using System.Collections;

public class MeshCopy : MonoBehaviour {

    public GameObject Go;
    public int X;
    public int Y;
    public int Z;

    public GUIText Gt;

	// Use this for initialization
	void Start () {
        //Go = Instantiate("Cube");
        meshWorld world = new meshWorld(X, Y, Z);
        Go.renderer.enabled = true;
        
        for(int x = 0;x<world.worldMatrix.GetUpperBound(0);x++)
        {
            for (int y = 0; y < world.worldMatrix.GetUpperBound(1); y++)
        {
            for (int z = 0; z < world.worldMatrix.GetUpperBound(2); z++)
        {
            if(world.worldMatrix[x,y,z]!=0)
            {
                world.world[x, y, z] = Instantiate(Go, new Vector3(x, y, z), new Quaternion(0.0F, 0.0F, 0.0F, 0.0F)) as GameObject;
            }
        }
        }
        }
        Go.renderer.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
      
    }

    public class meshWorld
    {

        public int[, ,] worldMatrix;
        public GameObject[, ,] world;


        public meshWorld()
        {
            for(int x = 0;x<11;x++)
            {
                for (int y = 0; y < 11; y++)
                {
                    for (int z = 0; z < 11; z++)
                    {
                        worldMatrix[x, y, z] = 0;
                    }
                }
            }
        }
        

        public meshWorld(int x_, int y_, int z_)
        {
            worldMatrix = new int[x_, y_, z_];
            world = new GameObject[x_, y_, z_];

            for (int x = 0; x < x_; x++)
            {
                for (int y = 0; y < y_; y++)
                {
                    for (int z = 0; z < z_; z++)
                    {
                        float r = perlin(x, z, x_, y_);
                        worldMatrix[x, (int)r, z] = 1;
                       
                    }
                }
            }
           
        }

        private float  perlin(int x,int z ,int xGap, int yGap)
        {
            return Mathf.PerlinNoise((float)x /yGap, (float)z / yGap)*yGap;
        }
    }
}
