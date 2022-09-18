namespace MathCore.Tests
{
    [TestClass]
    public class GeoLocationTests
    {
        [TestMethod]
        public void LatAngle()
        {
            const double lat = 55.755833;
            const double lon = 37.617778;

            const int expected = 55;

            var location = new GeoLocation(lat, lon);

            Assert.That.Value(location.LatAngle).IsEqual(expected);
        }

        [TestMethod]
        public void LatMinutes()
        {
            const double lat = 55.755833;
            const double lon = 37.617778;

            const int expected = 45;

            var location = new GeoLocation(lat, lon);

            Assert.That.Value(location.LatMinutes).IsEqual(expected);
        }

        [TestMethod]
        public void LatSeconds()
        {
            const double lat = 55.755833;
            const double lon = 37.617778;

            const double expected = 20.9988;

            var location = new GeoLocation(lat, lon);

            Assert.That.Value(location.LatSeconds).IsEqual(expected, 1e-3);
        }

        [TestMethod]
        public void LonAngle()
        {
            const double lat = 55.755833;
            const double lon = 37.617778;

            const int expected = 37;

            var location = new GeoLocation(lat, lon);

            Assert.That.Value(location.LonAngle).IsEqual(expected);
        }

        [TestMethod]
        public void LonMinutes()
        {
            const double lat = 55.755833;
            const double lon = 37.617778;

            const int expected = 37;

            var location = new GeoLocation(lat, lon);

            Assert.That.Value(location.LonMinutes).IsEqual(expected);
        }

        [TestMethod]
        public void LonSeconds()
        {
            const double lat = 55.755833;
            const double lon = 37.617778;

            const double expected = 4;

            var location = new GeoLocation(lat, lon);

            Assert.That.Value(location.LonSeconds).IsEqual(expected, 1e-3);
        }

        [TestMethod]
        public void Location_ToString()
        {
            const double lat = 55.755833;
            const double lon = 37.617778;

            var location = new GeoLocation(lat, lon);

            var str = location.ToString();

            Assert.That.Value(str).AsNotNull().Where(s => s.Length).GreaterThan(0);
            var lat_lon = str.Split(',');
            Assert.That.Value(lat_lon.Length).IsEqual(2);

            var lat_str = lat_lon[0].Trim();
            var lon_str = lat_lon[1].Trim();
            Assert.That.Value(lat_str.Length).GreaterThan(0);
            Assert.That.Value(lon_str.Length).GreaterThan(0);

            Assert.That.Value(lat_str).EndWith("N");
            Assert.That.Value(lon_str).EndWith("E");

            lat_str = lat_str.TrimEnd('N', '\'');
            lon_str = lon_str.TrimEnd('E', '\'');

            Assert.That.Value(lat_str).StartWith("55°45'20.9988");
            Assert.That.Value(lon_str).StartWith("37°37'04.000");
        }

        private readonly GeoLocation _Point1 = new(55.96984993756632, 37.38720697324383);
        private readonly GeoLocation _Point2 = new(55.977960194179026, 37.44150599819203);

        [TestMethod]
        public void Distance()
        {
            const double expected_distance = 3500;

            var distance = _Point1.DistanceTo(_Point2);

            Assert.That.Value(distance).IsEqual(expected_distance, 1e-0);
        }

        [TestMethod]
        public void Heading()
        {
            const double expected_heading = 75;

            var heading = _Point1.HeadingTo(_Point2);

            Assert.That.Value(heading).IsEqual(expected_heading, 1e-0);
        }

        [TestMethod]
        public void Destination()
        {
            var (actual_lat, actual_lon) = _Point1.Destination(75, 3500);

            var (expected_lat, expected_lon) = _Point2;

            Assert.That.Value(actual_lat).IsEqual(expected_lat, 2.02e-5);
            Assert.That.Value(actual_lon).IsEqual(expected_lon, 2.02e-5);
        }
    }
}
