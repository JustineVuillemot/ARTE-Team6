using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    bool readyForNewLine = true;
    public Line linePrefab;
    public float startdistanceBetweenLines, breakDuration, focusArea;
    public int numberOfLine;

    public List<Line> lines = new List<Line>();

    public AnimationCurve focusCurve;

    bool gameOver;

    public Player player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("space"))
        {
            if (gameOver)
            {
                Application.LoadLevel(0);
            }

            if (readyForNewLine)
            {
                StartNewline();
            }
        }

        if (lines.Count > 1)
        {
            //Focus();
        }

        if (lines.Count > 0)
        {
            Line currentLine = lines[lines.Count - 1];
            Vector3 positionOfPoint = currentLine.GetComponent<LineRenderer>().GetPosition(currentLine.GetComponent<LineRenderer>().positionCount - 1);
            player.UpdatePosition(positionOfPoint);
        }
    }

    void Focus()
    {
        
        Line currentLine = lines[lines.Count - 1];
        float distance = currentLine.DistanceToClosestPoint(lines[lines.Count - 2].lineRenderer);
        Vector3 positionOfPoint = currentLine.GetComponent<LineRenderer>().GetPosition(currentLine.GetComponent<LineRenderer>().positionCount - 1);

        if (distance < focusArea)
        {

            /*float percentage = 1 - distance / focusDistance;
            Time.timeScale = 1 - focusCurve.Evaluate(percentage) * timeScale;
            Camera.main.GetComponent<Zoom>().Focus(focusCurve.Evaluate(percentage), positionOfPoint);
    */

            Camera.main.GetComponent<Zoom>().Focus();

        }
        else
        {
            //   Time.timeScale = 1;
            // Camera.main.GetComponent<Zoom>().Focus(0, positionOfPoint);
            Camera.main.GetComponent<Zoom>().Unfocus();

        }


    }

    public void StartNewline()
    {
        Line newLine = Instantiate(linePrefab);
        lines.Add(newLine);
        newLine.startHeight = numberOfLine * startdistanceBetweenLines;
        newLine.zPos = numberOfLine;
        readyForNewLine = false;
        player.gameObject.SetActive(true);

        newLine.GetComponent<SortingGroup>().sortingOrder = -numberOfLine;

        numberOfLine++;

    }

    public void OnLineFinished()
    {
        StartCoroutine(Break());
        player.gameObject.SetActive(false);
    }

    IEnumerator Break()
    {
        yield return new WaitForSeconds(breakDuration);
        readyForNewLine = true;

    }

    public void GameOver()
    {
        Camera.main.GetComponent<Shake>().StartShake();
        StartCoroutine(ChangeColorOfLines());
        gameOver = true;
        player.gameObject.SetActive(false);

    }

    IEnumerator ChangeColorOfLines()
    {
        float t = 0;
        float p = 0;
        float duration = 0.5f;
        Color backgroundColor = Camera.main.backgroundColor;
        List<Color> fromColors = new List<Color>();
        foreach (Line line in lines)
        {
            fromColors.Add(line.GetComponent<FillLine>().GetColor());
        }

        while (t < duration)
        {
            t += Time.deltaTime;
            p = t / duration;

            
            for (int i = 0; i < lines.Count; i++)
            {
                lines[i].GetComponent<FillLine>().SetColor(Color.Lerp(fromColors[i], Color.white, p));
            }
            Camera.main.backgroundColor = Color.Lerp(backgroundColor, Color.black, p);

            yield return null;
        }
    }

    
}
