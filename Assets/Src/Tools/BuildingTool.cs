using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Src.Tools
{
    public class BuildingTool : ITool
    {
        private GameObject prefab;
        private GameObject newBuilding;
        private Grid grid;
        private bool unableToBuild;
        private Tilemap debugTilemap;

        private Tile tileWhite;
        private Tile tileRed;
        private List<Vector3Int> totalOccupiedCells;
        private List<Vector3Int> cellsOccupiedByBuilding = new();

        public BuildingTool(GameObject prefab, Tile tileWhite, Tile tileRed, List<Vector3Int> totalOccupiedCells)
        {
            this.prefab = prefab;
            this.tileWhite = tileWhite;
            this.tileRed = tileRed;
            this.totalOccupiedCells = totalOccupiedCells;

            grid = GameObject.Find("Grid").GetComponent<Grid>();
            debugTilemap = GameObject.Find("DebugTilemap").GetComponent<Tilemap>();
        }

        public void OnMouseMove(InputAction.CallbackContext context)
        {
            if (newBuilding)
            {
                Vector2 mouseScreen = context.ReadValue<Vector2>();
                Vector3 mouseGrid = MouseScreenToGrid(mouseScreen);
                newBuilding.GetComponent<Building>().position = mouseGrid;

                CheckIfPlaceOccupied();
            }
        }

        private void CheckIfPlaceOccupied()
        {
            debugTilemap.ClearAllTiles();
            cellsOccupiedByBuilding.Clear();

            unableToBuild = false;

            Vector3Int anchorCell = grid.WorldToCell(newBuilding.transform.position);
            Vector2 sizeInGridCells = newBuilding.GetComponent<Building>().sizeInGridCells;
            for (var x = anchorCell.x; x < anchorCell.x + sizeInGridCells.x; x++)
            {
                for (var y = anchorCell.y; y < anchorCell.y + sizeInGridCells.x; y++)
                {
                    Vector3Int cell = new Vector3Int(x, y);
                    cellsOccupiedByBuilding.Add(cell);

                    if (totalOccupiedCells.Exists(occupiedCell => occupiedCell == cell))
                    {
                        unableToBuild = true;
                        debugTilemap.SetTile(cell, tileRed);
                    }
                    else
                        debugTilemap.SetTile(cell, tileWhite);
                }
            }
        }

        private Vector3 MouseScreenToGrid(Vector2 mousePosition)
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(mousePosition);
            Vector3Int cell = grid.WorldToCell(mouseWorld);
            return grid.GetCellCenterWorld(cell);
        }

        public void OnMouseLeftClick(InputAction.CallbackContext context)
        {
            if (context.canceled && !unableToBuild)
            {
                totalOccupiedCells.AddRange(cellsOccupiedByBuilding);
                newBuilding = null;
                Init();
            }
        }

        public void OnMouseRightClick(InputAction.CallbackContext context)
        {
            Debug.Log("BuildingTool.OnMouseMove");
        }

        public void Init()
        {
            Camera.main.GetComponent<Physics2DRaycaster>().eventMask = 0;
            newBuilding = Object.Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }

        public void Clean()
        {
            Camera.main.GetComponent<Physics2DRaycaster>().eventMask = ~0;
            
            Object.Destroy(newBuilding);
            newBuilding = null;

            debugTilemap.ClearAllTiles();
        }
    }
}