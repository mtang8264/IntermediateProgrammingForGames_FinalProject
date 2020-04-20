using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 100f)]
    float oxygen;
    Slider oxygenBar;   // The display bar which shows the oxygen level

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
    float oxygenGainSpeed;  // The speed at which the oxygen bar fills up while inhaling
    [SerializeField]
    float emptyLungSize;    // The scale that the lungs reach when they are empty. We assume they should reach 1f scale while full.
    float oxygenLossTimer;  // This is the timer used to calculate when you should start losing oxygen
    [SerializeField]
    AnimationCurve oxygenLossRate;  // This curve defines how quickly you lose oxygen while not inhaling
    [SerializeField]
    float oxygenLossMultiplier; // This is a number multiplied by the oxygen loss rate
    Slider lungCapacityBar;
    Image lungCapacityBarFill, lungCapacityBarHandle;
    [SerializeField]
    Gradient lungCapacityBarGradient;

    void Awake()
    {
        oxygenBar = GameObject.FindWithTag("OxygenBar").GetComponent<Slider>();

        // If the level uses the lungs set up for the lungs
        if (lungsActive)
        {
            lungs = GameObject.FindWithTag("Lungs");
            lungState = LungState.EMPTY;
            lungCapacity = 0f;
            lungCapacityBar = GameObject.FindWithTag("LungCapacityBar").GetComponent<Slider>();
            Image[] images = lungCapacityBar.GetComponentsInChildren<Image>();
            foreach(Image i in images)
            {
                if (i.name == "Fill")
                    lungCapacityBarFill = i;
                else if (i.name == "Handle")
                    lungCapacityBarHandle = i;
            }
        }
    }

    void Update()
    {
        oxygenBar.value = oxygen;

        if(lungsActive)
        {
            DrawLungs();
            UpdateLungState();
            OxygenLoss();
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
        lungCapacityBar.value = lungCapacity;
        lungCapacityBarFill.color = lungCapacityBarGradient.Evaluate(lungCapacity);
        lungCapacityBarHandle.color = lungCapacityBarGradient.Evaluate(lungCapacity);
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
                    BreathingSoundController.instance.StartInhaling();
                }
                break;
            case LungState.INHALING:    // If the lungs are inhaling
                oxygenLossTimer = 0f;
                lungCapacity += inhaleSpeed * Time.deltaTime;   // Lung capacity increases at a constant rate while inhaling
                oxygen += oxygenGainSpeed * Time.deltaTime; // While the lungs are inhaling the oxygen level also increases
                if (Input.GetKey(inhaleKey) == false || lungCapacity >= 1f) // If the player releases the inhale key or the max lung capacity is reached the lungs begin to hold
                {
                    lungState = LungState.HOLDING;
                    BreathingSoundController.instance.StopInhaling();
                }
                if(Input.GetKeyDown(exhaleKey)) // If you press the exhale key while still inhaling you will start exhaling
                {
                    lungState = LungState.EXHALING;
                    BreathingSoundController.instance.StartExhaling();
                }
                break;
            case LungState.HOLDING: // If the lungs are holding
                if (Input.GetKeyDown(exhaleKey))    // Pressing the exhale key will begin to exhale
                {
                    lungState = LungState.EXHALING;
                    BreathingSoundController.instance.StartExhaling();
                }
                break;
            case LungState.EXHALING:    // If the lungs are exhaling
                if (Input.GetKey(exhaleKey))    // The lungs exhale at a constant rate as long as you continue to hold the exhale key
                {
                    lungCapacity -= exhaleSpeed * Time.deltaTime;
                    BreathingSoundController.instance.ResumeExhaling();
                }
                else
                {
                    BreathingSoundController.instance.PauseExhaling();
                }
                if (lungCapacity <= Mathf.Epsilon)  // If the lungs capcity reaches 0 then the lungs are empty
                {
                    lungState = LungState.EMPTY;
                    BreathingSoundController.instance.PauseExhaling();
                }
                break;
        }
    }
    /// <summary>
    /// This function enforces the loss of oxygen while not breathing
    /// </summary>
    void OxygenLoss()
    {
        if(lungState == LungState.INHALING)
        {
            oxygenLossTimer = 0f;
            return;
        }
        oxygenLossTimer += Time.deltaTime;
        oxygen -= oxygenLossRate.Evaluate(oxygenLossTimer) * oxygenLossMultiplier * Time.deltaTime;
    }
    #endregion
}
