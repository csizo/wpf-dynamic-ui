using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Csizmazia.WpfDynamicUI.BusinessModel;

namespace Csizmazia.WpfDynamicUI.SampleBusinessModel.Riporting
{
    public class SimpleRiport : NavigationModel
    {
        private ObservableCollection<string> _forFilterNameList;

        private List<SimpleRiportDto> _riportList;

        private SimpleRiportDto _selectedRiportDtoListItem;

        #region filters

        [Display(AutoGenerateFilter = true)]
        public ObservableCollection<string> FilterNameList
        {
            get { return _forFilterNameList; }
        }

        [Display(AutoGenerateField = false)]
        [UIHint(UIHints.DisplayDelayedBindingTextBox)]
        public string SelectedFilterNameList { get; set; }

        [Display(AutoGenerateFilter = true)]
        [UIHint(UIHints.DisplayDelayedBindingTextBox)]
        public string FilterName { get; set; }

        [Display(AutoGenerateFilter = true)]
        [UIHint(UIHints.DisplayDelayedBindingTextBox)]
        public DateTime FilterValidFrom { get; set; }

        [Display(AutoGenerateFilter = true)]
        [UIHint(UIHints.DisplayDelayedBindingTextBox)]
        public DateTime FilterValidTo { get; set; }

        #endregion

        [UIHint(UIHints.DisplayDataGrid)]
        public IEnumerable<SimpleRiportDto> RiportDtoList
        {
            get { return _riportList.AsEnumerable(); }
        }

        [UIHint("", "", "VisibilityConverter", "NotNullToVisibleConverter")]
        public SimpleRiportDto SelectedRiportDtoListItem
        {
            get { return _selectedRiportDtoListItem; }
            set
            {
                if (SelectedRiportDtoListItem != value)
                {
                    SimpleRiportDto before = _selectedRiportDtoListItem;

                    _selectedRiportDtoListItem = value;
                    OnPropertyChanged("SelectedRiportDtoListItem", before, value);
                }
            }
        }

        public void RefreshFilterNames()
        {
            _forFilterNameList.Clear();
            for (int i = 0; i < new Random().Next(1, 100); i++)
            {
                _forFilterNameList.Add(string.Format("Name {0}", i));
            }

            BusinessApplication.Instance.ShowPopup("Filter names refreshed! Check FilterNameList dropdown");
        }


        [Display(AutoGenerateField = false, Name = "Simple riport")]
        public static void LoadSimpleRiport(Riportings workspaceModel)
        {
            BusinessApplication.Instance.OpenModel(() => new SimpleRiport());
        }

        protected override void OnOpened()
        {
            _riportList = new List<SimpleRiportDto>
                              {
                                  new SimpleRiportDto
                                      {
                                          Id = 1,
                                          Name = "Name 1",
                                          Description = "Description 1",
                                          ValidFrom = DateTime.Now
                                      },
                                  new SimpleRiportDto
                                      {
                                          Id = 2,
                                          Name = "Name 2",
                                          Description = "Description 2",
                                          ValidFrom = DateTime.Now
                                      },
                                  new SimpleRiportDto
                                      {
                                          Id = 3,
                                          Name = "Name 3",
                                          Description = "Description 3",
                                          ValidFrom = DateTime.Now
                                      },
                                  new SimpleRiportDto
                                      {
                                          Id = 4,
                                          Name = "Name 4",
                                          Description = "Description 4",
                                          ValidFrom = DateTime.Now
                                      }
                              };

            _forFilterNameList = new ObservableCollection<string>
                                     {
                                         "Name 1",
                                         "Name 2",
                                         "Name 3",
                                         "Name 4",
                                     };
            BusinessApplication.Instance.ShowPopup("Opened sample riport");
        }
    }
}