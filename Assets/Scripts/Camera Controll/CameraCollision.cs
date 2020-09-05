﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{

    public float minDistance = 1f;
    public float maxDistance = 4f;
    public float smooth = 10f;

    public float cameraRadius = 0.3f;
    Vector3 dollyDir;
    public Vector3 dollyDirAdjusted;
    public float distance;

    // Start is called before the first frame update
    void Awake()
    {
        dollyDir = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDir * maxDistance);
        RaycastHit hit;

        //Debug.DrawLine(transform.parent.position, desiredCameraPos, Color.black, 0.01f);
        Debug.DrawRay(transform.parent.position, desiredCameraPos - transform.parent.position, Color.black, 0.01f);
        if(Physics.SphereCast(transform.parent.position, cameraRadius, desiredCameraPos - transform.parent.position,  out hit, (transform.parent.position - desiredCameraPos).magnitude)) {
        //if(Physics.Linecast(transform.parent.position, desiredCameraPos, out hit)) {
            if(hit.collider.gameObject.tag != "Player" && hit.collider.gameObject.tag != "Grind") {
                //Debug.Log(hit.collider.gameObject);
                distance = Mathf.Clamp(hit.distance * 0.9f, minDistance, maxDistance);
            }
        } else {
            distance = maxDistance;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDir * distance, Time.deltaTime * smooth);
    }
}
