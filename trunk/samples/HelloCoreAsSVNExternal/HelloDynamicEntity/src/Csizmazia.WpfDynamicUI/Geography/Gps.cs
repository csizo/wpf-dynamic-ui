using System;
using System.Threading;
using GMap.NET;
using GMap.NET.MapProviders;

namespace Csizmazia.WpfDynamicUI.Geography
{
    public class Gps
    {
        public static Coordinate Locate(string location)
        {
            GeoCoderStatusCode status;

            PointLatLng? pos = GMapProviders.GoogleMap.GetPoint(location, out status);
            while (status == GeoCoderStatusCode.G_GEO_TOO_MANY_QUERIES)
            {
                //if too many queries error sleep a while and try again..
                Thread.Sleep(TimeSpan.FromSeconds(1));
                return Locate(location);
            }
            switch (status)
            {
                case GeoCoderStatusCode.Unknow:
                    break;
                case GeoCoderStatusCode.G_GEO_SUCCESS:
                    return new Coordinate(pos.Value.Lat, pos.Value.Lng);
                    break;
                case GeoCoderStatusCode.G_GEO_UNKNOWN_ADDRESS:
                    throw new InvalidOperationException("Unknown address.");
                    break;
                case GeoCoderStatusCode.G_GEO_BAD_REQUEST:
                    throw new InvalidOperationException();
                    break;
                case GeoCoderStatusCode.G_GEO_SERVER_ERROR:
                    throw new InvalidOperationException("Geo server is down.");
                    break;
                case GeoCoderStatusCode.G_GEO_MISSING_QUERY:
                    throw new InvalidOperationException();
                    break;
                case GeoCoderStatusCode.G_GEO_UNAVAILABLE_ADDRESS:
                    throw new InvalidOperationException("Address is not public");
                    break;
                case GeoCoderStatusCode.G_GEO_UNKNOWN_DIRECTIONS:
                    throw new InvalidOperationException();
                    break;
                case GeoCoderStatusCode.G_GEO_BAD_KEY:
                    throw new InvalidOperationException();
                    break;
                case GeoCoderStatusCode.ExceptionInCode:
                    throw new InvalidOperationException();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            throw new InvalidOperationException();
        }
    }
}