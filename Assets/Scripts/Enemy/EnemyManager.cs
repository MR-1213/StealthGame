using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;
using System.Linq;
using DG.Tweening;

public class EnemyManager : MonoBehaviour
{
    PlayerManager playerManager;
    
    public Transform point_1;
    public Transform point_2;
    public Transform point_3;
    public Transform point_4;
    public Transform point_Toilet;
    public Transform point_Player;

    NavMeshAgent navMeshAgent;
    Animator animator;
    public Text text;

    State currentState = State.MoveToDestination;
    State targetState = State.DoNothing;


    public bool isChasing;
    public bool isAttacking;
    public bool isExecution;
    bool stateEnter = false;
    float stateTime = 0;
    int lastAction = 1;

    Dictionary<Desire, float> desireDictionary = new Dictionary<Desire, float>();

    public enum State
    {
        MoveToDestination,
        GoToToilet,
        ChasePlayer,
        AttackPlayer,
        DoNothing,
    }

    enum Animation_State
    {
        Patrol = 0,
        Toilet = 1,
        Attack = 2,
    }

    enum Desire
    {
        Patrol,
        Toilet,
        Chase,
        Attack,
    }

    private void ChangeState(State newState)
    {
        currentState = newState;
        stateEnter = true;
        stateTime = 0;
        Debug.Log(currentState.ToString());
    }

    private void ChangeAnimationState(Animation_State state)
    {
        animator.SetInteger("ID", (int)state);
    }

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        playerManager = GameObject.Find("Player").GetComponent<PlayerManager>();
        foreach (Desire desire in Enum.GetValues(typeof(Desire)))
        {
            desireDictionary.Add(desire, 0f);
        } 

        ChangeState(State.MoveToDestination);   
    }
    
    private void Update() 
    {
        stateTime += Time.deltaTime;
        float speed = navMeshAgent.velocity.magnitude;

        animator.SetFloat("EnemySpeed", speed);

        if(currentState != State.GoToToilet)
        {
            desireDictionary[Desire.Toilet] += Time.deltaTime / 60.0f;
        }

        if(!isExecution)
        {
            if(isChasing)
            {
                desireDictionary[Desire.Chase] = 10.0f;
                ChangeState(State.MoveToDestination);        
            }
            else
            {
                desireDictionary[Desire.Chase] = 0;
                desireDictionary[Desire.Patrol] = 1.0f;
            }

            if(isAttacking)
            {
                desireDictionary[Desire.Attack] = 10.0f;
                ChangeState(State.MoveToDestination);
            }
            else
            {
                desireDictionary[Desire.Attack] = 0;
                desireDictionary[Desire.Patrol] = 1.0f;
            }
        }

        IOrderedEnumerable<KeyValuePair<Desire, float>> sortedDesire = desireDictionary.OrderByDescending(i => i.Value);

        text.text ="";
        foreach(KeyValuePair<Desire, float> sortedDesireElement in sortedDesire)
        {
            text.text += sortedDesireElement.Key.ToString() + ":" + sortedDesireElement.Value + "\n";
        }
        text.text += navMeshAgent.speed.ToString() + "\n";

        switch (currentState)
        {
            case State.MoveToDestination: {
                
                if(stateEnter)
                {
                    var topDesireElement = sortedDesire.ElementAt(0);

                    if(topDesireElement.Value >= 1.0f)
                    {
                        switch(topDesireElement.Key)
                        {
                            case Desire.Patrol:
                                navMeshAgent.SetDestination(GetPatrolPoint(lastAction));
                                navMeshAgent.speed = 2.0f;
                                targetState = State.DoNothing;
                                break;
                            case Desire.Toilet:
                                navMeshAgent.SetDestination(point_Toilet.position);
                                navMeshAgent.speed = 2.5f;
                                targetState = State.GoToToilet;
                                break;
                            case Desire.Chase:
                                navMeshAgent.SetDestination(point_Player.position);
                                navMeshAgent.speed = 3.5f;
                                targetState = State.ChasePlayer;
                                ChangeState(targetState);
                                isExecution = true;
                                break;
                            case Desire.Attack:
                                navMeshAgent.speed = 0;
                                targetState = State.AttackPlayer;
                                ChangeState(targetState);
                                isExecution = true;
                                break;
                        }
                    }
                }

                ChangeAnimationState(Animation_State.Patrol);
                if(navMeshAgent.remainingDistance <= 0.01f && !navMeshAgent.pathPending)
                {
                    ChangeState(targetState);
                    return;
                }

                return;
            }

            case State.DoNothing: {

                if(stateTime >= 5.0f)
                {
                    if(lastAction < 4)
                    {
                        lastAction++;
                    }
                    else
                    {
                        lastAction = 1;
                    }
                    ChangeState(State.MoveToDestination);
                    return;
                }
                
                return;
            }

            case State.GoToToilet: {
                
                if(stateEnter)
                {
                    navMeshAgent.enabled = false;
                    ChangeAnimationState(Animation_State.Patrol);
                    transform.position = point_Toilet.position;
                    transform.rotation = point_Toilet.rotation;
                }

                if(stateTime >= 7.0f)
                {
                    navMeshAgent.enabled = true;
                    desireDictionary[Desire.Toilet] = 0;
                    ChangeState(State.MoveToDestination);
                    return;
                }

                return;
            }

            case State.ChasePlayer: {

                if(stateEnter)
                {
                    navMeshAgent.SetDestination(point_Player.position);
                    navMeshAgent.speed = 3.5f;
                    ChangeAnimationState(Animation_State.Patrol);
                }
                
                if(navMeshAgent.remainingDistance <= 0.01f && !navMeshAgent.pathPending)
                {
                    desireDictionary[Desire.Chase] = 0;
                    ChangeState(State.MoveToDestination);
                    return;
                }
                else if(!isChasing && isAttacking)
                {
                    desireDictionary[Desire.Chase] = 0;
                    desireDictionary[Desire.Attack] = 10.0f;
                    ChangeState(State.MoveToDestination);
                    isExecution = false;
                    return;
                }

                return;
            }

            case State.AttackPlayer: {

                if(stateEnter)
                {
                    ChangeAnimationState(Animation_State.Attack);
                }

                if(!isAttacking && !isChasing)
                {
                    desireDictionary[Desire.Attack] = 0;
                    ChangeState(State.MoveToDestination);
                    return;
                }
                else if(!isAttacking && isChasing)
                {
                    desireDictionary[Desire.Attack] = 0;
                    desireDictionary[Desire.Chase] = 10.0f;
                    ChangeState(State.MoveToDestination);
                    isExecution = false;
                    return;
                }

                return;
            }
        }       
    }

    private Vector3 GetPatrolPoint(int lastAction)
    {
        switch(lastAction)
        {
            case 1:
                return point_1.position;
            case 2:
                return point_2.position;
            case 3:
                return point_3.position;
            case 4:
                return point_4.position;
        }
        return point_1.position;
    }

    public void MoveToLastPlayerPosition(Transform player)
    {
        navMeshAgent.SetDestination(player.position);
    }
    private void LateUpdate() 
    {
        if(stateTime != 0)
        {
            stateEnter = false;
        }    
    }
}
