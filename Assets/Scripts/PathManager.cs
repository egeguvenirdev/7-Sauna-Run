using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;

public class PathManager : MonoBehaviour
{
    [SerializeField] private GameObject[] pathList;

    // Start is called before the first frame update

    public PathCreator ReturnCurrenntRoad(int number)
    {
        return pathList[number].GetComponent<PathCreator>();
    }
}
