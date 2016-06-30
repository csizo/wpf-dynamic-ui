using System.Collections;

namespace Csizmazia.Collections
{
    /// <summary>
    /// Marker interface for a PagedQueryable
    /// </summary>
    public interface IPagedQueryable
    {
        string SortColumn { get; set; }
        SortDirection SortDirection { get; set; }
        bool CanMoveFirst { get; }
        bool CanMovePrevious { get; }
        bool CanMoveNext { get; }
        bool CanMoveLast { get; }
        void BeginInit();
        void EndInit();

        void ChangeSelection(IList removedItems, IList addedItems);

        void MoveFirst();


        void MovePrevious();

        void MoveNext();

        void MoveLast();
    }
}