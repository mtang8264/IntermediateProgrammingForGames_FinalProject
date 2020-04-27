using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelController : MonoBehaviour
{
    protected bool begun;
    protected Coroutine coroutine;

    void Start()
    {
        
    }

    void Update()
    {
        if(!begun)
        {
            coroutine = StartCoroutine(LevelRoutine());
            begun = true;
        }
    }

    void Clear()
    {
        coroutine = null;
    }

    public virtual IEnumerator LevelRoutine()
    {
        yield return null;
    }
}
