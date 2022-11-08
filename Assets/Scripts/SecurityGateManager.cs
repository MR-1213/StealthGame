using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityGateManager : MonoBehaviour
{
    LineRenderer lineRenderer;
    Vector3 direction;
    public LayerMask playerLayer;
    public float length;

    private void Start() 
    {
        direction = Vector3.right;
        length = 8.0f;

        lineRenderer = GetComponent<LineRenderer>();
        Vector3[] positions = new Vector3[]{
            transform.position,
            transform.position + (direction * length),
        };

        lineRenderer.startWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
        lineRenderer.SetPositions(positions);
    }

    private void Update() 
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;

        if(Physics.Raycast(rayCastOrigin, direction, out hit, length, playerLayer))
        {
            Debug.DrawRay(rayCastOrigin, direction * hit.distance, Color.yellow);
            Debug.Log("検知");
        }
    }

}
