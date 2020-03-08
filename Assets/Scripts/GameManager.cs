using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    bool readyForNewLine = true;
    public Line linePrefab;
    public float breakDuration, focusArea;
    public int numberOfLine;
    public float distanceBetweenLines;


    public List<Line> lines = new List<Line>();

    public AnimationCurve focusCurve;

    bool gameOver;

    public Player player;

    [Header("Sound")]
    public AudioClip[] ambientSounds;
    AudioSource audioSource;
    public float fadingDuration;
    Coroutine fadeOutCoroutine;
    public AnimationCurve fadingCurve;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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

            if (currentLine.GetComponent<LineRenderer>().positionCount > 0)
            {
                Vector3 positionOfPoint = currentLine.GetComponent<LineRenderer>().GetPosition(currentLine.GetComponent<LineRenderer>().positionCount - 1);
                player.UpdatePosition(positionOfPoint);
            }
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
        
        newLine.startHeight = //Camera.main.transform.position.y - Camera.main.orthographicSize + ((Camera.main.orthographicSize * 2) / (numberOfLine+1));
        numberOfLine * distanceBetweenLines - Camera.main.orthographicSize + distanceBetweenLines;
        newLine.zPos = numberOfLine;
        readyForNewLine = false;
        player.gameObject.SetActive(true);

        if(fadeOutCoroutine != null)
        StopCoroutine(fadeOutCoroutine);
        audioSource.volume = 1;
        audioSource.clip = ambientSounds[numberOfLine];
        audioSource.Play();

        newLine.GetComponent<SortingGroup>().sortingOrder = -numberOfLine;

        numberOfLine++;

    }

    public void OnLineFinished()
    {
        StartCoroutine(Break());
        player.gameObject.SetActive(false);
        fadeOutCoroutine = StartCoroutine(FadoutSound());
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
                Color currentColor = Color.Lerp(fromColors[i], Color.white, p);
                lines[i].GetComponent<FillLine>().SetColor(currentColor);
                lines[i].GetComponent<SpawnOnLine>().SetCreaturesColor(currentColor);
            }
            Camera.main.backgroundColor = Color.Lerp(backgroundColor, Color.black, p);

            yield return null;
        }
    }

    IEnumerator FadoutSound()
    {
        float t = 0; 

        while(t < fadingDuration)
        {
            t += Time.deltaTime;
            float p = t / fadingDuration;
            audioSource.volume = Mathf.Lerp(1,0, fadingCurve.Evaluate(p));
            yield return null; 
        }
        audioSource.Stop();
        audioSource.volume = 0;
    }
}
