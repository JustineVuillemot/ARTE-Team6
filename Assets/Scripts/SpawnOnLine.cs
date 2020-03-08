using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnOnLine : MonoBehaviour
{
    public LivingCreature prefab;

    private LineRenderer _line;
    private FillLine _fill;
    private int _currentSpawningPos = 2;

    private bool _isLineFinished = false;
    private int _minDistToBorder = 10;

    private List<LivingCreature> _creaturesOnLine = new List<LivingCreature>();

    // Start is called before the first frame update
    void Start()
    {
        _line = GetComponent<LineRenderer>();
        _fill = GetComponent<FillLine>();

        float randTime = Random.Range(0.0f, 1.0f);
        Invoke("Spawn", randTime);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void Spawn()
    {
        float randTime;

        //might be a pb at first time so we invoke for after
        if (_line == null || _line.positionCount <= _minDistToBorder)
        {
            randTime = Random.Range(0.0f, 1.0f);
            Invoke("Spawn", randTime);
            return;
        }

        //near the end of the line so we just stop the spawn
        if(_isLineFinished && (_line.positionCount - _currentSpawningPos) <= _minDistToBorder)
        {
            return;
        }

        _currentSpawningPos = Random.Range(_currentSpawningPos, _line.positionCount);
        LivingCreature creature = Instantiate(prefab, transform);
        creature.InitCreature(_fill.GetColor(), _line, _currentSpawningPos);

        _creaturesOnLine.Add(creature);

        randTime = Random.Range(0.0f, 1.0f);
        Invoke("Spawn", randTime);
    }

    public void OnLineFinished()
    {
        _isLineFinished = true;

        foreach(LivingCreature creature in _creaturesOnLine)
        {
            creature.EnableColliders(true);
        }
    }

    public void SetCreaturesColor(Color c)
    {
        foreach (LivingCreature creature in _creaturesOnLine)
        {
            creature.SetColor(c);
        }
    }
}
