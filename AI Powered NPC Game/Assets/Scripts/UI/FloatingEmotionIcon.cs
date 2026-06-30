using UnityEngine;
using UnityEngine.UI;

// Attached to: FloatingHeart (Image on the main Canvas)
// Only visible during active dialogue.
// Shows thinking cloud while waiting for API.
// Shows correct emotion sprite when response arrives.
public class FloatingEmotionIcon : MonoBehaviour
{
    public enum BubbleState
    {
        Thinking, Neutral, Warm, Sad, Angry,
        Hopeful, Surprised, Joyful
    }

    [Header("References")]
    public Transform eolindraTransform;
    public NPCEmotionSystem emotionSystem;
    public RectTransform rectTransform;
    public Image iconImage;

    [Header("Sprites -- drag one PNG per emotion")]
    public Sprite spriteThinking;
    public Sprite spriteNeutral;
    public Sprite spriteWarm;
    public Sprite spriteSad;
    public Sprite spriteAngry;
    public Sprite spriteHopeful;    // star PNG
    public Sprite spriteSurprised;  // exclamation PNG
    public Sprite spriteJoyful;     // laughing face PNG

    [Header("Position above her head (world units)")]
    public float worldYOffset = 2.5f;

    [Header("Bob Animation")]
    public float bobSpeed = 1.5f;
    public float bobAmount = 6f;

    // Tint colours for each state
    private static readonly Color ColThinking = new Color(1.00f, 1.00f, 0.55f, 1f); // yellow
    private static readonly Color ColNeutral = new Color(0.85f, 0.85f, 0.95f, 1f); // pale grey
    private static readonly Color ColWarm = new Color(1.00f, 1.00f, 1.00f, 1f); // white
    private static readonly Color ColSad = new Color(0.55f, 0.75f, 1.00f, 1f); // blue
    private static readonly Color ColAngry = new Color(1.00f, 1.00f, 1.00f, 1f); // white
    private static readonly Color ColHopeful = new Color(1.00f, 0.85f, 0.20f, 1f); // golden yellow
    private static readonly Color ColSurprised = new Color(0.80f, 0.40f, 1.00f, 1f); // purple
    private static readonly Color ColJoyful = new Color(0.20f, 0.90f, 0.60f, 1f); // bright teal

    private Camera mainCamera;
    private EmotionState lastEmotion = EmotionState.Neutral;
    private bool isThinking = false;
    private bool dialogueOpen = false;
    private float pulseTimer = 0f;

    void Start()
    {
        mainCamera = Camera.main;

        // Hidden by default -- only shows during dialogue
        if (iconImage != null) iconImage.enabled = false;
    }

    void LateUpdate()
    {
        if (eolindraTransform == null) return;
        if (mainCamera == null) { mainCamera = Camera.main; return; }

        // Only show and position when dialogue is open
        if (!dialogueOpen)
        {
            if (iconImage != null) iconImage.enabled = false;
            return;
        }

        PositionAboveEolindra();
        HandlePulse();

        // Only check emotion changes when NOT thinking
        // so the cloud does not get interrupted mid-pulse
        if (!isThinking)
            CheckEmotionChanged();
    }

    // Called by DialogueManager when dialogue opens
    public void ShowIcon()
    {
        dialogueOpen = true;
        if (iconImage != null) iconImage.enabled = true;

        // Show neutral until first response arrives
        SetState(BubbleState.Neutral);
    }

    // Called by DialogueManager when dialogue closes
    public void HideIcon()
    {
        dialogueOpen = false;
        isThinking = false;
        if (iconImage != null) iconImage.enabled = false;
        if (rectTransform != null) rectTransform.localScale = Vector3.one;
    }

    // Called by AIRequestHandler
    // true  = waiting for response, show cloud
    // false = response arrived, show emotion sprite
    public void SetThinking(bool thinking)
    {
        isThinking = thinking;

        if (thinking)
        {
            SetState(BubbleState.Thinking);
        }
        else
        {
            // Read emotion directly now -- lastEmotion may be stale
            if (emotionSystem != null)
            {
                lastEmotion = emotionSystem.currentEmotion;
                ApplyEmotionState(lastEmotion);
            }
        }
    }

    // Applies the correct sprite and colour for the given state
    public void SetState(BubbleState state)
    {
        if (iconImage == null) return;

        Sprite sprite;
        Color col;

        switch (state)
        {
            case BubbleState.Thinking:
                sprite = spriteThinking != null ? spriteThinking : spriteNeutral;
                col = ColThinking;
                break;

            case BubbleState.Warm:
                sprite = spriteWarm != null ? spriteWarm : spriteNeutral;
                col = ColWarm;
                break;

            case BubbleState.Sad:
                sprite = spriteSad != null ? spriteSad : spriteNeutral;
                col = ColSad;
                break;

            case BubbleState.Angry:
                sprite = spriteAngry != null ? spriteAngry : spriteNeutral;
                col = ColAngry;
                break;

            case BubbleState.Hopeful:
                sprite = spriteHopeful != null ? spriteHopeful : spriteNeutral;
                col = ColHopeful;
                break;

            case BubbleState.Surprised:
                sprite = spriteSurprised != null ? spriteSurprised : spriteNeutral;
                col = ColSurprised;
                break;

            case BubbleState.Joyful:
                sprite = spriteJoyful != null ? spriteJoyful : spriteWarm;
                col = ColJoyful;
                break;

            default:
                sprite = spriteNeutral != null ? spriteNeutral : spriteWarm;
                col = ColNeutral;
                break;
        }

        iconImage.sprite = sprite;
        iconImage.color = col;
        iconImage.preserveAspect = true;
        iconImage.enabled = true;
    }

    // Pulse animation while waiting for API response
    void HandlePulse()
    {
        if (!isThinking)
        {
            if (rectTransform != null)
                rectTransform.localScale = Vector3.one;
            pulseTimer = 0f;
            return;
        }

        pulseTimer += Time.deltaTime * 3f;
        float scale = 0.80f + Mathf.Abs(Mathf.Sin(pulseTimer)) * 0.35f;
        if (rectTransform != null)
            rectTransform.localScale = new Vector3(scale, scale, 1f);
    }

    // Moves the icon to sit above Eolindra's head on screen
    void PositionAboveEolindra()
    {
        Vector3 worldPos = eolindraTransform.position;
        worldPos.y += worldYOffset;

        Vector3 screenPos = mainCamera.WorldToScreenPoint(worldPos);

        if (screenPos.z < 0f)
        {
            if (iconImage != null) iconImage.enabled = false;
            return;
        }

        if (iconImage != null) iconImage.enabled = true;

        float bob = Mathf.Sin(Time.time * bobSpeed) * bobAmount;
        screenPos.y += bob;

        if (rectTransform != null)
            rectTransform.position = new Vector3(screenPos.x, screenPos.y, 0f);
    }

    // Checks every frame if emotion changed and updates sprite
    void CheckEmotionChanged()
    {
        if (emotionSystem == null) return;
        if (emotionSystem.currentEmotion == lastEmotion) return;
        lastEmotion = emotionSystem.currentEmotion;
        ApplyEmotionState(lastEmotion);
    }

    // Converts EmotionState enum to BubbleState and applies it
    void ApplyEmotionState(EmotionState emotion)
    {
        switch (emotion)
        {
            case EmotionState.Warm: SetState(BubbleState.Warm); break;
            case EmotionState.Sad: SetState(BubbleState.Sad); break;
            case EmotionState.Angry: SetState(BubbleState.Angry); break;
            case EmotionState.Hopeful: SetState(BubbleState.Hopeful); break;
            case EmotionState.Surprised: SetState(BubbleState.Surprised); break;
            case EmotionState.Joyful: SetState(BubbleState.Joyful); break;
            default: SetState(BubbleState.Neutral); break;
        }
    }
}