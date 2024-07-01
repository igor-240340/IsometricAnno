using System;
using System.Linq;
using UnityEngine;

public struct RoadSegmentType : IEquatable<RoadSegmentType>
{
    // Single road segment
    public static readonly RoadSegmentType single = new RoadSegmentType(new Vector3Int[0]);

    public static readonly RoadSegmentType crossroads = new RoadSegmentType(new[]
    {
        Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left
    });

    // Verticals
    public static readonly RoadSegmentType verticalUpDown = new RoadSegmentType(new[]
    {
        Vector3Int.up, Vector3Int.down
    });

    public static readonly RoadSegmentType verticalUp = new RoadSegmentType(new[]
    {
        Vector3Int.up
    });

    public static readonly RoadSegmentType verticalDown = new RoadSegmentType(new[]
    {
        Vector3Int.down
    });

    // Horizontals
    public static readonly RoadSegmentType horizontalLeftRight = new RoadSegmentType(new[]
    {
        Vector3Int.left, Vector3Int.right
    });

    public static readonly RoadSegmentType horizontalLeft = new RoadSegmentType(new[]
    {
        Vector3Int.left
    });

    public static readonly RoadSegmentType horizontalRight = new RoadSegmentType(new[]
    {
        Vector3Int.right
    });

    // Turns
    public static readonly RoadSegmentType turnNorthEast = new RoadSegmentType(new[]
    {
        Vector3Int.down, Vector3Int.right
    });

    public static readonly RoadSegmentType turnNorthWest = new RoadSegmentType(new[]
    {
        Vector3Int.down, Vector3Int.left
    });

    public static readonly RoadSegmentType turnSouthWest = new RoadSegmentType(new[]
    {
        Vector3Int.up, Vector3Int.left
    });

    public static readonly RoadSegmentType turnSouthEast = new RoadSegmentType(new[]
    {
        Vector3Int.up, Vector3Int.right
    });

    // T-junctions
    public static readonly RoadSegmentType tJunctionLeftClosed = new RoadSegmentType(new[]
    {
        Vector3Int.up, Vector3Int.right, Vector3Int.down
    });

    public static readonly RoadSegmentType tJunctionRightClosed = new RoadSegmentType(new[]
    {
        Vector3Int.up, Vector3Int.left, Vector3Int.down
    });

    public static readonly RoadSegmentType tJunctionUpClosed = new RoadSegmentType(new[]
    {
        Vector3Int.left, Vector3Int.right, Vector3Int.down
    });

    public static readonly RoadSegmentType tJunctionDownClosed = new RoadSegmentType(new[]
    {
        Vector3Int.left, Vector3Int.right, Vector3Int.up
    });

    private readonly Vector3Int[] neighborsDirections;

    private RoadSegmentType(Vector3Int[] neighborsDirections)
    {
        this.neighborsDirections = neighborsDirections.OrderBy(dir => dir.GetHashCode()).ToArray();
    }

    public RoadSegmentType(RoadSegment segment)
    {
        neighborsDirections = segment.neighbors.Select(neighbor => neighbor.pos - segment.pos).ToArray()
            .OrderBy(dir => dir.GetHashCode()).ToArray();
    }

    public bool Equals(RoadSegmentType other)
    {
        return this == other;
    }

    public static bool operator ==(RoadSegmentType a, RoadSegmentType b)
    {
        return a.neighborsDirections.SequenceEqual(b.neighborsDirections);
    }

    public static bool operator !=(RoadSegmentType a, RoadSegmentType b)
    {
        return !(a == b);
    }

    public override bool Equals(object other)
    {
        return other is RoadSegmentType other1 && Equals(other1);
    }

    public override int GetHashCode()
    {
        return neighborsDirections.Aggregate(0, (hash, next) => hash ^ next.GetHashCode());
    }
}