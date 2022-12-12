using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerManagement : MonoSingleton<PlayerManagement>
{
    [Header("Scripts")]
    [SerializeField] private RunnerScript runnerScript;

    [Header("Sounds")]
    [SerializeField] private AudioSource camSound;
    [SerializeField] private AudioClip explosionAudio;

    [Header("Saunas' transforms")]
    [SerializeField] private GameObject character;
    [SerializeField] private GameObject localMover;
    [SerializeField] private Transform mainCam;
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
    [SerializeField] private float sensivitiy = 2;
    [SerializeField] private float clampLocalRotZ = 7.5f;

    [Header("Saunas Customers")]
    [SerializeField] private List<GameObject> customers = new List<GameObject>();
    [SerializeField] private GameObject mainCharacter;
    private GameObject pickedObject;

    private int activeMaxCap;
    public bool canRotate = false;
    public bool canClick = true;
    private Vector3 mousePrevPosition = Vector3.zero;
    private float mouseDeltaPos = 0;

    private bool canRun = false;
    Sequence seq;

    void Start()
    {
        canClick = true;
        DOTween.Init();
        runnerScript.Init();
        activeMaxCap = startCap + PlayerPrefs.GetInt("Cap", 0);
        customers.Add(mainCharacter);
        UIManager.Instance.SetProgress(activeMaxCap, customers.Count);
    }

    void Update()
    {
        RotateZ();
        if (canClick)
        {
            if (Input.GetMouseButton(0))
            {
                StartMovement();
            }

            if (Input.GetMouseButtonUp(0))
            {
                StopMovement();
            }
        }
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
        camSound.PlayOneShot(explosionAudio);
        if ((activeMaxCap + addedCap) <= smallCap) //small sauna's calculations
        {
            if ((activeMaxCap + addedCap) <= 0)
            {
                StopMovement();
                if (canClick)
                {
                    UIManager.Instance.RestartButtonUI();
                }
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
            if (i <= activeMaxCap - 1)
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
            if (canClick )
            {
                UIManager.Instance.RestartButtonUI();
            }
            
            jumpPoint += new Vector3(0, 0, -5);
        }

        pickedObject.transform.parent = null;
        pickedObject.transform.DOJump(jumpPoint, 1, 1, 1);

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
            .SetEase(Ease.Linear).SetLoops(7, LoopType.Restart));
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

    public void SwitchPath(float targetPoint, int number, bool bumpy)
    {
        //INIS CIKIS ICIN AYRI YAZ TARGET POINT KULLASN
        if (number == 0)
        {
            seq = DOTween.Sequence();
            seq.Append(character.transform.DOMoveY(targetPoint + 2, 1.5f));
            seq.Append(character.transform.DOMoveY(targetPoint, 0.375f));
        }
        else
        {
            runnerScript.SwitchPathLine(number);
        }
        if (bumpy)
        {
            canRotate = true;
            PlayAnim(true);
            runParticle.SetActive(true);
            runParticle.GetComponent<ParticleSystem>().Play();
            runnerScript.runSpeed = 10;
        }
        else
        {
            canRotate = false;
            PlayAnim(false);
            runParticle.SetActive(false);
            runParticle.GetComponent<ParticleSystem>().Stop();
            runnerScript.runSpeed = 5;
        }
    }

    public void FinishAction()
    {
        canClick = false;
        StopMovement();
        PlayAnim(true);
        runParticle.SetActive(true);
        runParticle.GetComponent<ParticleSystem>().Play();

        seq.Append(mainCam.DOLocalMove(new Vector3(0, 4.15f, -8.756f), 2));

        float pathRange = (customers.Count - 1) * 7.5f;
        float pathDuration = customers.Count * .25f;
        seq.Join(transform.DOJump(new Vector3(0, 0.125f, transform.position.z + (int)pathRange), 0 , 1, pathDuration)
            .OnComplete(() =>
            {
                DragTheSauna();         
            }));
        seq.Join(localMover.transform.DOLocalMoveX(0, 1));
    }
    private void DragTheSauna()
    {
        Vector3 slidePosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + Random.Range(2, 5));
        seq.Append(transform.DOMove(slidePosition, 2)
                .OnComplete(() =>
                {
                    Invoke("EndLevel", 1f);
                    runParticle.GetComponent<ParticleSystem>().Stop();
                }));
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

    private void RotateZ()
    {
        if (Input.GetMouseButton(0) && canRotate)
        {
            float res = Screen.width / 2;
            Vector2 pos = Input.mousePosition;
            float mousePos = (pos.x - res) / 40f; //7.5 ile dene

            Debug.Log(mousePos);
            float z = Mathf.Clamp(mousePos, -clampLocalRotZ, clampLocalRotZ);
            character.transform.rotation = Quaternion.Lerp(character.transform.rotation, Quaternion.Euler(0, 0, z), Time.deltaTime * sensivitiy);

        }
        if (!canRotate)
        {
            character.transform.rotation = Quaternion.Lerp(character.transform.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * sensivitiy);
        }
    }

    public GameObject ReturnCustomer()
    {
        GameObject lastCustomer = customers[customers.Count - 1];
        customers.RemoveAt(customers.Count - 1);
        return lastCustomer;
    }

    public int ReturnCustomerCount()
    {
        return customers.Count;
    }

    private void EndLevel()
    {
        UIManager.Instance.NextLvUI();
    }
}
