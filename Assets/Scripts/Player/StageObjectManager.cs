using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageObjectManager : MonoBehaviour
{
    LineRenderer  lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public GameObject GetClosestObject()
    {
        GameObject[] objs;
        GameObject closestObject = null;
        objs = GameObject.FindGameObjectsWithTag("Obstacle");
        float distance = Mathf.Infinity;
        Vector3 playerPosition = transform.position;

        foreach (GameObject obj in objs)
        {
            Vector3 diff = obj.transform.position - playerPosition;
            float currentDistance = diff.sqrMagnitude;
            if (currentDistance < distance)
            {
                closestObject = obj;
                distance = currentDistance;
                var positions = new Vector3[]
                {
                    transform.position,               // 開始点
                    closestObject.transform.position,       // 終了点
                };
                lineRenderer.SetPositions(positions);
            }
        }

        return closestObject;
    }
}
