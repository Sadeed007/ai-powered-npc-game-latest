using UnityEngine;

// The seven emotional states Eolindra can be in.
// Neutral, Warm, Sad, Angry were original.
// Hopeful, Surprised, Joyful are new additions.
public enum EmotionState
{
    Neutral,
    Warm,
    Sad,
    Angry,
    Hopeful,
    Surprised,
    Joyful
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
        // Emotion is set separately by SetEmotionFromString()
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
            case "hopeful": currentEmotion = EmotionState.Hopeful; break;
            case "surprised": currentEmotion = EmotionState.Surprised; break;
            case "joyful": currentEmotion = EmotionState.Joyful; break;
            default: currentEmotion = EmotionState.Neutral; break;
        }

        Debug.Log("[Emotion] Set to: " + currentEmotion +
                  " (from LLM string: '" + emotionString + "')");
    }

    // Returns a text description of the current emotion.
    // Included in the system prompt so the LLM knows Eolindra's current mood.
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

            case EmotionState.Hopeful:
                return "cautiously hopeful for the first time in centuries, " +
                       "barely daring to believe things might actually change, " +
                       "voice slightly softer and more open than usual";

            case EmotionState.Surprised:
                return "genuinely surprised and caught off guard, " +
                       "this stranger has said something unexpected that " +
                       "broke through your usual composure";

            case EmotionState.Joyful:
                return "quietly joyful in a way that feels almost unfamiliar, " +
                       "like sunlight after two hundred years of shadow, " +
                       "a warmth you had forgotten you could feel";

            default:
                return "wary and formal, guarded but not hostile, " +
                       "watching the stranger carefully";
        }
    }
}