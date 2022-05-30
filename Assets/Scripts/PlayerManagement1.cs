using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using Bermuda.Runner;

public class PlayerManagement1 : MonoSingleton<PlayerManagement1>
{
    [Header("Scripts")]
    [SerializeField] private RunnerScript runnerScript;

    [Header("GameObjects")]
    [SerializeField] private GameObject[] mouseTypes = { };
    [SerializeField] private GameObject[] speacialMouses = { };
    [SerializeField] private GameObject character;
    [SerializeField] private GameObject paintObject;
    [SerializeField] private GameObject localMover;
    [SerializeField] private CamFollower camFollower;
    [SerializeField] private GameObject canvas;
    [SerializeField] private GameObject paint;

    private GameObject finishPoint;

    [Header("Text Settings")]
    [SerializeField] private TMP_Text objectText;
    [SerializeField] private Image progressBarImage;
    private int[] years = { 0, 7665, 9855, 11315, 12775, 14600, 19345 };

    private int playerTimeLine = 0;
    private int mouseCounter;

    [Header("Animation Settings")]
    [SerializeField] private float jumpHeight;
    [SerializeField] private float jumpTime;

    [Header("Sound Settings")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip debuffGate;
    [SerializeField] AudioClip buffGate;
    [SerializeField] AudioClip transformSound;
    [SerializeField] AudioClip miniGameSound;

    //private Monitor monitor;
    private bool canRun = true;
    private bool specialMouse = false;
    private bool oneTimeCheck = false;

    Sequence sequence;
    Sequence finishSequence;

    void Start()
    {
        SetTheDate();
        DOTween.Init();
        sequence = DOTween.Sequence();
        runnerScript.Init();
        finishPoint = GameObject.FindGameObjectWithTag("FinishPoint");
        //monitor = FindObjectOfType<Monitor>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && canRun)
        {
            runnerScript.StartToRun(true);
        }
    }

    public void AddHealth(int collectedTime)
    {
        playerTimeLine += collectedTime;
        CollectAnim();
        Shake();

        PlayCollectAudio(collectedTime);

        if (!specialMouse)
        {
            if (years[0] >= playerTimeLine)
            {
                OpenCurrentMouse(0);
            }
            else if (years[6] <= playerTimeLine)
            {
                OpenCurrentMouse(6);
            }
            else
            {
                for (int i = 1; i < years.Length - 1; i++)
                {
                    if (years[i] <= playerTimeLine && playerTimeLine < years[i + 1])
                    {
                        if (mouseCounter == i) break;
                        mouseCounter = i;

                        OpenCurrentMouse(i);
                        LevelUptAnim();
                    }
                }
            }
        }
        SetTheDate();
    }

    private void CollectAnim()
    {
        character.transform.localPosition = new Vector3(0, 0.25f, 0);
        float height = character.transform.position.y;
        sequence.Append(character.transform.DOLocalMoveY(height + jumpHeight, jumpTime).SetEase(Ease.InOutQuad).SetLoops(2, LoopType.Yoyo));
    }

    private void LevelUptAnim()
    {
        sequence.Join(character.transform.DOLocalRotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutQuad));
    }

    private void SetTheDate()
    {
        float progress = ((float)playerTimeLine % 365) / 365;
        progressBarImage.fillAmount = progress;

        int totalYears = 1964 + (playerTimeLine / 365);
        objectText.text = "" + totalYears;
    }

    private void OpenCurrentMouse(int currentMouse)
    {
        for (int i = 0; i < mouseTypes.Length; i++)
        {
            mouseTypes[i].SetActive(false);
        }
        var particle = ObjectPooler.Instance.GetPooledObject("evoParticle");
        particle.SetActive(true);
        particle.transform.SetParent(character.transform);
        particle.transform.localPosition = character.transform.localPosition + new Vector3(0, -0.25f, 0);
        particle.transform.rotation = Quaternion.identity;
        particle.SetActive(true);
        particle.GetComponent<ParticleSystem>().Play();

        mouseTypes[currentMouse].SetActive(true);
    }

    public void OpenSpeacialMouse()
    {
        sequence = DOTween.Sequence();

        for (int i = 0; i < mouseTypes.Length; i++)
        {
            mouseTypes[i].SetActive(false);
        }

        for (int i = 0; i < speacialMouses.Length; i++)
        {
            speacialMouses[i].SetActive(false);
        }

        speacialMouses[Random.Range(0, speacialMouses.Length)].SetActive(true);
        audioSource.PlayOneShot(transformSound);
        Shake();

        character.transform.localPosition = new Vector3(0, 0.25f, 0);
        character.transform.localRotation = Quaternion.Euler(0, 0, 0);

        float height = character.transform.position.y;
        sequence.Join(character.transform.DOLocalMoveY(height + jumpHeight, jumpTime).SetEase(Ease.InOutQuad).SetLoops(2, LoopType.Yoyo));

        sequence.Join(character.transform.DOLocalRotate(new Vector3(0, 360, 0), 1f, RotateMode.FastBeyond360)
            .SetEase(Ease.InOutQuad));
        specialMouse = true;
    }

    public void StopMovement()
    {
        runnerScript.StartToRun(false);
        canRun = false;
        canvas.SetActive(false);
    }

    public void ToFinishPoint()
    {
        finishSequence = DOTween.Sequence();
        Vector3 localTarget = finishPoint.transform.position + new Vector3(0, 0, 1);
        finishSequence.Append(localMover.transform.DOMove(localTarget, 2).SetEase(Ease.Linear));
        finishSequence.Join(transform.transform.DOMove(finishPoint.transform.position, 2).SetEase(Ease.Linear));
        Invoke("CallTheBoss", 1.5f);
    }

    private void CallTheBoss()
    {
        runnerScript.StopMovement();
        audioSource.PlayOneShot(miniGameSound);
        // monitor.SpawnTheMonitor();
        camFollower.FinisPosition();
    }

    private void PlayCollectAudio(int check)
    {
        if (check >= 0)
        {
            audioSource.PlayOneShot(buffGate);
        }
        else
        {
            audioSource.PlayOneShot(debuffGate);
        }
    }

    public void Shake()
    {
        Haptic.Instance.HapticFeedback(MoreMountains.NiceVibrations.HapticTypes.LightImpact);
    }

    public int ReturnMouseIndex()
    {
        if (specialMouse)
        {
            return 10;
        }
        else
        {
            return mouseCounter + 1;
        }
    }

    public void StartClean(bool check)
    {
        if (check && !oneTimeCheck)
        {
            paint.SetActive(true);
            camFollower.StopMovement();
            oneTimeCheck = true;
            //runnerScript.canClean = check;
        }      
    }
}
