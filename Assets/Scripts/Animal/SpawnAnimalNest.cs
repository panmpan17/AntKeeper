using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAnimalNest : MonoBehaviour
{
    [SerializeField]
    private Transform antNestCollection;
    [SerializeField]
    private GameObject fireAntPrefab;
    [SerializeField]
    private int startFireAntCount;
    [SerializeField]
    private GameObject navtiveAntPrefab;
    [SerializeField]
    private int startNativeAntCount;
    [SerializeField]
    private Transform[] antNestSpawnPoints;


    void Start()
    {
        InstainateAntNestOnStart();
    }

    void InstainateAntNestOnStart()
    {
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < antNestSpawnPoints.Length; i++)
            positions.Add(antNestSpawnPoints[i].position);

        for (int i = 0; i < startFireAntCount; i++)
        {
            int index = Random.Range(0, positions.Count);
            InstainateAntNestAtPosition(fireAntPrefab, positions[index]);
            positions.RemoveAt(index);
        }

        for (int i = 0; i < startNativeAntCount; i++)
        {
            int index = Random.Range(0, positions.Count);
            InstainateAntNestAtPosition(navtiveAntPrefab, positions[index]);
            positions.RemoveAt(index);
        }
    }

    void InstainateAntNestAtPosition(GameObject prefab, Vector3 position)
    {
        GameObject newNest = Instantiate(prefab, position, Quaternion.identity, antNestCollection);
        newNest.transform.position = position;

        var hub = newNest.GetComponent<AntNestHub>();
        hub.Freeze();

        var growControl = newNest.GetComponent<AntRouteGrowControl>();
        growControl.InitialSizeOnStart = true;
    }

    public void Spawn(bool isFireAnt, Vector3 position)
    {
        GameObject newAntNest = Instantiate(isFireAnt ? fireAntPrefab : navtiveAntPrefab, antNestCollection);
        newAntNest.transform.position = position;
    }
}
