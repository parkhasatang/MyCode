using UnityEngine;

[CreateAssetMenu(fileName = "New Wig", menuName = "WigSO/Wig")]
public class WigSO : ScriptableObject
{
    // 가발 ID
    public string uniqueID;
    
    // 표시 순서를 위한 인덱스
    public int index;

    // 이름
    public string wigName;

    // 판매 가격
    public int sellingPrice;

    // 가발 머리카락
    public string hairMaterial;

    // 가발 설명
    [TextArea]
    public string description;
    
    // 가발 Y오프셋
    public float yOffset;
}