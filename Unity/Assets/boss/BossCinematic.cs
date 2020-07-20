using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BossCinematic : MonoBehaviour
{
    
    public float detectionDistance;
    public float baseFOV = 60;
    public Camera cinematicCamera;
    public Camera mainCamera;
    public Camera[] otherCameras;
    public MinerController controller;
    public float cinematicLengthMilliseconds = 3000f;
    public float totalRotation = 720f;
    public AnimationCurve speedModifierCurve;
    public AnimationCurve radiusOverTime;
    public float minHeight;
    public float maxHeight;
    public AnimationCurve heightOverTime;
    public AnimationCurve lerpToPlayerCamOverTime;

    public ZoneSpawner spawner;

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
                float lerpCoef = lerpToPlayerCamOverTime.Evaluate(progress);
                Vector3 newPos = Vector3.Lerp(newCameraPosition,mainCamera.transform.position,lerpCoef);
                Quaternion newRot = Quaternion.Lerp(cinematicCamera.transform.rotation, mainCamera.transform.rotation,
                    lerpCoef);
                cinematicCamera.transform.position = newPos;
                cinematicCamera.transform.rotation = newRot;

                cinematicCamera.fieldOfView = Mathf.Lerp(baseFOV, mainCamera.fieldOfView,speedModifierCurve.Evaluate(progress));
            }
            else
            {
                StopCinematic();
            }
        }
        else if(!cinematicHasOccured)
        {
            float distance = (transform.position-controller.transform.position).magnitude;
            Debug.Log("No cinematic yet. dist = " + distance);
            if (distance <= detectionDistance)
            {
                Debug.Log("LETS GO BIATCH");
                cinematicStartPosition = controller.transform.position;
                SetupCinematic();
                StartCinematic();
            }
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
        if(controller != null) controller.canMove = false;
        spawner.BeginSpawning();
    }
    
    private void StopCinematic()
    {
        Debug.Log("Cinematic Stopped");
        sw.Stop();
        sw.Reset();
        cinematicIsRunning = false;
        ToggleCameras();
        cinematicCamera.enabled = false;
        if(controller != null) controller.canMove = true;
    }
    
    private void ToggleCameras()
    {
        mainCamera.enabled = !mainCamera.enabled;
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
