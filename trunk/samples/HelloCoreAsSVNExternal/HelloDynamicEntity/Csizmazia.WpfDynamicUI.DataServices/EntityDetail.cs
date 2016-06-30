using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using Csizmazia.WpfDynamicUI.BusinessModel;

namespace Csizmazia.WpfDynamicUI.DataServices
{
    public abstract class EntityDetail<TContext, TEntity> : NavigationModel
        where TContext : DbContext, new()
        where TEntity : class
    {
        [Display(AutoGenerateField = false)]
        public TEntity Item { get; set; }

        /*
        CodeGen properties here...
         * 
         * getters,setters,reference lookups, collection editors...
        */


        public void Reset()
        {
        }

        public void Save()
        {
            //base.Repository.Context.Entry(item).
        }

        public void Delete()
        {
        }

        public static void Open(EntityListModel<TContext, TEntity> listModel)
        {
        }
    }
}