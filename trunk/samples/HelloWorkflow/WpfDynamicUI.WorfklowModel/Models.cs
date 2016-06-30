using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Csizmazia.WpfDynamicUI.BusinessModel.BuiltInModel;

namespace WpfDynamicUI.WorfklowModel
{
    public class StartupModel : StartupModelBase
    {
        public void StartSample()
        {
            BusinessApplication.Instance.CloseCurrentModelAndNavigateTo(() => new MainModel());
        }
    }
    public class MainModel : NavigationModel
    {

        protected override void OnActivated()
        {
            About = "Here is an example of a main model...\r\nThis model will be an entry point for EntryFormModel model";
        }

        /// <summary>
        /// Readonly property about this functionality
        /// </summary>
        [Editable(false)]
        [StringLength(1000)]
        public string About { get; private set; }


        /// <summary>
        /// Name entered @EntryFormModel
        /// </summary>
        [Editable(false)]
        public string Name { get; internal set; }

        /// <summary>
        /// defines an application level entry point for Main Model... This makes the model available on application level.
        /// </summary>
        [Display(AutoGenerateField = true, Name = "WokflowSample", Description = "Opens the sample workflow...")]
        public static void ApplicationEntryPoint()
        {
            BusinessApplication.Instance.OpenModel(() => new MainModel());
        }
    }

    public class EntryFormModel : NavigationModel
    {
        /// <summary>
        /// Name property with validation attributes attached
        /// </summary>
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Name { get; set; }

        /// <summary>
        /// contextual sayhello method
        /// </summary>
        public void SayHello()
        {
            var message = string.Format("Hello '{0}'", Name);
            BusinessApplication.Instance.ShowPopup(message);

            //set MainModel Name property to actual name
            MainModel.Name = Name;
        }

        /// <summary>
        /// condition property for SayHello method
        /// </summary>
        [Display(AutoGenerateField = false)]
        public bool CanSayHello { get; private set; }

        /// <summary>
        /// Called when properties changed... We are interested in setting CanSayHello on Name property changed
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="before"></param>
        /// <param name="after"></param>
        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName == "Name")
                CanSayHello = !string.IsNullOrEmpty(Name);

            base.OnPropertyChanged(propertyName, before, after);
        }

        /// <summary>
        /// private properties are not displayed... mow storing parent model instance
        /// </summary>
        private MainModel MainModel { get; set; }

        /// <summary>
        /// Module entry point. (accepts only MainModel instance)
        /// </summary>
        /// <param name="mainModel"></param>
        [Display(AutoGenerateField = true, Name = "Open Say hello")]
        public static void OpenWorkflowSayHello(MainModel mainModel)
        {
            //opens this model and make it active when called (clicked by the end user)
            BusinessApplication.Instance.OpenModel(() => new EntryFormModel()
            {
                //set the parent model
                MainModel = mainModel,
                //set the parent name property in the form
                Name = mainModel.Name
            });

        }
    }
}
