using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    public float minZoom, maxZoom;
    GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

        
    }

    public void Focus(float percentage, Vector3 point) {
        Vector3 idlePosition = new Vector3(Camera.main.transform.position.x, gameManager.numberOfLine * gameManager.startdistanceBetweenLines, Camera.main.transform.position.z);

        GetComponent<Camera>().orthographicSize = Mathf.Lerp(minZoom, maxZoom, percentage);
        Vector3 targetPosition = Vector3.Lerp(idlePosition, new Vector3(point.x, point.y, Camera.main.transform.position.z), percentage);
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, targetPosition, Time.deltaTime * 5);

    }
}
