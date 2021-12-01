﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public int m_NumRoundsToWin = 5;            // 게임의 전체 판 수
    public float m_StartDelay = 3f;             // RoudStarting과 RoundPlaying 사이의 대기시간 
    public float m_EndDelay = 3f;               // RoundPlaying과 RoundEnding 사이의 대기 시간
    public CameraControl m_CameraControl;       // CameraControl 스크립트의 레퍼런스
    public Text m_MessageText;                  // 승리 메시지 등을 내 보낼 텍스트 레퍼런스   
    public GameObject m_TankPrefab;             // 탱크 프리팹 레퍼런스
    public TankManager[] m_Tanks;               // A collection of managers for enabling and disabling different aspects of the tanks.


    private int m_RoundNumber;                  // 현재 라운드 수
    private WaitForSeconds m_StartWait;         // 라운드가 시작되는 동안 사용되는 딜레이 
    private WaitForSeconds m_EndWait;           // 라운드가 시작되는 동안 사용되는 딜레이 
    private TankManager m_RoundWinner;          // 현재의 판에 누가 이겼는가에 대한 매니저 레퍼런스
    private TankManager m_GameWinner;           // 게임 전체를 누가 이겼는가에 대한 매니저 레퍼런스

    //게임 턴제를 위한 시간 설정
    private float time_start;
    private float time_current;
    private float time_Max = 15.0f;
    private bool isEnded;

    int prevTurn, curTurn;

    // 게임 시작을 위한 초기 세팅 + 게임 시작
    private void Start()
    {
        curTurn = 0;

        m_StartWait = new WaitForSeconds(m_StartDelay); // 시작 딜레이 지정
        m_EndWait = new WaitForSeconds(m_EndDelay); // 엔딩 딜레이 지정

        SpawnAllTanks(); // 탱크 스폰
        SetCameraTargets(); // 카메라 세팅

        // 게임 시작: 코루틴 반복
        StartCoroutine(GameLoop());
    }


    // 모든 탱크를 지정된 위치와 방향에 스폰 및 값 세팅
    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance =
                Instantiate(m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation) as GameObject;
            TankShooting ts = m_Tanks[i].m_Instance.GetComponent<TankShooting>();
            ts.TurnNext += TurnNext;
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }

        m_Tanks[0].m_Instance.GetComponent<TankProperties>().SetRect(new Rect(0.0f, 0.0f, 0.5f, 1.0f));
        m_Tanks[1].m_Instance.GetComponent<TankProperties>().SetRect(new Rect(0.5f, 0.0f, 1.0f, 1.0f));
    }


    private void SetCameraTargets()
    {
        // Create a collection of transforms the same size as the number of tanks.
        Transform[] targets = new Transform[m_Tanks.Length];

        // For each of these transforms...
        for (int i = 0; i < targets.Length; i++)
        {
            // ... set it to the appropriate tank transform.
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        // These are the targets the camera should follow.
        m_CameraControl.m_Targets = targets;
    }


    // This is called from start and will run each phase of the game one after another.
    private IEnumerator GameLoop()
    {
        // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
        yield return StartCoroutine(RoundStarting());

        // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
        yield return StartCoroutine(RoundPlaying());

        // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
        yield return StartCoroutine(RoundEnding());

        // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
        if (m_GameWinner != null)
        {
            // If there is a game winner, restart the level.
            Application.LoadLevel(Application.loadedLevel);
        }
        else
        {
            // If there isn't a winner yet, restart this coroutine so the loop continues.
            // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        // As soon as the round starts reset the tanks and make sure they can't move.
        ResetAllTanks();
        DisableTankControl();

        // Snap the camera's zoom and position to something appropriate for the reset tanks.
        m_CameraControl.SetStartPositionAndSize();

        // Increment the round number and display text showing the players what round it is.
        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_StartWait;
    }

    void TurnNext()
    {
        Reset_Timer();

        prevTurn = curTurn;
        curTurn = (curTurn + 1) % m_Tanks.Length;
        m_Tanks[prevTurn].DisableControl();
        m_Tanks[curTurn].EnableControl();
    }


    private IEnumerator RoundPlaying()
    {
        m_CameraControl.Off();
        // As soon as the round begins playing let the players control the tanks.
        // EnableTankControl ();
        Reset_Timer();

        // Clear the text from the screen.
        m_MessageText.text = string.Empty;
        m_Tanks[curTurn].EnableControl();

        // While there is not one tank left...
        while (!OneTankLeft())
        {
            if (Check_Timer()) TurnNext();

            // ... return on the next frame.
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        // Stop tanks from moving.
        DisableTankControl();

        // Clear the winner from the previous round.
        m_RoundWinner = null;

        // See if there is a winner now the round is over.
        m_RoundWinner = GetRoundWinner();

        // If there is a winner, increment their score.
        if (m_RoundWinner != null)
            m_RoundWinner.m_Wins++;

        // Now the winner's score has been incremented, see if someone has one the game.
        m_GameWinner = GetGameWinner();

        // Get a message based on the scores and whether or not there is a game winner and display it.
        string message = EndMessage();
        m_MessageText.text = message;

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_EndWait;
    }


    // This is used to check if there is one or fewer tanks remaining and thus the round should end.
    private bool OneTankLeft()
    {
        // Start the count of tanks left at zero.
        int numTanksLeft = 0;

        // Go through all the tanks...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // ... and if they are active, increment the counter.
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        // If there are one or fewer tanks remaining return true, otherwise return false.
        return numTanksLeft <= 1;
    }


    // This function is to find out if there is a winner of the round.
    // This function is called with the assumption that 1 or fewer tanks are currently active.
    private TankManager GetRoundWinner()
    {
        // Go through all the tanks...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // ... and if one of them is active, it is the winner so return it.
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        // If none of the tanks are active it is a draw so return null.
        return null;
    }


    // This function is to find out if there is a winner of the game.
    private TankManager GetGameWinner()
    {
        // Go through all the tanks...
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            // ... and if one of them has enough rounds to win the game, return it.
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        // If no tanks have enough rounds to win, return null.
        return null;
    }


    // Returns a string message to display at the end of each round.
    private string EndMessage()
    {
        // By default when a round ends there are no winners so the default end message is a draw.
        string message = "DRAW!";

        // If there is a winner then change the message to reflect that.
        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        // Add some line breaks after the initial message.
        message += "\n\n\n\n";

        // Go through all the tanks and add each of their scores to the message.
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        // If there is a game winner, change the entire message to reflect that.
        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }


    // This function is used to turn all the tanks back on and reset their positions and properties.
    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }

    private void Reset_Timer()
    {
        time_start = Time.time;
        time_current = 0;
        //text_Timer.text = $"{time_current:N2}";
        isEnded = false;
        Debug.Log("Start");
    }

    private bool Check_Timer()
    {
        time_current = Time.time - time_start;
        if (time_current < time_Max)
        {
            m_MessageText.text = $"{time_current:N2}";

            return false;
        }
        else return true;
    }
}