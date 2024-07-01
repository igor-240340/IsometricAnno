using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Src.Tools
{
    public class PathTool : ITool
    {
        private Tilemap debugTilemap;
        private Tile tileWhite;

        private List<GameObject> selectedBuildings = new();
        private Road road;
        private GameObject pathAnimatorPrefab;

        public PathTool(Tile tileWhite, Road road, GameObject pathAnimatorPrefab)
        {
            this.tileWhite = tileWhite;
            this.road = road;
            this.pathAnimatorPrefab = pathAnimatorPrefab;

            debugTilemap = GameObject.Find("DebugTilemap").GetComponent<Tilemap>();
        }

        public void OnMouseMove(InputAction.CallbackContext context)
        {
        }

        public void OnMouseLeftClick(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                RaycastBuildings(Mouse.current.position.ReadValue());
            }
        }

        private void RaycastBuildings(Vector2 mousePosition)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(mouseRay, 100000000);

            if (hit.collider != null)
            {
                selectedBuildings.Add(hit.collider.gameObject);
                VisualizeSelectedBuildings();
                if (selectedBuildings.Count == 2)
                {
                    FindPathBetweenBuildings(selectedBuildings[0], selectedBuildings[1]);
                    selectedBuildings.Clear();
                }
            }
            else
            {
                ResetSelection();
            }
        }

        private void FindPathBetweenBuildings(GameObject a, GameObject b)
        {
            List<RoadSegment> adjacentWithA = FindSegmentsAdjacentWith(a);
            List<RoadSegment> adjacentWithB = FindSegmentsAdjacentWith(b);

            var allPaths = new List<Queue<RoadSegment>>();
            Queue<RoadSegment> pathBetwenAB = new Queue<RoadSegment>();
            foreach (var segmentNextToA in adjacentWithA)
            {
                foreach (var segmentNextToB in adjacentWithB)
                {
                    pathBetwenAB = road.FindPath(segmentNextToA, segmentNextToB);
                    if (pathBetwenAB.Count > 0)
                        allPaths.Add(pathBetwenAB);
                }
            }

            if (allPaths.Count == 0)
            {
                Debug.Log("There is no a path");
                return;
            }

            Queue<RoadSegment> shortestPath = allPaths.OrderBy(path => path.Count).ToArray()[0];
            Object.Instantiate(pathAnimatorPrefab, Vector3.zero, Quaternion.identity).GetComponent<PathAnimator>().StartAnimation(shortestPath);
            // GameObject.Find("PathAnimator").GetComponent<PathAnimator>().StartAnimation(shortestPath);
            ResetSelection();
            selectedBuildings.Clear();
        }

        private List<RoadSegment> FindSegmentsAdjacentWith(GameObject building)
        {
            var adjacentWithBuilding = new List<RoadSegment>();

            List<Vector3Int> buildingCellsCoords = building.GetComponent<Building>().occupiedCellsCoords;
            foreach (var buildingCellCoord in buildingCellsCoords)
            {
                foreach (var segment in road.segments)
                {
                    Vector3Int segmentCellCoord = segment.pos;

                    if ((segmentCellCoord - buildingCellCoord).magnitude <= 1)
                        adjacentWithBuilding.Add(segment);
                }
            }

            return adjacentWithBuilding;
        }

        private void VisualizeSelectedBuildings()
        {
            debugTilemap.ClearAllTiles();
            selectedBuildings.ForEach(building =>
            {
                building.GetComponent<Building>().occupiedCellsCoords
                    .ForEach(cellPos => debugTilemap.SetTile(cellPos, tileWhite));
            });
        }

        private void ResetSelection()
        {
            selectedBuildings.Clear();
            debugTilemap.ClearAllTiles();
        }

        public void OnMouseRightClick(InputAction.CallbackContext context)
        {
            ResetSelection();
            selectedBuildings.Clear();
        }

        public void Init()
        {
            Camera.main.GetComponent<Physics2DRaycaster>().eventMask = 0;
        }

        public void Clean()
        {
            Camera.main.GetComponent<Physics2DRaycaster>().eventMask = ~0;
        }
    }
}