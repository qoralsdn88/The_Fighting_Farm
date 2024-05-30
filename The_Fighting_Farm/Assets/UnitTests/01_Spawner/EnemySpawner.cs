using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
	[SerializeField]
	private SpawnPoint[] spawnPoints;

    [SerializeField]
    private GameObject particlePrefab;

    private Dictionary<string, GameObject[]> poolingTable;

    private void Start()
    {
        Debug.Assert(spawnPoints != null);

        
        poolingTable = new Dictionary<string, GameObject[]>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            SpawnPoint point = spawnPoints[i];
            GameObject obj = new GameObject(point.name);

            GameObject[] pool = CreateObjectPool(obj, spawnPoints[i]);

            poolingTable.Add(point.name, pool);
        }


        string[] names = new string[spawnPoints.Length];

        for (int i = 0; i < spawnPoints.Length; i++)
            names[i] = spawnPoints[i].name;


        StartCoroutine(SpawnPoolObject(names));
    }

    private GameObject[] CreateObjectPool(GameObject parent, SpawnPoint spawnPoint)
    {
        GameObject[] result = new GameObject[spawnPoint.SpawnCount];

        for(int i = 0; i < spawnPoint.SpawnCount; i++)
        {
            result[i] = Instantiate<GameObject>(spawnPoint.EnemyPrefab, parent.transform, false);
            result[i].name = $"{spawnPoint.EnemyPrefab.name}_{i:000}";

            Vector3 position = new Vector3();
            position.x = spawnPoint.SpawnPoints[i].x;
            position.z = spawnPoint.SpawnPoints[i].y;
            result[i].transform.localPosition = position;
            
            result[i].SetActive(false);
        }

        return result;
    }

    public void Spawn(string spawnPointName)
    {
        GameObject[] pool = poolingTable[spawnPointName];


        GameObject spawnObject = null;
        foreach(GameObject obj in pool)
        {
            if(obj.activeSelf == false)
            {
                spawnObject = obj;

                break;
            }
        }

        if(spawnObject == null)
            return;


        Instantiate<GameObject>(particlePrefab, spawnObject.transform, false);
        spawnObject.SetActive(true);
    }

    private IEnumerator SpawnPoolObject(string[] spawnPointNames)
    {
        while(true)
        {
            int random = Random.Range(0, spawnPointNames.Length);
            Spawn(spawnPointNames[random]);

            float time = Random.Range(0.01f, 0.05f);

            yield return new WaitForSeconds(time);
        }
    }
}