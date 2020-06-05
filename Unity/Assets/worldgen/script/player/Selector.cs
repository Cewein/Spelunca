using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selector : MonoBehaviour
{
    public ChunkManager ckm;

    Vector3 front;
    Vector3 position;

    // Update is called once per frame
    void Update()
    {
        front = Camera.main.transform.forward;
        position = Camera.main.transform.position;

        int layerMask = 1 << 8;
        layerMask = ~layerMask;

        RaycastHit hit;

        if(Input.GetKey(KeyCode.Mouse0))
        {
            if (Physics.Raycast(position, front, out hit, Mathf.Infinity, layerMask))
            {
                Debug.DrawRay(position, front * hit.distance, Color.red);
                Debug.Log("HIT AT : " + hit.distance + "m");

                hit.transform.SendMessage("Hit");

                Vector3 pos = hit.transform.position;

                Vector3 chunk = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(pos.z)) / ckm.chunkSize;
                Vector3 crystal = hit.transform.gameObject.transform.position;

                Destroy(hit.transform.gameObject);
                print(chunk);
                print(crystal);

            }
            else
            {
                Debug.DrawRay(position, front * 1000, Color.white);
                Debug.Log("NOT HIT DUMMY!");
            }
        }
        
    }
}
