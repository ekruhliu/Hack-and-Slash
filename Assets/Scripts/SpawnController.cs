using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    public GameObject ZombieM;
    public GameObject ZombieW;
    public GameObject Player;
    
    void Start() { StartCoroutine(Spawner()); }

    IEnumerator Spawner()
    {
        ZombieM.GetComponent<Zombies>().player = Player;
        ZombieW.GetComponent<Zombies>().player = Player;
        Vector3 randOffset = new Vector3(Random.Range(0.0f, 2.0f), 0, Random.Range(0.0f, 2.0f));
        Instantiate(ZombieM, transform.position + randOffset, Quaternion.identity);
        randOffset = new Vector3(Random.Range(0.0f, 2.0f), 0, Random.Range(0.0f, 2.0f));
        Instantiate(ZombieW, transform.position + randOffset, Quaternion.identity);
        yield return new WaitForSeconds(20f);
        StartCoroutine(Spawner());
    }
}
