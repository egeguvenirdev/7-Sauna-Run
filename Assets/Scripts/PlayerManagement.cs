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
    [SerializeField] private int smallCap;
    [SerializeField] private int mediumCap;
    [SerializeField] private int bigCap;
    [SerializeField] private int currentCustomerCount = 1;

    [Header("Saunas Customers")]
    [SerializeField] private List<GameObject> customers = new List<GameObject>();
    [SerializeField] private GameObject mainCharacter;
    [SerializeField] private GameObject thrashCharacter;
    private GameObject pickedObject;

    private int activeMaxCap;

    private bool canRun = false;
    Sequence seq;

    void Start()
    {
        DOTween.Init();
        runnerScript.Init();
        activeMaxCap = smallCap;

        customers.Add(mainCharacter);
    }

    void Update()
    {
    }

    public void AddCustomer(GameObject addedCharacter)
    {
        if (currentCustomerCount == activeMaxCap)
        {
            return;
        }

        if (smallPool.activeSelf)
        {
            addedCharacter.transform.SetParent(smallPool.transform);
            addedCharacter.transform.position = smallTransforms[currentCustomerCount].transform.position;
        }

        if (mediumPool.activeSelf)
        {
            addedCharacter.transform.SetParent(mediumPool.transform);
            addedCharacter.transform.position = mediumTransforms[currentCustomerCount].transform.position;
        }

        if (bigPool.activeSelf)
        {
            addedCharacter.transform.SetParent(bigPool.transform);
            addedCharacter.transform.position = bigTransforms[currentCustomerCount].transform.position;
        }

        currentCustomerCount++;
        customers.Add(addedCharacter);
        Debug.Log(currentCustomerCount);
    }

    public void ThrowCustomer()
    {
        seq = DOTween.Sequence();
        if (currentCustomerCount == 1)
        {
            //kill the game
        }

        pickedObject = customers[currentCustomerCount - 1];
        Vector3 jumpPoint = new Vector3(pickedObject.transform.position.x, pickedObject.transform.position.y + 1, pickedObject.transform.position.z);
        seq.Append(pickedObject.transform.DOJump(jumpPoint, 1, 1, 1)
            .OnComplete(() => { pickedObject.transform.parent = null; }));

        currentCustomerCount--;
        customers.RemoveAt(currentCustomerCount);
        Debug.Log(currentCustomerCount);
    }

    public void AddCap(int addedCap)
    {
        if ((activeMaxCap + addedCap) <= smallCap) //extra small sauna's calculations
        {
            if (activeMaxCap > smallCap) //medium to small
            {
                ClearSmallList();
                for (int i = 1; i < activeMaxCap; i++)
                {
                    smallTransforms[i] = mediumTransforms[i];
                }
                //BURAYA ICERIDEN ATMA KODU YAZ
                ClearMediumList();
            }

            if (!smallPool.activeSelf)
            {
                activeMaxCap += addedCap;
                //PLAY PARTICLE FOR SWITCH FORM
            }
            smallPool.SetActive(true);
            mediumPool.SetActive(false);
            bigPool.SetActive(false);

            if ((activeMaxCap + addedCap) <= 1)
            {
                currentCustomerCount = 1; //GERIYE KALANLARI DOK
                activeMaxCap = 1;
                return;
            }
            activeMaxCap += addedCap;
            return;
        }

        if ((activeMaxCap + addedCap) > smallCap && (activeMaxCap + addedCap) <= mediumCap) // small sauna's calculations
        {
            if (activeMaxCap > mediumCap) //big to medium
            {
                ClearMediumList();
                for (int i = 1; i < activeMaxCap; i++)
                {
                    mediumTransforms[i] = bigTransforms[i];
                }
                //BURAYA ICERIDEN ATMA KODU YAZ
            }
            if (activeMaxCap <= smallCap) //small to medium
            {
                ClearMediumList();
                for (int i = 1; i < activeMaxCap; i++)
                {
                    mediumTransforms[i] = smallTransforms[i];
                }
            }

            if (!mediumPool.activeSelf)
            {
                activeMaxCap += addedCap;
                //PLAY PARTICLE FOR SWITCH FORM
            }

            smallPool.SetActive(false);
            mediumPool.SetActive(true);
            bigPool.SetActive(false);

            activeMaxCap += addedCap;
            return;
        }

        if ((activeMaxCap + addedCap) > mediumCap) // big sauna's calculations
        {
            if (activeMaxCap > smallCap) //medium to small
            {
                ClearBigList();
                for (int i = 1; i < activeMaxCap; i++)
                {
                    bigTransforms[i] = mediumTransforms[i];
                }
                ClearMediumList();
            }

            if (bigPool.activeSelf)
            {
                activeMaxCap += addedCap;
                //PLAY PARTICLE FOR SWITCH FORM
            }

            smallPool.SetActive(false);
            mediumPool.SetActive(false);
            bigPool.SetActive(true);

            if ((activeMaxCap + addedCap) > bigCap)
            {
                activeMaxCap = bigCap;
                return;
            }
        }
    }

    private void ClearSmallList()
    {
        for (int i = 1; i < smallTransforms.Length; i++)
        {
            smallTransforms[i] = null;
        }
    }

    private void ClearMediumList()
    {
        for (int i = 1; i < mediumTransforms.Length; i++)
        {
            mediumTransforms[i] = null;
        }
    }

    private void ClearBigList()
    {
        for (int i = 1; i < bigTransforms.Length; i++)
        {
            bigTransforms[i] = null;
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
