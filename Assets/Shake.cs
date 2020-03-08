using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public float intensity, duration;
    public AnimationCurve shakeCurve;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartShake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    IEnumerator ShakeCoroutine()
    {
        float t = 0;
        Vector3 startPosition = transform.position;

        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            transform.position = startPosition + new Vector3(0,shakeCurve.Evaluate(p) * intensity, 0);
            yield return null;
        }
    }
}
