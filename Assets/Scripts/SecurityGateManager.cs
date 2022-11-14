using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityGateManager : MonoBehaviour
{
    EnemySearchPlayer enemySearchPlayer;

    LineRenderer lineRenderer;
    Vector3 direction;
    public LayerMask playerLayer;
    public float length;

    private void Start() 
    {
        enemySearchPlayer = GameObject.Find("Enemy").GetComponent<EnemySearchPlayer>();

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
            Debug.Log("検知された・・・");
            enemySearchPlayer.SetDetectPosition(hit);
        }
    }

}
