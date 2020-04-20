using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelController : MonoBehaviour
{
    bool begun;
    Coroutine coroutine;

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

    public abstract IEnumerator LevelRoutine();
}
