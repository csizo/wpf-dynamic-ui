using System;
using System.ComponentModel.DataAnnotations;
using Csizmazia.WpfDynamicUI.BusinessModel;
using Data;

namespace Service
{
    public abstract class EntityModel : Model
    {
        private Lazy<DbEntities> _context;

        protected internal DbEntities Context
        {
            get
            {
                if (!_context.IsValueCreated)
                {
                    _context.Value.SavingChanges += (o, e) =>
                                                        {
                                                            IsDirty = false;

                                                        };
                }
                return _context.Value;
            }
        }

        [Display(AutoGenerateField = false)]
        public bool IsDirty { get; protected set; }

        protected override void OnOpened()
        {
            _context = new Lazy<DbEntities>(() => new DbEntities());
            base.OnOpened();
            //CanClose = true;
        }
        protected override void OnClosed()
        {
            if (_context.IsValueCreated)
            {
                _context.Value.Dispose();
            }
            base.OnClosed();
        }

        public void Close()
        {
            Application.CloseCurrentModel();
        }

        //[Display(AutoGenerateField = false)]
        //public bool CanClose { get; private set; }


        protected override void OnPropertyChanged(string propertyName, object before, object after)
        {
            //if (propertyName == "IsDirty")
            //{
            //    CanClose = !IsDirty;
            //}
            //else if (propertyName != "CanClose")
            //{
            //    IsDirty = true;
            //}
            IsDirty = true;

            base.OnPropertyChanged(propertyName, before, after);
        }
    }
}