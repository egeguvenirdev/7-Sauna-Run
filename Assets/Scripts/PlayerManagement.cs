using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerManagement : MonoSingleton<PlayerManagement>
{
    [Header("Scripts")]
    [SerializeField] private RunnerScript runnerScript;

    [Header("Saunas' transforms")]
    [SerializeField] private GameObject character;
    [SerializeField] private GameObject localMover;
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
    [SerializeField] private GameObject runParticle;

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
        seq = DOTween.Sequence();
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
            if ((activeMaxCap + addedCap) <= 0)
            {
                StopMovement();
                UIManager.Instance.RestartButtonUI();
                ThrowCustomer();
                return;
            }

            if (!smallPool.activeSelf)
            {
                PlayParticle("SFE");
                FadeEffect();
                smallPool.SetActive(true);
                mediumPool.SetActive(false);
                bigPool.SetActive(false);
            }
            activeMaxCap += addedCap;
            UIManager.Instance.SetProgress((float)activeMaxCap, (float)customers.Count);
            SetCustomersLocations(smallTransforms);
            return;
        }

        if ((activeMaxCap + addedCap) > smallCap && (activeMaxCap + addedCap) <= mediumCap) // medium sauna's calculations
        {

            if (!mediumPool.activeSelf)
            {
                PlayParticle("MFE");
                FadeEffect();
                smallPool.SetActive(false);
                mediumPool.SetActive(true);
                bigPool.SetActive(false);
            }
            activeMaxCap += addedCap;
            UIManager.Instance.SetProgress((float)activeMaxCap, (float)customers.Count);
            SetCustomersLocations(mediumTransforms);
            return;
        }

        if ((activeMaxCap + addedCap) > mediumCap) // big sauna's calculations
        {
            if (!bigPool.activeSelf)
            {
                PlayParticle("BFE");
                FadeEffect();
                smallPool.SetActive(false);
                mediumPool.SetActive(false);
                bigPool.SetActive(true);
            }

            if ((activeMaxCap + addedCap) >= bigCap)
            {
                activeMaxCap = bigCap;
                UIManager.Instance.SetProgress((float)activeMaxCap, (float)customers.Count);
                SetCustomersLocations(bigTransforms);
                return;
            }
            activeMaxCap += addedCap;
            UIManager.Instance.SetProgress((float)activeMaxCap, (float)customers.Count);
            SetCustomersLocations(bigTransforms);
            return;
        }
    }

    private void SetCustomersLocations(GameObject[] activeList)
    {
        int test = customers.Count;
        for (int i = 0; i < test; i++)
        {
            if (i <= activeMaxCap-1)
            {
                customers[i].transform.position = activeList[i].transform.position;
            }
            else
            {
                ThrowCustomer();
            }
        }
    }

    public void ThrowCustomer()
    {
        pickedObject = customers[customers.Count - 1];
        pickedObject.GetComponent<ModelManager>().PlayFlyingAnim();
        Vector3 jumpPoint = new Vector3(pickedObject.transform.position.x, pickedObject.transform.position.y + 1, pickedObject.transform.position.z);
        if (customers.Count == 1)
        {
            StopMovement();
            UIManager.Instance.RestartButtonUI();
            jumpPoint += new Vector3 (0, 0, -5);
        }

        seq.Append(pickedObject.transform.DOJump(jumpPoint, 1, 1, 1)
            .OnComplete(() => { pickedObject.transform.parent = null; }));

        customers.RemoveAt(customers.Count - 1);
        UIManager.Instance.SetProgress((float)activeMaxCap, (float)customers.Count);
    }

    private void PlayParticle(string name)
    {
        var particle = ObjectPooler.Instance.GetPooledObject(name);
        particle.transform.position = transform.position + new Vector3(0, 1.5f, 1f);
        particle.transform.rotation = Quaternion.identity;
        particle.SetActive(true);
        particle.GetComponent<ParticleSystem>().Play();
    }

    private void FadeEffect()
    {
        seq = DOTween.Sequence();
        Vector3 rotateVector = new Vector3(0, 360, 0);
        seq.Append(character.transform.DORotate(rotateVector, 0.1f, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear).SetLoops(5, LoopType.Restart));
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

    public void FinishAction()
    {
        StopMovement();
        runParticle.SetActive(true);
        runParticle.GetComponent<ParticleSystem>().Play();

        float pathRange = customers.Count * 5;
        seq.Append(transform.DOJump(new Vector3(0, 0.125f, 120 + (int)pathRange), 10, 1, 5).SetSpeedBased()
            .OnComplete(() =>
            {
                //start dance
                DragTheSauna();
                PlayAnim(true);
            }));
        seq.Join(localMover.transform.DOLocalMoveX(0, 1));
    }
    private void DragTheSauna()
    {
        Vector3 slidePosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + Random.Range(4, 15));
        seq.Append(transform.DOMove(slidePosition, 1)
                .OnComplete(() => { 
                    Invoke("EndLevel", 1f);
                    runParticle.GetComponent<ParticleSystem>().Stop(); }));
    }

    private void PlayAnim(bool chill)
    {
        if (chill)
        {
            for (int i = 0; i < customers.Count; i++)
            {
                customers[i].GetComponent<ModelManager>().PlayCheeringAnim();
            }
        }
        else
        {
            for (int i = 0; i < customers.Count; i++)
            {
                customers[i].GetComponent<ModelManager>().PlaySittingAnim();
            }
        }
    }

    private void EndLevel()
    {
        UIManager.Instance.NextLvUI();
    }
}
