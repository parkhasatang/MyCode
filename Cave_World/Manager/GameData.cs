using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public class GameData
{
    // 이름, 레벨, 코인, 착용중인 무기    
    public Vector3 playerPosition;
    public SlotNum[] _slotNum;
    public List<SlotData> _slots;

    public Vector3Int[] changedCeilingTiles;
    public Vector3Int[] changedTiles;    
}
