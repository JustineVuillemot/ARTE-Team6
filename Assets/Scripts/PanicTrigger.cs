using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanicTrigger : MonoBehaviour
{
    public LivingCreature _creature;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (enabled && collision.CompareTag("Player"))
        {
            //Debug.Log("Entered panic trigger");
            _creature.FromOkToPanic();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (enabled && collision.CompareTag("Player"))
        {
            //Debug.Log("Exited panic trigger");
            _creature.FromPanicToOk();
        }
    }
}
