using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("JU Voxel/JU Voxel Terrain")]
public class JUVoxelTerrain : MonoBehaviour
{
    [JUHeader("Generation Settings")]
    public bool GenerateInfiniteChunks = true;
    private JUVoxelTerrain ChunkToInfiniteGeneration;
    [Range(3,90)]
    public float DistanceToGenerateChunk = 8;
    public bool GenerateOnStart = true;
    public TerrainType Type;
    public LayerMask PlayerLayerMask;
    private GameObject mPlayer;
    [JUSubHeader("Block Prefabs")]
    public GameObject FistBlock;
    [HideInInspector] private List<BlockMatrix4x4Data> GPUFirstBlocksData = new List<BlockMatrix4x4Data>(2);
    public GameObject MidBlock;
    [HideInInspector] private List<BlockMatrix4x4Data> GPUMidBlocksData = new List<BlockMatrix4x4Data>(2);
    public GameObject LastBlock;
    [HideInInspector] private List<BlockMatrix4x4Data> GPULastBlocksData = new List<BlockMatrix4x4Data>(2);


    [HideInInspector]public JUVoxelTerrain ChunkRight,ChunkLeft,ChunkForward,ChunkBackward;
    [HideInInspector]public JUVoxelTerrain ChunkRightBack, ChunkLeftBack, ChunkRightForward, ChunkLeftForward;

    [HideInInspector] bool chunkForward;
    [HideInInspector] bool chunkRightForward;
    [HideInInspector] bool chunkRight;
    [HideInInspector] bool chunkRightBack;
    [HideInInspector] bool chunkBackward;
    [HideInInspector] bool chunkLeftBack;
    [HideInInspector] bool chunkLeft;
    [HideInInspector] bool chunkLeftForward;

    [JUHeader("Terrain Scale")]
    [Range(2,100)]
    public int Length = 2;
    [Range(2, 100)]
    public int Width = 2;
    [Range(0, 10)]
    public int Height = 0;
    public bool UsePerlingNoise = true;
    [Range(1,50)]
    public int FrequenceNoise = 1;
    [Range(0, 20)]
    public int AmplitudeNoise = 0;
    [HideInInspector] private float Y;
    [HideInInspector] private float Ycurrent;
    [HideInInspector] private Vector3 CenterTerrain;



    [JUSubHeader("Vegetation Prefabs")]
    [JUHeader("Vegetation Settings")]
    public GameObject GrassBlock;
    public GameObject TreeBlock;
    [JUSubHeader("Vegetation Density")]
    public _VegetationSettings VegetationSettings;


    [JUHeader("Gizmo Terrain Preview")]
    public bool EnableTerrainPreview = true;
#if UNITY_EDITOR
    public GizmosChunkSettings PreviewSettings;
#endif



    [HideInInspector] public Transform Parent;
    [HideInInspector] Vector3 ChunkPosition;

    private void Awake()
    {
        ChunkToInfiniteGeneration = this;
        GPUFirstBlocksData.Clear();
        GPULastBlocksData.Clear();
        GPUMidBlocksData.Clear();
        CenterTerrain = new Vector3(transform.position.x + Width / 2, transform.position.y, transform.position.z + Length / 2);
    }
    void Start()
    {
        //Find Player
        mPlayer = GameObject.FindGameObjectWithTag("Player");
        if (mPlayer == null && GenerateInfiniteChunks == true)
        {
            GenerateInfiniteChunks = false;
            Debug.LogError("No player was found, infinite world generation has been disabled, try adding the Player to the scene, if there is already a player in the scene, place the '' Player '' tag on your player, or turn off the infinite world generation");
        }

        gameObject.name = "Chunk " + Random.Range(1000,9000);
        //Set Terrain Points
        leftbackpoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        rightbackpoint = new Vector3(transform.position.x + Width, transform.position.y, transform.position.z);
        rightforwardpoint = new Vector3(transform.position.x + Width, transform.position.y, transform.position.z + Length);
        leftforwardpoint = new Vector3(transform.position.x, transform.position.y, transform.position.z + Length);        
        FindSidesChunks();

        //Setposition
        ChunkPosition = transform.position;

        //Parent of blocks
        Parent = this.transform;

        if (GenerateOnStart == true)
        {
           StartCoroutine(GenerateChunk());
        }

        if (GenerateInfiniteChunks == true)
        {
            InvokeRepeating("FindSidesChunks", 0, 1f);
            InvokeRepeating("CreateChunkIfNeed", 0.1f, 0.3f);
        }
    }

    //----------Control the chunk
    Vector3 leftbackpoint;
    Vector3 rightbackpoint;
    Vector3 rightforwardpoint;
    Vector3 leftforwardpoint;
    public void CreateChunkIfNeed()
    {
        
        var distpl = Vector3.Distance(new Vector3(CenterTerrain.x, 0, CenterTerrain.z), new Vector3(mPlayer.transform.position.x, 0, mPlayer.transform.position.z));

        //LimitTogenerateChunks
        if (distpl < Length / 4 + Width / 4)
        {
            //diagonals areas
            var bac_D_leftcheck = Physics.OverlapBox(leftbackpoint, new Vector3(Width / 2, 100 + AmplitudeNoise, Length / 2), transform.rotation, PlayerLayerMask);
            var bac_D_rigtcheck = Physics.OverlapBox(rightbackpoint, new Vector3(Width / 2, 100 + AmplitudeNoise, Length / 2), transform.rotation, PlayerLayerMask);

            var for_D_rigtcheck = Physics.OverlapBox(rightforwardpoint, new Vector3(Width / 2, 100 + AmplitudeNoise, Length / 2), transform.rotation, PlayerLayerMask);
            var for_D_leftcheck = Physics.OverlapBox(leftforwardpoint, new Vector3(Width / 2, 100 + AmplitudeNoise, Length / 2), transform.rotation, PlayerLayerMask);

            //----------Spawn Chunks

            float offsetarea = -Width / 8 + Length / 8;

            //forward
            if (ChunkForward == null && distpl > DistanceToGenerateChunk && mPlayer.transform.position.z > CenterTerrain.z && mPlayer.transform.position.x < CenterTerrain.x + Length + offsetarea && mPlayer.transform.position.x > CenterTerrain.x - Length + offsetarea)
            {
                CreateANewChunk(ChunkToInfiniteGeneration, transform.position + transform.forward * Length);
                //var newchunk = (GameObject)Instantiate(this.gameObject, transform.position + transform.forward * Length, transform.rotation);
                //newchunk.GetComponent<ChunkManager>().DeleteThisChunk();
                //newchunk.GetComponent<ChunkManager>().GenerateChunk();
                print("Created new Chunk on forward");
            }
            //Backward
            if (ChunkBackward == null && distpl > DistanceToGenerateChunk && mPlayer.transform.position.z < CenterTerrain.z && mPlayer.transform.position.x < CenterTerrain.x + Length + offsetarea && mPlayer.transform.position.x > CenterTerrain.x - Length + offsetarea)
            {
                CreateANewChunk(ChunkToInfiniteGeneration, transform.position - transform.forward * Length);

                /*var newchunk = (GameObject)Instantiate(this.gameObject, transform.position - transform.forward * Length, transform.rotation);
                newchunk.GetComponent<ChunkManager>().DeleteThisChunk();
                newchunk.GetComponent<ChunkManager>().GenerateChunk();*/
                print("Created new Chunk on backward");
            }
            //Right
            if (ChunkRight == null && distpl > DistanceToGenerateChunk && mPlayer.transform.position.x > CenterTerrain.x && mPlayer.transform.position.z < CenterTerrain.z + Length + offsetarea && mPlayer.transform.position.z > CenterTerrain.z - Length + offsetarea)
            {
                CreateANewChunk(ChunkToInfiniteGeneration, transform.position + transform.right * Length);
                /*var newchunk = (GameObject)Instantiate(this.gameObject, transform.position + transform.right * Length, transform.rotation);
                newchunk.GetComponent<ChunkManager>().DeleteThisChunk();
                newchunk.GetComponent<ChunkManager>().GenerateChunk(); */
                print("Created new Chunk on right");
            }
            //Left
            if (ChunkLeft == null && distpl > DistanceToGenerateChunk && mPlayer.transform.position.x < CenterTerrain.x && mPlayer.transform.position.z < CenterTerrain.z + Length + offsetarea && mPlayer.transform.position.z > CenterTerrain.z - Length + offsetarea)
            {
                CreateANewChunk(ChunkToInfiniteGeneration, transform.position - transform.right * Length);
                /*var newchunk = (GameObject)Instantiate(this.gameObject, transform.position - transform.right * Length, transform.rotation);
                newchunk.GetComponent<ChunkManager>().DeleteThisChunk();
                newchunk.GetComponent<ChunkManager>().GenerateChunk(); */
                print("Created new Chunk on left");
            }

            //---Diagonals chunks creates

            //forward left
            if (ChunkLeftForward == null && for_D_leftcheck.Length > 0)
            {
                CreateANewChunk(ChunkToInfiniteGeneration, transform.position + transform.forward * Length - transform.right * Width);
                /*var newchunk = (GameObject)Instantiate(this.gameObject, transform.position + transform.forward * Length - transform.right * Width, transform.rotation);
                newchunk.GetComponent<ChunkManager>().DeleteThisChunk();
                newchunk.GetComponent<ChunkManager>().GenerateChunk(); */
                print("Created new Chunk on forward left");
            }
            //forward right
            if (ChunkRightForward == null && for_D_rigtcheck.Length > 0)
            {
                CreateANewChunk(ChunkToInfiniteGeneration, transform.position + transform.forward * Length + transform.right * Width);
                /*var newchunk = (GameObject)Instantiate(this.gameObject, transform.position + transform.forward * Length + transform.right * Width, transform.rotation);
                newchunk.GetComponent<ChunkManager>().DeleteThisChunk();
                newchunk.GetComponent<ChunkManager>().GenerateChunk(); */
                print("Created new Chunk on forward right");
            }
            //backward left
            if (ChunkLeftBack == null && bac_D_leftcheck.Length > 0)
            {
                CreateANewChunk(ChunkToInfiniteGeneration, transform.position - transform.forward * Length - transform.right * Width);
                /*var newchunk = (GameObject)Instantiate(this.gameObject, transform.position - transform.forward * Length - transform.right * Width, transform.rotation);
                newchunk.GetComponent<ChunkManager>().DeleteThisChunk();
                newchunk.GetComponent<ChunkManager>().GenerateChunk(); */
                print("Created new Chunk on backward left");
            }
            //backward right
            if (ChunkRightBack == null && bac_D_rigtcheck.Length > 0)
            {
                CreateANewChunk(ChunkToInfiniteGeneration, transform.position - transform.forward * Length + transform.right * Width);
                /*/var newchunk = (GameObject)Instantiate(this.gameObject, transform.position - transform.forward * Length + transform.right * Width, transform.rotation);
                newchunk.GetComponent<ChunkManager>().DeleteThisChunk();
                newchunk.GetComponent<ChunkManager>().GenerateChunk(); */
                print("Created new Chunk on backward right");
            }
        }
        FindSidesChunks();

        //Destroy chunk
        if (distpl > Length + Width)
        {
            Destroy(this.gameObject);
            print("Destroyed a far Chunk");
        }
    }


    //---------- INSTANCED RENDERING ---------

    private void Update()
    {
        RenderBatches();
    }

    //----------This will delete all components of your cube except colliders
    private void RenderBatches()
    {
        for (int i = 0; i < GPUFirstBlocksData.Count; i++)
        {
            Graphics.DrawMeshInstanced(GPUFirstBlocksData[i].BlockMesh, 0, GPUFirstBlocksData[i].BlockMaterial, GPUFirstBlocksData[i].BlocksMatrices);
        }
        for (int i = 0; i < GPUMidBlocksData.Count; i++)
        {
            Graphics.DrawMeshInstanced(GPUMidBlocksData[i].BlockMesh, 0, GPUMidBlocksData[i].BlockMaterial, GPUMidBlocksData[i].BlocksMatrices);
        }
        for (int i = 0; i < GPULastBlocksData.Count; i++)
        {
            Graphics.DrawMeshInstanced(GPULastBlocksData[i].BlockMesh, 0, GPULastBlocksData[i].BlockMaterial, GPULastBlocksData[i].BlocksMatrices);
        }
    }

    private void ConvertBlockToInstancedRenderer(GameObject Block, int blocktypeIndex)
    {
        //Get Visuals
        MeshFilter meshFilter = Block.GetComponent<MeshFilter>();
        Renderer meshRenderer = Block.GetComponent<Renderer>();

        //Get Mesh and material
        Mesh mesh = meshFilter.sharedMesh;
        Material material = meshRenderer.material;

        //Get Transform properties
        Transform tr = Block.transform;
        Matrix4x4 matrix = Matrix4x4.TRS(tr.position, tr.rotation, tr.localScale);


        //Set GPU Instancing
        if (blocktypeIndex == 0)
        {
            if (GPUFirstBlocksData.Count == 0) { GPUFirstBlocksData.Add(new BlockMatrix4x4Data()); }

            int LastItem = GPUFirstBlocksData.Count - 1;
            if (GPUFirstBlocksData[LastItem].BlocksMatrices.Count < 1021)
            {
                //Set Matrices, Meshes and Materials
                GPUFirstBlocksData[LastItem].BlocksMatrices.Add(matrix);
                GPUFirstBlocksData[LastItem].BlockMesh = mesh;
                GPUFirstBlocksData[LastItem].BlockMaterial = material;
            }
            else
            {
                //if it exceeds the 1023 limit, it will create another separate list
                GPUFirstBlocksData.Add(new BlockMatrix4x4Data());
            }
        }
        if (blocktypeIndex == 1)
        {
            if (GPUMidBlocksData.Count == 0) { GPUMidBlocksData.Add(new BlockMatrix4x4Data()); }

            int LastItem  = GPUMidBlocksData.Count - 1;
            if (GPUMidBlocksData[LastItem].BlocksMatrices.Count < 1021)
            {
                //Set Matrices, Meshes and Materials
                GPUMidBlocksData[LastItem].BlocksMatrices.Add(matrix);
                GPUMidBlocksData[LastItem].BlockMesh = mesh;
                GPUMidBlocksData[LastItem].BlockMaterial = material;
            }
            else
            {
                //if it exceeds the 1023 limit, it will create another separate list
                GPUMidBlocksData.Add(new BlockMatrix4x4Data());
            }
        }
        if (blocktypeIndex == 2)
        {
            if (GPULastBlocksData.Count == 0) { GPULastBlocksData.Add(new BlockMatrix4x4Data()); }

            int LastItem = GPULastBlocksData.Count - 1;
            if (GPULastBlocksData[LastItem].BlocksMatrices.Count < 1021)
            {
                //Set Matrices, Meshes and Materials
                GPULastBlocksData[LastItem].BlocksMatrices.Add(matrix);
                GPULastBlocksData[LastItem].BlockMesh = mesh;
                GPULastBlocksData[LastItem].BlockMaterial = material;
            }
            else
            {
                //if it exceeds the 1023 limit, it will create another separate list
                GPULastBlocksData.Add(new BlockMatrix4x4Data());
            }
        }

        //Destroy Visual Components
        Destroy(meshRenderer);
        Destroy(meshFilter);
    }

    public void DestroyInstancedBlock(GameObject Block)
    {
        for (int i = 0; i < GPUFirstBlocksData.Count; i++)
        {
            for (int ii = 0; ii < GPUFirstBlocksData[i].BlocksMatrices.Count; ii++)
            {
                Matrix4x4 Currentmatrix = GPUFirstBlocksData[i].BlocksMatrices[ii];
                Matrix4x4 SelectedBlockMatrix = Matrix4x4.TRS(Block.transform.position, Block.transform.rotation, Block.transform.localScale);
                if (Currentmatrix == SelectedBlockMatrix)
                {
                    //Remove Block Matrix and GameObject
                    GPUFirstBlocksData[i].BlocksMatrices.RemoveAt(ii);
                    Destroy(Block);
                    Debug.Log("Destroyed Block: " + ii);
                    return;
                }
            }
        }
        for (int i = 0; i < GPUMidBlocksData.Count; i++)
        {
            for (int ii = 0; ii < GPUMidBlocksData[i].BlocksMatrices.Count; ii++)
            {
                Matrix4x4 Currentmatrix = GPUMidBlocksData[i].BlocksMatrices[ii];
                Matrix4x4 SelectedBlockMatrix = Matrix4x4.TRS(Block.transform.position, Block.transform.rotation, Block.transform.localScale);
                if (Currentmatrix == SelectedBlockMatrix)
                {
                    //Remove Block Matrix and GameObject
                    GPUMidBlocksData[i].BlocksMatrices.RemoveAt(ii);
                    Destroy(Block);
                    Debug.Log("Destroyed Block: " + ii);
                    return;
                }
            }
        }
        for (int i = 0; i < GPULastBlocksData.Count; i++)
        {
            for (int ii = 0; ii < GPULastBlocksData[i].BlocksMatrices.Count; ii++)
            {
                Matrix4x4 Currentmatrix = GPULastBlocksData[i].BlocksMatrices[ii];
                Matrix4x4 SelectedBlockMatrix = Matrix4x4.TRS(Block.transform.position, Block.transform.rotation, Block.transform.localScale);
                if (Currentmatrix == SelectedBlockMatrix)
                {
                    //Remove Block Matrix and GameObject
                    GPULastBlocksData[i].BlocksMatrices.RemoveAt(ii);
                    Destroy(Block);
                    Debug.Log("Destroyed Block: " + ii);
                    return;
                }
            }
        }
        Destroy(Block);
    }

    [System.Serializable]
    public class BlockMatrix4x4Data
    {
        public List<Matrix4x4> BlocksMatrices = new List<Matrix4x4>();
        public Material BlockMaterial;
        public Mesh BlockMesh;
    }




    //---------- CHUNK GENERATION ----------


    //----------Generate the chunk
    IEnumerator GenerateChunk()
    {

        //Delete old chunk to create another
        DeleteAllChunkBlocks();

        //Disable Preview of block terrain
        EnableTerrainPreview = false;

        Parent = this.transform;

        CenterTerrain = new Vector3(transform.position.x + Width / 2, transform.position.y, transform.position.z + Length / 2);
        for (int x = 0; x < Width; x++)
        {
            yield return new WaitForEndOfFrame();
            for (int z = 0; z < Length; z++)
            {
                //generate perlin noise
                if (UsePerlingNoise && AmplitudeNoise > 0 && FrequenceNoise > 0)
                {
                    Y = Mathf.PerlinNoise((ChunkPosition.x + x) / FrequenceNoise, (ChunkPosition.z + z) / FrequenceNoise) * AmplitudeNoise;
                }
                else { Y = 0; }

                if (Type == TerrainType.NonVoxel) { Ycurrent = Y; }
                else { Ycurrent = Mathf.Floor(Y); } //Convert float height to integer

                //Instantiate a new block
                GameObject NewBlock = GameObject.Instantiate(FistBlock,
                    transform.position + transform.right * x + transform.up * Ycurrent + transform.forward * z, Quaternion.identity, Parent);
                NewBlock.gameObject.isStatic = true;
                ConvertBlockToInstancedRenderer(NewBlock, 0);


                //Layers Spawner
                if (Height > 0)
                {
                    for (int i = 1; i < Height; i++)
                    {
                        if (i < Height - 1)
                        {
                            GameObject NewLayerBlock = Instantiate(MidBlock,
                        NewBlock.transform.position - NewBlock.transform.up * i, Quaternion.identity, Parent);
                            NewLayerBlock.gameObject.isStatic = true;

                            ConvertBlockToInstancedRenderer(NewLayerBlock, 1);
                        }
                        if (i == Height - 1)
                        {
                            GameObject NewLayerBlock = Instantiate(LastBlock,
                       NewBlock.transform.position - NewBlock.transform.up * i, Quaternion.identity, Parent);
                            NewLayerBlock.gameObject.isStatic = true;
                            ConvertBlockToInstancedRenderer(NewLayerBlock, 2);
                        }
                    }
                }
                //Grass Spawner
                if (VegetationSettings.EnableGrass == true)
                {
                    var Ygrass = Mathf.PerlinNoise((ChunkPosition.x + x) / VegetationSettings.FrequenceGrass, (ChunkPosition.z + z) / VegetationSettings.FrequenceGrass) * VegetationSettings.AmplitudeGrass;
                    if (Ygrass < VegetationSettings.FrequenceGrass)
                    {
                        GameObject NewGrass = Instantiate(GrassBlock, NewBlock.transform.position + transform.up * 0.5f,
                            Quaternion.Euler(0, Random.Range(0, 360), 0), NewBlock.transform);
                        var randomscale = Random.Range(VegetationSettings.MinGrassScale, VegetationSettings.MaxGrassScale);
                        var randomheight = Random.Range(VegetationSettings.MinGrassHeight, VegetationSettings.MaxGrassHeight);
                        NewGrass.transform.localScale = new Vector3(randomscale, randomheight, randomscale);
                        NewGrass.gameObject.isStatic = true;
                    }
                }
                //Tree Spawner
                if (VegetationSettings.EnableTrees == true)
                {
                    var YTree = Mathf.PerlinNoise((ChunkPosition.x + x) / VegetationSettings.FrequenceTree, (ChunkPosition.z + z) / VegetationSettings.FrequenceTree) * VegetationSettings.AmplitudeTree;
                    if (YTree < VegetationSettings.FrequenceTree)
                    {
                        GameObject NewTree = Instantiate(TreeBlock, NewBlock.transform.position + transform.up * 0.4f,
                            Quaternion.Euler(0, Random.Range(0, 360), 0), NewBlock.transform);
                        var randomscale = Random.Range(VegetationSettings.MinTreeScale, VegetationSettings.MaxTreeScale);
                        var randomheight = Random.Range(VegetationSettings.MinTreeHeight, VegetationSettings.MaxTreeHeight);
                        NewTree.transform.localScale = new Vector3(randomscale, randomheight, randomscale);
                        NewTree.gameObject.isStatic = true;
                    }
                }

            }
        }

        //Delete Chunks in the same position
        JUVoxelTerrain[] chunks = GameObject.FindObjectsOfType<JUVoxelTerrain>();
        if (chunks != null)
        {
            //Set Sides
            for (int i = 0; i < chunks.Length; i++)
            {
                //Delete Chunks on the same position
                for (int ia = 0; ia < chunks.Length; ia++)
                {
                    if (chunks[i].transform.position == chunks[ia].transform.position && chunks[i].gameObject != chunks[ia].gameObject)
                    {
                        Destroy(chunks[i].gameObject);
                        chunks = GameObject.FindObjectsOfType<JUVoxelTerrain>();
                        Debug.LogWarning("Destroyed a chunk in same position");
                    }
                }
            }
        }

        //Find and set sides chunks
        FindSidesChunks();
    }

    //Set Sides Chunks References
    public void FindSidesChunks()
    {
        JUVoxelTerrain[] chunks = GameObject.FindObjectsOfType<JUVoxelTerrain>();
        if (chunks != null)
        {
            //Set Sides
            for (int i = 0; i < chunks.Length; i++)
            {
                if (chunks[i].transform.position.z == transform.position.z + Length && chunks[i].transform.position.x == transform.position.x)
                {
                    //Forward
                    ChunkForward = chunks[i];
                    chunkForward = true;
                }
                if (chunks[i].transform.position.z == transform.position.z + Length && chunks[i].transform.position.x == transform.position.x + Width)
                {
                    //Forward Right
                    ChunkRightForward = chunks[i];
                    chunkRightForward = true;
                }
                if (chunks[i].transform.position.z == transform.position.z && chunks[i].transform.position.x == transform.position.x + Width)
                {
                    //Right
                    ChunkRight = chunks[i];
                    chunkRight = true;
                }
                if (chunks[i].transform.position.z == transform.position.z - Length && chunks[i].transform.position.x == transform.position.x + Width)
                {
                    //Backward Right
                    ChunkRightBack = chunks[i];
                    chunkRightBack = true;
                }
                if (chunks[i].transform.position.z == transform.position.z - Length && chunks[i].transform.position.x == transform.position.x)
                {
                    //Backward
                    ChunkBackward = chunks[i];
                    chunkBackward = true;
                }
                if (chunks[i].transform.position.z == transform.position.z - Length && chunks[i].transform.position.x == transform.position.x - Width)
                {
                    //Backward Left
                    ChunkLeftBack = chunks[i];
                    chunkLeftBack = true;
                }
                if (chunks[i].transform.position.z == transform.position.z && chunks[i].transform.position.x == transform.position.x - Width)
                {
                    //Left
                    ChunkLeft = chunks[i];
                    chunkLeft = true;
                }
                if (chunks[i].transform.position.z == transform.position.z + Length && chunks[i].transform.position.x == transform.position.x - Width)
                {
                    //Forward Left
                    ChunkLeftForward = chunks[i];
                    chunkLeftForward = true;
                }
            }

            //Unset Unnecessary references
            if(chunkForward == false)
            {
                ChunkForward = null;
            }

            if (chunkRightForward == false)
            {
                ChunkRightForward = null;
            }

            if (chunkRight == false)
            {
                ChunkRight = null;
            }

            if (chunkRightBack == false)
            {
                ChunkRightBack = null;
            }

            if (chunkBackward == false)
            {
                ChunkBackward = null;
            }

            if (chunkLeftBack == false)
            {
                ChunkLeftBack = null;
            }

            if (chunkLeft == false)
            {
                ChunkLeft = null;
            }

            if (chunkLeftForward == false)
            {
                ChunkLeftForward = null;
            }
        }
    }

    //----------Delete the chunk
    public void DeleteAllChunkBlocks()
    {
        EnableTerrainPreview = true;
        var childblocks = new GameObject[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            childblocks[i] = transform.GetChild(i).gameObject;
        }
        for (int i = transform.childCount; i > 0; i--)
        {
            DestroyImmediate(childblocks[i -1]);
        }
    }
    public void DeleteThis()
    {
        Destroy(this.gameObject);
    }

    public void CreateANewChunk(JUVoxelTerrain Chunk, Vector3 ChunkPosition)
    {
        //Create another gameobject and atach a new Chunk Manager
        GameObject NewChunk = new GameObject("CreatedChunk");
        NewChunk.transform.position = ChunkPosition;
        NewChunk.AddComponent<JUVoxelTerrain>();

        //Copy all settings
        JUVoxelTerrain c = NewChunk.GetComponent<JUVoxelTerrain>();
        /*c.AmplitudeNoise = Chunk.AmplitudeNoise;
        c.FrequenceNoise = Chunk.FrequenceNoise;
        c.GenerateInfiniteChunks = Chunk.GenerateInfiniteChunks;
        c.GenerateOnStart = true;
        c.Height = Chunk.Height;
        c.Block = Chunk.Block;
        c.EarthBlock = Chunk.EarthBlock;
        c.GrassBlock = Chunk.GrassBlock;
        c.LastBlock = Chunk.LastBlock;
        c.TreeBlock = Chunk.TreeBlock;
        c.DrawInstanced = Chunk.DrawInstanced;
        c.EnableTerrainPreview = Chunk.EnableTerrainPreview;
        c.Length = Chunk.Length;
        c.Width = Chunk.Width;*/

        System.Reflection.FieldInfo[] fields = c.GetType().GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(c.GetComponent(c.GetType()), field.GetValue(Chunk));
        }
    }



    //----------Enums 
    public enum TypeOfDraw { Box, Sphere }
    public enum TerrainType { NonVoxel, Voxel }




    [System.Serializable]
    public class _VegetationSettings
    {
        [Header("Grass")]
        public bool EnableGrass = true;
        [Range(0, 10)]
        public float FrequenceGrass = 1.5f;
        [Range(0, 10)]
        public float AmplitudeGrass = 4f;
        [Range(0.01f, 1f)]
        public float MinGrassScale = 0.4f;
        [Range(0.01f, 1f)]
        public float MaxGrassScale = 0.8f;
        [Range(0.01f, 1f)]
        public float MinGrassHeight = 0.1f;
        [Range(0.01f, 1f)]
        public float MaxGrassHeight = 0.3f;

        [Header("Tree")]
        public bool EnableTrees = true;
        [Range(0, 5)]
        public float FrequenceTree = 0.3f;
        [Range(0, 5)]
        public float AmplitudeTree = 2;
        [Range(0.01f, 1f)]
        public float MinTreeScale = 0.5f;
        [Range(0.01f, 1f)]
        public float MaxTreeScale = 1f;
        [Range(0.01f, 1f)]
        public float MinTreeHeight = 0.5f;
        [Range(0.01f, 1f)]
        public float MaxTreeHeight = 1f;
    }


    


    //----------Editor
#if UNITY_EDITOR
    [System.Serializable]
    public class GizmosChunkSettings
    {
        [Header("Draw Gizmos Settings")]
        public Color PreviewColor = Color.white;
        public TypeOfDraw DrawTerrain;
        public bool ShowGenerateAreas;
        public bool ShowTerrain = true;
        public bool ShowGrass = false;
        public bool ShowTrees = true;
    }
    //----------DRAW DEBUG PREVIEW
    private void OnDrawGizmos()
    {
        if (EnableTerrainPreview == true)
        { 
            for (int x = 0; x < Width; x++)
            {
                for (int z = 0; z < Length; z++)
                {
                    Gizmos.color = PreviewSettings.PreviewColor;

                    //Blocks Visualization
                    if (UsePerlingNoise)
                    {
                        Y = Mathf.PerlinNoise((transform.position.x + x) / FrequenceNoise, (transform.position.z + z) / FrequenceNoise) * AmplitudeNoise;
                    }
                    else { Y = 0; }

                    if (Type == TerrainType.NonVoxel) { Ycurrent = Y; }
                    else { Ycurrent = Mathf.Floor(Y); } //Convert float height to integer

                    if (PreviewSettings.DrawTerrain == TypeOfDraw.Box && PreviewSettings.ShowTerrain == true)
                    {
                        Gizmos.DrawCube(transform.position + transform.right * x + transform.up * Ycurrent + transform.forward * z, new Vector3(1, 1, 1));
                    }
                    if (PreviewSettings.DrawTerrain == TypeOfDraw.Sphere && PreviewSettings.ShowTerrain == true)
                    {
                        Gizmos.DrawSphere(transform.position + transform.right * x + transform.up * Ycurrent + transform.forward * z, 1);
                    }


                    //Grass Visualization
                    if (VegetationSettings.EnableGrass == true && PreviewSettings.ShowGrass == true)
                    {
                        var Ygrass = Mathf.PerlinNoise((ChunkPosition.x + x) / VegetationSettings.FrequenceGrass, (ChunkPosition.z + z) / VegetationSettings.FrequenceGrass) * VegetationSettings.AmplitudeGrass;
                        if (Ygrass < VegetationSettings.FrequenceGrass)
                        {
                            Gizmos.DrawMesh(GrassBlock.GetComponent<MeshFilter>().sharedMesh, -1, transform.position + transform.right * x + transform.up * Ycurrent * 1f + transform.forward * z,
                                Quaternion.identity, new Vector3(VegetationSettings.MinGrassScale, VegetationSettings.MinGrassHeight, VegetationSettings.MinGrassScale));
                            
                        }
                    }

                    //Tree Visualization
                    if (VegetationSettings.EnableTrees == true && PreviewSettings.ShowTrees == true)
                    {
                        var YTrees = Mathf.PerlinNoise((ChunkPosition.x + x) / VegetationSettings.FrequenceTree, (ChunkPosition.z + z) / VegetationSettings.FrequenceTree) * VegetationSettings.AmplitudeTree;
                        if (YTrees < VegetationSettings.FrequenceTree)
                        {
                            Gizmos.DrawMesh(TreeBlock.GetComponent<MeshFilter>().sharedMesh, -1, transform.position + transform.right * x + transform.up * Ycurrent * 1f + transform.forward * z, Quaternion.identity, new Vector3(VegetationSettings.MinTreeScale, VegetationSettings.MinTreeHeight, VegetationSettings.MinTreeScale));
                        }
                    }
                }
            }

            //TerrainCenter Visualization
            var center = new Vector3(transform.position.x + Width / 2, transform.position.y - Height / 2, transform.position.z + Length / 2);
            Handles.Label(center, "Center");

            Gizmos.color = Color.blue * 8f;
            Gizmos.DrawRay(center, Vector3.forward);

            Gizmos.color = Color.red * 8f;
            Gizmos.DrawRay(center, Vector3.right);

            Gizmos.color = Color.green * 8f;
            Gizmos.DrawRay(center, Vector3.up);

            //Terrain Center Check Visualization
            if (GenerateInfiniteChunks && PreviewSettings.ShowGenerateAreas)
            {
                //Direction Visualization
                leftbackpoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                rightbackpoint = new Vector3(transform.position.x + Width, transform.position.y, transform.position.z);
                rightforwardpoint = new Vector3(transform.position.x + Width, transform.position.y, transform.position.z + Length);
                leftforwardpoint = new Vector3(transform.position.x, transform.position.y, transform.position.z + Length);

                Gizmos.color = Color.blue;

                //Spawn Distance Visualization
                Gizmos.DrawWireCube(center, new Vector3(DistanceToGenerateChunk, 0,DistanceToGenerateChunk));

                //Diagonais cubes
                Gizmos.DrawWireCube(leftbackpoint, new Vector3( Width / 2, 0, Length / 2));
                Gizmos.DrawWireCube(leftforwardpoint, new Vector3(Width / 2, 0, Length / 2));
                Gizmos.DrawWireCube(rightbackpoint, new Vector3(Width / 2, 0, Length / 2));
                Gizmos.DrawWireCube(rightforwardpoint, new Vector3(Width / 2, 0, Length / 2));

                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(center + transform.right * Width, new Vector3(Width - 1, 0, Length - 1));
                Gizmos.DrawWireCube(center - transform.right * Width, new Vector3(Width - 1, 0, Length - 1));
                Gizmos.DrawWireCube(center + transform.forward * Length, new Vector3(Width - 1, 0, Length - 1));
                Gizmos.DrawWireCube(center - transform.forward * Length, new Vector3(Width -1, 0, Length - 1));

                Gizmos.DrawWireCube(center + transform.right * Width + transform.forward * Length, new Vector3(Width - 1, 0, Length - 1));
                Gizmos.DrawWireCube(center - transform.right * Width + transform.forward * Length, new Vector3(Width - 1, 0, Length - 1));
                Gizmos.DrawWireCube(center - transform.forward * Length + transform.right * Width, new Vector3(Width - 1, 0, Length - 1));
                Gizmos.DrawWireCube(center - transform.forward * Length - transform.right * Width, new Vector3(Width - 1, 0, Length - 1));
            }

            //Terrain Area Outline Visualization
            Gizmos.color = PreviewSettings.PreviewColor;
            var point1 = new Vector3(transform.position.x - 2,transform.position.y,transform.position.z - 2);
            var point2 = new Vector3(transform.position.x + 1 + Width, transform.position.y, transform.position.z + 1 + Length);

            Gizmos.DrawLine(point1, new Vector3(point1.x, point1.y, point1.z + Length + 2));
            Gizmos.DrawLine(point1, new Vector3(point1.x + Width + 2,point1.y,point1.z));

            Gizmos.DrawLine(point2, new Vector3(point2.x, point2.y, point2.z - Length - 2));
            Gizmos.DrawLine(point2, new Vector3(point2.x - Width - 2, point2.y, point2.z));
            //Info Visualization
            Handles.Label(new Vector3(point2.x - Width/4 - 2, point2.y, point2.z), "width:" + Width);
            Handles.Label(new Vector3(point2.x, point2.y, point2.z - Length/4 - 2), "length:" + Length);

            //Height Visualization
            if (Height > 0)
            {
                //Up Lines
                Gizmos.DrawLine(point1, new Vector3(point1.x, point1.y - Height, point1.z));
                Gizmos.DrawLine(point2, new Vector3(point2.x, point2.y - Height, point2.z));

                var point3 = new Vector3(transform.position.x - 2, transform.position.y - Height, transform.position.z - 2);
                var point4 = new Vector3(transform.position.x + 1 + Width, transform.position.y - Height, transform.position.z + 1 + Length);

                //Side Lines
                Gizmos.DrawLine(point3, new Vector3(point3.x, point3.y, point3.z + Length + 2));
                Gizmos.DrawLine(point3, new Vector3(point3.x + Width + 2, point3.y, point3.z));

                Gizmos.DrawLine(point4, new Vector3(point4.x, point4.y, point4.z - Length - 2));
                Gizmos.DrawLine(point4, new Vector3(point4.x - Width - 2, point4.y, point4.z));

                //Info Visualization
                Handles.color = PreviewSettings.PreviewColor;
                Handles.Label(new Vector3(point4.x, point4.y, point4.z), "height:" + Height);
            }
        }
    }

    [CustomEditor(typeof(JUVoxelTerrain))]
    public class ChunkTerrainEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            JUVoxelTerrain chunk = (JUVoxelTerrain)target;
            JUEditor.CustomEditorUtilities.JU_AssetTitle();
            DrawDefaultInspector();
        }
    }
#endif
}

