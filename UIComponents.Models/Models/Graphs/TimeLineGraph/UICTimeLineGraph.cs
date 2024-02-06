
namespace UIComponents.Models.Models.Graphs.TimeLineGraph;

public class UICTimeLineGraph : UIComponent
{

    #region Properties
    public List<LineGraph> LineGraphs { get; set; } = new();



    public DateTime Start { get; set; } = DateTime.Now.AddDays(-5);
    public DateTime End { get; set; } = DateTime.Now;

    public DateTime? MinStart { get; set; } 
    public DateTime? MaxEnd { get; set; }

    public string Width { get; set; } = "100%";
    public string Height { get; set; } = "100%";
    #region Legend

    public bool ShowLegend { get; set; } = true;
    public Position LegendPosition { get; set; } = Position.Top;

    #endregion

    #region DisplayFormats
    public string DisplayFormatDay { get; set; } = "DD/MM";
    public string DisplayFormatHour { get; set; } = "DD/MM - HH:mm";
    public string DisplayFormatMinute { get; set; } = "HH:mm";
    public string DisplayFormatSecond { get; set; } = "HH:mm:ss";
    #endregion

    #region Live data

    /// <summary>
    /// Live update data if conditions are met
    /// </summary>
    public bool EnableLiveData { get; set; } = true;

    /// <summary>
    /// The minimum interval for fetching the missing data untill now. The delay may be longer depending on the range of the graph
    /// <br>There is no use in loading live data every second if the graph show data from a entire month</br>
    /// <br>Only works on grapsh where <see cref="LineGraph.Source"/> is provided and <see cref="LineGraph.DisableFutureLoading"/> == true</br>
    /// </summary>
    /// <remarks>
    /// if <see cref="EnablePanning"/> is enabled, this interval only works if the current time is still visible on the graph.
    /// <br> If not, this data will automatically be fetched when moving the graph</br>
    /// </remarks>0
    public TimeSpan LoadLiveDataMinInterval { get; set; } = TimeSpan.FromSeconds(3);

    /// <summary>
    /// If the current time is visible in the on the graph and <see cref="DateTime.Now"/> + <see cref="MoveGraphAfterLiveUpdate"/> is larger than the current end of the graph,
    /// <br>the graph will move this timespan to always maintain a small empty range</br>
    /// </summary>
    public TimeSpan MoveGraphAfterLiveUpdate { get; set; } = TimeSpan.Zero;
    #endregion

    #region Pan / Zoom
    public bool EnablePanning { get; set; } = true;
    public bool EnableZoom { get; set; } = true;

    /// <summary>
    /// You cannot zoom in any further if the time range that is visible on screen reaches this value
    /// </summary>
    public TimeSpan? ZoomInLimit { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>
    /// You cannot zoom out any further if the time range that is visible on screen reaches this value
    /// </summary>
    public TimeSpan? ZoomOutLimit { get; set; } = TimeSpan.FromDays(366);

    #endregion

    /// <summary>
    /// Show log messages that can be used to learn the component
    /// </summary>
    public bool EnableConsoleLog { get; set; } = true;




    /// <summary>
    /// If a <see cref="LineGraph"/> does not have a <see cref="LineGraph.LineColor"/> or <see cref="LineGraph.BackgroundColor"/> provided, The index of the linegraph is used to select one of these colorsets.
    /// <br>Since the order of this list is respected, it is recommended to use very distinct colors after eachother</br>
    /// </summary>
    /// <remarks>
    /// If more linegraphs are used than color presets are available, this will restart the list of values</remarks>
    public List<(string LineColor, string BackgroundColor)> LineColors { get; set; } = new()
    {
        ("#4287f5", "#4287f570"), //blue
        ("#f54e42", "#f54e4270"), //red
        ("#78f542", "#78f54270"), //green
        ("#f542b3", "#f542b370"), //purple
        ("#f5e942", "#f5e94270"), //yellow
        ("#42f2f5", "#42f2f570"), //cyan
        ("#ed871a", "#ed871a70"), //orange
    };


    #endregion

    #region Methods

    public UICTimeLineGraph Add(LineGraph item)
    {
        return Add(out var x, item);
    }
    public UICTimeLineGraph Add<T>(out T added, T item) where T : LineGraph
    {
        added = item;
        LineGraphs.Add(item);
        return this;
    }

    #endregion

    #region DataModels

    


    public class LineGraph
    {

        #region Ctor
        public LineGraph(Translatable label, List<LineGraphPoint> points)
        {
            Label = label;
            Points = points;
        }

        public LineGraph(Translatable label, string id, string source)
        {
            Label = label;
            LineGraphId = id;
            Source = source;
        }
        #endregion
        public Translatable Label { get; set; }
        public List<LineGraphPoint> Points { get; set; } = new();


        /// <summary>
        /// A source to fetch data, this can be used for reloading or loading aditional data
        /// </summary>
        public string Source { get; set; }
        /*Example of ControllerMethod:
         * 
         * [HttpPost]
         * public async Task<IActionResult> GetData(UICTimeLineGraph.RequestLineGraphDataModel request)
         * {
         *      List<UICTimeLineGraph.LineGraphPoint> data = await ....
         *      return Json(data);
         * }
         * 
         * */

        /// <summary>
        /// Use clientside caching to minimize data processing
        /// </summary>
        public bool CacheData { get; set; }

        /// <summary>
        /// When zooming in or out, remove the cached data.
        /// <br>This is recommended when using <see cref="DataDecimation.LargestTriangleThreeBuckets(List{LineGraphPoint}, int)"/></br>
        /// </summary>
        public bool DisableCachingOnZoom { get; set; }

        /// <summary>
        /// When a <see cref="Source"/> is provided, request will not ask data in the future.
        /// <br>This is to make sure that the live update can ask for only the missing data between requests</br>
        /// </summary>
        /// <remarks>
        /// The graph remembers the dateranges that have already been requested, and does not request the same range again without full reload
        /// </remarks>
        public bool DisableFutureLoading { get; set; } = true;

        /// <summary>
        /// Determine if this filter is active by default
        /// </summary>
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// If true, the graph will not go diagonal
        /// </summary>
        public bool Stepped { get; set; }

        /// <summary>
        /// Color the area under the line
        /// </summary>
        public bool Fill { get; set; }

        /// <summary>
        /// The color of the points, depending on <see cref="PointRadius"/> or the <see cref="Fill"/> color
        /// </summary>
        public string BackgroundColor { get; set; }

        /// <summary>
        /// The Color of the line
        /// </summary>
        public string LineColor { get; set; }
        public int PointRadius { get; set; }

        /// <summary>
        /// Makes the graph curve. Results in less accurate graph but a more fluent line
        /// </summary>
        public float Tension { get; set; }



        public string LineGraphId { get; set; }

        /// <summary>
        /// This property can be used to post additional data with the <see cref="Source"/> 
        /// <br>You can find this object in the <see cref="RequestLineGraphDataModel.AdditionalPostData"/> of a request</br>
        /// </summary>
        public object AdditionalPostData { get; set; }

        #region Methods
        public LineGraphPoint Add(LineGraphPoint graphPoint)
        {
            Points.Add(graphPoint);
            return graphPoint;
        }
        public LineGraph Add2(LineGraphPoint graphPoint)
        {
            Points.Add(graphPoint);
            return this;
        }
        #endregion
    }
    public class LineGraphPoint
    {
        public LineGraphPoint(DateTime dateTime, double value, string id = null)
        {
            DateTime = dateTime;
            Value = value;
            Id = id;
        }
        public LineGraphPoint()
        {

        }
        [System.Text.Json.Serialization.JsonPropertyName("v")]
        public double Value { get; set; }

        [System.Text.Json.Serialization.JsonIgnore]
        public DateTime DateTime { get; set; }



        [System.Text.Json.Serialization.JsonPropertyName("d")]
        public string DateValue { get => DateTime.ToString("s"); }

        /// <summary>
        /// Optional: Can be used to further identify points on the graph
        /// </summary>
        public string Id { get; set; }
    }
    #endregion

    public enum Position
    {
        Top,
        Bottom,
        Left,
        Right,
    }
}
