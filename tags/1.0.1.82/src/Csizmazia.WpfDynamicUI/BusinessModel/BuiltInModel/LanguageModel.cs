using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Csizmazia.Collections;
using Csizmazia.WpfDynamicUI.Collections;
using Csizmazia.WpfDynamicUI.Localization;
using Csizmazia.WpfDynamicUI.Properties;
using LinqKit;

namespace Csizmazia.WpfDynamicUI.BusinessModel.BuiltInModel
{
    public class LanguageModel : NavigationModel
    {
        private PagedQueryable<CultureInfoDto> _cultures;

        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        public PagedQueryable<CultureInfoDto> Cultures
        {
            get { return _cultures; }
        }

        [Display(AutoGenerateFilter = true, ResourceType = typeof(Resources))]
        [UIHint(UIHints.DisplayDelayedBindingTextBox)]
        public string FilterCultureSystemName { get; set; }

        [Display(AutoGenerateFilter = true, ResourceType = typeof(Resources))]
        [UIHint(UIHints.DisplayDelayedBindingTextBox)]
        public string FilterCultureNativeName { get; set; }

        [Display(AutoGenerateFilter = true, ResourceType = typeof(Resources))]
        [UIHint(UIHints.DisplayDelayedBindingTextBox)]
        public string FilterCultureDisplayName { get; set; }


        [Display(AutoGenerateFilter = true, ResourceType = typeof(Resources))]
        [UIHint(UIHints.DisplayDelayedBindingTextBox)]
        public string FilterCultureEnglishName { get; set; }

        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        public CultureInfoDto SelectedCulturesItem { get; set; }

        protected override void OnOpened()
        {
            IOrderedQueryable<CultureInfoDto> cultureQuery =
                from c in CultureInfo.GetCultures(CultureTypes.NeutralCultures).AsQueryable()
                select new CultureInfoDto(c)
                    into m
                    orderby m.NativeName
                    select m;


            _cultures = new PagedQueryable<CultureInfoDto>(cultureQuery);
        }


        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName.StartsWith("Filter"))
                Cultures.Filter = BuildFilter();


            base.OnPropertyChanged(propertyName, before, after);
        }


        private Expression<Func<CultureInfoDto, bool>> BuildFilter()
        {
            Expression<Func<CultureInfoDto, bool>> condition = c => true;

            if (!string.IsNullOrEmpty(FilterCultureSystemName))
                condition = condition.And(c => c.SystemName.Contains(FilterCultureSystemName));

            if (!string.IsNullOrEmpty(FilterCultureDisplayName))
                condition = condition.And(c => c.DisplayName.Contains(FilterCultureDisplayName));

            if (!string.IsNullOrEmpty(FilterCultureEnglishName))
                condition = condition.And(c => c.EnglishName.Contains(FilterCultureEnglishName));

            if (!string.IsNullOrEmpty(FilterCultureNativeName))
                condition = condition.And(c => c.NativeName.Contains(FilterCultureNativeName));

            return condition;
        }

        [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
        public static void ChangeLanguage(AboutModel aboutModel)
        {
            BusinessApplication.Instance.OpenModel(() => new LanguageModel());
        }

        #region Nested type: CultureInfoDto

        public class CultureInfoDto : NotifyPropertyChanged
        {
            public CultureInfoDto(CultureInfo cultureInfo)
            {
                CultureInfo = cultureInfo;
                SystemName = cultureInfo.Name;
                NativeName = cultureInfo.NativeName;
                DisplayName = cultureInfo.DisplayName;
                EnglishName = cultureInfo.EnglishName;
            }

            [Display(AutoGenerateField = false)]
            public CultureInfo CultureInfo { get; set; }

            [Editable(false)]
            public string SystemName { get; private set; }

            [Editable(false)]
            public string NativeName { get; private set; }

            [Editable(false)]
            public string DisplayName { get; private set; }

            [Editable(false)]
            public string EnglishName { get; private set; }

            [Display(AutoGenerateField = true, ResourceType = typeof(Resources))]
            public void UseLanguage()
            {
                CultureManager.Instance.CurrentCulture = CultureInfo;

                string message = string.Format(Resources.CultureInfoChanged,
                                               CultureInfo.NativeName);
                BusinessApplication.Instance.ShowPopup(message);
            }
        }

        #endregion
    }
}