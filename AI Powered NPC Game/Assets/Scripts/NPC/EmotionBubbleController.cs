using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Attached to: EmotionBubble (the empty child object above Eolindra)
// Floats above her head, always faces the camera, bobs gently.
// Uses ONE sprite (the heart) tinted differently for each emotion.
public class EmotionBubbleController : MonoBehaviour
{
    [Header("Drag Eolindra here")]
    public NPCEmotionSystem emotionSystem;

    [Header("UI References")]
    public Image bubblePanel;      // BubblePanel background Image
    public Image bubbleIconImage;  // BubbleIcon Image (the heart sprite)
    public TMP_Text labelText;        // BubbleLabel TMP text

    [Header("The single heart sprite")]
    public Sprite heartSprite;        // drag Heart.png here

    [Header("Bob Animation")]
    public float bobSpeed = 1.4f;
    public float bobHeight = 0.06f;

    // Internal
    private Camera mainCamera;
    private Vector3 startLocalPos;
    private EmotionState lastEmotion = EmotionState.Neutral;

    // Panel background colours per emotion
    private static readonly Color PanelNeutral = new Color(0.27f, 0.27f, 0.43f, 0.90f);
    private static readonly Color PanelWarm = new Color(0.13f, 0.55f, 0.29f, 0.90f);
    private static readonly Color PanelSad = new Color(0.16f, 0.30f, 0.68f, 0.90f);
    private static readonly Color PanelAngry = new Color(0.74f, 0.13f, 0.13f, 0.90f);

    // Heart sprite tint colours per emotion
    private static readonly Color TintNeutral = new Color(0.75f, 0.75f, 0.85f, 1.00f); // grey-white
    private static readonly Color TintWarm = new Color(1.00f, 0.80f, 0.85f, 1.00f); // pink-white
    private static readonly Color TintSad = new Color(0.55f, 0.75f, 1.00f, 1.00f); // blue
    private static readonly Color TintAngry = new Color(1.00f, 0.40f, 0.20f, 1.00f); // orange-red

    void Start()
    {
        mainCamera = Camera.main;
        startLocalPos = transform.localPosition;
        ApplyEmotion(EmotionState.Neutral);
    }

    void Update()
    {
        FaceCamera();
        Bob();
        CheckEmotionChanged();
    }

    // Rotate to always face the camera (billboard effect)
    void FaceCamera()
    {
        if (mainCamera == null) return;
        transform.LookAt(
            transform.position + mainCamera.transform.rotation * Vector3.forward,
            mainCamera.transform.rotation * Vector3.up);
    }

    // Gentle up-down float animation
    void Bob()
    {
        float newY = startLocalPos.y
                   + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.localPosition = new Vector3(
            startLocalPos.x, newY, startLocalPos.z);
    }

    // Watch for emotion changes every frame
    void CheckEmotionChanged()
    {
        if (emotionSystem == null) return;
        if (emotionSystem.currentEmotion == lastEmotion) return;

        lastEmotion = emotionSystem.currentEmotion;
        ApplyEmotion(lastEmotion);
    }

    // Update panel colour, heart tint, and label for the given emotion
    void ApplyEmotion(EmotionState emotion)
    {
        Color panelCol;
        Color heartTint;
        string label;

        switch (emotion)
        {
            case EmotionState.Warm:
                panelCol = PanelWarm;
                heartTint = TintWarm;
                label = "Warm";
                break;

            case EmotionState.Sad:
                panelCol = PanelSad;
                heartTint = TintSad;
                label = "Sad";
                break;

            case EmotionState.Angry:
                panelCol = PanelAngry;
                heartTint = TintAngry;
                label = "Angry";
                break;

            default: // Neutral
                panelCol = PanelNeutral;
                heartTint = TintNeutral;
                label = "Neutral";
                break;
        }

        // Apply background colour
        if (bubblePanel != null)
            bubblePanel.color = panelCol;

        // Apply heart sprite and tint
        if (bubbleIconImage != null)
        {
            bubbleIconImage.sprite = heartSprite;
            bubbleIconImage.color = heartTint;
            bubbleIconImage.preserveAspect = true;
        }

        // Update label
        if (labelText != null)
            labelText.text = label;

        Debug.Log("[EmotionBubble] " + label);
    }
}