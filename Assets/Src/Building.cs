using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class Building : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] public Vector2 sizeInGridCells;
    [SerializeField] private Tile debugTileWhite;

    public List<Vector3Int> occupiedCellsCoords = new();

    private Tilemap debugTilemap;
    private Grid grid;
    private Vector3Int anchorCellCoord;
    private GameActions gameActions;

    private void Awake()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        debugTilemap = GameObject.Find("DebugTilemap").GetComponent<Tilemap>();

        gameActions = new GameActions();
        gameActions.Mouse.MouseLeftClick.performed += OnMouseLeftClick;
        gameActions.Mouse.MouseRightClick.performed += OnMouseRightClick;
    }

    private void OnMouseLeftClick(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        if (IsMouseOverUI(mousePosition))
            return;

        if (IsMouseClickedOutside(mousePosition))
            Deselect();
    }

    private void OnMouseRightClick(InputAction.CallbackContext context)
    {
        OnMouseLeftClick(context);
    }

    private bool IsMouseClickedOutside(Vector2 mousePosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
        return hit.collider == null || hit.collider.gameObject != gameObject;
    }

    private bool IsMouseOverUI(Vector2 mousePosition)
    {
        var pointerEventData = new PointerEventData(EventSystem.current) {position = mousePosition};

        var results = new List<RaycastResult>();
        var graphicsRaycaster = GameObject.Find("UIContainer").GetComponent<GraphicRaycaster>();
        graphicsRaycaster.Raycast(pointerEventData, results);

        return results.Count > 0;
    }

    public Vector3 position
    {
        set
        {
            transform.position = value;
            UpdateAnchorCellCoord();
            RecalculateOccupiedCellsCoords();
        }
    }

    private void UpdateAnchorCellCoord()
    {
        anchorCellCoord = grid.WorldToCell(transform.position);
    }

    private void RecalculateOccupiedCellsCoords()
    {
        occupiedCellsCoords.Clear();

        for (var x = anchorCellCoord.x; x < anchorCellCoord.x + sizeInGridCells.x; x++)
        {
            for (var y = anchorCellCoord.y; y < anchorCellCoord.y + sizeInGridCells.y; y++)
                occupiedCellsCoords.Add(new Vector3Int(x, y, 0));
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Select();
    }

    protected virtual void Select()
    {
        occupiedCellsCoords.ForEach(cellPos => debugTilemap.SetTile(cellPos, debugTileWhite));
        gameActions.Mouse.Enable();
    }

    protected virtual void Deselect()
    {
        debugTilemap.ClearAllTiles();
        gameActions.Mouse.Disable();
    }

    public virtual void OnPlace()
    {
    }
}