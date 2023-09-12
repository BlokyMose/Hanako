using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            [InspectorName(nameof(Hmm) + "  :(")]
            Hmm = 11,
            [InspectorName(nameof(SquintEyes)+"  =_=")]
            SquintEyes = 12,

            // Happy
            [InspectorName(nameof(HappyGrin) +"  :D")]
            HappyGrin = 21,
            [InspectorName(nameof(HappyCat) +"  :3")]
            HappyCat = 22,
            [InspectorName(nameof(SmileTooMuch) +"  :>")]
            SmileTooMuch = 23,

            // Angry
            [InspectorName(nameof(Unimpressed) +"  -_-")]
            Unimpressed = 31,
            [InspectorName(nameof(UnimpressedMaybe) +"  (:(")]
            UnimpressedMaybe = 32,
            [InspectorName(nameof(Grumpy) +"  ):(")]
            Grumpy = 33,
            [InspectorName(nameof(LostTemper) +"  >:O")]
            LostTemper = 34,
            [InspectorName(nameof(Angry) +"  >:(")]
            Angry = 35,
            [InspectorName(nameof(ChallengeAccepted) +"  >:)")]
            ChallengeAccepted = 36,

            // Shocked
            [InspectorName(nameof(DieInside) +"  o_o")]
            DieInside = 41,
            [InspectorName(nameof(Scared) +"  0_0")]
            Scared = 42,
            [InspectorName(nameof(Shocked) +"  :O")]
            Shocked = 43,

            // Misc
            [InspectorName(nameof(YKnow) +"  :-)")]
            YKnow = 51,
            [InspectorName(nameof(Duck) +"  O3O")]
            Duck = 52,
            [InspectorName(nameof(MakingExcuses) +"  =3=")]
            MakingExcuses = 53,
            [InspectorName(nameof(Handsome) +"  :-)")]
            Handsome = 54,
            [InspectorName(nameof(Blush) +"  O//O")]
            Blush = 55,

            // Sad
            [InspectorName(nameof(Sad)+"  :(")]
            Sad = 61
        }
        public enum BodyAnimation
        {
            Idle = 0,

            // Thinking
            Think = 11,
            Dilemma = 12,
            Wave = 13,
            Supportive = 14,
            Nod = 15,
            Bow = 16,

            // Happy
            Happy = 21,
            HappyJumping = 22,
            Dance = 23,
            Excited = 24,

            // Angry
            Unimpressed = 31,
            Angry = 32,

            // Sad
            Disappointed = 41,
            Pleading = 42,

            // Scared
            Stiffed = 51,
            Scared = 52,
            PointingScared = 53,

            // Misc
            Attack = 61,
            RunHop = 62,
            PushedMidAir = 63,
            Die = 64,
            Summon = 65,
            Summoned = 66,
        }
    }
}
