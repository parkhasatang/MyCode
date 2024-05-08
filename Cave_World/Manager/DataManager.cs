using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using UnityEngine;
using UnityEngine.Tilemaps;


// 저장하는 방법
// 1. 저장할 데이터가 존재
// 2. 데이터를 제이슨으로 변환
// 3. 제이슨을 외부에 저장


// 불러오는 법
// 1. 외부에 저장된 제이슨을 가져옴
// 2. 제이슨을 데이터 형태로 변환
// 3. 불러온 데이터를 사용


public class DataManager : MonoBehaviour
{
    public static DataManager instance;
    public GameObject player;
    private Inventory inventory;
    
    string path;
    string fileName = "SaveFile";

    public Tilemap _tilemap;
    public Tilemap _ceilingtile;
        
    

    
    private void Awake()
    {        
        instance = this;
        DontDestroyOnLoad(this.gameObject);
        path = Application.persistentDataPath + "/";
        inventory = player.GetComponent<Inventory>();        
    }
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(path);
        if (File.Exists(path + fileName))
        {
            LoadData();
        }
    }   
    // Update is called once per frame
    void Update()
    {
        
    }

    public void SaveData()
    {
        // 저장할 데이터 객체화
        GameData newGameData = new GameData();
        newGameData._slotNum = new SlotNum[26];
        newGameData._slots = new List<SlotData>();
        for (int i = 0; i < 26; i++)
        {
            newGameData._slotNum[i] = new SlotNum();
            SlotData slot = new SlotData()
            {
                isEmpty = true,
                isChoose = false,
                item = null,
                stack = 0
            };
            newGameData._slots.Add(slot);
        }
             //플레이어 위치 저장
            newGameData.playerPosition = player.transform.position;

        // 맵 저장

        List<Vector3Int> changedTiles = new List<Vector3Int>();
        BoundsInt bounds = _tilemap.cellBounds;
        for(int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for(int y = bounds.yMin; y< bounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                TileBase tile = _tilemap.GetTile(tilePosition);
                if(tile == null)
                {
                    changedTiles.Add(tilePosition); 
                }
            }
        }
        
        List<Vector3Int> changedCeilingTiles = new List<Vector3Int>();
        BoundsInt ceilingBounds = _ceilingtile.cellBounds;
        for (int x = ceilingBounds.xMin; x < ceilingBounds.xMax; x++)
        {
            for(int y = ceilingBounds.yMin; y < ceilingBounds.yMax; y++)
            {
                Vector3Int tilePosition = new Vector3Int(x, y, 0);
                TileBase tile = _ceilingtile.GetTile(tilePosition);
                if(tile == null)
                {
                    changedCeilingTiles.Add(tilePosition);
                }
            }
        }

        newGameData.changedTiles = changedTiles.ToArray();              
        newGameData.changedCeilingTiles = changedCeilingTiles.ToArray();
        // 아이템 정보 저장
        for (int i = 0; i < 26; i++)
        {
            newGameData._slotNum[i] = inventory.invenSlot[i];
            newGameData._slots[i] = inventory.slots[i];
        }

        // 제이슨 데이터로 변환
        string data = JsonUtility.ToJson(newGameData);
        File.WriteAllText(path + fileName, data);
    }

    public void LoadData()
    {
        // 제이슨 데이터 코드로 변환 
        string data = File.ReadAllText(path + fileName);
        GameData loadGameData = JsonUtility.FromJson<GameData>(data);

        // 플레이어 위치 설정
        player.transform.position = loadGameData.playerPosition;

        // 맵 정보 설정
        foreach (Vector3Int tilePosition in loadGameData.changedTiles)
        {
            _tilemap.SetTile(tilePosition, null);
        }

        foreach(Vector3Int tilePosition in loadGameData.changedCeilingTiles)
        {
            _ceilingtile.SetTile(tilePosition, null);
        }

        TilemapManager.instance.tilemap = _tilemap;
        TilemapManager.instance.ceilingTile = _ceilingtile;
        // 아이템 정보 설정
        for (int i = 0; i < 26; i++)
        {
            inventory.invenSlot[i] = loadGameData._slotNum[i];
            inventory.slots[i] = loadGameData._slots[i];
            inventory.StackUpdate(i);
        }        
    }
}
