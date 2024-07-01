using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

namespace Src.Tools
{
    public class RoadTool : ITool
    {
        private bool unableToBuild;
        private bool started;
        private bool xIsFirst;
        private bool yIsFirst;

        private Vector3Int startCell;
        private Vector3Int endCell;
        private List<Vector3Int> cells = new();

        private Vector2Int drawingDir;

        private Tile tileWhite;
        private Tile tileRed;
        private Tilemap debugTilemap;

        private List<Vector3Int> totalOccupiedCells;

        private Road road;

        public RoadTool(Tile tileWhite, Tile tileRed, Road road, List<Vector3Int> totalOccupiedCells)
        {
            this.tileWhite = tileWhite;
            this.tileRed = tileRed;
            this.road = road;
            this.totalOccupiedCells = totalOccupiedCells;

            debugTilemap = GameObject.Find("DebugTilemap").GetComponent<Tilemap>();
        }

        public void OnMouseMove(InputAction.CallbackContext context)
        {
            if (!started)
                return;

            cells.Clear();
            debugTilemap.ClearAllTiles();

            endCell = MouseScreenToGridCell(context.ReadValue<Vector2>());

            DetectFirstAxis();

            cells.Add(startCell);
            CalculateCellsCoordsAlongXAxis();
            CalculateCellsCoordsAlongYAxis();

            unableToBuild = false;
            cells.ForEach(cell =>
            {
                if (totalOccupiedCells.Exists(occupiedCell => occupiedCell == cell))
                {
                    unableToBuild = true;
                    debugTilemap.SetTile(cell, tileRed);
                }
                else
                    debugTilemap.SetTile(cell, tileWhite);
            });
        }

        private void DetectFirstAxis()
        {
            Vector3Int diff = endCell - startCell;
            NormalizeDrawingDirectionVector(diff);
            if (diff.magnitude <= 0)
            {
                xIsFirst = false;
                yIsFirst = false;
                return;
            }

            if (xIsFirst || yIsFirst)
                return;

            // X axis is the first
            if ((diff.x != 0 && diff.y == 0) || (diff.x != 0 && diff.y != 0))
                xIsFirst = true;
            // Y axis is the first
            else if (diff.y != 0 && diff.x == 0)
                yIsFirst = true;
        }

        private void NormalizeDrawingDirectionVector(Vector3Int diff)
        {
            drawingDir.x = diff.x != 0 ? diff.x / Math.Abs(diff.x) : 0;
            drawingDir.y = diff.y != 0 ? diff.y / Math.Abs(diff.y) : 0;
        }

        private void CalculateCellsCoordsAlongXAxis()
        {
            int fixedY = xIsFirst ? startCell.y : endCell.y;

            int currentX = startCell.x;
            while (currentX != endCell.x)
            {
                currentX += drawingDir.x;
                cells.Add(new Vector3Int(currentX, fixedY, 0));
            }
        }

        private void CalculateCellsCoordsAlongYAxis()
        {
            int fixedX = xIsFirst ? endCell.x : startCell.x;
            int currentY = startCell.y;
            while (currentY != endCell.y)
            {
                currentY += drawingDir.y;
                cells.Add(new Vector3Int(fixedX, currentY, 0));
            }
        }

        public void OnMouseLeftClick(InputAction.CallbackContext context)
        {
            if (unableToBuild)
                return;

            if (context.canceled && !started)
            {
                startCell = MouseScreenToGridCell(Mouse.current.position.ReadValue());
                started = true;
            }
            else if (context.canceled)
            {
                totalOccupiedCells.AddRange(cells);
                road.CreateNewSegments(cells.ToArray());

                Clean();
            }
        }

        private Vector3Int MouseScreenToGridCell(Vector2 pos)
        {
            Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(pos);
            return GameObject.Find("Grid").GetComponent<Grid>().WorldToCell(mouseWorld);
        }

        public void OnMouseRightClick(InputAction.CallbackContext context)
        {
            Clean();
        }

        public void Init()
        {
            Camera.main.GetComponent<Physics2DRaycaster>().eventMask = 0;
        }

        public void Clean()
        {
            Camera.main.GetComponent<Physics2DRaycaster>().eventMask = ~0;
            
            cells.Clear();
            debugTilemap.ClearAllTiles();
            started = false;
            unableToBuild = false;
        }
    }
}