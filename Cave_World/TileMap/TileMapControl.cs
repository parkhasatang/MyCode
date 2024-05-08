using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileMapControl : MonoBehaviour
{
    public TileBase wallTile;
    public TileBase ceilingTile;

    public void CreateTile(int x, int y)
    {
        Vector3 cellPosition = new Vector3(x, y, 0);
        TilemapManager.instance.tilemap.SetTile(TilemapManager.instance.tilemap.WorldToCell(cellPosition), wallTile);
        // ceilingTile을 추가하기 위해 정보 받아오기.
        Vector3Int UpCeilingPosition = new Vector3Int(Mathf.FloorToInt(x), Mathf.FloorToInt(y)+1,0);
        if (!TilemapManager.instance.tilemap.GetTile(UpCeilingPosition))
        {
            Debug.Log("있었다.");
            TilemapManager.instance.ceilingTile.SetTile(TilemapManager.instance.ceilingTile.WorldToCell(UpCeilingPosition), ceilingTile);
            TilemapManager.instance.ceilingTile.SetTile(TilemapManager.instance.ceilingTile.WorldToCell(cellPosition), null);
        }
        else if (TilemapManager.instance.tilemap.GetTile(UpCeilingPosition))
        {
            TilemapManager.instance.ceilingTile.SetTile(TilemapManager.instance.ceilingTile.WorldToCell(cellPosition), null);
        }
    }

    public void DestroyTile(int x, int y)
    {
        Vector3 cellPosition = new Vector3(x, y, 0);
        TilemapManager.instance.tilemap.SetTile(TilemapManager.instance.tilemap.WorldToCell(cellPosition), null);
    }
}
