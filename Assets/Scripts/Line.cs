﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    float percentage, height, addedHeight;
    public float speed, heightInfluence, xScale,yScale, startHeight;
    public LineRenderer lineRenderer;
    GameManager gameManager;
    FillLine fill;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        lineRenderer = GetComponent<LineRenderer>();
        fill = GetComponent<FillLine>();

         StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        //two points at the bottom
        lineRenderer.positionCount = 2;

        Vector3 bottomRightPos = new Vector3(-xScale / 2 + xScale * 0, -5.0f);
        lineRenderer.SetPosition(0, bottomRightPos);

        Vector3 bottomLeftPos = new Vector3(-xScale / 2 + xScale * 0, -5.0f);
        lineRenderer.SetPosition(1, bottomLeftPos);

        while (percentage < 1)
        {
            if (Input.GetKey("space"))
            {
                addedHeight = Mathf.Lerp(addedHeight, heightInfluence, Time.deltaTime);
            }
            else
            {
                addedHeight = Mathf.Lerp(addedHeight, -heightInfluence, Time.deltaTime);
                //heightPercentage -= Mathf.Lerp(heightPercentage, heightInfluence, Time.deltaTime); //+= heightInfluence * Time.deltaTime;

                //heightPercentage -= heightInfluence * Time.deltaTime;

            }
            height += addedHeight;

            percentage += Time.deltaTime * speed;
            lineRenderer.positionCount++;
            Vector3 newPointPosition = new Vector3(-xScale / 2 + xScale * percentage, -yScale / 2 + height + startHeight);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPointPosition);

            //also updating bottom right corner position
            bottomRightPos.x = newPointPosition.x;
            lineRenderer.SetPosition(0, bottomRightPos);

            fill.CreateFilledShape(lineRenderer);

            if (CheckForCollision())
            {
                OnLineCollided();
            }

            yield return null;
        }

        gameManager.OnLineFinished();
    }

    void OnLineCollided()
    {
        Debug.Log("lineCollided");
    }

    bool CheckForCollision()
    {
        float heightOfCurrentLine = lineRenderer.GetPosition(lineRenderer.positionCount - 1).y;

        //check for collision 

        //If line below exists
        if (gameManager.lines.Count > 1)
        {
            Debug.Log(DistanceToClosestPoint(gameManager.lines[gameManager.lines.Count - 2].lineRenderer));

            return heightOfCurrentLine <= HeightOfOtherLineAtSameDistance(gameManager.lines[gameManager.lines.Count - 2].lineRenderer) ;

        }
        return false;
    }

    public float DistanceToClosestPoint(LineRenderer otherLine)
    {
        Vector3 currentPoint = lineRenderer.GetPosition(lineRenderer.positionCount - 1);

        float closestDistance = 100;

        for (int i = 0; i < otherLine.positionCount; i++)
        {
            float distance = Vector3.Distance(currentPoint, otherLine.GetPosition(i));
            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }
        return closestDistance;
    }


    float HeightOfOtherLineAtSameDistance(LineRenderer otherLine)
    {
        Vector3 currentPoint = lineRenderer.GetPosition(lineRenderer.positionCount - 1);

        for(int i = 0; i < otherLine.positionCount; i++) 
        {
            if(otherLine.GetPosition(i).x >= currentPoint.x)
            {
                return otherLine.GetPosition(i).y;
            } 
        }

        return 0;
    }


}
