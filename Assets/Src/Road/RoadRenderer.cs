using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Src
{
    public class RoadRenderer
    {
        private Tile[] tiles;
        private Tilemap roadTilemap;
        
        private Road road;

        private Dictionary<RoadSegmentType, TileIndex> tileIndexes = new();

        public RoadRenderer(Road road, Tile[] tiles)
        {
            this.road = road;
            this.tiles = tiles;
            
            roadTilemap = GameObject.Find("RoadTilemap").GetComponent<Tilemap>();

            tileIndexes.Add(RoadSegmentType.single, TileIndex.HORIZONTAL);
            tileIndexes.Add(RoadSegmentType.crossroads, TileIndex.CROSSROADS);
            tileIndexes.Add(RoadSegmentType.horizontalLeftRight, TileIndex.HORIZONTAL);
            tileIndexes.Add(RoadSegmentType.horizontalLeft, TileIndex.HORIZONTAL_LEFT);
            tileIndexes.Add(RoadSegmentType.horizontalRight, TileIndex.HORIZONTAL_RIGHT);
            tileIndexes.Add(RoadSegmentType.verticalUpDown, TileIndex.VERTICAL);
            tileIndexes.Add(RoadSegmentType.verticalUp, TileIndex.VERTICAL_UP);
            tileIndexes.Add(RoadSegmentType.verticalDown, TileIndex.VERTICAL_DOWN);
            tileIndexes.Add(RoadSegmentType.turnNorthEast, TileIndex.TURN_NE);
            tileIndexes.Add(RoadSegmentType.turnNorthWest, TileIndex.TURN_NW);
            tileIndexes.Add(RoadSegmentType.turnSouthEast, TileIndex.TURN_SE);
            tileIndexes.Add(RoadSegmentType.turnSouthWest, TileIndex.TURN_SW);
            tileIndexes.Add(RoadSegmentType.tJunctionDownClosed, TileIndex.T_JUNCTION_DOWN_CLOSED);
            tileIndexes.Add(RoadSegmentType.tJunctionLeftClosed, TileIndex.T_JUNCTION_LEFT_CLOSED);
            tileIndexes.Add(RoadSegmentType.tJunctionRightClosed, TileIndex.T_JUNCTION_RIGHT_CLOSED);
            tileIndexes.Add(RoadSegmentType.tJunctionUpClosed, TileIndex.T_JUNCTION_UP_CLOSED);
        }

        public void OnUpdate()
        {
            roadTilemap.ClearAllTiles();
            road.segments.ForEach(DrawWithTile);
        }

        private void DrawWithTile(RoadSegment seg)
        {
            RoadSegmentType type = new RoadSegmentType(seg);
            TileIndex index = tileIndexes[type];
            Tile tile = tiles[(int) index];
            roadTilemap.SetTile(seg.pos, tile);
        }
    }
}