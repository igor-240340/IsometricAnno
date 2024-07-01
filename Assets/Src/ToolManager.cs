using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Src;
using Src.Tools;

public class ToolManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> buildingPrefabs;

    [SerializeField] private Tile tileWhite;
    [SerializeField] private Tile tileRed;
    [SerializeField] private Tile[] tiles;
    [SerializeField] private GameObject pathAnimatorPrefab;

    private Dictionary<string, ITool> tools = new();
    private ITool activeTool;

    private List<Vector3Int> totalOccupiedCells = new();

    private Road road = new();
    private RoadRenderer roadRenderer;

    void Start()
    {
        tools.Add("RoadTool", new Src.Tools.RoadTool(tileWhite, tileRed, road, totalOccupiedCells));
        tools.Add("DestroyTool", new Src.Tools.DestroyTool(road, totalOccupiedCells));
        tools.Add("HouseBuildingTool", new Src.Tools.BuildingTool(buildingPrefabs[0], tileWhite, tileRed, totalOccupiedCells));
        tools.Add("ForesterHutBuildingTool", new Src.Tools.BuildingTool(buildingPrefabs[1], tileWhite, tileRed, totalOccupiedCells));
        tools.Add("WarehouseBuildingTool", new Src.Tools.BuildingTool(buildingPrefabs[2], tileWhite, tileRed, totalOccupiedCells));
        tools.Add("PathTool", new Src.Tools.PathTool(tileWhite, road, pathAnimatorPrefab));

        roadRenderer = new RoadRenderer(road, tiles);
    }

    void Update()
    {
        roadRenderer.OnUpdate();
    }

    public void OnMouseMove(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        if (IsMouseOverUI(mousePosition))
            return;

        activeTool?.OnMouseMove(context);
    }

    public void OnMouseLeftClick(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        if (IsMouseOverUI(mousePosition))
            return;

        activeTool?.OnMouseLeftClick(context);
    }

    public void OnMouseRightClick(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        if (IsMouseOverUI(mousePosition))
            return;
        
        activeTool?.OnMouseRightClick(context);
    }

    // Tool toggles
    public void OnRoadToggleChanged(bool value)
    {
        SwitchActiveTool(tools["RoadTool"], value);
    }

    public void OnHouseToggleChanged(bool value)
    {
        SwitchActiveTool(tools["HouseBuildingTool"], value);
    }

    public void OnWarehouseToggleChanged(bool value)
    {
        SwitchActiveTool(tools["WarehouseBuildingTool"], value);
    }

    public void OnForesterHutToggleChanged(bool value)
    {
        SwitchActiveTool(tools["ForesterHutBuildingTool"], value);
    }

    public void OnDestroyToggleChanged(bool value)
    {
        SwitchActiveTool(tools["DestroyTool"], value);
    }
    
    public void OnPathToggleChanged(bool value)
    {
        SwitchActiveTool(tools["PathTool"], value);
    }

    private void SwitchActiveTool(ITool tool, bool value)
    {
        if (value)
        {
            activeTool?.Clean();
            activeTool = tool;
            activeTool.Init();
        }
        else if (activeTool == tool)
        {
            activeTool?.Clean();
            activeTool = null;
        }
    }

    private bool IsMouseOverUI(Vector2 mousePosition)
    {
        var pointerEventData = new PointerEventData(EventSystem.current) {position = mousePosition};

        var results = new List<RaycastResult>();
        var graphicsRaycaster = GameObject.Find("UIContainer").GetComponent<GraphicRaycaster>();
        graphicsRaycaster.Raycast(pointerEventData, results);

        return results.Count > 0;
    }
}