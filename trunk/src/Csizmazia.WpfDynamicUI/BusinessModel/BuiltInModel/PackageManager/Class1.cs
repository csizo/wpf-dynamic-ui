using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using Csizmazia.Collections;
using Csizmazia.WpfDynamicUI.Collections;

namespace Csizmazia.WpfDynamicUI.BusinessModel.BuiltInModel.PackageManager
{
    //public class PackageManager : NavigationModel
    //{


    //    [UIHint(UIHints.DisplayDataGrid)]
    //    public TweakedObservableCollection<Package> Packages { get; private set; }

    //    public Package SelectedPackagesItem { get; set; }


    //    protected override void OnOpened()
    //    {
    //        var reader = XmlReader.Create("http://wpfdynamicui.codeplex.com/project/feeds/rss?ProjectRSSFeed=codeplex%3a%2f%2frelease%2fwpfdynamicui");
    //        var feed = SyndicationFeed.Load(reader);

    //        ////Find items by Jesper
    //        //feed.Items.Where(i => i.Authors.Any(p => p.Name == "Jesper"));

    //        //Order by publish date
    //        var ordered = from f in feed.Items.OrderBy(i => i.PublishDate)
    //                      select new Package(f);
                              
    //        Packages = new TweakedObservableCollection<Package>(ordered);
    //        //Packages.AddRange(ordered);

    //        base.OnOpened();
    //    }

    //    public static void OpenPackageManager(AboutModel aboutModel)
    //    {
    //        OpenModel<PackageManager>();
    //    }

    //}

    //public class Package : NotifyPropertyChanged
    //{
    //    private readonly SyndicationItem _syndicationItem;

    //    public string Name { get; set; }
    //    public string Description { get; set; }
    //    public string Url { get; set; }
    //    public Package(SyndicationItem syndicationItem)
    //    {
    //        _syndicationItem = syndicationItem;
    //        Name = _syndicationItem.Title.Text;
    //        Description = _syndicationItem.Summary.Text;
    //        Url = _syndicationItem.Links.FirstOrDefault().Uri.ToString();

    //    }

    //    public void Deploy() { }
    //    public void Delete() { }
    //}
    //public class Deployment : NotifyPropertyChanged
    //{

    //}
}
