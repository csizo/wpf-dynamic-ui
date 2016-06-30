using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.BusinessModel;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel.Riporting.Charts
{
    public class ChartingForm : ChartingBase
    {
        [UIHint(UIHints.DisplayChart, "",
            UIHints.ChartControlParameters.ChartType, UIHints.ChartControlParameters.ChartTypeArea,
            UIHints.ChartControlParameters.ChartCategoryProperty, "Area",
            UIHints.ChartControlParameters.ChartValueProperty, "TotalSales")]
        public ObservableCollection<ChartDataDto> ChartDataDefault
        {
            get { return ChartData; }
        }


        protected override void OnActivated()
        {
            BusinessApplication.Instance.ShowPopup("Visualize the datapoints in charts...");
        }


        [Display(AutoGenerateField = true, Name = "Load chart samples")]
        public static void LoadChartSample(WorkspaceModel workspaceModel)
        {
            BusinessApplication.Instance.OpenModel(() => new ChartingForm());
        }
    }
}