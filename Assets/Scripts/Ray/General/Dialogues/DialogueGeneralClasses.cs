using System.Collections;
using System.Collections.Generic;
using static Hanako.Dialogue.DialogueEnums;

namespace Hanako.Dialogue
{
    public class DialogueRuntimeData
    {
        public class CharacterRuntimeProperties
        {
            CharID charID;
            SwapCharMode swapMode;
            int swapWithIndex = -1;
            CharID swapWithCharID;

            public CharacterRuntimeProperties(CharID charID, SwapCharMode swapMode, int swapWithIndex, CharID swapWithCharID)
            {
                this.charID = charID;
                this.swapMode = swapMode;
                this.swapWithIndex = swapWithIndex;
                this.swapWithCharID = swapWithCharID;
            }

            public CharID CharID { get => charID; }
            public SwapCharMode SwapMode { get => swapMode; }
            public int SwapWithIndex { get => swapWithIndex; }
            public CharID SwapWithCharID { get => swapWithCharID; }
        }

        DialogueData dialogueData;
        List<CharacterRuntimeProperties> characters = new();

        public DialogueData DialogueData { get => dialogueData; }
        public List<CharacterRuntimeProperties> Chars { get => characters; }

        public DialogueRuntimeData(DialogueData dialogueData, List<CharacterRuntimeProperties> characters)
        {
            this.dialogueData = dialogueData;
            this.characters = characters;
        }
    }
}
