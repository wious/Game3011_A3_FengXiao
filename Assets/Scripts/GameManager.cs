using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject menuPanel;
    public GameObject winPanel;
    public GameObject losePanel;
    public bool isStart = false;
    public AudioSource source;


    public int dLevel = 0;
    private int timer = 0;
    public int pointCounter = 0;

    public TMP_Text timerText;
    public TMP_Text moveCounterText;
    public TMP_Text instructionText;

    private void Awake()
    {
        source = GetComponent<AudioSource>();
        menuPanel.SetActive(true);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    public void PlaySFX()
    {
        source.Play();
    }

    private void Update()
    {
        timerText.text = timer.ToString();
        moveCounterText.text = pointCounter.ToString();
        WinConfition();
        LoseCondition();
    }

    private void WinConfition()
    {
        if (isStart && pointCounter >= 30)
        {
            winPanel.SetActive(true);
        }
    }

    private void LoseCondition()
    {
        if (isStart && pointCounter < 30 && timer <= 0)
        {
            losePanel.SetActive(true);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene("Minigame");
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void EasyLevel()
    {
        isStart = true;
        timer = 65;
        menuPanel.SetActive(false);
        dLevel = 1;
        instructionText.text = "Match 3";
        BoardManager.instance.StartMinigame(dLevel);
        StartCoroutine(countTime(1.0f));
    }
    public void MediumLevel()
    {
        isStart = true;
        timer = 55;
        menuPanel.SetActive(false);
        dLevel = 2;
        instructionText.text = "Match 4";
        BoardManager.instance.StartMinigame(dLevel);
        StartCoroutine(countTime(1.0f));
    }
    public void HardLevel()
    {
        isStart = true;
        timer = 45;
        menuPanel.SetActive(false);
        dLevel = 3;
        instructionText.text = "Match 5 ";
        BoardManager.instance.StartMinigame(dLevel);
        StartCoroutine(countTime(1.0f));
    }

    private IEnumerator countTime(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        timer--;
        StartCoroutine(countTime(1.0f));
    }
}
