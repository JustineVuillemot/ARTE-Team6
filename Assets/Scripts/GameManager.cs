using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    bool lineIsGenerating;
    public Line linePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("space"))
        {
            if(!lineIsGenerating)
            {
                StartNewline();
            }
        }       
    }

    public void StartNewline()
    {
        Instantiate(linePrefab);
        lineIsGenerating = true;
    }

    public void OnLineFinished()
    {
        lineIsGenerating = false;
    }
}
