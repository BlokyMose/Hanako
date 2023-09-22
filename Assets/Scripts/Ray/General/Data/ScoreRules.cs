using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hanako
{
    [CreateAssetMenu(fileName ="ScoreRules_", menuName ="SO/Score Rules")]

    public class ScoreRules : ScriptableObject
    {
        [System.Serializable]
        public class Rule
        {
            [SerializeField]
            string paramName;

            [SerializeField]
            string displayName;

            [SerializeField]
            int scoreMultiplier = 1;

            [SerializeField]
            Color fontColor = Color.white;

            [SerializeField, TextArea]
            string displayFormat = "{displayName}<indent=350>{value}  x{scoreMultiplier}";

            public string ParamName { get => paramName; }
            public string DisplayName { get => displayName; }
            public int ScoreMultiplier { get => scoreMultiplier; }
            public Color FontColor { get => fontColor; }
            public string DisplayFormat { get => displayFormat; }
        }

        [SerializeField]
        string openToken = "{";

        [SerializeField]
        string closeToken = "}";

        [SerializeField]
        List<Rule> rules = new();

        public List<Rule> Rules { get => rules; }
        public string OpenToken { get => openToken; }
        public string CloseToken { get => closeToken; }
    }
}
