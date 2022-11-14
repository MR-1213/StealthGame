using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEditor;

public class EnemySearchPlayer : MonoBehaviour
{
    EnemyManager enemyManager;
    NavMeshAgent navMeshAgent;
    Animator animator;

    [SerializeField]
    private SphereCollider searchArea;
    
    private Vector3 detectPosition;
    public float searchAngle = 120f;
    public LayerMask obstacleLayer;
    public bool isDetecting;

    private void Start()
    {
        enemyManager = GetComponent<EnemyManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }
    
    private void Update() 
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y += 1.0f;
        if(other.CompareTag("Player"))
        {
            Vector3 playerDirection = other.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, playerDirection);

            if(Physics.Raycast(rayCastOrigin, playerDirection, out hit, searchArea.radius, obstacleLayer))
            {
                if(hit.collider.CompareTag("Obstacle")) return;
            }

            double eachDistance = Math.Sqrt(Math.Pow(playerDirection.x, 2) + Math.Pow(playerDirection.z, 2));

            if(angle <= searchAngle || eachDistance < 1.0)
            {
                enemyManager.isFounding = true;
                transform.rotation = Quaternion.LookRotation(playerDirection);
                animator.SetInteger("ID", 0);
                navMeshAgent.SetDestination(other.transform.position);
                navMeshAgent.speed = 3.5f;
            }

            if(angle <= searchAngle && eachDistance < (searchArea.radius / 2.0f))
            { 
                enemyManager.isFounding = true;
                transform.rotation = Quaternion.LookRotation(playerDirection);
                animator.SetInteger("ID", 2);
                animator.SetFloat("EnemySpeed", 0);
                Debug.Log("攻撃");
            }
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            Vector3 lastPlayerPosition = other.transform.position;
            navMeshAgent.SetDestination(lastPlayerPosition);
            enemyManager.isFounding = false;
        }    
    }

    public void SetDetectPosition(RaycastHit hit)
    {
        navMeshAgent.SetDestination(hit.transform.position);
        navMeshAgent.speed = 3.5f;
    }
}
