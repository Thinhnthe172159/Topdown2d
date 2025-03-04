using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TitlemapVisualizer : MonoBehaviour
{
    [SerializeField]
    private Tilemap floorTilemap, wallTilemap;
    [SerializeField]
    private TileBase floorTile, wallTop, wallSiderRight, wallSiderLeft, wallBottom, wallFull,
        wallInnerCornerDownLeft, wallInnerCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerDownRight, wallDiagonalCornerUpLeft, wallDiagonalCornerUpRight;
    [SerializeField]
    public List<GameObject> bossRoomObjects;
    [SerializeField]
    public List<GameObject> roomObjects;
    public void PainFloorTiles(IEnumerable<Vector2Int> floorPositions)
    {
        PainTiles(floorPositions, floorTilemap, floorTile);
    }

    private void PainTiles(IEnumerable<Vector2Int> positions, Tilemap tileMap, TileBase tile)
    {
        foreach (var position in positions)
        {
            PaintSingleTitle(tileMap, tile, position);
        }
    }

    private void PaintSingleTitle(Tilemap tilemap, TileBase title, Vector2Int position)
    {
        var tilePostions = tilemap.WorldToCell((Vector3Int)position);
        tilemap.SetTile(tilePostions, title);
    }
    
    public void Clear()
    {
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
    }

    public void PainSingleBasicWall(Vector2Int wallPosition, string binaryType)
    {
        int type = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;
        if(WallTypesHelper.wallTop.Contains(type))
        {
            tile = wallTop;
        }
        else if (WallTypesHelper.wallSideLeft.Contains(type))
        {
            tile = wallSiderLeft;
        }
        else if (WallTypesHelper.wallSideRight.Contains(type))
        {
            tile = wallSiderRight;
        }
        else if (WallTypesHelper.wallBottm.Contains(type))
        {
            tile = wallBottom;
        }
        else
        {
            tile = wallFull;
        }
        PaintSingleTitle(wallTilemap, tile, wallPosition);
    }

    internal void PainSingleCornerWall(Vector2Int position, string binaryType)
    {
        int typetASInt = Convert.ToInt32(binaryType, 2);
        TileBase tile = null;

        if(WallTypesHelper.wallInnerCornerDownLeft.Contains(typetASInt))
        {
            tile = wallInnerCornerDownLeft;
        }
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typetASInt))
        {
            tile = wallInnerCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typetASInt))
        {
            tile = wallDiagonalCornerDownLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typetASInt))
        {
            tile = wallDiagonalCornerDownRight;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typetASInt))
        {
            tile = wallDiagonalCornerUpLeft;
        }
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typetASInt))
        {
            tile = wallDiagonalCornerUpRight;
        }
        else if (WallTypesHelper.wallFullEightDirections.Contains(typetASInt))
        {
            tile = wallFull;
        }
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typetASInt))
        {
            tile = wallBottom;
        }

        if (tile != null)
            PaintSingleTitle(wallTilemap, tile, position);
    }
}
    