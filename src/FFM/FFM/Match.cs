using System;

namespace FFM
{
    public class Match<T>
    {
        public readonly T Data;
        public readonly int Score;

        public Match(T data, int score)
        {
            if (score < 0)
                throw new ArgumentOutOfRangeException("score");

            Data = data;
            Score = score;
        }

        public override string ToString()
        {
            return string.Format("Data: {0}, Score: {1}", Data, Score);
        }
    }
}