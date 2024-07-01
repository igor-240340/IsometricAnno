using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathAnimator : MonoBehaviour
{
    [SerializeField] private GameObject[] carrierPrefabs;

    private Grid grid;
    private Dictionary<Vector3Int, GameObject> carriers = new();

    private GameObject activeCarrier;

    private void Awake()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();

        carriers[Vector3Int.up] = Instantiate(carrierPrefabs[0]);
        carriers[Vector3Int.right] = Instantiate(carrierPrefabs[1]);
        carriers[Vector3Int.down] = Instantiate(carrierPrefabs[2]);
        carriers[Vector3Int.left] = Instantiate(carrierPrefabs[3]);
    }

    void Start()
    {
    }

    void Update()
    {
    }

    IEnumerator MoveAlongPath(Queue<RoadSegment> path)
    {
        if (path.Count > 1)
        {
            RoadSegment curSeg = path.Dequeue();
            Vector3 curSegCenterPosWorld = grid.GetCellCenterWorld(curSeg.pos);

            RoadSegment nextSeg = path.Peek();
            Vector3 nextSegCenterPosWorld = grid.GetCellCenterWorld(nextSeg.pos);

            Vector3Int dir = nextSeg.pos - curSeg.pos;
            GameObject carrierAnimated = carriers[dir];

            // Interrupt current animation only if the direction of movement is changed.
            // This helps to prevent loosing animation frames.
            if (activeCarrier != carrierAnimated)
            {
                activeCarrier?.SetActive(false);
                carrierAnimated.SetActive(true);
                activeCarrier = carrierAnimated;
            }

            carrierAnimated.transform.position = curSegCenterPosWorld;

            while (true)
            {
                carrierAnimated.transform.position = Vector3.MoveTowards(carrierAnimated.transform.position,
                    nextSegCenterPosWorld,
                    0.5f * Time.deltaTime);
                if (carrierAnimated.transform.position == nextSegCenterPosWorld)
                    break;

                yield return null;
            }

            StartCoroutine(MoveAlongPath(path));
        }
        else
        {
            Debug.Log("Path finished");
            
            foreach (var keyValuePair in carriers)
                Destroy(keyValuePair.Value);
            
            Destroy(gameObject);
        }
    }

    public void StartAnimation(Queue<RoadSegment> path)
    {
        StartCoroutine(MoveAlongPath(path));
    }
}