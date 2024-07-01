using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Src.Tools
{
    public class DestroyTool : ITool
    {
        private List<Vector3Int> totalOccupiedCells;
        private Road road;
        private Grid grid;

        public DestroyTool(Road road, List<Vector3Int> totalOccupiedCells)
        {
            this.road = road;
            this.totalOccupiedCells = totalOccupiedCells;

            grid = GameObject.Find("Grid").GetComponent<Grid>();
        }

        public void OnMouseMove(InputAction.CallbackContext context)
        {
        }

        public void OnMouseLeftClick(InputAction.CallbackContext context)
        {
            if (context.canceled)
            {
                RaycastObjects(Mouse.current.position.ReadValue());
            }
        }

        private void RaycastObjects(Vector2 mousePosition)
        {
            Ray mouseRay = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection(mouseRay, 100000000);

            if (hit.collider != null)
            {
                Debug.DrawLine(Vector2.zero, hit.point, Color.magenta, 1000000000);

                Vector3Int anchorCell = grid.WorldToCell(hit.collider.gameObject.transform.position);
                Vector2 sizeInGridCells = hit.collider.gameObject.GetComponent<Building>().sizeInGridCells;
                for (var x = anchorCell.x; x < anchorCell.x + sizeInGridCells.x; x++)
                {
                    for (var y = anchorCell.y; y < anchorCell.y + sizeInGridCells.x; y++)
                    {
                        Vector3Int cell = new Vector3Int(x, y);
                        totalOccupiedCells.Remove(cell);
                    }
                }

                Object.Destroy(hit.collider.gameObject);
            }
            else
            {
                RaycastRoad(mousePosition);
            }
        }

        private void RaycastRoad(Vector2 mousePosition)
        {
            Vector3Int mouseCell = MouseScreenToGrid(mousePosition);
            road.DeleteSegmentByPos(mouseCell);
            totalOccupiedCells.Remove(mouseCell);
        }

        public void OnMouseRightClick(InputAction.CallbackContext context)
        {
        }

        private Vector3Int MouseScreenToGrid(Vector2 pos)
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(pos);
            return GameObject.Find("Grid").GetComponent<Grid>().WorldToCell(mouseWorld);
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