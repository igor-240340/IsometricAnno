using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Src
{
    public class Road
    {
        public bool changed;
        public List<RoadSegment> segments = new();

        public void CreateNewSegments(Vector3Int[] coords)
        {
            changed = true;

            foreach (var coord in coords)
            {
                if (CoordOccupied(coord))
                    continue;

                RoadSegment newSegment = new RoadSegment(coord);
                segments.Add(newSegment);
                LinkWithAdjacentSegments(newSegment);
            }
        }

        private bool CoordOccupied(Vector3Int coord)
        {
            return GetSegmentByPos(coord) != null;
        }

        private void LinkWithAdjacentSegments(RoadSegment segment)
        {
            Vector3Int[] directions = {Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left};
            foreach (var dir in directions)
            {
                RoadSegment adjacentSegment = GetSegmentByPos(segment.pos + dir);
                if (adjacentSegment != null)
                    LinkRoadSegments(segment, adjacentSegment);
            }
        }

        private RoadSegment GetSegmentByPos(Vector3Int pos)
        {
            return segments.Find(roadSegment => roadSegment.pos == pos);
        }

        private void LinkRoadSegments(RoadSegment a, RoadSegment b)
        {
            a.neighbors.Add(b);
            b.neighbors.Add(a);
        }

        public Queue<RoadSegment> FindPath(RoadSegment start, RoadSegment end)
        {
            TraverseFrom(start);
            Queue<RoadSegment> path = BacktracePath(end, start);
            segments.ForEach(segment => segment.WipeTraverseInformation());
            return path;
        }

        private void TraverseFrom(RoadSegment start)
        {
            var visitingQueue = new Queue<RoadSegment>();
            visitingQueue.Enqueue(start);

            while (visitingQueue.Count > 0)
            {
                RoadSegment currentSegment = visitingQueue.Dequeue();

                currentSegment.neighbors.ForEach(neighbour =>
                {
                    if (neighbour.visited || neighbour.queuedForVisiting)
                        return;

                    neighbour.prevSegment = currentSegment;
                    visitingQueue.Enqueue(neighbour);
                    neighbour.queuedForVisiting = true;
                });

                currentSegment.visited = true;
            }
        }

        private Queue<RoadSegment> BacktracePath(RoadSegment end, RoadSegment start)
        {
            var path = new Queue<RoadSegment>();

            // There is no path from start to end
            if (end.prevSegment == null)
                return path;

            path.Enqueue(end);
            var currentSegment = end;
            do
            {
                currentSegment = currentSegment.prevSegment;
                path.Enqueue(currentSegment);
            } while (currentSegment != start);

            return new Queue<RoadSegment>(path.Reverse());
        }

        public void DeleteSegmentByPos(Vector3Int pos)
        {
            RoadSegment deletedSeg = segments.Find(seg => seg.pos == pos);
            if (deletedSeg == null)
                return;

            changed = true;

            deletedSeg.neighbors.ForEach(seg => seg.UnlinkFromSegment(deletedSeg));
            segments.Remove(deletedSeg);
        }
    }
}