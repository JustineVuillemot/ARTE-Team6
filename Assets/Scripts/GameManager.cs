using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    bool readyForNewLine;
    public Line linePrefab;
    public float breakDuration, focusArea;
    public int numberOfLine;
    public float distanceBetweenLines, startPositionOfTitle;
    public float normalSoundVolume;

    public List<Line> lines = new List<Line>();

    public AnimationCurve focusCurve;

    bool gameOver;

    public Player player;

    [Header("Creatures")]
    [SerializeField]
    public Creature2DArray[] creatures; 

    [Header("Sound")]
    public AudioClip[] normalSounds, ambientSounds;
    public AudioSource normalAudiosource;
    public AudioSource ambientAudiosource;
    public float fadingDuration;
    Coroutine fadeOutCoroutine;
    public AnimationCurve fadingCurve;

    public SpriteRenderer titlePrefab;
    public Sprite[] titles;
    public Sprite gameTitle;
    public GameObject tutoAnim;
    public GameObject victoryAnim;
    public GameObject victoryButton;

    public Color[] colors;

    private float lineXScale = 18;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GameStart());

        float camHalfWidth = Camera.main.orthographicSize * Screen.width / Screen.height;
        lineXScale = (camHalfWidth + 0.1f) * 2;
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown("space"))
        {
            if (gameOver && readyForNewLine)
            {
                SceneManager.LoadScene(0);
            }

            if (readyForNewLine)
            {
                if (numberOfLine < 7)
                {
                    StartNewline();
                }
            }
        }

        if(!gameOver && readyForNewLine && numberOfLine == 7)
        {
            Victory();
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
        
        newLine.startHeight = numberOfLine * distanceBetweenLines - Camera.main.orthographicSize + distanceBetweenLines;
        newLine.xScale = lineXScale;
        newLine.zPos = numberOfLine;
        readyForNewLine = false;

        player.gameObject.SetActive(true);

        newLine.GetComponent<FillLine>().SetColor(colors[numberOfLine]);

        if(fadeOutCoroutine != null)
        StopCoroutine(fadeOutCoroutine);
        normalAudiosource.volume = normalSoundVolume;
        normalAudiosource.clip = normalSounds[numberOfLine];
        normalAudiosource.Play();

        ambientAudiosource.clip = ambientSounds[numberOfLine];
        ambientAudiosource.Play();

        newLine.GetComponent<SortingGroup>().sortingOrder = -numberOfLine;
        newLine.GetComponent<SpawnOnLine>().prefabs = creatures[numberOfLine].array;

        numberOfLine++;

    }

    public void OnLineFinished()
    {
       
        player.gameObject.SetActive(false);
        fadeOutCoroutine = StartCoroutine(FadoutSound());

        StartCoroutine(ShowTitle(titles[7 - numberOfLine], 0.5f));
    }

    private IEnumerator GameStart()
    {
        yield return ShowTitle(gameTitle, 1.0f);

        readyForNewLine = false;

        tutoAnim.SetActive(true);

        yield return new WaitForSeconds(2.0f);

        tutoAnim.SetActive(false);

        readyForNewLine = true;
    }

   
    public void GameOver()
    {
        Camera.main.GetComponent<Shake>().StartShake();
        StartCoroutine(ChangeColorOfLines());
        gameOver = true;
        readyForNewLine = false;
        player.gameObject.SetActive(false);
        StartCoroutine(FadoutSound());
        ambientAudiosource.Stop();

        StartCoroutine(ShowTitle(titles[7 - numberOfLine], 0.5f));
    }

    public void Victory()
    {
        //Debug.Log("Victory !");

        victoryAnim.SetActive(true);
        victoryButton.SetActive(true);

        //to be able to replay by hitting space
        gameOver = true;
    }

    IEnumerator ChangeColorOfLines()
    {
        float t = 0;
        float p = 0;
        float duration = 0.5f;
        Color backgroundColor = Camera.main.backgroundColor;
        List<Color> fromColors = new List<Color>();
        List<Color> toColors = new List<Color>();
        foreach (Line line in lines)
        {
            Color c = line.GetComponent<FillLine>().GetColor();
            fromColors.Add(c);

            float h, s, v;
            Color.RGBToHSV(c, out h, out s, out v);
            s = 0.0f;
            toColors.Add(Color.HSVToRGB(h, s, v));
        }

        while (t < duration)
        {
            t += Time.deltaTime;
            p = t / duration;

            
            for (int i = 0; i < lines.Count; i++)
            {
                Color currentColor = Color.Lerp(fromColors[i], toColors[i], p);
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
            normalAudiosource.volume = Mathf.Lerp(normalSoundVolume,0, fadingCurve.Evaluate(p));
            yield return null; 
        }
        normalAudiosource.Stop();
        normalAudiosource.volume = 0;
    }
    

    IEnumerator ShowTitle(Sprite title, float scale)
    {
        SpriteRenderer newTitle = Instantiate(titlePrefab, new Vector3(0,0,0), new Quaternion());
        newTitle.sprite = title;
        newTitle.transform.localScale = new Vector3(scale, scale, 1);

        while (newTitle.color.a < 1)
        {
            newTitle.color += new Color(0, 0, 0, 2 * Time.deltaTime);
            //newTitle.transform.position = Vector3.Lerp(newTitle.transform.position, new Vector3(), newTitle.color.a);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        while (newTitle.color.a > 0)
        {
            newTitle.color -= new Color(0, 0, 0, 2* Time.deltaTime);
            //newTitle.transform.position = Vector3.Lerp(newTitle.transform.position, new Vector3(-startPositionOfTitle,0), 1 - newTitle.color.a);

            yield return null;
        }

        readyForNewLine = true;
    }
}

[System.Serializable]
public class Creature2DArray
{
    public LivingCreature[] array;
}
