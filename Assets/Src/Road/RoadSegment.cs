using System.Collections.Generic;
using UnityEngine;

public class RoadSegment
{
    public Vector3Int pos;
    public List<RoadSegment> neighbors = new();

    public RoadSegment prevSegment;
    public bool visited;
    public bool queuedForVisiting;

    public RoadSegment(Vector3Int pos)
    {
        this.pos = pos;
    }

    public void WipeTraverseInformation()
    {
        visited = false;
        queuedForVisiting = false;
        prevSegment = null;
    }

    public void UnlinkFromSegment(RoadSegment seg)
    {
        neighbors.Remove(seg);
    }
}