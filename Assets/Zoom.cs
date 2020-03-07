using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    public float zoomScale, slowmotionFactor, zoomSpeed;
    float standardZoom;
    GameManager gameManager;
    bool zoomIn;

    // Start is called before the first frame update
    void Start()
    {
        standardZoom = GetComponent<Camera>().orthographicSize;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(0, gameManager.numberOfLine * gameManager.startdistanceBetweenLines, Camera.main.transform.position.z), Time.deltaTime * 2);
        if(zoomIn)
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, slowmotionFactor, Time.deltaTime * zoomSpeed);
            GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, zoomScale, Time.deltaTime * zoomSpeed);
            LineRenderer currentLine = gameManager.lines[gameManager.numberOfLine-1].lineRenderer;
            Vector3 positionOfPlayer = currentLine.GetPosition(currentLine.positionCount-1);
            transform.position = Vector3.Lerp(transform.position, new Vector3(positionOfPlayer.x, positionOfPlayer.y, transform.position.z), Time.deltaTime * zoomSpeed);
        } else
        {
            Time.timeScale = Mathf.Lerp(Time.timeScale, 1, Time.deltaTime * zoomSpeed);
             GetComponent<Camera>().orthographicSize = Mathf.Lerp(GetComponent<Camera>().orthographicSize, standardZoom, Time.deltaTime * zoomSpeed);

            transform.position = Vector3.Lerp(transform.position, new Vector3(0,0,Camera.main.transform.position.z), Time.deltaTime * zoomSpeed);
        }

    }

    public void Focus() {
        /*
        Vector3 idlePosition = new Vector3(0, gameManager.numberOfLine * gameManager.startdistanceBetweenLines, Camera.main.transform.position.z);

        GetComponent<Camera>().orthographicSize = Mathf.Lerp(minZoom, maxZoom, percentage);
        Vector3 targetPosition = Vector3.Lerp(idlePosition, new Vector3(point.x, point.y, Camera.main.transform.position.z), percentage);
        Camera.main.transform.position = targetPosition;
        */
        zoomIn = true;
    }

    public void Unfocus()
    {
        zoomIn = false;
    }

    
}
