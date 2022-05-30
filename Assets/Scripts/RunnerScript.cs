using System.Collections;
using System.Collections.Generic;
using Bermuda.Runner;
using Bermuda.Animation;
using UnityEngine;
using PathCreation;
using DG.Tweening;

public class RunnerScript : MonoBehaviour
{
    [Header("Scripts and Transforms")]
    [SerializeField] private Transform model;
    [SerializeField] private Transform localMoverTarget;
    [SerializeField] private PathCreator pathCreator;
    [SerializeField] private SimpleAnimancer animancer;
    [SerializeField] private PlayerSwerve playerSwerve;
    [SerializeField] private PathManager pathManager;

    [Header("Path Settings")]
    [SerializeField] private float distance = 0;
    [SerializeField] private float startDistance = 0;
    [SerializeField] private float clampLocalX = 2f;
    

    [Header("Run Settings")]
    [SerializeField] private float runSpeed = 2;
    [SerializeField] private float localTargetswipeSpeed = 2f;
    [SerializeField] private float swipeLerpSpeed = 2f;
    [SerializeField] private float swipeRotateLerpSpeed = 2f;

    private Vector3 oldPosition;
    private bool canRun = false;
    private bool canSwerve = false;
    private bool canFollow = true;
    private bool moveEnabled = false;
    private string currentAnimName = "Walking";

    void Awake()
    {
        playerSwerve.OnSwerve += PlayerSwipe_OnSwerve;
        distance = startDistance;
        oldPosition = localMoverTarget.localPosition;
    }

    public void Init()
    {
        pathCreator = FindObjectOfType<PathCreator>();
    }

    private void Start()
    {
        PlayAnimation("StartIdle");
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePath();
        FollowLocalMoverTarget();
        oldPosition = model.localPosition;
    }

    public void StartToRun(bool checkRun)
    {
        if (checkRun)
        {
            canRun = true;
            canSwerve = true;
            canFollow = true;
        }
        else
        {
            canRun = false;
            canSwerve = false;
        }
    }

    void UpdatePath()
    {
        if (canRun)
        {
            distance += runSpeed * Time.deltaTime;
            transform.position = pathCreator.path.GetPointAtDistance(distance);
            transform.eulerAngles = pathCreator.path.GetRotationAtDistance(distance).eulerAngles + new Vector3(0f, 0f, 90f);
        }
    }

    private void PlayerSwipe_OnSwerve(Vector2 direction)
    {
        /*if (canClean)
        {
            canFollow = false;
            model.localPosition = model.localPosition + Vector3.right * direction.x * localModelSwipeSpeed * Time.deltaTime;
            model.localPosition = model.localPosition + Vector3.forward * direction.y * localModelSwipeSpeed * Time.deltaTime;

            Vector3 pos = model.position;
            pos.x = Mathf.Clamp(pos.x, -clampModelLocalX, clampModelLocalX);
            pos.z = Mathf.Clamp(pos.z, (transform.position.z + -clampModelLocalZ), (transform.position.z + clampModelLocalZ));
            model.position = pos;
        }*/

        if (canSwerve)
        {
            localMoverTarget.localPosition = localMoverTarget.localPosition + Vector3.right * direction.x * localTargetswipeSpeed * Time.deltaTime;
            ClampLocalPosition();
        }
    }

    void ClampLocalPosition()
    {
        Vector3 pos = localMoverTarget.localPosition;
        pos.x = Mathf.Clamp(pos.x, -clampLocalX, clampLocalX);
        localMoverTarget.localPosition = pos;

    }

    void FollowLocalMoverTarget()
    {
        if (canFollow)
        {
            Vector3 direction = localMoverTarget.localPosition - oldPosition;
            animancer.GetAnimatorTransform().forward = Vector3.Lerp(animancer.GetAnimatorTransform().forward, direction, swipeRotateLerpSpeed * Time.deltaTime);

            //swipe the object
            Vector3 nextPos = new Vector3(localMoverTarget.localPosition.x, model.localPosition.y, model.localPosition.z); ;
            model.localPosition = Vector3.Lerp(model.localPosition, nextPos, swipeLerpSpeed * Time.deltaTime);
        }
    }

    public void PlayAnimation(string animName)
    {
        animancer.PlayAnimation(animName);

        currentAnimName = animName;
    }

    public void PlayAnimation(string animName, float speed)
    {
        animancer.PlayAnimation(animName);
        animancer.SetStateSpeed(speed);
        currentAnimName = animName;
    }

    public void DodgeBack()
    {
        StartCoroutine(DodgeBackProcess());
    }

    IEnumerator DodgeBackProcess()
    {
        canSwerve = false;
        canRun = false;
        animancer.PlayAnimation("Hit");

        yield return new WaitForSeconds(0.933f);

        animancer.PlayAnimation(currentAnimName);
        canRun = true;
        canSwerve = true;
    }

    public void CanSwerve()
    {
        canSwerve = false;
    }

    public void SwitchPathLine()
    {
        pathCreator = pathManager.ReturnCurrenntRoad();
        distance = 0;
    }

    public void StopMovement()
    {
        //canFollow = false;
        canRun = false;
        canSwerve = false;
    }

    public void ResetCharacter()
    {
        PlayAnimation("StartIdle");
        distance = 0;
        localMoverTarget.localPosition = new Vector3 (0, 0, 1f);
        canRun = false;
        canSwerve = false;
    }
}
