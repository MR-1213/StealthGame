using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using UnityEngine.AI;
using DG.Tweening;

public class EnemyManager : MonoBehaviour
{
    
    public Transform point_1;
    public Transform point_2;
    public Transform point_3;
    public Transform point_4;
    public Transform point_Toilet;
    public Transform point_Player;

    NavMeshAgent navMeshAgent;
    Animator animator;

    State currentState = State.MoveToDestination;
    State targetState = State.DoNothing;

    public bool isMissing = false;
    public bool isAttacking = false;
    bool stateEnter = false;
    float stateTime = 0;
    int lastAction = 1;

    Dictionary<Desire, float> desireDictionary = new Dictionary<Desire, float>();

    enum State
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

        IOrderedEnumerable<KeyValuePair<Desire, float>> sortedDesire = desireDictionary.OrderByDescending(i => i.Value);

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
                            case Desire.Toilet:
                                navMeshAgent.SetDestination(point_Toilet.position);
                                navMeshAgent.speed = 2.5f;
                                targetState = State.GoToToilet;
                                break;
                            case Desire.Chase:
                                navMeshAgent.SetDestination(point_Player.position);
                                navMeshAgent.speed = 3.5f;
                                targetState = State.ChasePlayer;
                                break;
                            case Desire.Attack:
                                navMeshAgent.speed = 0;
                                targetState = State.AttackPlayer;
                                break;
                        }
                    }
                    else
                    {
                        switch(lastAction)
                        {
                            case 1:
                                navMeshAgent.SetDestination(point_1.position);
                                break;
                            case 2:
                                navMeshAgent.SetDestination(point_2.position);
                                break;
                            case 3:
                                navMeshAgent.SetDestination(point_3.position);
                                break;
                            case 4:
                                navMeshAgent.SetDestination(point_4.position);
                                break;
                        }
                        navMeshAgent.speed = 2.0f;
                        targetState = State.DoNothing;
                    }
                }

                ChangeAnimationState(Animation_State.Patrol);
                if(navmeshAgent.remainingDistance <= 0.01f && !navmeshAgent.pathPending)
                {
                    ChangeState(targetState);
                    return;
                }

                return;
            }

            case State.DoNothing: {

                if(stateTime >= 5.0f)
                {
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
                    transform.rotation = point_Toilet.rotaiton;
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
                
                if(isAttacking)
                {
                    return;//プレイヤーが攻撃範囲に入ったらAttackPlayerステートへ
                }

                if(isMissing)
                {
                    return;//プレイヤーを見失ったとき
                }
                return;
                //ただ壁の陰に入っただけの時はどうするか考える！
            }

            case State.AttackPlayer: {

                if(!isAttacking)
                {
                    return;//プレイヤーが攻撃範囲から離れたときはChasePlayerステートへ
                }

                return;
            }
        }       
    }

    private void LateUpdate() 
    {
        if(stateTime != 0)
        {
            stateEnter = false;
        }    
    }
}
