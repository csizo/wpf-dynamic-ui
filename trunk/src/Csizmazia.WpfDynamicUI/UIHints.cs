namespace Csizmazia.WpfDynamicUI
{
    /// <summary>
    /// UI hints. Can be used in <see cref="System.ComponentModel.DataAnnotations.UIHintAttribute"/>
    /// </summary>
    public static class UIHints
    {
        /// <summary>
        /// Override Display to DisplayMultiLineTextBox
        /// </summary>
        public const string DisplayMultiLineTextBox = "DisplayMultiLineTextBox";

        /// <summary>
        /// Override Display to DisplayRichTextBox
        /// <para>Rich text box is currently broken...</para>
        /// </summary>
        public const string DisplayRichTextBox = "DisplayRichTextBox";

        /// <summary>
        /// Override Display to DisplayDataGrid
        /// </summary>
        public const string DisplayDataGrid = "DisplayDataGrid";

        /// <summary>
        /// Override Display to DisplayChart
        /// </summary>
        public const string DisplayChart = "DisplayChart";

        /// <summary>
        /// Override Display to DelayedBindingTextBox
        /// <para>Usable for filter controls</para>
        /// </summary>
        public const string DisplayDelayedBindingTextBox = "DelayedBindingTextBox";

        /// <summary>
        /// Override Display to DisplayMap
        /// </summary>
        public const string DisplayMap = "DisplayMap";

        /// <summary>
        /// Override Display to DisplayHyperlink
        /// </summary>
        public const string DisplayHyperlink = "DisplayHyperlink";

        /// <summary>
        /// Override Display to DisplayWebBrowser
        /// </summary>
        public const string DisplayWebBrowser = "DisplayWebBrowser";

        /// <summary>
        /// Override Display to DisplayComboBox
        /// </summary>
        public const string DisplayComboBox = "DisplayComboBox";

        public static class ComboBoxControlParameters
        {
            /// <summary>
            /// DisplayMemberPath Setting key
            /// </summary>
            public const string DisplayMemberPath = "DisplayMemberPath";
        }

        public static class ItemsControlParameters
        {
            /// <summary>
            /// SelectedItemBindingPath Setting key
            /// </summary>
            public const string SelectedItemBindingPath = "SelectedItemBindingPath";
        }

        #region Nested type: ChartControlParameters

        /// <summary>
        /// Chart control parameters...
        /// </summary>
        public static class ChartControlParameters
        {
            /// <summary>
            /// Chart Type Setting key
            /// </summary>
            public const string ChartType = "ChartType";

            /// <summary>
            /// Sets chart type to Area
            /// </summary>
            public const string ChartTypeArea = "AreaSeries";

            /// <summary>
            /// Sets chart type to Line
            /// </summary>
            public const string ChartTypeLine = "LineSeries";

            /// <summary>
            /// Sets chart type to Pie
            /// </summary>
            public const string ChartTypePie = "PieSeries";

            /// <summary>
            /// Sets chart type to Scatter
            /// </summary>
            public const string ChartTypeScatter = "ScatterSeries";

            /// <summary>
            /// Sets chart type to Bar
            /// </summary>
            public const string ChartTypeBar = "BarSeries";

            /// <summary>
            /// Sets chart type to Bubble
            /// </summary>
            public const string ChartTypeBubble = "BubbleSeries";

            /// <summary>
            /// Chart Category property Setting key
            /// </summary>
            public const string ChartCategoryProperty = "ChartCategory";

            /// <summary>
            /// Chart Value property Setting key
            /// </summary>
            public const string ChartValueProperty = "ChartValue";
        }

        #endregion

        #region Nested type: MapControlParameters

        public static class MapControlParameters
        {
            /// <summary>
            /// Map Latitude property Setting key
            /// </summary>
            public const string MapLatitudePropertyName = "Latitude";

            /// <summary>
            /// Map Longitude property Setting key
            /// </summary>
            public const string MapLongitudePropertyName = "Longitude";

            /// <summary>
            /// Map Marker Tooltip property Setting key
            /// </summary>
            public const string MapMarkerTooltipPropertyName = "Tooltip";
        }

        #endregion

        public static class DisplayParameters
        {
            public const string Height = "Height";

            public const string Width = "Width";

            public const string MaxHeight = "MaxHeight";

            public const string MaxWidth = "MaxWidth";

            public const string MinHeight = "MinHeight";

            public const string MinWidth = "MinWidth";
        }
    }
}