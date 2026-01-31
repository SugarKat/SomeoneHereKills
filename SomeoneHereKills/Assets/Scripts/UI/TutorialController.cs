using System;
using TMPro;
using UnityEngine;

public class TutorialController : MonoBehaviour
{
    [Serializable]
    public class Step
    {
        [TextArea(2, 6)] public string text;

        public Mode mode;

        public RectTransform uiTarget;

        public Camera worldCamera;
        public Transform worldTarget;
        public Vector2 worldHoleSizePx = new Vector2(220, 140);

        public Vector2 padding = new Vector2(12, 12);
    }

    public enum Mode { UI, World }

    [Header("Refs")]
    public SpotlightOverlay overlay;
    public RectTransform tooltipPanel;
    public TMP_Text tooltipText;

    [Header("Steps")]
    public Step[] steps;

    [Header("Pause")]
    public bool pauseGameWhileTutorial = true;

    int index = -1;

    float prevTimeScale = 1f;
    bool paused;

    void OnEnable()
    {
        if (pauseGameWhileTutorial) PauseGame();
    }

    void OnDisable()
    {
        if (pauseGameWhileTutorial) ResumeGame();
    }

    void Start()
    {
        Next();
    }

    void PauseGame()
    {
        if (paused) return;
        prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        paused = true;
    }

    void ResumeGame()
    {
        if (!paused) return;
        Time.timeScale = prevTimeScale;
        paused = false;
    }

    public void Next()
    {
        index++;
        if (index >= steps.Length)
        {
            End();
            return;
        }
        Show(steps[index]);
    }

    public void Prev()
    {
        index = Mathf.Max(-1, index - 1);
        Next();
    }

    void Show(Step s)
    {
        tooltipText.text = s.text;
        overlay.padding = s.padding;

        if (s.mode == Mode.UI)
        {
            overlay.FocusUI(s.uiTarget);
            PlaceTooltipBottomCenter();
        }
        else
        {
            if (s.worldCamera == null || s.worldTarget == null)
            {
                Debug.LogWarning("Tutorial step missing worldCamera or worldTarget");
                return;
            }

            var size = s.worldHoleSizePx;
            if (size.x <= 0 || size.y <= 0) size = new Vector2(260, 180);

            overlay.FocusWorld(s.worldCamera, s.worldTarget.position, size);
            PlaceTooltipBottomCenter();
        }

        if (!tooltipPanel.gameObject.activeSelf) tooltipPanel.gameObject.SetActive(true);
        if (!overlay.gameObject.activeSelf) overlay.gameObject.SetActive(true);
    }

    void PlaceTooltipBottomCenter()
    {
        tooltipPanel.anchorMin = tooltipPanel.anchorMax = new Vector2(0.5f, 0.08f);
        tooltipPanel.pivot = new Vector2(0.5f, 0.5f);
        tooltipPanel.anchoredPosition = Vector2.zero;
    }

    void End()
    {
        if (pauseGameWhileTutorial) ResumeGame();

        overlay.gameObject.SetActive(false);
        tooltipPanel.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
