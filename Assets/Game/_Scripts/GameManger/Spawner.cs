using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour {

	public GameObject [] meteorPrefabs;
	public float distance = 20f;
    public float intervalTime = 0f;

    void Start ()
	{
        StartCoroutine(SpawnMeteor());
    }

    IEnumerator SpawnMeteor()
    {
        yield return new WaitForSeconds(intervalTime);

        Vector3 pos = Random.onUnitSphere * distance;
        int randomIndex = Random.Range(0, meteorPrefabs.Length);
        GameObject Monster = Instantiate(meteorPrefabs[randomIndex], pos, Quaternion.identity);
       
        StartCoroutine(SpawnMeteor());
    }

}
