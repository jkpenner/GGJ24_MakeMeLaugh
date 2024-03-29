using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class StartupManager : MonoBehaviour
{
    [SerializeField] float confirmDelay = 3f;

    [Header("Screen One")]
    [SerializeField] GameObject screenOne;
    [SerializeField] GameObject keyCountDown;
    [SerializeField] TMP_Text keyCountText;
    [SerializeField] TMP_Text keyCountDownText;
    [SerializeField] RectTransform keyCountDownFill;

    [Header("Screen Two")]
    [SerializeField] GameObject screenTwo;
    [SerializeField] TMP_Text resultText;

    Keyboard current;
    Key[] keys;
    HashSet<Key> pressed = new HashSet<Key>();
    private int maxPressedKeys;

    private bool keyHit = false;
    private float keyCounter = 0f;

    private void Awake()
    {
        current = Keyboard.current;
        if (current is null)
        {
            Debug.LogWarning("No keyboard found");
        }

        keys = new Key[(int)Key.IMESelected - 1];
        for (int i = 1; i < (int)Key.IMESelected; i++)
        {
            keys[i - 1] = (Key)i;
        }
    }

    private void Start()
    {
        screenOne.gameObject.SetActive(true);
        screenTwo.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (screenOne.gameObject.activeSelf)
        {
            keyCountDown.SetActive(keyHit);

            if (keyHit)
            {
                

                keyCounter += Time.deltaTime;

                var scale = keyCountDownFill.localScale;
                scale.x = Mathf.Clamp01(keyCounter / confirmDelay);
                keyCountDownFill.localScale = scale;

                keyCountDownText.text = Mathf.CeilToInt(Mathf.Clamp(confirmDelay - keyCounter, 0f, confirmDelay)).ToString();

                if (keyCounter >= confirmDelay)
                {
                    screenOne.gameObject.SetActive(false);
                    screenTwo.gameObject.SetActive(true);
                    keyHit = false;
                    keyCounter = 0f;

                    var limited = Mathf.Min(maxPressedKeys, GameConsts.MaxKeySeqence);

                    resultText.text = $"Your Keyboard allows for up to {maxPressedKeys} key pressed simultaneously.\nSequence lengths will be limited to {limited} charaters.";
                }
            }

            foreach (Key key in keys)
            {
                if (current[key].wasPressedThisFrame)
                {
                    keyHit = true;

                    if (pressed.Add(key))
                    {
                        maxPressedKeys = Mathf.Max(maxPressedKeys, pressed.Count);
                        keyCountText.text = maxPressedKeys.ToString();
                    }
                }

                if (current[key].wasReleasedThisFrame)
                {
                    if (pressed.Remove(key))
                    {
                        maxPressedKeys = Mathf.Max(maxPressedKeys, pressed.Count);
                        keyCountText.text = maxPressedKeys.ToString();
                    }
                }


            }
        }

        if (screenTwo.gameObject.activeSelf)
        {
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                Application.Quit();
            }

            if (current.spaceKey.wasPressedThisFrame)
            {
                PlayerPrefs.SetInt(GameConsts.MaxSequenceLengthKey, Mathf.Min(maxPressedKeys, GameConsts.MaxKeySeqence));
                SceneManager.LoadScene(GameConsts.MainMenuBuildIndex);

                Debug.Log("Load Main Menu");
            }

            if (current.backspaceKey.wasPressedThisFrame)
            {
                maxPressedKeys = 0;
                pressed.Clear();
                keyCountText.text = maxPressedKeys.ToString();

                screenOne.gameObject.SetActive(true);
                screenTwo.gameObject.SetActive(false);
            }
        }
    }
}