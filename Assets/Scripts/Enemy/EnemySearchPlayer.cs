using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System;

public class EnemySearchPlayer : MonoBehaviour
{
    EnemyManager enemyManager;
    PlayerManager playerManager;
    PlayerLocomotion playerLocomotion;

    NavMeshAgent navMeshAgent;
    Animator animator;
    private AudioSource audioSource;

    [SerializeField]
    private SphereCollider searchArea; 
    [SerializeField]
    private float searchAngle = 120f;
    public LayerMask obstacleLayer;
    public bool isDetecting = false;
    private float attackInterval;

    private void Start()
    {
        enemyManager = GetComponent<EnemyManager>();
        playerLocomotion = GameObject.Find("Player").GetComponent<PlayerLocomotion>();
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }
    
    private void OnTriggerEnter(Collider other) 
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y += 1.0f;
        if(other.CompareTag("Player") && navMeshAgent.enabled == true)
        {
            Vector3 playerDirection = other.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, playerDirection);

            if(Physics.Raycast(rayCastOrigin, playerDirection, out hit, searchArea.radius, obstacleLayer))
            {
                if(hit.collider.CompareTag("Obstacle")) return;
            }

            enemyManager.ChangeToChasingAndAttackingState();
        } 
    }

    private void OnTriggerStay(Collider other)
    {
        RaycastHit hit;
        NavMeshHit navHit;
        Vector3 rayCastOrigin = transform.position;
        rayCastOrigin.y += 1.0f;
        if(other.CompareTag("Player") && navMeshAgent.enabled == true)
        {
            isDetecting = true;
            Vector3 playerDirection = other.transform.position - transform.position;
            float angle = Vector3.Angle(transform.forward, playerDirection);

            if(Physics.Raycast(rayCastOrigin, playerDirection, out hit, searchArea.radius, obstacleLayer))
            {
                if(hit.collider.CompareTag("Obstacle")) return;
            }

            double eachDistance = Math.Sqrt(Math.Pow(playerDirection.x, 2) + Math.Pow(playerDirection.z, 2));

            if(angle <= searchAngle || eachDistance < 1.0f)
            {
                enemyManager.isFounding = true;
                transform.rotation = Quaternion.LookRotation(playerDirection);
                animator.SetInteger("ID", 0);

                if(NavMesh.SamplePosition(other.transform.position, out navHit, 1.0f, NavMesh.AllAreas) && playerManager.disableSamplePosition)
                {
                    other.transform.position = navHit.position;
                }
                navMeshAgent.SetDestination(other.transform.position);
                navMeshAgent.speed = 3.5f;
            }

            if(angle <= searchAngle && eachDistance < (searchArea.radius / 2.0f))
            { 
                attackInterval += Time.deltaTime;
                enemyManager.isFounding = true;
                transform.rotation = Quaternion.LookRotation(playerDirection);
                animator.SetInteger("ID", 0);
                if(NavMesh.SamplePosition(other.transform.position, out navHit, 1.0f, NavMesh.AllAreas) && playerManager.disableSamplePosition)
                {
                    other.transform.position = navHit.position;
                }
                navMeshAgent.SetDestination(other.transform.position);

                if(attackInterval < 3.0f) return;
                playerManager.DecreaseHP();
                attackInterval = 0;
                audioSource.PlayOneShot(audioSource.clip);
            }
        }    
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player") && navMeshAgent.enabled == true)
        {
            enemyManager.isFounding = false;
            isDetecting = false;
        }    
    }

    public void SetDetectPosition(RaycastHit hit)
    {
        navMeshAgent.SetDestination(hit.transform.position);
        navMeshAgent.speed = 3.5f;
        enemyManager.isFounding = true;
        enemyManager.ChangeToChasingAndAttackingState();
    }
}
