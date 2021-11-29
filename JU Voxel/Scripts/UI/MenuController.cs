using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

[AddComponentMenu("JU Voxel/Extras/Menu Controller")]
public class MenuController : MonoBehaviour
{
    [Header("Menu Parts")]
    public Animator BlackScreen;

    [Header("Post Processing")]
    public PostProcessProfile PostProcessProfile;
    public Toggle Toggle_AmbientOcclusion;
    public Toggle Toggle_Bloom;
    public Toggle Toggle_ColorCorrection;
    public Toggle Toggle_MotionBlur;
    public Toggle Toggle_ChromaticAberration;
    public Toggle Toggle_Vignette;

    [HideInInspector] int scene_index;
    void Start()
    {
        #region Menu Loads
        Toggle_AmbientOcclusion.isOn = PostProcessProfile.GetSetting<AmbientOcclusion>().active;
        Toggle_Bloom.isOn = PostProcessProfile.GetSetting<Bloom>().active;
        Toggle_ChromaticAberration.isOn = PostProcessProfile.GetSetting<ChromaticAberration>().active;
        Toggle_ColorCorrection.isOn = PostProcessProfile.GetSetting<ColorGrading>().active;
        Toggle_MotionBlur.isOn = PostProcessProfile.GetSetting<MotionBlur>().active;
        Toggle_Vignette.isOn = PostProcessProfile.GetSetting<Vignette>().active;
        #endregion

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }


    #region Menu Buttons Functions
    public void _LoadScene(int SceneIndex)
    {
        scene_index = SceneIndex;
        load_scene_with_time(0.7f);
        BlackScreen.SetBool("darken", true);
    }
    public void _ExitGame()
    {
        Application.Quit();
    }
    #endregion

    #region Options Functions

    public void _SetQuality(int qualityy)
    {
        QualitySettings.SetQualityLevel(qualityy);
        QualitySettings.lodBias = PlayerPrefs.GetFloat("renderdistance");
    }
    public void _SetResolution(int resolutionLevel)
    {

        if (resolutionLevel == 0)
        {
            Screen.SetResolution(800, 480, true);
            PlayerPrefs.SetInt("resolution", resolutionLevel);
        }
        if (resolutionLevel == 1)
        {
            Screen.SetResolution(1280, 720, true);
            PlayerPrefs.SetInt("resolution", resolutionLevel);
        }
        if (resolutionLevel == 2)
        {
            Screen.SetResolution(1920, 1080, true);
            PlayerPrefs.SetInt("resolution", resolutionLevel);
        }
    }
    public void _DeleteData()
    {
        PlayerPrefs.DeleteAll();
    }
    public void PAmbientOcc(bool isactive)
    {
        PostProcessProfile.GetSetting<AmbientOcclusion>().active = isactive;
    }
    public void PColorCorr(bool isactive)
    {
        PostProcessProfile.GetSetting<ColorGrading>().active = isactive;
    }
    public void PBloom(bool isactive)
    {
        PostProcessProfile.GetSetting<Bloom>().active = isactive;
    }
    public void PMotionBlur(bool isactive)
    {
        PostProcessProfile.GetSetting<MotionBlur>().active = isactive;
    }
    public void PVignette(bool isactive)
    {
        PostProcessProfile.GetSetting<Vignette>().active = isactive;
    }
    public void PChromaticAbe(bool isactive)
    {
        PostProcessProfile.GetSetting<ChromaticAberration>().active = isactive;
    }

    #endregion

    #region Credits Functions
    public void _OpenURL(string URL)
    {
        Application.OpenURL(URL);
    }
    #endregion


    #region Load Scene Functions
    public void load_scene_with_time(float TimeToInitLoadScene)
    {
        Invoke("load_current_sceneindex", TimeToInitLoadScene);
    }
    private void load_current_sceneindex()
    {
        SceneManager.LoadScene(scene_index);
    }
    #endregion
}
