
using static UIComponents.Models.Models.Graphs.TimeLineGraph.UICTimeLineGraph;

namespace UIComponents.Models.Models.Graphs.TimeLineGraph;


public static class DataDecimation
{
    /// <summary>
    /// https://gist.github.com/DanielWJudge/63300889f27c7f50eeb7
    /// </summary>
    public static IEnumerable<LineGraphPoint> LargestTriangleThreeBuckets(List<LineGraphPoint> data, int threshold)
    {
        int dataLength = data.Count;
        if (threshold >= dataLength || threshold == 0)
            return data; // Nothing to do

        List<LineGraphPoint> sampled = new List<LineGraphPoint>(threshold);

        // Bucket size. Leave room for start and end data points
        double every = (double)(dataLength - 2) / (threshold - 2);

        int a = 0;
        LineGraphPoint maxAreaPoint = new LineGraphPoint();
        int nextA = 0;

        sampled.Add(data[a]); // Always add the first point

        for (int i = 0; i < threshold - 2; i++)
        {
            // Calculate point average for next bucket (containing c)
            double avgX = 0;
            double avgY = 0;
            int avgRangeStart = (int)(Math.Floor((i + 1) * every) + 1);
            int avgRangeEnd = (int)(Math.Floor((i + 2) * every) + 1);
            avgRangeEnd = avgRangeEnd < dataLength ? avgRangeEnd : dataLength;

            int avgRangeLength = avgRangeEnd - avgRangeStart;

            for (; avgRangeStart < avgRangeEnd; avgRangeStart++)
            {
                avgX += data[avgRangeStart].DateTime.Ticks; // * 1 enforces Number (value may be Date)
                avgY += data[avgRangeStart].Value;
            }
            avgX /= avgRangeLength;

            avgY /= avgRangeLength;

            // Get the range for this bucket
            int rangeOffs = (int)(Math.Floor((i + 0) * every) + 1);
            int rangeTo = (int)(Math.Floor((i + 1) * every) + 1);

            // Point a
            double pointAx = data[a].DateTime.Ticks; // enforce Number (value may be Date)
            double pointAy = data[a].Value;

            double maxArea = -1;

            for (; rangeOffs < rangeTo; rangeOffs++)
            {
                // Calculate triangle area over three buckets
                double area = Math.Abs((pointAx - avgX) * (data[rangeOffs].Value - pointAy) -
                                       (pointAx - data[rangeOffs].DateTime.Ticks) * (avgY - pointAy)
                                  ) * 0.5;
                if (area > maxArea)
                {
                    maxArea = area;
                    maxAreaPoint = data[rangeOffs];
                    nextA = rangeOffs; // Next a is this b
                }
            }

            sampled.Add(maxAreaPoint); // Pick this point from the bucket
            a = nextA; // This a is the next a (chosen b)
        }

        sampled.Add(data[dataLength - 1]); // Always add last

        return sampled;
    }

    public static List<LineGraphPoint> AveragePerTimespan(List<LineGraphPoint> data, TimeSpan timeSpan)
    {
        if (!data.Any())
            return new();

        List<LineGraphPoint> points = new List<LineGraphPoint>();

        //2 points are taken in the timespan. if the timespan contains points that are this percent more than the start or end, this point is also rendered.
        //Example: start: 100, end: 150, outsideAverage:0.2 , values above 160 or below 40 are also drawn
        double outsideAverage = 0.2; 

        DateTime start = data[0].DateTime;
        DateTime end = start.Add(timeSpan);

        do
        {
            var pointsInRange = data.Where(x => x.DateTime > start && x.DateTime <= end);
            int pointsCount = pointsInRange.Count();
            if (pointsCount == 0)
                continue;
            List<LineGraphPoint> addPoints = new();
            var firstPoint = pointsInRange.First();
            addPoints.Add(firstPoint);
            if (pointsInRange.Count() > 1)
            {
                var lastPoint = pointsInRange.Last();
                addPoints.Add(lastPoint);
                if (pointsCount > 2)
                {
                    var lowestValue = Math.Min(firstPoint.Value, lastPoint.Value);
                    var highestValue = Math.Min(firstPoint.Value, lastPoint.Value);
                    var margin = (highestValue - lowestValue) * outsideAverage;
                    var lowMargin = lowestValue - margin;
                    var highMargin = highestValue + margin;
                    var highestPoint = pointsInRange.Where(x => x.Value > highMargin).OrderByDescending(x => x.Value).FirstOrDefault();
                    if (highestPoint != null)
                        addPoints.Add(highestPoint);
                    var lowestPoint = pointsInRange.Where(x => x.Value < lowMargin).OrderBy(x => x.Value).FirstOrDefault();
                    if (lowestPoint != null)
                        addPoints.Add(lowestPoint);
                }

            }
            points.AddRange(addPoints.OrderBy(x => x.DateValue));

            start = end;
            end = start.Add(timeSpan);
        } while (end < data.Last().DateTime);
        points.Add(data.Last());
        return points;
    }
}
