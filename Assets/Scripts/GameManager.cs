using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    bool readyForNewLine = true;
    public Line linePrefab;
    public float startdistanceBetweenLines, breakDuration, focusDistance;
    public int numberOfLine;

    public List<Line> lines = new List<Line>();

    public AnimationCurve focusCurve;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown("space"))
        {
            if(readyForNewLine)
            {
                StartNewline();
            }
        }

        if (lines.Count > 1)
        {
            //Focus();
        }
    }

    void Focus()
    {
        Line currentLine = lines[lines.Count - 1];
        float distance = currentLine.DistanceToClosestPoint(lines[lines.Count - 2].lineRenderer);
        Vector3 positionOfPoint = currentLine.lineRenderer.GetPosition(currentLine.lineRenderer.positionCount - 1);

        if (distance < focusDistance)
        {
            float percentage = 1 - distance / focusDistance;
            Time.timeScale = 1 - focusCurve.Evaluate(percentage) * 0.9f;
            Camera.main.GetComponent<Zoom>().Focus(focusCurve.Evaluate(percentage), positionOfPoint);
        } else
        {
            Time.timeScale = 1;
            Camera.main.GetComponent<Zoom>().Focus(0, positionOfPoint);

        }
    }

    public void StartNewline()
    {
        Line newLine = Instantiate(linePrefab);
        lines.Add(newLine);
        newLine.startHeight = numberOfLine * startdistanceBetweenLines;
        newLine.zPos = numberOfLine;
        readyForNewLine = false;

        newLine.GetComponent<SortingGroup>().sortingOrder = -numberOfLine;

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
