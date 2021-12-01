using System;
using UnityEngine;

[Serializable]
public class TankManager
{
    // This class is to manage various settings on a tank.
    // It works with the GameManager class to control how the tanks behave
    // and whether or not players have control of their tank in the 
    // different phases of the game.

    public Color m_PlayerColor;                             // 탱크의 색깔
    public Transform m_SpawnPoint;                          // 탱크가 스폰되는 위치와 방향
    [HideInInspector] public int m_PlayerNumber;            // 탱크 매니저가 플레이어를 구분하는 구분자
    [HideInInspector] public string m_ColoredPlayerText;    // 탱크색과 일치하는 텍스트
    [HideInInspector] public GameObject m_Instance;         // 생성된 탱크 인스턴스에 대한 레퍼런스
    [HideInInspector] public int m_Wins;                    // 플레이어가 이긴 횟수


    private TankMovement m_Movement;                        // TankMovement 스크립트에 대한 레퍼런스(컨트롤 가능 여부)
    private TankShooting m_Shooting;                        // TankShooting 스크립트 레퍼런스(컨트롤 가능 여부)
    private GameObject m_CanvasGameObject;                  // 라운드마다 world space UI를 끄는데 사용

    private TankShooting ts;

    AudioListener m_audio;

    public void Setup()
    {
        // 컴포넌트들에 대한 레퍼런스 받아옴
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;
        // m_audio = m_Instance.GetComponent<Camera>().GetComponent<AudioListener>();

        // 스크립트에 이 플레이어의 번호를 세팅
        m_Movement.m_PlayerNumber = m_PlayerNumber;
        m_Shooting.m_PlayerNumber = m_PlayerNumber;

        // 플레이어의 색과 일치하는 문자열을 생성
        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        // 탱크를 구성하는 모든 렌더러들을 얻어 옴
        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        // 모든 렌더러의 색을 플레이어의 색으로 바꿈
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }


    // 조작 불가능 메소드
    public void DisableControl()
    {
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
        // m_audio.enabled = false;
    }


    // 조작 가능 메소드
    public void EnableControl()
    {
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
        m_Shooting.On();
        // m_audio.enabled = true;
    }


    // Used at the start of each round to put the tank into it's default state.
    // 탱크의 스크립트와 UI를 모두 멈췄다 작동했다 할 수 있습니다.
    // 위치도 스폰포인트로 옮기고 인스턴스 자체를 껐다가 켜서 새로 시작할 수 있게 합니다.
    public void Reset()
    {
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}