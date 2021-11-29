using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[AddComponentMenu("JU Voxel/Extras/Game Controller")]
public class GameController : MonoBehaviour
{
    GameObject MyPlayer;
    public Animator BlackScreen;

    private bool IsPausable = true;
    public bool IsPaused;
    public GameObject PauseMenu;

    void Start()
    {
        MyPlayer = FindObjectOfType<BasicFirstPersonCharacter>().gameObject;
    }
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            IsPaused = !IsPaused;
        }
        if (IsPaused == true)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            PauseMenu.SetActive(true);
            Time.timeScale = 0.001f;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            PauseMenu.SetActive(false);
            Time.timeScale = 1f; ;
        }
        if(IsPausable == false)
        {
            IsPaused = false;
        }
    }
    //Menu Actions
    public void _PauseGame()
    {
        IsPaused = !IsPaused;
    }
    public void _SaveAndBack()
    {
        IsPaused = false;

        BlackScreen.SetBool("darken", true);

        IsPausable = false;

        Invoke("LoadMenu", 0.7f);
    }
    public void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
    public void _RestartLevel()
    {
        BlackScreen.SetBool("darken", true);
        Invoke("restartlevel", 0.7f);
    }
    private void restartlevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
