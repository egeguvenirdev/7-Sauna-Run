using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerManagement : MonoSingleton<PlayerManagement>
{
    [Header("Scripts")]
    [SerializeField] private RunnerScript runnerScript;

    [Header("Saunas' transforms")]
    [SerializeField] private GameObject smallPool;
    [SerializeField] private GameObject[] smallTransforms;
    [Space]
    [SerializeField] private GameObject mediumPool;
    [SerializeField] private GameObject[] mediumTransforms;
    [Space]
    [SerializeField] private GameObject bigPool;
    [SerializeField] private GameObject[] bigTransforms;
    [SerializeField] private GameObject holder;
    [Space]
    [SerializeField] private int startCap = 5;
    [SerializeField] private int smallCap;
    [SerializeField] private int mediumCap;
    [SerializeField] private int bigCap;

    [Header("Saunas Customers")]
    [SerializeField] private List<GameObject> customers = new List<GameObject>();
    [SerializeField] private GameObject mainCharacter;
    private GameObject pickedObject;

    private int activeMaxCap;

    private bool canRun = false;
    Sequence seq;

    void Start()
    {
        DOTween.Init();
        runnerScript.Init();
        activeMaxCap = startCap;
        customers.Add(mainCharacter);
        UIManager.Instance.SetProgress(activeMaxCap, customers.Count);
    }

    void Update()
    {
    }

    public void AddCustomer(GameObject addedCharacter)
    {
        if (customers.Count == activeMaxCap)
        {
            return;
        }

        if (smallPool.activeSelf)
        {
            addedCharacter.transform.SetParent(holder.transform);
            addedCharacter.transform.position = smallTransforms[customers.Count].transform.position;
        }

        if (mediumPool.activeSelf)
        {
            addedCharacter.transform.SetParent(holder.transform);
            addedCharacter.transform.position = mediumTransforms[customers.Count].transform.position;
        }

        if (bigPool.activeSelf)
        {
            addedCharacter.transform.SetParent(holder.transform);
            addedCharacter.transform.position = bigTransforms[customers.Count].transform.position;
        }

        customers.Add(addedCharacter);
        UIManager.Instance.SetProgress((float)activeMaxCap, (float)customers.Count);
    }

    public void AddCap(int addedCap)
    {
        if ((activeMaxCap + addedCap) <= smallCap) //small sauna's calculations
        {
            if ((activeMaxCap + addedCap) <= 1)
            {
                ThrowCustomer();
                activeMaxCap = 1;
                UIManager.Instance.SetProgress((float)activeMaxCap, (float)customers.Count);
                SetCustomersLocations(smallTransforms);
                return;
            }

            if (!smallPool.activeSelf)
            {
                //play particle
                smallPool.SetActive(true);
                mediumPool.SetActive(false);
                bigPool.SetActive(false);
                SetCustomersLocations(smallTransforms);
                Debug.Log("small set cust");
            }
            activeMaxCap += addedCap;
            UIManager.Instance.SetProgress((float)activeMaxCap, (float)customers.Count);
            return;
        }

        if ((activeMaxCap + addedCap) > smallCap && (activeMaxCap + addedCap) <= mediumCap) // medium sauna's calculations
        {

            if (!mediumPool.activeSelf)
            {
                //play particle
                SetCustomersLocations(mediumTransforms);
                smallPool.SetActive(false);
                mediumPool.SetActive(true);
                bigPool.SetActive(false);
            }
            activeMaxCap += addedCap;
            UIManager.Instance.SetProgress((float)activeMaxCap, (float)customers.Count);
            return;
        }

        if ((activeMaxCap + addedCap) > mediumCap) // big sauna's calculations
        {
            if ((activeMaxCap + addedCap) >= bigCap)
            {
                activeMaxCap = bigCap;
                UIManager.Instance.SetProgress((float)activeMaxCap, (float)customers.Count);
                SetCustomersLocations(bigTransforms);
                return;
            }

            if (!bigPool.activeSelf)
            {
                //play particle
                smallPool.SetActive(false);
                mediumPool.SetActive(false);
                bigPool.SetActive(true);
                SetCustomersLocations(bigTransforms);
            }
            activeMaxCap += addedCap;
            UIManager.Instance.SetProgress((float)activeMaxCap, (float)customers.Count);
            return;
        }
    }

    private void SetCustomersLocations(GameObject[] activeList)
    {
        for (int i = 1; i < customers.Count; i++)
        {
            if (i <= activeList.Length)
            {
                customers[i].transform.position = activeList[i].transform.position;
            }

            else
            {
                var temp = customers[i];
                Destroy(temp);
                customers.RemoveAt(i);
            }
        }
    }

    public void ThrowCustomer()
    {
        seq = DOTween.Sequence();
        if (customers.Count == 1)
        {
            StopMovement();
            UIManager.Instance.RestartButtonUI();
            return;
        }

        pickedObject = customers[customers.Count - 1];
        Vector3 jumpPoint = new Vector3(pickedObject.transform.position.x, pickedObject.transform.position.y + 1, pickedObject.transform.position.z);
        seq.Append(pickedObject.transform.DOJump(jumpPoint, 1, 1, 1)
            .OnComplete(() => { pickedObject.transform.parent = null; }));

        Debug.Log(customers.Count);
        customers.RemoveAt(customers.Count - 1);
        Debug.Log(customers.Count);
        UIManager.Instance.SetProgress((float)activeMaxCap, (float)customers.Count);
    }

    public bool CanSit()
    {
        if (customers.Count + 1 <= activeMaxCap)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void StartMovement()
    {
        runnerScript.StartToRun(true);
    }

    public void StopMovement()
    {
        runnerScript.StartToRun(false);
    }

    public void SwitchPath(float targetPoint, int number)
    {
        //INIS CIKIS ICIN AYRI YAZ TARGET POINT KULLASN
        seq = DOTween.Sequence();
        if (number == 0)
        {
            seq.Append(transform.DOMoveY(targetPoint + 2, 1.5f));
            seq.Append(transform.DOMoveY(targetPoint, 0.375f)
                .OnComplete(() => { runnerScript.SetPlayerHeight(targetPoint); }));
        }
        else
        {
            runnerScript.SwitchPathLine(number);
        }
    }
}
