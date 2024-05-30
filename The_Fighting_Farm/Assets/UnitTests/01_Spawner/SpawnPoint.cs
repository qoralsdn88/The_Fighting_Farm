using UnityEngine;

//[CreateAssetMenu(fileName = "SpawnPoint", menuName = "Playables/Enemy Spawn Point", order = 1)]
public class SpawnPoint : ScriptableObject
{
	[SerializeField]
	public Vector2 MapSize = new Vector2(-23.0f, +23.0f);

	[SerializeField]
	public GameObject EnemyPrefab;

	[SerializeField]
	public int SpawnCount = 20;
	
	[SerializeField]
	public Vector2[] SpawnPoints;
}