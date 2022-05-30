using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ali.Helper;

public class GameManager : MonoSingleton<GameManager>
{
    [SerializeField] private Material mat;
    private int totalMoney;

    [SerializeField] private Color32[] colorTypes = { };
    [SerializeField] private Material[] skyboxes = { };

    void Start()
    {
        int levelIndex = HCLevelManager.Instance.GetGlobalLevelIndex() + 1;

        if(levelIndex % 5 == 0)
        {
            SetGroundColor();
        }

        if(HCLevelManager.Instance.GetGlobalLevelIndex() == 0) //if its a new start
        {
            totalMoney = 0;
            PlayerPrefs.SetInt("TotalMoney", totalMoney);
        }

        if(PlayerPrefs.GetInt("TotalMoney") >= 0) //if the total amount and level are higher than  1;
        {
            SetTotalMoney(0);
        }
    }

    public void SetTotalMoney(int collectedAmount)
    {
        totalMoney = PlayerPrefs.GetInt("TotalMoney", 0) + collectedAmount;
        PlayerPrefs.SetInt("TotalMoney", totalMoney);
        string money = FormatFloatToReadableString((float)PlayerPrefs.GetInt("TotalMoney", 0));
        UIManager.Instance.SetTotalMoney(money);

        totalMoney = 0;
    }

    public static string FormatFloatToReadableString(float value)
    {
        float number = value;
        if (number < 1000)
        {
            return ((int)number).ToString();
        }
        string result = number.ToString();

        if (result.Contains(","))
        {
            result = result.Substring(0, 4);
            result = result.Replace(",", string.Empty);
        }
        else
        {
            result = result.Substring(0, 3);
        }

        do
        {
            number /= 1000;
        }
        while (number >= 1000);
        number = Mathf.CeilToInt(number);
        if (value >= 1000000000000000)
        {
            result = result + "Q";
        }
        else if (value >= 1000000000000)
        {
            result = result + "T";
        }
        else if (value >= 1000000000)
        {
            result = result + "B";
        }
        else if (value >= 1000000)
        {
            result = result + "M";
        }
        else if (value >= 1000)
        {
            result = result + "K";
        }

        if (((int)number).ToString().Length > 0 && ((int)number).ToString().Length < 3)
        {
            result = result.Insert(((int)number).ToString().Length, ".");
        }
        return result;
    }

    private void SetGroundColor()
    {
        mat.SetColor("_HColor", colorTypes[Random.Range(0,5)]);
        RenderSettings.skybox = skyboxes[Random.Range(0, 5)];
    }
}
