

using static UIComponents.Models.Models.Graphs.TimeLineGraph.UICTimeLineGraph;

namespace UIComponents.Models.Models.Graphs.TimeLineGraph;

public class RequestLineGraphDataModel
{
    public DateTime StartUTC { get; set; }
    public DateTime EndUTC { get; set; }

    public DateTime StartLocal => StartUTC.ToLocalTime();
    public DateTime EndLocal => EndUTC.ToLocalTime();

    /// <summary>
    /// This is the time diffrence between 2 markers on the timeline
    /// </summary>
    public TimeSpan Scale { get => TimeSpan.FromMilliseconds(_scaleInMilliseconds); }
    public int _scaleInMilliseconds { get; set; }
    /// <summary>
    /// The Id defined in <see cref="LineGraph.LineGraphId"/>
    /// </summary>
    public string LineGraphId { get; set; }

    /// <summary>
    /// This is the same object as <see cref="LineGraph.AdditionalPostData"/> and may be usefull for handling the request
    /// </summary>
    public object AdditionalPostData { get; set; }


    /// <summary>
    /// This function takes the available data and reduces it so the dataset is not to large.
    /// <br>This takes average data between points to reduce the size</br>
    /// </summary>
    /// <param name="points">The original found datapoints</param>
    /// <param name="maxPointsCount">The maximum size of the dataset</param>
    /// <returns></returns>
    public IEnumerable<LineGraphPoint> ReducePoints(List<LineGraphPoint> points, int maxPointsCount)
    {
        if (points.Count() <= maxPointsCount)
            return points;

        return DataDecimation.LargestTriangleThreeBuckets(points, maxPointsCount);
    }
}
