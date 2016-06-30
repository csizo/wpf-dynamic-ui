using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Csizmazia.WpfDynamicUI.Geography;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel.Map
{
    public class RuntimeMarkerMapRiport : NavigationModel
    {
        private ObservableCollection<GpsData> _gpsMap = new ObservableCollection<GpsData>();

        [Display(AutoGenerateField = true, Prompt = "Enter address to looking for")]
        public string Location { get; set; }

        [UIHint(UIHints.DisplayMap)]
        public ObservableCollection<GpsData> GpsMap
        {
            get { return _gpsMap; }
            set { _gpsMap = value; }
        }

        [Editable(false)]
        [Display(AutoGenerateField = false)]
        public bool CanLookupMarker { get; set; }


        protected override void OnDeactivated()
        {
            GpsMap.Clear();
        }

        public void LookupMarker()
        {
            //...
            try
            {
                Coordinate coordinate = Gps.Locate(Location);

                GpsMap.Add(new GpsData
                               {
                                   Latitude = coordinate.Latidude,
                                   Longitude = coordinate.Longitude,
                                   Description = Location
                               });

                BusinessApplication.Instance.ShowPopup(string.Format("Placed marker for {0}", Location));
                Location = null;
            }
            catch (InvalidOperationException ex)
            {
                BusinessApplication.Instance.ShowPopup(ex.Message);
            }
        }

        public static void RuntimeMap(WorkspaceModel workspaceModel)
        {
            BusinessApplication.Instance.OpenModel(() => new RuntimeMarkerMapRiport());
        }

        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName == "Location")
                CanLookupMarker = !string.IsNullOrEmpty(Location);

            base.OnPropertyChanged(propertyName, before, after);
        }
    }
}