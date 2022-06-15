using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using Ali.Helper;

public class UIManager : MonoSingleton<UIManager>
{
    private Bermuda.Runner.BermudaRunnerCharacter bermudaRunnerCharacter;

    //Main UIs
    [SerializeField] private GameObject tapToPlayUI;
    [SerializeField] private GameObject nextLvMenuUI;
    [SerializeField] private GameObject restartLvUI;
    [Space]

    //pause button ui uthilities
    [SerializeField] private Toggle vibrationToggle;
    [SerializeField] private Toggle soundToggle;

    [Space]
    //status texts UIs
    [SerializeField] private TMP_Text currentLV;
    [SerializeField] private TMP_Text totalMoneyText;

    [Space]
    //status texts
    [SerializeField] private Image progressBarImage;
    [SerializeField] private TMP_Text progressBarText;

    public bool isPaused;

    private void Awake()
    {
        /*if (PlayerPrefs.GetInt("vibrationOnOff") == 0)
        {
            vibrationToggle.GetComponent<Toggle>().isOn = false;
        }

        if (PlayerPrefs.GetInt("soundOnOff") == 0)
        {
            soundToggle.GetComponent<Toggle>().isOn = false;
        }*/
    }

    private void Start()
    {
        isPaused = true;
        DOTween.Init();
        LevelText();
    }

    public void TapToPlayButton()
    {
        tapToPlayUI.SetActive(false);
        PlayerManagement.Instance.StartMovement();
        isPaused = false;
    }

    public void NextLevelButton()
    {
        nextLvMenuUI.SetActive(false);
        isPaused = false;
        HCLevelManager.Instance.LevelUp();
        LevelText();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //SAHNEYI YUKLE BASTAN
    }

    public void RestartLevelButton()
    {
        restartLvUI.SetActive(false);
        isPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void NextLvUI()
    {
        if (!isPaused) //if the game not stopped
        {
            tapToPlayUI.SetActive(false);
            nextLvMenuUI.SetActive(true);
            isPaused = true;
        }
    }

    public void RestartButtonUI()
    {
        if (!isPaused) //if the game not stopped
        {
            restartLvUI.SetActive(true);
            isPaused = true;
        }
    }

    public void PauseButtonUI()
    {
        if (!isPaused) //if the game not stopped
        {
            tapToPlayUI.SetActive(true);
            isPaused = true;
        }
    }

    public void UIVibrationToggle(bool checkOnOff)
    {
        if (checkOnOff)
        {
            vibrationToggle.GetComponent<Toggle>().isOn = true;
            PlayerPrefs.SetInt("vibrationOnOff", 1);
        }
        else
        {
            vibrationToggle.GetComponent<Toggle>().isOn = false;
            PlayerPrefs.SetInt("vibrationOnOff", 0);
        }
    }

    public void UISoundToggle(bool checkOnOff)
    {
        if (checkOnOff)
        {
            soundToggle.GetComponent<Toggle>().isOn = true;
            PlayerPrefs.SetInt("soundOnOff", 1);
        }
        else
        {
            soundToggle.GetComponent<Toggle>().isOn = false;
            PlayerPrefs.SetInt("soundOnOff", 0);
        }
    }

    public void LevelText()
    {
        int levelInt = HCLevelManager.Instance.GetGlobalLevelIndex() + 1;
        currentLV.text = "Level " + levelInt;
    }

    public void SetProgress(float max, float min)
    {
        float progress = max / min;
        progressBarImage.fillAmount = progress;
        progressBarText.text = min + " / " + max;
    }

    public void SetTotalMoney(string money)
    {
        totalMoneyText.text = "$" + money;
    }

    public void UIQuitGame()
    {
        Application.Quit();
    }
}
