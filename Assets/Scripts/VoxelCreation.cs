using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class VoxelCreation : MonoBehaviour {

    public GUIText gt = new GUIText();

    public float X;
    public float Y;    
    public float Z;    

    public bool recalc = false;
    public bool explode = false;
  
    private int[, ,] worldArrayUP ;
    private GameObject[, ,] gameObjectArrUP;
    private bool destroied = false;

	// Use this for initialization
	void Start () {
                              
        int worldScaleX=(int)X;
        int worldScaleY = (int)Y;
        int worldScaleZ = (int)Z;

        GameObject[, ,] GameObjectArr = new GameObject[worldScaleX, worldScaleY, worldScaleZ];
        GameObject GameObjectC = gameObjectGen();         
        int[, ,] WorldArray = testMatrix(worldScaleX, worldScaleY, worldScaleZ);
        meshToGOadd(GameObjectC);   
        //GameObject[,,] GameObjectArr = new GameObject[worldScaleX,worldScaleY,worldScaleZ];
       

        //Fill for methods with sidecontroll
        //WorldFillUnParam(testMatrix(worldScale), worldScale);
       
        meshCopy(WorldArray, GameObjectArr, GameObjectC);

        GameObjectC.renderer.enabled = false;
    }
	
	// Update is called once per frame
    void Update () {
        if (explode)
        {
            GameObject[] objects_ = GameObject.FindGameObjectsWithTag("GOgenTag");
            foreach (GameObject go in objects_)
            {
                go.rigidbody.AddTorque(X, Y, Z);
                //go.rigidbody.angularVelocity = new Vector3(X, Y, Z);
                //go.rigidbody.MoveRotation(new Quaternion(X, Y, Z,5.0f));
                //go.rigidbody.rotation = new Quaternion(X, Y, Z, 5.0f);
            }
        }
        else
        {
            GameObject[] objects_ = GameObject.FindGameObjectsWithTag("GOgenTag");
            foreach (GameObject go in objects_)
            {
                go.rigidbody.AddTorque(0,0,0,0);

                go.rigidbody.angularVelocity = new Vector3(0, 0, 0);
                go.rigidbody.MoveRotation(new Quaternion(0,0,0, 0.0f));
                go.rigidbody.rotation = new Quaternion(0,0,0, 0.0f);
                //go.rigidbody.freezeRotation = true;
            }
        }

        if(recalc)
        {
            if (!destroied) { 
                GameObject[] objects = GameObject.FindGameObjectsWithTag("GOgenTag");
                foreach (GameObject go in objects)
                {
                    Destroy(go);
                }
            }

            GameObject gameObjectUP = gameObjectGen();
            gameObjectArrUP = new GameObject[(int)X, (int)Y, (int)Z];
            worldArrayUP = testMatrix((int)X, (int)Y, (int)Z);
            meshToGOadd(gameObjectUP);
        
            meshCopy(worldArrayUP, gameObjectArrUP, gameObjectUP);
            recalc = false;
            destroied = false;
        }
    }



    #region samplePerlInit
        public int[,,] testMatrix(int worldScale)
            {
                System.Random rnd = new System.Random();
                int[,,] world = new int[worldScale,worldScale,worldScale];

                for (int x = 0; x < worldScale; x++)
                {
                    for (int y = 0; y < worldScale; y++)
                    {
                        for (int z = 0; z < worldScale; z++)
                        {
                            world[x, y, z] = 0;                        
                        }
                    }
                }
            
                for (int x = 0; x < worldScale-1; x++)
                {                
                    for (int z = 0; z < worldScale-1; z++)
                    {

                        //quarterSphereInt(x, y, z, world, worldScale);
                        perlNoiseInt(x, z, world, worldScale,rnd);
                    }                
                }

                return world;
            }
        public int[, ,] testMatrix(int worldScaleX,int worldScaleY,int worldScaleZ)
        {
            System.Random rnd = new System.Random();
            int[, ,] world = new int[worldScaleX, worldScaleY, worldScaleZ];

            for (int x = 0; x < worldScaleX; x++)
            {
                for (int y = 0; y < worldScaleY; y++)
                {
                    for (int z = 0; z < worldScaleZ; z++)
                    {
                        world[x, y, z] = 0;
                    }
                }
            }

            for (int x = 0; x < worldScaleX - 1; x++)
            {
                for (int z = 0; z < worldScaleZ - 1; z++)
                {

                    //quarterSphereInt(x, y, z, world, worldScale);
                    perlNoiseInt(x, z, world, worldScaleY, rnd);
                }
            }

            return world;
        }   
        public void perlNoiseInt(int x, int z, int[, ,] world, int worldScale, System.Random rnd)
        {
               
            float yy = Mathf.PerlinNoise((float)x/worldScale, (float)z/worldScale) * worldScale;
            int y = (int)yy;
           
            world[x, y, z] = 1;
           
        }

    #endregion


    #region CreationPassedObjects
        public void WorldFillUnParam(int[, ,] worldMatrix, int worldScale)
            {
                int meshCnt = 0;
                
                for (int x = 0; x < worldScale; x++)
                {
                    for (int y = 0; y < worldScale; y++)
                    {
                        for (int z = 0; z < worldScale; z++)
                        {
                            if (worldMatrix[x, y, z] != 0)
                            {
                                if (SidesCheck(x, y, z, worldMatrix, worldScale).Count != 0)
                                {
                                    meshCreate(ref meshCnt, x, y, z, worldMatrix, worldScale);
                                }
                            }
                        }
                    }
                }
            }
        public void WorldFill(int[, ,] worldMatrix, int worldScale, List<Vector3> verticesListP, List<int> trianglesListP, List<Vector2> UVListP)
        {
            int meshCnt = 0;
      
            for (int x = 0; x < worldScale; x++)
            {
                for (int y = 0; y < worldScale; y++)
                {
                    for (int z = 0; z < worldScale; z++)
                    {
                        if (worldMatrix[x, y, z] != 0)
                        {
                            if (SidesCheck(x, y, z, worldMatrix, worldScale).Count != 0)
                            {
                                Debug.Log("Filling world chunck at cord (x,y,z) = " + "(" + x + "," + y + "," + z + ")");
                                VerticesFill(x, y, z, verticesListP);
                                UVFill(x, y, z, UVListP);
                                trianglesFill(x, y, z, worldMatrix, worldScale, trianglesListP);
                                meshCreate(verticesListP, trianglesListP, UVListP, ref meshCnt, x, y, z);
                            }
                        }
                    }
                }
            }
        }

        #region unparametrizedCreation
            public void VerticesFill(int x, int y, int z, List<Vector3> verticesListP)
            {
                verticesListP.Add(new Vector3(0 + x, 0 + y, 0 + z));
                verticesListP.Add(new Vector3(0 + x, 1 + y, 0 + z));
                verticesListP.Add(new Vector3(1 + x, 1 + y, 0 + z));
                verticesListP.Add(new Vector3(1 + x, 0 + y, 0 + z));

                verticesListP.Add(new Vector3(0 + x, 0 + y, 1 + z));
                verticesListP.Add(new Vector3(0 + x, 1 + y, 1 + z));
                verticesListP.Add(new Vector3(1 + x, 1 + y, 1 + z));
                verticesListP.Add(new Vector3(1 + x, 0 + y, 1 + z));

            }    
            public void UVFill(int x, int y, int z, List<Vector2> UVList)
            {
                UVList.AddRange(new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) });       
            }
            public void trianglesFill(int x, int y, int z, int[, ,] worldMatrix, int worldScale, List<int> trianglesList)
            {
                List<int> sidesListP =  SidesCheck(x, y, z, worldMatrix, worldScale);
                Debug.Log("Overal sides added " + sidesListP.Count);

                foreach (int side in sidesListP)
                {            
                    Debug.Log("Side is ready for adding " + side);
                    switch(side)
                    {
                        case 0:
                            trianglesList.AddRange(new int[]{0,1,2,0,2,3});
                            Debug.Log("Triangles adde for side " + side);
                        break;
                        case 1:
                            trianglesList.AddRange(new int[] { 3,2,6,3,6,7 });
                            Debug.Log("Triangles adde for side " + side);
                        break;
                        case 2:
                            trianglesList.AddRange(new int[] { 7,6,5,7,5,4 });
                            Debug.Log("Triangles adde for side " + side);
                        break;
                        case 3:
                            trianglesList.AddRange(new int[] { 4,5,1,4,1,0 });
                            Debug.Log("Triangles adde for side " + side);
                        break;
                        case 4:
                            trianglesList.AddRange(new int[] {1,5,6,1,6,2 });
                            Debug.Log("Triangles adde for side " + side);
                        break;
                        case 5:
                            trianglesList.AddRange(new int[] { 4,0,3,4,3,7});
                            Debug.Log("Triangles adde for side " + side);
                        break;
                    }
                }
        
            }       
            public void meshCreate(List<Vector3> verticesListP, List<int> trianglesListP,List<Vector2> UVListP, ref int meshCnt,int x,int y,int z)
            {
                meshCnt = meshCnt + 1;

                Mesh Mesh = new Mesh();
                GameObject Go = new GameObject();
                Go.name = "GeneratedObject_" + meshCnt;
                //Go.transform.position = new Vector3(x,y,z);
                //Debug.Log("GameObject position set to (x,y,z) " + "(" + x + "," + y + "," + z + ")");

                MeshFilter Mf = (MeshFilter)Go.AddComponent(typeof(MeshFilter));
                MeshRenderer Mr = Go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
       
                Mf.mesh = Mesh;
                Debug.Log("Mesh count = " + meshCnt);
       
                Mesh.name = "GeneratedMesh_" + meshCnt;
                Debug.Log("Mesh generated with name " + Mesh.name);
        
                Mesh.vertices = verticesListP.ToArray();
                Debug.Log("Vertices added to mesh with count of " +verticesListP.Count);

                Mesh.uv = UVListP.ToArray();
                Debug.Log("UV's added to mesh with count of " + 8);

                Mesh.triangles = trianglesListP.ToArray();
                Debug.Log("Triangles added to mesh with count of " + trianglesListP.Count);

                Mesh.RecalculateNormals();
                Debug.Log("Mesh recalculated");

                verticesListP.Clear();
                UVListP.Clear();
                trianglesListP.Clear();



            }
        #endregion

        #region parametrizedCreation
            public  List<Vector3>  VerticesFill(int x, int y, int z)
            {
                List<Vector3> verticesListP = new List<Vector3>();

                verticesListP.Add(new Vector3(0 + x, 0 + y, 0 + z));
                verticesListP.Add(new Vector3(0 + x, 1 + y, 0 + z));
                verticesListP.Add(new Vector3(1 + x, 1 + y, 0 + z));
                verticesListP.Add(new Vector3(1 + x, 0 + y, 0 + z));

                verticesListP.Add(new Vector3(0 + x, 0 + y, 1 + z));
                verticesListP.Add(new Vector3(0 + x, 1 + y, 1 + z));
                verticesListP.Add(new Vector3(1 + x, 1 + y, 1 + z));
                verticesListP.Add(new Vector3(1 + x, 0 + y, 1 + z));
                return verticesListP;
            }
            public List<Vector2> UVFill(int x, int y, int z)
            {
                List<Vector2> UVList = new List<Vector2>();

                UVList.AddRange(new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) });

                return UVList;
            }
            public List<int> trianglesFill(int x, int y, int z, int[, ,] worldMatrix, int worldScale )
            {
                List<int> trianglesList = new List<int>();
                List<int> sidesListP = SidesCheck(x, y, z, worldMatrix, worldScale);
                Debug.Log("Overal sides added " + sidesListP.Count);

                foreach (int side in sidesListP)
                {
                    Debug.Log("Side is ready for adding " + side);
                    switch (side)
                    {
                        case 0:
                            trianglesList.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
                            Debug.Log("Triangles adde for side " + side);
                            break;
                        case 1:
                            trianglesList.AddRange(new int[] { 3, 2, 6, 3, 6, 7 });
                            Debug.Log("Triangles adde for side " + side);
                            break;
                        case 2:
                            trianglesList.AddRange(new int[] { 7, 6, 5, 7, 5, 4 });
                            Debug.Log("Triangles adde for side " + side);
                            break;
                        case 3:
                            trianglesList.AddRange(new int[] { 4, 5, 1, 4, 1, 0 });
                            Debug.Log("Triangles adde for side " + side);
                            break;
                        case 4:
                            trianglesList.AddRange(new int[] { 1, 5, 6, 1, 6, 2 });
                            Debug.Log("Triangles adde for side " + side);
                            break;
                        case 5:
                            trianglesList.AddRange(new int[] { 4, 0, 3, 4, 3, 7 });
                            Debug.Log("Triangles adde for side " + side);
                            break;
                    }
                }

                return trianglesList;
            }
            public List<int> SidesCheck(int x, int y, int z, int[, ,] worldMatrix, int worldScale)
            {
                List<int> sidesList = new List<int>();
       

                if (x == 0)
                {
                    sidesList.Add(3);
                    if (x == worldScale)
                    {
                        sidesList.Add(1);
                    }
                    else
                    {
                        if (worldMatrix[x + 1, y, z] == 0)
                        {
                            sidesList.Add(1);
                        }
                    }
                }
                else
                {
                    if (x == worldScale)
                    {
                        sidesList.Add(1);
                    }
                    else
                    {
                        if (worldMatrix[x - 1, y, z] == 0)
                        {
                            sidesList.Add(3);
                        }
                        if (worldMatrix[x + 1, y, z] == 0)
                        {
                            sidesList.Add(1);
                        }
                    }
                }



                if (y == 0)
                {
                    sidesList.Add(5);
                    if (y == worldScale)
                    {
                        sidesList.Add(4);
                    }
                    else
                    {
                        if (worldMatrix[x, y + 1, z] == 0)
                        {
                            sidesList.Add(4);
                        }
                    }
                }
                else
                {
                    if (y == worldScale)
                    {
                        sidesList.Add(4);
                    }
                    else
                    {
                        if (worldMatrix[x, y - 1, z] == 0)
                        {
                            sidesList.Add(5);
                        }
                        if (worldMatrix[x, y + 1, z] == 0)
                        {
                            sidesList.Add(4);
                        }
                    }
                }




                if (z == 0)
                {
                    sidesList.Add(0);
                    if (z == worldScale)
                    {
                        sidesList.Add(2);
                    }
                    else
                    {
                        if (worldMatrix[x, y, z + 1] == 0)
                        {
                            sidesList.Add(2);
                        }
                    }
                }
                else
                {
                    if (z == worldScale)
                    {
                        sidesList.Add(2);
                    }
                    else
                    {
                        if (worldMatrix[x, y, z - 1] == 0)
                        {
                            sidesList.Add(0);
                        }
                        if (worldMatrix[x, y, z + 1] == 0)
                        {
                            sidesList.Add(2);
                        }
                    }
                }



                return sidesList;
            }
            public void meshCreate(ref int meshCnt, int x, int y, int z,int[, ,] worldMatrix, int worldScale)
            {
                meshCnt = meshCnt + 1;

                Mesh Mesh = new Mesh();
                GameObject Go = new GameObject();
     
                Go.name = "GeneratedObject_" + meshCnt;
                //Go.transform.position = new Vector3(x,y,z);
                //Debug.Log("GameObject position set to (x,y,z) " + "(" + x + "," + y + "," + z + ")");
                
                MeshFilter Mf = (MeshFilter)Go.AddComponent(typeof(MeshFilter));
                MeshRenderer Mr = Go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

                //Collider cl = (Collider)Go.AddComponent(typeof(Collider));

                Mf.mesh = Mesh;
                Debug.Log("Mesh count = " + meshCnt);

                Mesh.name = "GeneratedMesh_" + meshCnt;
                Debug.Log("Mesh generated with name " + Mesh.name);

                Mesh.vertices = VerticesFill(x, y, z).ToArray();
                Debug.Log("Vertices added to mesh with count of " + VerticesFill(x, y, z).Count);

                Mesh.uv = UVFill(x, y, z).ToArray();
                Debug.Log("UV's added to mesh with count of " + UVFill(x, y, z).Count);

                Mesh.triangles = trianglesFill(x, y, z, worldMatrix, worldScale).ToArray();
                Debug.Log("Triangles added to mesh with count of " + trianglesFill(x, y, z, worldMatrix, worldScale).Count);

                Mesh.RecalculateNormals();
                Debug.Log("Mesh recalculated");       

            } 
        #endregion

    #endregion


    #region meshCopy
        public GameObject gameObjectGen()
        {
            GameObject Go = new GameObject();
            Go.name = "GOgen";
            
            //Go.tag = "GOgenTag";
            return Go;
        }
        public void meshToGOadd(GameObject Go)
        {
          
            Mesh Mesh = new Mesh();

            MeshFilter Mf = (MeshFilter)Go.AddComponent(typeof(MeshFilter));
            MeshRenderer Mr = Go.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            BoxCollider Bo = Go.AddComponent(typeof(BoxCollider)) as BoxCollider;
            Rigidbody Rb = Go.AddComponent(typeof(Rigidbody)) as Rigidbody;

            Bo.size = new Vector3(1, 1, 1);
            Bo.center = new Vector3(0.5F, 0.5F, 0.5F);

            Rb.useGravity = false;

            Mf.mesh = Mesh;

            Mesh.name = "GeneratedMesh";
            Go.name = "GoGen";
            Go.tag = "GOgenTag";

            Mesh.vertices = VerticesFill(0, 0, 0).ToArray();
            Mesh.uv = UVFill(0, 0, 0).ToArray();
            Mesh.triangles = trianglesSampleFill().ToArray();
                    
        }
        public List<int> trianglesSampleFill()
        {
            
            List<int> trianglesList = new List<int>();

            trianglesList.AddRange(new int[] { 0, 1, 2, 0, 2, 3 });
            Debug.Log("Triangles adde for side " +0);
                           
            trianglesList.AddRange(new int[] { 3, 2, 6, 3, 6, 7 });
            Debug.Log("Triangles adde for side " + 1);
                           
            trianglesList.AddRange(new int[] { 7, 6, 5, 7, 5, 4 });
            Debug.Log("Triangles adde for side " + 2);
                          
            trianglesList.AddRange(new int[] { 4, 5, 1, 4, 1, 0 });
            Debug.Log("Triangles adde for side " + 3);
                          
            trianglesList.AddRange(new int[] { 1, 5, 6, 1, 6, 2 });
            Debug.Log("Triangles adde for side " + 4);
                          
            trianglesList.AddRange(new int[] { 4, 0, 3, 4, 3, 7 });
            Debug.Log("Triangles adde for side " + 5);

            return trianglesList;
        }
        public void meshCopy(int[,,] world,GameObject [,,] worldObjects,GameObject Go)  
        {
            int GOcnt = 0;
            for(int x=world.GetLowerBound(0);x<world.GetUpperBound(0);x++)
            {
            for(int y=world.GetLowerBound(1);y<world.GetUpperBound(1);y++)
            {
            for(int z=world.GetLowerBound(2);z<world.GetUpperBound(2);z++)
            {
                if(world[x, y, z] == 1)
                {
                    worldObjects[x, y, z] = (GameObject)Instantiate(Go, new Vector3(x, y, z), new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
                    GOcnt = GOcnt + 1;
                    worldObjects[x, y, z].name = "GOgen_" + GOcnt;
                }
            }
            }
            }

        }

    #endregion

}
