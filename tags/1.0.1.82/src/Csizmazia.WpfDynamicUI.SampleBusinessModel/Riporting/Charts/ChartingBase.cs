using System;
using Csizmazia.Collections;
using Csizmazia.WpfDynamicUI.BusinessModel;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel.Riporting.Charts
{
    public abstract class ChartingBase : NavigationModel
    {
        protected TweakedObservableCollection<ChartDataDto> ChartData;

        protected override void OnOpened()
        {
            ChartData = new TweakedObservableCollection<ChartDataDto>(new[]
                                                                          {
                                                                              new ChartDataDto
                                                                                  {Area = "East", TotalSales = 10000},
                                                                              new ChartDataDto
                                                                                  {Area = "West", TotalSales = 20000},
                                                                              new ChartDataDto
                                                                                  {Area = "North", TotalSales = 12500},
                                                                              new ChartDataDto
                                                                                  {Area = "South", TotalSales = 7500}
                                                                          });
        }

        protected override void OnClosed()
        {
            ChartData.Clear();
        }

        public void ReloadChart()
        {
            var rnd = new Random();

            for (int i = 0; i < ChartData.Count; i++)
            {
                ChartData[i].TotalSales = rnd.Next(5000, 30000);
            }
        }
    }
}