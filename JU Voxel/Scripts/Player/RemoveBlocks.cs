using UnityEngine;

public class RemoveBlocks : MonoBehaviour
{
    [JUSubHeader("Destroy blocks with Mouse")]
    public string BlockTag = "Block";
    public Transform Camera;
    public float RaycastDistance = 4f;
    public GameObject BlockHighlighter;
    public GameObject BlockDestroyingParticle;
    private void Start()
    {
        BlockHighlighter.transform.SetParent(null);
    }
    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.position, Camera.forward, out hit, RaycastDistance))
        {
            if (hit.transform.tag == BlockTag)
            {
                BlockHighlighter.SetActive(true);
                BlockHighlighter.transform.position = hit.transform.position;
                BlockHighlighter.transform.rotation = hit.transform.rotation;
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    hit.transform.GetComponentInParent<JUVoxelTerrain>().DestroyInstancedBlock(hit.transform.gameObject);
                    GameObject pfx = (GameObject)Instantiate(BlockDestroyingParticle, hit.transform.position, Quaternion.identity);
                    Destroy(pfx, 2f);
                }
            }
            else
            {
                BlockHighlighter.SetActive(false);
            }
        }
        else
        {
            BlockHighlighter.SetActive(false);
        }
    }
}
