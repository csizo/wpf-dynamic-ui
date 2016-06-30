using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Csizmazia.Discovering;
using Csizmazia.WpfDynamicUI.Properties;

namespace Csizmazia.WpfDynamicUI.BusinessModel.BuiltInModel
{
    public class AboutModel : NavigationModel
    {
        private const string _description = "About the Business Application Library";

        private readonly List<LicenseModel> _Components = new List<LicenseModel>
                                                              {
                                                                  new LicenseModel
                                                                      {
                                                                          Name = "Wpf Toolkit",
                                                                          Description =
                                                                              "The WPF Toolkit is a collection of WPF features and components that are being made available outside of the normal .NET Framework ship cycle. The WPF Toolkit not only allows users to get new functionality more quickly, but allows an efficient means for giving feedback to the product team.",
                                                                          Url = "http://wpf.codeplex.com/",
                                                                          License = Resources.LicenseMs_PL
                                                                      },
                                                                  new LicenseModel
                                                                      {
                                                                          Name = "Extended Wpf Toolkit",
                                                                          Description =
                                                                              "The Extended WPF Toolkit is a collection of WPF controls, components and utilities made available outside the normal WPF Toolkit.",
                                                                          Url = "http://wpftoolkit.codeplex.com/",
                                                                          License = Resources.LicenseMs_PL
                                                                      },
                                                                  new LicenseModel
                                                                      {
                                                                          Name = "GMAP.NET",
                                                                          Description =
                                                                              "GMap.NET is great and Powerful, Free, cross platform, open source .NET control. Enable use routing, geocoding, directions and maps from Coogle, Yahoo!, Bing, OpenStreetMap, ArcGIS, Pergo, SigPac, Yandex, Mapy.cz, Maps.lt, iKarte.lv, NearMap, OviMap, CloudMade, WikiMapia in Windows Forms & Presentation, supports caching and runs on windows mobile!",
                                                                          Url = "http://greatmaps.codeplex.com/",
                                                                          License = Resources.LicenseMIT
                                                                      },
                                                              };

        private readonly string _assembly;
        private bool _canInitializeModelCache = true;


        public AboutModel()
        {
            _assembly = System.Reflection.Assembly.GetEntryAssembly().FullName;
        }

        [Editable(false)]
        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        public string Description
        {
            get { return _description; }
        }

        [Editable(false)]
        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        public string Assembly
        {
            get { return _assembly; }
        }


        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        [UIHint(UIHints.DisplayDataGrid)]
        public List<LicenseModel> Components
        {
            get { return _Components; }
        }

        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        public LicenseModel SelectedComponentsItem { get; set; }

        [Display(AutoGenerateField = false)]
        public bool CanInitializeModelCache
        {
            get { return _canInitializeModelCache; }
            set { _canInitializeModelCache = value; }
        }

        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        public void InitializeExtensionCache()
        {
            foreach (IModel model in ExportedModels)
            {
                Type type = model.GetType();
                type.GetInstanceProperties();
                type.GetStaticMethods();
                type.GetInstanceMethods();
                type.GetInstanceFields();

                type.GetStaticProperties();
            }


            BusinessApplication.Instance.ShowPopup("Cache is filled!");
            CanInitializeModelCache = false;
        }


        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        public static void AboutApplication()
        {
            OpenModel<AboutModel>();
        }
    }
}