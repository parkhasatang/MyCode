using System.Collections.Generic;

namespace DialogueSystem
{
    [System.Serializable]
    public class DialogueData
    {
        public List<DialogueEntry> dialogues;
    }

    [System.Serializable]
    public class DialogueEntry
    {
        public string id;
        public List<ConversationPiece> conversations;
        public int requirementValue;
        public string requirementStat;
        public List<ConversationPiece> success;
        public List<ConversationPiece> failure;
    }

    [System.Serializable]
    public class ConversationPiece
    {
        public string speaker;
        public string text;
    }
}
