using System.Collections;
using System.Collections.Generic;

namespace Hanako
{
    [System.Serializable]
    public class ScoreDetail
    {
        string paramName;
        int value;

        public ScoreDetail(string paramName, int value)
        {
            this.paramName = paramName;
            this.value = value;
        }

        public string ParamName { get => paramName; }
        public int Value { get => value; }
    }
}
