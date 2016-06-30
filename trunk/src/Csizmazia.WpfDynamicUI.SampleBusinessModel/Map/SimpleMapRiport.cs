using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.BusinessModel;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel.Map
{
    public class SimpleMapRiport : NavigationModel
    {
        private ObservableCollection<GpsData> _gpsMap = new ObservableCollection<GpsData>();

        [UIHint(UIHints.DisplayMap)]
        public ObservableCollection<GpsData> GpsMap
        {
            get { return _gpsMap; }
            set { _gpsMap = value; }
        }

        protected override void OnActivated()
        {
            GpsMap.Add(new GpsData
                           {
                               Latitude = 47.4990537731981,
                               Longitude = 19.0438556671143,
                               Description = "Budapest"
                           });
        }

        protected override void OnDeactivated()
        {
            GpsMap.Clear();
        }

        public static void SimpleMap(WorkspaceModel workspaceModel)
        {
            BusinessApplication.Instance.OpenModel(() => new SimpleMapRiport());
        }
    }
}