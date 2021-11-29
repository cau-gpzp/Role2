using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("JU Voxel/Extras/FPS Counter")]
[RequireComponent(typeof(Text))]
public class FPSCounter : MonoBehaviour
{
    public int FPS;
    private Text fpsText;

    private void Start()
    {
        fpsText = GetComponent<Text>();
    }
    public void Update()
    {
        float current = 0;
        current = Time.frameCount / Time.unscaledDeltaTime;
        FPS = (int)current;
        fpsText.text = FPS.ToString() + " FPS";


    }
}