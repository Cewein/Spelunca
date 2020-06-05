﻿
using UnityEngine;

public class Artefact : MonoBehaviour, ICollectible
{
    [SerializeField] private string name = "";
    [SerializeField] private float amplitude = 1.5f;
    [SerializeField] private float speed = 3;
    private float y;

    private void Start()
    {
        y = transform.position.y;
    }

    private void Update()
    {
        transform.position = new Vector3(transform.position.x, y + amplitude * Mathf.Sin(speed * Time.time),transform.position.z);
    }

    public bool IsReachable(Ray ray, float distance)
    {
        return Vector3.Distance(ray.origin, transform.position) < distance;
    }

    public void Collect()
    {
       Debug.Log(name + "is collected");
    }

    public void Emphase(bool isEmphased)
    {
       //TODO
    }
}