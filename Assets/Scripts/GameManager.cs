using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool readyForNewLine = true;
    public Line linePrefab;
    public float startdistanceBetweenLines, breakDuration;
    int numberOfLine;

    public List<Line> lines = new List<Line>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, new Vector3(Camera.main.transform.position.x, numberOfLine * startdistanceBetweenLines, Camera.main.transform.position.z), Time.deltaTime * 2);

        if(Input.GetKeyDown("space"))
        {
            if(readyForNewLine)
            {
                StartNewline();
            }
        }       
    }

    public void StartNewline()
    {
        Line newLine = Instantiate(linePrefab);
        lines.Add(newLine);
        newLine.startHeight = numberOfLine * startdistanceBetweenLines;
        readyForNewLine = false;
        numberOfLine++;

    }

    public void OnLineFinished()
    {
        StartCoroutine(Break());
    }

    IEnumerator Break()
    {
        yield return new WaitForSeconds(breakDuration);
        readyForNewLine = true;

    }
}
