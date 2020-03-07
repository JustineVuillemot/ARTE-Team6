using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    public float minZoom, maxZoom, maxDistanceToOtherLine;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 idlePosition = new Vector3(Camera.main.transform.position.x, gameManager.numberOfLine * gameManager.startdistanceBetweenLines, Camera.main.transform.position.z);

        if (gameManager.lines.Count > 1)
        {
            Line currentLine = gameManager.lines[gameManager.lines.Count - 1];
            float distance = currentLine.DistanceToClosestPoint(gameManager.lines[gameManager.lines.Count - 2].lineRenderer);
            float percentageOfZoom = 1 - distance / maxDistanceToOtherLine;
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(minZoom, maxZoom, percentageOfZoom);

            Vector3 positionOfPoint = currentLine.lineRenderer.GetPosition(currentLine.lineRenderer.positionCount - 1);
            Vector3 targetPosition = Vector3.Lerp(idlePosition, new Vector3(positionOfPoint.x, positionOfPoint.y, Camera.main.transform.position.z), percentageOfZoom);
            Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, Time.deltaTime * 2);

        }
        else
        {
            Camera.main.transform.position = idlePosition;

        }
    }
}
