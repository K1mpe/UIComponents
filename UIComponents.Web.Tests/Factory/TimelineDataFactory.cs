using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UIComponents.Models.Models.Graphs.TimeLineGraph;

namespace UIComponents.Web.Tests.Factory;

public static class TimelineDataFactory
{

    private static bool _initialized;
    private static List<(DateTime Timestamp, double Value)> _graphData1 { get; set; } = new();
    private static List<(DateTime Timestamp, double Value)> _graphData2 { get; set; } = new();

    public static Task Initialize()
    {
        if (_initialized)
            return Task.CompletedTask;

        var start = DateTime.Today.AddDays(-1);
        var end = DateTime.Today.AddDays(2);

        double lastValue = 0;
        var random = new Random();
        _initialized = true;
        while (start < end)
        {
            var addValue = (random.NextDouble() - 0.5) * 10;
            lastValue += addValue;
            var timestamp = start;
            _graphData1.Add(new(timestamp, lastValue));
            _graphData2.Add(new(timestamp, lastValue+100));
            start = start.AddSeconds(1);
            //Console.WriteLine($"{timestamp.ToLongDateString()} - {lastValue}");
        }



        return Task.CompletedTask;
    }

    public static List<UICTimeLineGraph.LineGraphPoint> GetPoint1(DateTime start, DateTime end)
    {
        if (!_initialized)
            _ = Initialize();
        return _graphData1.Where(x => x.Timestamp >= start && x.Timestamp <= end).Select(x => new UICTimeLineGraph.LineGraphPoint(x.Timestamp, x.Value)).ToList();
    }
    public static List<UICTimeLineGraph.LineGraphPoint> GetPoint2(DateTime start, DateTime end)
    {
        return _graphData2.Where(x => x.Timestamp >= start && x.Timestamp <= end).Select(x => new UICTimeLineGraph.LineGraphPoint(x.Timestamp, x.Value)).ToList();
    }
}
