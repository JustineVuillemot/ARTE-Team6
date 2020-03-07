using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line : MonoBehaviour
{
    float percentage, heightPercentage, addedHeight;
    public float speed, heightInfluence, xScale,yScale;
    LineRenderer lineRenderer;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        lineRenderer = GetComponent<LineRenderer>();

         StartCoroutine(Generate());
    }

    IEnumerator Generate()
    {
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
            heightPercentage += addedHeight;

            percentage += Time.deltaTime * speed;
            lineRenderer.positionCount++;
            Vector3 newPointPosition = new Vector3(-xScale / 2 + xScale * percentage, -yScale / 2 + yScale * heightPercentage);
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, newPointPosition);
            yield return new WaitForEndOfFrame();
        }
        gameManager.OnLineFinished();
                    
    }
}
