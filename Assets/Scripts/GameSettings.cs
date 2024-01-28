using UnityEngine;

[CreateAssetMenu(menuName = "Game Settings")]
public class GameSettings : ScriptableObject
{
    public TextAsset sequenceSource;
    public int maxKeysHeldAtOnce = 5;
}