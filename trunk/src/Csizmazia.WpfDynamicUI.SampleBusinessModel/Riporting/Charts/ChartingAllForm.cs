using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.BusinessModel;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel.Riporting.Charts
{
    public class ChartingAllForm : ChartingBase
    {
        [UIHint(UIHints.DisplayChart, "",
            UIHints.ChartControlParameters.ChartCategoryProperty, "Area",
            UIHints.ChartControlParameters.ChartValueProperty, "TotalSales")]
        public ObservableCollection<ChartDataDto> ChartDataArea
        {
            get { return ChartData; }
        }

        [UIHint(UIHints.DisplayChart, "",
            UIHints.ChartControlParameters.ChartType, UIHints.ChartControlParameters.ChartTypeBar,
            UIHints.ChartControlParameters.ChartCategoryProperty, "Area",
            UIHints.ChartControlParameters.ChartValueProperty, "TotalSales")]
        public ObservableCollection<ChartDataDto> ChartDataBar
        {
            get { return ChartData; }
        }

        [UIHint(UIHints.DisplayChart, "",
            UIHints.ChartControlParameters.ChartType, UIHints.ChartControlParameters.ChartTypeScatter,
            UIHints.ChartControlParameters.ChartCategoryProperty, "Area",
            UIHints.ChartControlParameters.ChartValueProperty, "TotalSales")]
        public ObservableCollection<ChartDataDto> ChartDataScatter
        {
            get { return ChartData; }
        }

        [UIHint(UIHints.DisplayChart, "",
            UIHints.ChartControlParameters.ChartType, UIHints.ChartControlParameters.ChartTypePie,
            UIHints.ChartControlParameters.ChartCategoryProperty, "Area",
            UIHints.ChartControlParameters.ChartValueProperty, "TotalSales")]
        public ObservableCollection<ChartDataDto> ChartDataPie
        {
            get { return ChartData; }
        }

        [UIHint(UIHints.DisplayChart, "",
            UIHints.ChartControlParameters.ChartType, UIHints.ChartControlParameters.ChartTypeLine,
            UIHints.ChartControlParameters.ChartCategoryProperty, "Area",
            UIHints.ChartControlParameters.ChartValueProperty, "TotalSales")]
        public ObservableCollection<ChartDataDto> ChartDataLine
        {
            get { return ChartData; }
        }

        public static void LoadAllChartTypes(ChartingForm chartingForm)
        {
            BusinessApplication.Instance.OpenModel(() => new ChartingAllForm());
        }
    }
}