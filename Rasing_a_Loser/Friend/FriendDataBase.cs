using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "FriendDatabase", menuName = "Scriptable Object/Friend Database")]
public class FriendDatabase : ScriptableObject
{
    public List<FriendInfo> friends = new List<FriendInfo>();
}