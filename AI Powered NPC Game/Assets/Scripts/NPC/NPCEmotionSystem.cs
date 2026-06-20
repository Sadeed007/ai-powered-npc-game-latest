using UnityEngine;

// The four emotional states Eolindra can be in.
public enum EmotionState
{
    Neutral,
    Warm,
    Sad,
    Angry     
}

// Attached to: Eolindra
// Tracks trust score and emotion state.
// Read by DialogueManager and the trust bar UI.
public class NPCEmotionSystem : MonoBehaviour
{
    [Header("Trust Score (0 = distrusts player, 100 = fully trusts)")]
    [Range(0, 100)]
    public float trustScore = 0f;

    [Header("Current emotion -- set by LLM, not by thresholds")]
    public EmotionState currentEmotion = EmotionState.Neutral;

    // Called by DialogueManager after every AI response.
    // The trust amount comes FROM the LLM -- no keyword arrays.
    public void ModifyTrust(float amount)
    {
        trustScore = Mathf.Clamp(trustScore + amount, 0f, 100f);
        // NOTE: emotion is no longer set here.
        // It is set by SetEmotionFromString() which is called separately.
        Debug.Log("[Trust] Score: " + trustScore + " | Emotion: " + currentEmotion);
    }

    // Lets other scripts force an emotion manually if needed.
    public void SetEmotion(EmotionState emotion)
    {
        currentEmotion = emotion;
    }

    // Converts the LLM's returned emotion string into the EmotionState enum.
    // Called by DialogueManager after every AI response.
    public void SetEmotionFromString(string emotionString)
    {
        if (string.IsNullOrEmpty(emotionString))
        {
            currentEmotion = EmotionState.Neutral;
            return;
        }

        switch (emotionString.ToLower().Trim())
        {
            case "warm": currentEmotion = EmotionState.Warm; break;
            case "sad": currentEmotion = EmotionState.Sad; break;
            case "angry": currentEmotion = EmotionState.Angry; break;
            default: currentEmotion = EmotionState.Neutral; break;
        }

        Debug.Log("[Emotion] Set to: " + currentEmotion +
                  " (from LLM string: '" + emotionString + "')");
    }

    // Returns a text description of the current emotion.
    // This is included in the system prompt so the LLM knows Eolindra's mood.
    public string GetEmotionDescription()
    {
        switch (currentEmotion)
        {
            case EmotionState.Warm:
                return "warm and open, beginning to trust this person, " +
                       "more willing to share memories of the forest";
            case EmotionState.Sad:
                return "melancholic and reflective, speaking softly, " +
                       "remembering things that were lost";
            case EmotionState.Angry:
                return "stern and protective, giving a clear warning to " +
                       "respect the forest";
            default:
                return "wary and formal, guarded but not hostile, " +
                       "watching the stranger carefully";
        }
    }
}