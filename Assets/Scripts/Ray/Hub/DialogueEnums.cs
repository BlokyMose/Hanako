using System.Collections;
using System.Collections.Generic;

namespace Hanako.Dialogue
{
    public static class DialogueEnums
    {
        public enum BubbleAnimation { Hidden, Still, Swaying }
        public enum BubbleShape { Round, Rectangle, Spiky, Wiggly, Grumpy, RectangleBlack, Epiphany }
        public enum CharParentAnimation { Hidden, Idle }
        public enum FaceAnimation
        {
            Idle = 0,

            // Thinking
            Hmm = 11,
            SquintEyes = 12,

            // Happy
            HappyGrin = 21,
            HappyCat = 22,
            SmileTooMuch = 23,

            // Angry
            Unimpressed = 31,
            UnimpressedMaybe = 32,
            Grumpy = 33,
            LostTemper = 34,
            Angry = 35,
            ChallengeAccepted = 36,

            // Shocked
            DieInside = 41,
            Scared = 42,
            Shocked = 43,

            // Misc
            YKnow = 51,
            Duck = 52,
            MakingExcuses = 53,
            Handsome = 54,
            Blush = 55,

            // Sad
            Sad = 61
        }

    }
}
