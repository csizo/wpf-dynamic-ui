namespace Csizmazia.WpfDynamicUI.BusinessModel
{
    public interface IModel
    {
        BusinessApplication Application { get; }
        string ModelTitle { get; }
        ModelState ModelState { get; }
    }
}