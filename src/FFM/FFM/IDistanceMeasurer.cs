namespace FFM
{
    //Any implementation MUST form a Metric Space (http://en.wikipedia.org/wiki/Metric_space). 
    public interface IDistanceMeasurer<in T>
    {
        int Measure(T x, T y);
    }
}