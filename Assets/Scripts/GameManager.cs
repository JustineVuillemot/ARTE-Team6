﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    bool readyForNewLine = true;
    public Line linePrefab;
    public float startdistanceBetweenLines, breakDuration;
    public int numberOfLine;

    public List<Line> lines = new List<Line>();

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
