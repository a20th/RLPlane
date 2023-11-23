using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject ringPrefab;
    public GameObject obj;
    public void Spawn(Vector3 pos)
    {
        Vector3 newPosition = transform.position + pos;
        if(obj != null)
        {
            Destroy(obj);
        }
        obj = Instantiate(ringPrefab, newPosition, Quaternion.Euler(0,0,0), this.transform);
    }
}
