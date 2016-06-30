using System;
using System.Collections.Generic;
using System.Linq;
using Csizmazia.WpfDynamicUI.BusinessModel.BuiltInModel;

namespace Csizmazia.WpfDynamicUI.BusinessModel
{
    public class BusinessApplication : NotifyPropertyChanged
    {
        private const string WindowTitleString = "Wpf Dynamic User Interface";
        public static readonly BusinessApplication Instance = new BusinessApplication();

        private string _statusBarText = "[ready]";

        private BusinessApplication()
        {
            _openedModelStack = new ModelStack(this);


            //get startup model
            var exports = MefComposition.Container.Instance.GetExports<IModel>().ToList();
            var startupModel = exports.Select(e => e.Value).OfType<StartupModelBase>().FirstOrDefault(e => !e.GetType().IsAbstract);

            if (startupModel != null)
                OpenModel(() => startupModel);
            else
            {
                OpenModel(() => new StartupRequiredModel());
            }
            MefComposition.Container.Instance.ReleaseExports(exports);
        }

        public string WindowTitle { get; set; }


        public string StatusBarText
        {
            get { return _statusBarText; }
            set { _statusBarText = value; }
        }

        /// <summary>
        /// gets the popup text to display
        /// </summary>
        public string PopupText { get; private set; }

        /// <summary>
        /// get or sets if the popu is displayed
        /// </summary>
        public bool IsPopupTextOpened { get; set; }

        /// <summary>
        /// set poputext and display it
        /// </summary>
        /// <param name="text"></param>
        public void ShowPopup(string text)
        {
            PopupText = text;
            IsPopupTextOpened = true;
        }

        #region ModelHost

        /// <summary>
        /// the opened model stack
        /// </summary>
        private readonly ModelStack _openedModelStack;

#if BETA
        [Obsolete("Use CurrentModel property instead. This property will be removed when doing finaly release.", false)]
        public Model ActiveModel { get { return CurrentModel; } }
#endif

        /// <summary>
        /// Gets the current model
        /// </summary>
        public Model CurrentModel
        {
            get { return _openedModelStack.Count > 0 ? _openedModelStack.Peek() : null; }
        }

        /// <summary>
        /// the opened model stack
        /// </summary>
        internal ModelStack OpenedModelStack
        {
            get { return _openedModelStack; }
        }

#if BETA
        [Obsolete("Use CanCloseCurrentModel property instead. This property will be removed when doing finaly release.", false)]
        public bool CanCloseActiveModel { get { return CanCloseCurrentModel; } }
#endif

        /// <summary>
        /// gets if current model could be closed
        /// </summary>
        public bool CanCloseCurrentModel
        {
            get { return OpenedModelStack.Count > 0; }
        }
#if BETA
        [Obsolete("Use OpenModel method instead. This  will be removed when doing finaly release.", false)]
        public void OpenActiveModel(Func<Model> modelProvider)
        {
            OpenModel(modelProvider);
        }
#endif

        /// <summary>
        /// Opens model
        /// </summary>
        /// <param name="modelProvider">the activeModel factory</param>
        public void OpenModel(Func<Model> modelProvider)
        {
            Model model = modelProvider();
            OpenedModelStack.Push(model);
        }

#if BETA
        [Obsolete("Use CloseCurrentModel method instead. This  will be removed when doing finaly release.", false)]
        public void CloseActiveModel()
        {
            CloseCurrentModel();
        }
#endif

        /// <summary>
        /// Close the current model and return to previous one...
        /// </summary>
        public void CloseCurrentModel()
        {
            if (OpenedModelStack.Count > 0)
            {
                _openedModelStack.Pop();
            }
        }

#if BETA
        [Obsolete("Use CloseCurrentModelAndNavigateTo method instead. This  will be removed when doing finaly release.", false)]
        public void CloseActiveModelAndNavigateTo(Func<Model> modelProvider)
        {
            CloseCurrentModelAndNavigateTo(modelProvider);
        }
#endif

        /// <summary>
        /// Close the current model and navigate to model provided
        /// </summary>
        /// <param name="modelProvider"></param>
        public void CloseCurrentModelAndNavigateTo(Func<Model> modelProvider)
        {
            if (OpenedModelStack.Count > 0)
            {
                OpenedModelStack.Pop();
            }
            Model model = modelProvider();

            OpenedModelStack.Push(model);
        }

        #endregion


        public int OpenedModelCount { get { return _openedModelStack.Count; } }

        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            if (propertyName == "CurrentModel")
            {
                if (CurrentModel == null)
                    WindowTitle = WindowTitleString;
                else
                {
                    WindowTitle = WindowTitleString + " - " + CurrentModel.ModelTitle;
                }
            }
            base.OnPropertyChanged(propertyName, before, after);
        }

        #region Nested type: ModelStack

        /// <summary>
        /// Custom stack with model state handling
        /// </summary>
        internal class ModelStack : Stack<Model>
        {
            private readonly BusinessApplication _businessApplication;

            public ModelStack(BusinessApplication businessApplication)
            {
                _businessApplication = businessApplication;
            }

            public new void Pop()
            {
                Model before = base.Pop();

                before.ModelState = ModelState.Deactivated;
                before.ModelState = ModelState.Closed;

                Model after = Count > 0 ? Peek() : null;

                _businessApplication.OnPropertyChanged("CurrentModel", before, after);
                _businessApplication.RaisePropertyChanged("OpenedModelCount");
#if BETA
                _businessApplication.OnPropertyChanged("ActiveModel", before, after);
#endif
                if (after != null) after.ModelState = ModelState.Activated;
            }

            public new void Push(Model model)
            {
                Model before = Count > 0 ? Peek() : null;
                if (before != null)
                    before.ModelState = ModelState.Deactivated;

                base.Push(model);

                _businessApplication.OnPropertyChanged("CurrentModel", before, model);
                _businessApplication.RaisePropertyChanged("OpenedModelCount");
#if BETA
                _businessApplication.OnPropertyChanged("ActiveModel", before, model);
#endif
                model.ModelState = ModelState.Opened;
                model.ModelState = ModelState.Activated;
            }
        }

        #endregion
    }
}