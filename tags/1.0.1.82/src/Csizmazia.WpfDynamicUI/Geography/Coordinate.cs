using System;

namespace Csizmazia.WpfDynamicUI.Geography
{
    public struct Coordinate : IEquatable<Coordinate>
    {
        private readonly double _latidude;

        private readonly double _longitude;

        public Coordinate(double latitude, double longitude)
        {
            _latidude = latitude;
            _longitude = longitude;
        }

        public double Latidude
        {
            get { return _latidude; }
        }

        public double Longitude
        {
            get { return _longitude; }
        }

        #region IEquatable<Coordinate> Members

        public bool Equals(Coordinate other)
        {
            return _latidude.Equals(other._latidude) && _longitude.Equals(other._longitude);
        }

        #endregion
    }
}