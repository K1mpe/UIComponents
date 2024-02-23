
using UIComponents.Defaults.Models.Graphs;

namespace UIComponents.Models.Models.Graphs.TimeLineGraph
{
    public class UICTimeLineGraph : UIComponent
    {

        #region Properties
        public List<LineGraph> LineGraphs { get; set; } = new();



        public DateTime Start { get; set; } = DateTime.Now.AddDays(-5);
        public DateTime End { get; set; } = DateTime.Now;

        public DateTime? MinStart { get; set; }
        public DateTime? MaxEnd { get; set; }

        public string Width { get; set; } = UIComponents.Defaults.Models.Graphs.TimeLineGraphDefault.Width;
        public string Height { get; set; } = UIComponents.Defaults.Models.Graphs.TimeLineGraphDefault.Height;

        public string MaxHeight { get; set; } = UIComponents.Defaults.Models.Graphs.TimeLineGraphDefault.Maxheight;

        #region Legend

        public bool ShowLegend { get; set; } = true;
        public Position LegendPosition { get; set; } = Position.Top;

        #endregion

        #region DisplayFormats

        /// <summary>
        /// A Hex color to set the major dates on the timeline. Feature is ignored if this is null.
        /// </summary>
        public string ColorMajor { get; set; } = UIComponents.Defaults.Models.Graphs.TimeLineGraphDefault.ColorMajor;
        public string DisplayFormatDay { get; set; } = UIComponents.Defaults.Models.Graphs.TimeLineGraphDefault.DisplayFormatDay;
        public string DisplayFormatHour { get; set; } = UIComponents.Defaults.Models.Graphs.TimeLineGraphDefault.DisplayFormatHour;
        public string DisplayFormatMinute { get; set; } = UIComponents.Defaults.Models.Graphs.TimeLineGraphDefault.DisplayFormatMinute;
        public string DisplayFormatSecond { get; set; } = UIComponents.Defaults.Models.Graphs.TimeLineGraphDefault.DisplayFormatSecond;
        #endregion

        #region Live data

        public bool EnableNowIndicator { get; set; } = TimeLineGraphDefault.EnableNowIndicator;
        public string NowIndicatorColor { get; set; } = TimeLineGraphDefault.NowIndicatorColor;

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
        public bool EnablePanning { get; set; } = UIComponents.Defaults.Models.Graphs.TimeLineGraphDefault.EnablePanning;
        public bool EnableZoom { get; set; } = UIComponents.Defaults.Models.Graphs.TimeLineGraphDefault.EnableZoom;

        /// <summary>
        /// You cannot zoom in any further if the time range that is visible on screen reaches this value
        /// </summary>
        public TimeSpan? ZoomInLimit { get; set; } = TimeSpan.FromMinutes(1);

        /// <summary>
        /// You cannot zoom out any further if the time range that is visible on screen reaches this value
        /// </summary>
        public TimeSpan? ZoomOutLimit { get; set; } = TimeSpan.FromDays(366);

        #endregion

        #region Scripts
        /// <summary>
        /// The base script used for rendering Chart.js
        /// </summary>
        public string Script_ChartJs { get; set; } = UIComponents.Defaults.Models.Graphs.TimeLineGraphDefault.Script_ChartJs;

        /// <summary>
        /// Adapter for moment in Chart.Js
        /// </summary>
        public string Script_ChartJs_Adapter_Moment { get; set; } = UIComponents.Defaults.Models.Graphs.TimeLineGraphDefault.Script_ChartJs_Adapter_Moment;


        /// <summary>
        /// Plugin to enable zooming or panning on the chart
        /// </summary>
        public string Script_ChartJs_Plugin_Zoom { get; set; } = UIComponents.Defaults.Models.Graphs.TimeLineGraphDefault.Script_ChartJs_Plugin_Zoom;

        /// <summary>
        /// Used for mobile support for zooming and panning
        /// </summary>
        public string Script_HammerJs { get; set; } =  UIComponents.Defaults.Models.Graphs.TimeLineGraphDefault.Script_HammerJs;
        #endregion

        /// <summary>
        /// Show log messages that can be used to learn the component
        /// </summary>
        public bool EnableConsoleLog { get; set; } = UIComponents.Defaults.Models.Graphs.TimeLineGraphDefault.ConsoleLog;




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
            /// <remarks>
            /// [HttpPost] GetData(UICTimeLineGraph.RequestLineGraphDataModel request){ return Json( List(UICTimeLineGraph.LineGraphPoint));}
            /// </remarks>
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
            public bool CacheData { get; set; } = LineGraphDefaults.CacheData;

            /// <summary>
            /// When zooming in or out, remove the cached data.
            /// <br>This is recommended when using <see cref="DataDecimation.LargestTriangleThreeBuckets(List{LineGraphPoint}, int)"/></br>
            /// </summary>
            public bool RemoveCacheOnZoom { get; set; } = LineGraphDefaults.RemoveCacheOnZoom;

            /// <summary>
            /// When a <see cref="Source"/> is provided, request will not ask data in the future.
            /// <br>This is to make sure that the live update can ask for only the missing data between requests</br>
            /// </summary>
            /// <remarks>
            /// The graph remembers the dateranges that have already been requested, and does not request the same range again without full reload
            /// </remarks>
            public bool DisableFutureLoading { get; set; } = LineGraphDefaults.DisableFutureLoading;

            /// <summary>
            /// Determine if this filter is active by default
            /// </summary>
            public bool Enabled { get; set; } = LineGraphDefaults.Enabled;

            /// <summary>
            /// If true, the graph will not go diagonal
            /// </summary>
            public bool Stepped { get; set; } = LineGraphDefaults.Stepped;

            /// <summary>
            /// Color the area under the line
            /// </summary>
            public bool Fill { get; set; } = LineGraphDefaults.Fill;

            /// <summary>
            /// The color of the points, depending on <see cref="PointRadius"/> or the <see cref="Fill"/> color
            /// </summary>
            public string BackgroundColor { get; set; }

            public double BorderWidth { get; set; } = 1;

            /// <summary>
            /// The Color of the line
            /// </summary>
            public string LineColor { get; set; }
            public int PointRadius { get; set; } = LineGraphDefaults.PointRadius;

            /// <summary>
            /// The proximity to a point for the tooltip to show
            /// </summary>
            public int PointHitRadius { get; set; } = LineGraphDefaults.PointHitRadius;

            /// <summary>
            /// Makes the graph curve. Results in less accurate graph but a more fluent line
            /// </summary>
            public double Tension { get; set; } = LineGraphDefaults.Tension;



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
}


namespace UIComponents.Defaults.Models.Graphs
{

    public static class TimeLineGraphDefault
    {
        public static string Width { get; set; } = "100%";
        public static string Height { get; set; } = "1000px";
        public static string Maxheight { get; set; } = "85vh";

        public static string DisplayFormatDay { get; set; } = "DD/MM";
        public static string DisplayFormatHour { get; set; } = "DD/MM - HH:mm";
        public static string DisplayFormatMinute { get; set; } = "HH:mm";
        public static string DisplayFormatSecond { get; set; } = "HH:mm:ss";

        public static string ColorMajor { get; set; } = "#FF0000";
        public static bool EnablePanning { get; set; } = true;
        public static bool EnableZoom { get; set; } = true;

        public static bool EnableNowIndicator { get; set; } = true;
        public static string NowIndicatorColor { get; set; } = "#FF000030";


        public static string Script_ChartJs { get; set; } = "/lib/Chart.js/chart.umd.min.js";
        public static string Script_ChartJs_Adapter_Moment { get; set; } = "/lib/chartjs-adapter-moment/chartjs-adapter-moment.min.js";
        public static string Script_ChartJs_Plugin_Zoom { get; set; } = "/lib/chartjs-plugin-zoom/chartjs-plugin-zoom.min.js";
        public static string Script_HammerJs { get; set; } = "/lib/hammer.js/hammer.min.js";

        public static bool ConsoleLog { get; set; }
    }

    public static class LineGraphDefaults
    {
        public static bool DisableFutureLoading { get; set; } = true;
        public static bool CacheData { get; set; } = true;
        public static bool RemoveCacheOnZoom { get; set; } = true;


        public static int PointRadius { get; set; }
        public static int PointHitRadius { get; set; } = 100;
        public static double BorderWidth { get; set; } = 1;
        public static double Tension { get; set; }
        public static bool Fill { get; set; }
        public static bool Stepped { get; set; }
        public static bool Enabled { get; set; } = true;
    }
}