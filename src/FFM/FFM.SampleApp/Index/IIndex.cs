using System.Collections.Generic;

namespace FFM.SampleApp.Index
{
    public interface IIndex<T>
    {
        void Add(T data);
        List<Match<T>> Matches(T query, int maxDistance);
    }
}