using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemy", order = 1)]
public class EnemyData : ScriptableObject
{
    [field: SerializeField] public int spawnCost { get; private set; }
    [field: SerializeField] public int hp { get; private set; }
    [field: SerializeField] public float movementSpeed { get; private set; }
    [field: SerializeField] public int exp { get; private set; }
}
