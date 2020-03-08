using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    float percentage, height, addedHeight;
    public float speed, heightInfluence, xScale,yScale, startHeight;
    public int zPos;
    public LineRenderer lineRenderer;
    GameManager gameManager;
    FillLine fill;
    SpawnOnLine spawnScript;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        lineRenderer = GetComponent<LineRenderer>();
        fill = GetComponent<FillLine>();
        spawnScript = GetComponent<SpawnOnLine>();

         StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
        //two points at the bottom
        lineRenderer.positionCount = 2;

        Vector3 bottomRightPos = new Vector3(-xScale / 2 + xScale * 0, -7.0f, zPos);
        lineRenderer.SetPosition(0, bottomRightPos);

        Vector3 bottomLeftPos = new Vector3(-xScale / 2 + xScale * 0, -7.0f, zPos);
        lineRenderer.SetPosition(1, bottomLeftPos);

        while (percentage < 1)
        {
            if (Input.GetKey("space"))
            {
                addedHeight = Mathf.Lerp(addedHeight, heightInfluence, 3* Time.deltaTime);
            }
            else
            {
                addedHeight = Mathf.Lerp(addedHeight, -heightInfluence, 3* Time.deltaTime);
                //heightPercentage -= Mathf.Lerp(heightPercentage, heightInfluence, Time.deltaTime); //+= heightInfluence * Time.deltaTime;

                //heightPercentage -= heightInfluence * Time.deltaTime;

            }
            height += addedHeight;

            percentage += Time.deltaTime * speed;
            lineRenderer.positionCount++;
            Vector3 newPointPosition = new Vector3(-xScale / 2 + xScale * percentage, height + startHeight, zPos);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPointPosition);

            //also updating bottom right corner position
            bottomRightPos.x = newPointPosition.x;
            lineRenderer.SetPosition(0, bottomRightPos);

            fill.CreateFilledShape(lineRenderer);

            if (CheckForCollision())
            {
                OnLineCollided();
                yield break;
            }

            yield return null;
        }

        gameManager.OnLineFinished();
        spawnScript.OnLineFinished();
    }

    void OnLineCollided()
    {
        gameManager.GameOver();
    }

    bool CheckForCollision()
    {
        float heightOfCurrentLine = lineRenderer.GetPosition(lineRenderer.positionCount - 1).y;

        //check for collision 
        //If line below exists
        if (gameManager.lines.Count > 1)
        {


            return heightOfCurrentLine <= HeightOfOtherLineAtSameDistance(gameManager.lines[gameManager.lines.Count - 2].lineRenderer) 
                || heightOfCurrentLine <= Camera.main.transform.position.y - Camera.main.orthographicSize
                || heightOfCurrentLine >= Camera.main.transform.position.y + Camera.main.orthographicSize;

        } else
        {
            return heightOfCurrentLine <= Camera.main.transform.position.y - Camera.main.orthographicSize
                || heightOfCurrentLine >= Camera.main.transform.position.y + Camera.main.orthographicSize;

        }
    }

    public float DistanceToClosestPoint(LineRenderer otherLine)
    {
        if(GetComponent<LineRenderer>().positionCount <= 0)
        {
            return 0;
        }
        Vector3 currentPoint = GetComponent<LineRenderer>().GetPosition(GetComponent<LineRenderer>().positionCount - 1);

        float closestDistance = 100;

        for (int i = 0; i < otherLine.positionCount; i++)
        {
            float distance = Vector2.Distance(currentPoint, otherLine.GetPosition(i));
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

        for(int i = 2; i < otherLine.positionCount; i++) 
        {
            if(otherLine.GetPosition(i).x >= currentPoint.x)
            {
                return otherLine.GetPosition(i).y;
            } 
        } 

        return -1000;
    }


}
