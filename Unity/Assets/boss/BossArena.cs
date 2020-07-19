using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BossArena : MonoBehaviour
{
    public Camera cinematicCamera;
    public Camera[] otherCameras;
    public float cinematicLengthMilliseconds = 3000f;
    public float totalRotation = 720f;
    public AnimationCurve speedModifierCurve;
    public AnimationCurve radiusOverTime;
    public float minHeight;
    public float maxHeight;
    public AnimationCurve heightOverTime;
    
    private Vector3 cinematicStartPosition;
    private float radius;
    private float startRadian;

    private bool cinematicHasOccured = false;
    private bool cinematicIsRunning = false;
    private Stopwatch sw = new Stopwatch();

    private void Update()
    {
        if(cinematicIsRunning) {
            float progress = getProgress();
            if (progress <= 1)
            {
                float newProgress = speedModifierCurve.Evaluate(progress);//allows to speed up the camera at the beginning and brake the camera at the end
                Vector3 newCameraPosition = getPositionForCamera(newProgress);
                cinematicCamera.transform.position = newCameraPosition;
                cinematicCamera.transform.LookAt(transform.position);
            }
            else
            {
                StopCinematic();
            }
        } 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody.gameObject.layer == LayerMask.NameToLayer("Player") && !cinematicHasOccured)
        {
            cinematicStartPosition = other.attachedRigidbody.gameObject.transform.position;
            SetupCinematic();
            StartCinematic();
            
        }
    }

    private void SetupCinematic()
    {
        cinematicHasOccured = true;
        Vector3 direction = (transform.position-cinematicStartPosition);
        radius = direction.magnitude;
        //Freeze du jeu;
        ToggleCameras();
        cinematicCamera.enabled = true;
        Vector3 normalized = direction.normalized;
        startRadian = Mathf.Atan2(normalized.z,normalized.x);
    }
    
    private void StartCinematic()
    {
        Debug.Log("Cinematic Started");
        sw.Restart();
        cinematicIsRunning = true;
    }
    
    private void StopCinematic()
    {
        Debug.Log("Cinematic Stoped");
        sw.Stop();
        sw.Reset();
        cinematicIsRunning = false;
        ToggleCameras();
        cinematicCamera.enabled = false;
    }
    
    private void ToggleCameras()
    {
        foreach (var cam in otherCameras)
        {
            cam.enabled = !cam.enabled;
        }
    }

    private float getProgress()
    {
        return sw.ElapsedMilliseconds / cinematicLengthMilliseconds;
    }
    

    private Vector3 getPositionForCamera(float progress)
    {
        float radian = Mathf.PI + startRadian + ((1-progress) * totalRotation * Mathf.Deg2Rad);
        float cos = Mathf.Cos(radian);
        float sin = Mathf.Sin(radian);
        float newRadius = radiusOverTime.Evaluate(progress)*radius;
        float x = cos * newRadius;
        float y = heightOverTime.Evaluate(progress) * (maxHeight - minHeight) + minHeight;
        float z = sin * newRadius;
        return new Vector3(x,y,z) +  transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
        if (cinematicIsRunning)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(cinematicCamera.transform.position,transform.position);
        }
    }
}
