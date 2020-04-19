using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 100f)]
    float oxygen;

    [Header("Lungs")]
    [SerializeField]
    bool lungsActive;   // This determines if the lungs are used in this level
    GameObject lungs;   // The game object reference for the lungs
    enum LungState {EMPTY, INHALING, HOLDING, EXHALING };   // The possible states the lungs can be in
    LungState lungState;
    [Range(0f,1f)]
    float lungCapacity; // The capacity of the lungs. This is like how much they are between empty(0f) and full(1f)
    [SerializeField]
    KeyCode inhaleKey, exhaleKey;   // The keys used to inhale and exhale
    [SerializeField]
    float inhaleSpeed, exhaleSpeed; // The rate at which the lungs inhale and exhale
    [SerializeField]
    float emptyLungSize;    // The scale that the lungs reach when they are empty. We assume they should reach 1f scale while full.

    void Awake()
    {
        // If the level uses the lungs set up for the lungs
        if (lungsActive)
        {
            lungs = GameObject.FindWithTag("Lungs");
            lungState = LungState.EMPTY;
            lungCapacity = 0f;
        }
    }

    void Update()
    {
        if(lungsActive)
        {
            DrawLungs();
            UpdateLungState();
        }
    }

    #region Lung_Functions
    /// <summary>
    /// This function updates the visual display for the lungs.
    /// The size of the lungs is just a lerp based on the current lung capacity.
    /// </summary>
    void DrawLungs()
    {
        float s = Mathf.Lerp(emptyLungSize, 1f, lungCapacity);
        Vector3 scale = new Vector3(s, s, s);
        lungs.transform.localScale = scale;
    }
    /// <summary>
    /// This function updates the lungState based on which key is pressed.
    /// </summary>
    void UpdateLungState()
    {
        switch (lungState)
        {
            case LungState.EMPTY:   // If the lungs are empty
                if (Input.GetKeyDown(inhaleKey))    // The lungs will begin to inhal when you press the inhale key
                {
                    lungState = LungState.INHALING;
                }
                break;
            case LungState.INHALING:    // If the lungs are inhaling
                lungCapacity += inhaleSpeed * Time.deltaTime;   // Lung capacity increases at a constant rate while inhaling
                if (Input.GetKey(inhaleKey) == false || lungCapacity >= 1f) // If the player releases the inhale key or the max lung capacity is reached the lungs begin to hold
                {
                    lungState = LungState.HOLDING;
                }
                break;
            case LungState.HOLDING: // If the lungs are holding
                if (Input.GetKeyDown(exhaleKey))    // Pressing the exhale key will begin to exhale
                {
                    lungState = LungState.EXHALING;
                }
                break;
            case LungState.EXHALING:    // If the lungs are exhaling
                if (Input.GetKey(exhaleKey))    // The lungs exhale at a constant rate as long as you continue to hold the exhale key
                {
                    lungCapacity -= exhaleSpeed * Time.deltaTime;
                }
                if (lungCapacity <= Mathf.Epsilon)  // If the lungs capcity reaches 0 then the lungs are empty
                {
                    lungState = LungState.EMPTY;
                }
                break;
        }
    }
    #endregion
}
