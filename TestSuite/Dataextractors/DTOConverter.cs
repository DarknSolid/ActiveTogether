using DatabaseSeeding.Dataextractors.Dogparks;
using EntityLib.Entities;
using ModelLib.DTOs.DogPark;
using NetTopologySuite.Geometries;
using NpgsqlTypes;
using static EntityLib.Entities.Enums;

namespace DatabaseSeeding.Dataextractors
{
    public static class DTOConverter
    {
        public static List<DogParkCreateDTO> ConvertToDogParkCreateDTO(List<OsmWay> osmWays)
        {
            var dtos = new List<DogParkCreateDTO>();

            foreach (var way in osmWays)
            {
                var polygon = GetPolygonFromNodes(way.Nodes);
                var center = GetCenterPolygon(polygon);
                dtos.Add(new DogParkCreateDTO
                {
                    Name = way.Name,
                    Description = "This dogpark was imported from OpenStreetMap",
                    Point = new NpgsqlPoint(x:center.X, y:center.Y),
                    Bounds = polygon,
                    SquareKilometers = (float)CalculatePolygonAreaSquareMeters(polygon) / 1_000_000f,
                    Facilities = GetFacilitiesFromOsmWay(way)
                });
            }
            return dtos;
        }

        private static List<DogParkFacilityType> GetFacilitiesFromOsmWay(OsmWay way)
        {
            var result = new HashSet<DogParkFacilityType>();
            var escapedTags = new HashSet<string>();
            List<(string, string)> tags = way.Nodes.SelectMany(n => n.GetTags() ?? new()).ToList();
            tags.AddRange(way.Tags);

            // assume that the park is leashed:
            result.Add(DogParkFacilityType.Leashed);

            tags.ForEach(kv =>
            {
                var k = kv.Item1;
                var v = kv.Item2;
                switch (k)
                {
                    case "barrier":
                        if (v == "fence")
                        {
                            result.Add(DogParkFacilityType.Fenced);
                        }
                        break;
                    case "landuse":
                        if (v == "forest")
                        {
                            result.Add(DogParkFacilityType.Forest);
                        }
                        break;
                    case "leisure":
                        if (v == "dog_park")
                        {
                            result.Remove(DogParkFacilityType.Leashed);
                            result.Add(DogParkFacilityType.Unleashed);
                        }
                        break;
                    case "dog":
                        if (v == "leashed")
                        {
                            if (result.Contains(DogParkFacilityType.Unleashed))
                            {
                                throw new ArgumentException($"Way {way.Id} contains dog:leashed and dog:unleashed tags.");
                            }
                            result.Add(DogParkFacilityType.Leashed);
                        }
                        else if (v == "unleashed")
                        {
                            result.Remove(DogParkFacilityType.Leashed);
                            result.Add(DogParkFacilityType.Unleashed);
                        }
                        break;
                    case "access":
                        if (v == "private")
                        {
                            result.Add(DogParkFacilityType.Private);
                        }
                        break;
                    default:
                        escapedTags.Add($"{k}: {v}");
                        break;
                }
            });

            //Console.WriteLine("Escaped tags: ");
            //escapedTags.ToList().ForEach(tag => Console.WriteLine(tag));

            return result.ToList();
        }

        public static NpgsqlPolygon GetPolygonFromNodes(List<OsmNode> nodes)
        {
            var coordinates = nodes.Select(n =>
                new NpgsqlPoint
                {
                    X = n.Lon,
                    Y = n.Lat
                }
            );

            return new NpgsqlPolygon(coordinates);
        }

        public static Point GetCenterPolygon(NpgsqlPolygon polygon)
        {
            Polygon polygon1 = new Polygon(new LinearRing(polygon.Select(p => new Coordinate { X = p.X, Y = p.Y }).ToArray()));
            return polygon1.Centroid;
        }

        /// <summary>
        /// Taken from: https://stackoverflow.com/questions/2861272/polygon-area-calculation-using-latitude-and-longitude-generated-from-cartesian-s
        /// </summary>
        /// <param name="polygon"></param>
        /// <returns></returns>
        private static double CalculatePolygonAreaSquareMeters(NpgsqlPolygon polygon)
        {
            double area = 0;

            if (polygon.Count > 2)
            {
                for (var i = 0; i < polygon.Count - 1; i++)
                {
                    var p1 = polygon[i];
                    var p2 = polygon[i + 1];
                    area += ConvertToRadian(p2.X - p1.X) * (2 + Math.Sin(ConvertToRadian(p1.Y)) + Math.Sin(ConvertToRadian(p2.Y)));
                }

                // 6378137 is the value of earth radius in metres
                area = area * 6371009 * 6371009 / 2;
            }

            return Math.Abs(area);
        }

        private static double ConvertToRadian(double input)
        {
            return input * Math.PI / 180;
        }
    }
}
