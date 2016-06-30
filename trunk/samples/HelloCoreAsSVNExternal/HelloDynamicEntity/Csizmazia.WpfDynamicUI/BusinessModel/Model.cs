using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using Csizmazia.Discovering;
using Container = Csizmazia.WpfDynamicUI.MefComposition.Container;

namespace Csizmazia.WpfDynamicUI.BusinessModel
{
    /// <summary>
    /// Base class for all Workflow models.
    /// <para>Workflow model can be instanciated frequently... Use OnOpened and OnClosed overrides to perform time consuming operations - initialization and cleanup.</para>
    /// </summary>
    [InheritedExport(typeof(IModel))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public abstract class Model : NotifyPropertyChanged, IDataErrorInfo, IModel
    {
        private static readonly string[] NonValidatedProperties = new[] { "Error", "ModelState", "ModelTitle" };

        /// <summary>
        /// properties error messages
        /// </summary>
        private readonly Dictionary<string, string> _validationErrorDictionary = new Dictionary<string, string>();

        /// <summary>
        /// list of exported <see cref="Model"/>s
        /// </summary>
        private List<Model> _exportedModels;

        /// <summary>
        /// list of Mef <see cref="Model"/> exports
        /// </summary>
        private IEnumerable<Lazy<IModel>> _mefExportedModels;

        /// <summary>
        /// Model state
        /// </summary>
        private ModelState _modelState;

        protected Model()
        {
            ModelTitle = GetType().Name;
        }

        [Display(AutoGenerateField = false)]
        public string HelpText { get; protected set; }

        protected IEnumerable<T> FindModel<T>() where T : Model
        {
            return ExportedModels.OfType<T>();
        }

        /// <summary>
        /// Gets the exported models
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        protected IEnumerable<Model> ExportedModels
        {
            get
            {
                if (_mefExportedModels == null)
                    _mefExportedModels = Container.Instance.GetExports<IModel>();

                _exportedModels = new List<Model>(_mefExportedModels.Select(m => m.Value).OfType<Model>());
                return _exportedModels;
            }
        }

        #region IDataErrorInfo Members

        /// <summary>
        /// Gets the model Aggregated Error(s)
        /// </summary>
        [Display(AutoGenerateField = false)]
        public string Error
        {
            get { return string.Join(Environment.NewLine, _validationErrorDictionary.Values); }
        }

        /// <summary>
        /// Validates the model property
        /// </summary>
        /// <param name="propertyName">The property name</param>
        /// <returns>Error for the property</returns>
        [Display(AutoGenerateField = false)]
        public string this[string propertyName]
        {
            get
            {
                string value;
                _validationErrorDictionary.TryGetValue(propertyName, out value);
                return value;
            }
        }

        #endregion


        /// <summary>
        /// Gets the <see cref="BusinessApplication"/> instance
        /// </summary>
        [Display(AutoGenerateField = false)]
        public BusinessApplication Application
        {
            get { return BusinessApplication.Instance; }
        }

        /// <summary>
        /// Gets the ModelTitle property
        /// </summary>
        [Display(AutoGenerateField = false)]
        public string ModelTitle { get; protected set; }

        /// <summary>
        /// Gets the model state
        /// </summary>
        [Display(AutoGenerateField = false)]
        public ModelState ModelState
        {
            get { return _modelState; }
            internal set
            {
                if (_modelState != value)
                {
                    ModelState before = _modelState;

                    _modelState = value;

                    switch (value)
                    {
                        case ModelState.Opened:
                            HandleOpened();
                            break;
                        case ModelState.Activated:
                            HandleActivated();
                            break;
                        case ModelState.Deactivated:
                            HandleDeactivated();
                            break;
                        case ModelState.Closed:
                            HandleClosed();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("value");
                    }
                    OnPropertyChanged("ModelState", before, value);
                }
            }
        }


        /// <summary>
        /// Validates this model
        /// </summary>
        /// <returns>true if model is valid</returns>
        protected bool ValidateModel()
        {
            //todo cache properties
            return GetType().GetInstanceProperties()
                .Where(pi => pi.GetGetMethod().IsPublic)
                .Select(pi => pi.Name)
                .Aggregate(true, (current, propertyName) => current & this[propertyName] == null);
        }

        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            ValidateProperty(propertyName, before, after);


            base.OnPropertyChanged(propertyName, before, after);
        }

        private void ValidateProperty(string propertyName, object before, object after)
        {
            //Validation if property is public and not in blacklist
            if (NonValidatedProperties.Contains(propertyName) ||
                GetType().GetInstanceProperties().Any(pi => pi.Name == propertyName) == false)
                return;


            var context = new ValidationContext(this, null, null)
                              {
                                  MemberName = propertyName
                              };
            var results = new List<ValidationResult>();
            if (Validator.TryValidateProperty(after, context, results))
            {
                if (_validationErrorDictionary.ContainsKey(propertyName))
                {
                    _validationErrorDictionary.Remove(propertyName);
                    RaisePropertyChanged("Error");
                }
            }
            else
            {
                _validationErrorDictionary[propertyName] = string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage));
                RaisePropertyChanged("Error");
            }
        }


        private void HandleOpened()
        {
            //handle opened state transition
            OnOpened();
        }

        private void HandleActivated()
        {
            //handle activated state transition
            OnActivated();
        }

        private void HandleDeactivated()
        {
            //handle deactivated state transition
            OnDeactivated();
        }

        private void HandleClosed()
        {
            //handle closed state transition


            if (_mefExportedModels != null)
            {
                //releasing exported models
                _exportedModels.Clear();
                Container.Instance.ReleaseExports(_mefExportedModels);
            }

            OnClosed();
        }

        /// <summary>
        /// called when model is opened (new instance is about to be displayed)
        /// </summary>
        protected virtual void OnOpened() { }

        /// <summary>
        /// called when model is activated
        /// </summary>
        protected virtual void OnDeactivated() { }

        /// <summary>
        /// called when model is deactivated
        /// </summary>
        protected virtual void OnActivated() { }

        /// <summary>
        /// called when model is about to be closed...
        /// </summary>
        protected virtual void OnClosed() { }

        /// <summary>
        /// Opens the specified model
        /// <para>Shortcut for BusinessApplication.Instance.OpenModel(() =&gt; new TModel())</para>
        /// </summary>
        /// <typeparam name="TModel">Model to open</typeparam>
        protected static void OpenModel<TModel>() where TModel : Model, new()
        {
            BusinessApplication.Instance.OpenModel(() => new TModel());
        }
    }


}