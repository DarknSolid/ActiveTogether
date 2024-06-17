using Newtonsoft.Json;
using System.Globalization;
using System.Xml;
using TestSuite;

namespace DatabaseSeeding.Dataextractors.Dogparks
{
    public class OpenStreetMapExtractor
    {
        private XmlTextReader _xtr;
        private HashSet<long> _targetedWayIds = new();
        private HashSet<long> _targetedNodeIds = new();
        private long _totalLineCount;

        /// <summary>
        /// The relation search first scans the xml file's "way" nodes and saves the relevant element ids. 
        /// The second scan then only saves the xml elements that are related to all dog park "way" elements
        /// </summary>
        /// <param name="filePath">the path to the osm file from OpenStreetMap</param>
        /// <returns></returns>
        public List<OsmWay> Run(string filePath, string? saveFolderPath = null)
        {
            var oldNumberDecimalSeparator = SetNumberDecimalSeparatorTo(".");

            _totalLineCount = CountLinesReader(filePath);
            _targetedWayIds.Clear();
            _targetedNodeIds.Clear();

            Console.WriteLine("Opening file...");
            _xtr = new XmlTextReader(filePath);
            Console.WriteLine("done.");

            FindDogParkWayRelations();
            _xtr.Close();

            _xtr = new XmlTextReader(filePath);

            var ways = new List<OsmWay>();
            var nodeIdToNode = new Dictionary<long, OsmNode>();
            using (var progressBar = new ProgressBar())
            {
                Console.WriteLine("Extracting Dog Parks...");
                while (_xtr.Read())
                {
                    progressBar.Report((double)_xtr.LineNumber / _totalLineCount);

                    if (_xtr.NodeType == XmlNodeType.Element)
                    {
                        switch (_xtr.Name)
                        {
                            case "node":
                                var node = ParseNode();
                                if (node != null)
                                {
                                    nodeIdToNode.Add(node.Id, node);
                                }
                                break;

                            case "way":
                                var way = ParseWay(nodeIdToNode);
                                if (way != null)
                                {
                                    ways.Add(way);
                                }
                                break;
                        }
                    }
                }
                _xtr.Close();
                Console.WriteLine("done.");
            }
            if (saveFolderPath != null)
            {
                using (StreamWriter w = new StreamWriter(saveFolderPath + $"{DateTime.Now.ToString("yyyy-M-d H-m-s")}-OpenStreetMapExctraction.json"))
                {
                    JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                    string json = JsonConvert.SerializeObject(ways, settings);
                    w.Write(json);
                }
            }
            SetNumberDecimalSeparatorTo(oldNumberDecimalSeparator);
            return ways;
        }

        private OsmNode? ParseNode()
        {
            long.TryParse(_xtr.GetAttribute("id"), out long nodeId);
            if (!_targetedNodeIds.Contains(nodeId))
            {
                return null;
            }
            float.TryParse(_xtr.GetAttribute("lat"), out float lat);
            float.TryParse(_xtr.GetAttribute("lon"), out float lon);
            OsmNode node;
            if (!_xtr.IsEmptyElement)
            {
                var nodeTags = ReadNodeContent();
                node = new OsmNodeTagged
                {
                    Id = nodeId,
                    Lat = lat,
                    Lon = lon,
                    Tags = nodeTags
                };
            }
            else
            {
                node = new OsmNode
                {
                    Id = nodeId,
                    Lat = lat,
                    Lon = lon
                };
            }
            return node;
        }

        private OsmWay? ParseWay(Dictionary<long, OsmNode> nodeIdToNode)
        {
            long.TryParse(_xtr.GetAttribute("id"), out long wayId);
            if (!_targetedWayIds.Contains(wayId))
            {
                return null;
            }

            var (nodeRefs, wayTags) = ReadWayContent();
            var wayNodes = new List<OsmNode>();
            var nodes = new List<OsmNode>();

            foreach (var nodeRef in nodeRefs)
            {
                nodes.Add(nodeIdToNode[nodeRef]);
            }

            return new OsmWay
            {
                Id = wayId,
                Name = wayTags.FirstOrDefault(kv => kv.Item1 == "name").Item2 ?? "<NO NAME>",
                Nodes = nodes,
                Tags = wayTags
            };
        }

        private static string SetNumberDecimalSeparatorTo(string decimalSeparator)
        {
            var oldCulture = Thread.CurrentThread.CurrentCulture;
            CultureInfo currentCulture = (CultureInfo) Thread.CurrentThread.CurrentCulture.Clone();
            currentCulture.NumberFormat.NumberDecimalSeparator = decimalSeparator;
            Thread.CurrentThread.CurrentCulture = currentCulture;
            return oldCulture.NumberFormat.NumberDecimalSeparator;
        }

        private long CountLinesReader(string filePath)
        {
            var lineCounter = 0L;
            using (var reader = new StreamReader(filePath))
            {
                while (reader.ReadLine() != null)
                {
                    lineCounter++;
                }
                return lineCounter;
            }
        }

        public List<OsmWay> LoadPreviousExtraction(string filePath)
        {
            List<OsmWay> items = null;
            using (StreamReader r = new StreamReader(filePath))
            {
                JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<OsmWay>>(json, settings);
            }
            return items;
        }

        private void FindDogParkWayRelations()
        {
            Console.WriteLine("Filtering for way elements with the dog_park leisure tag...");
            using (var progressBar = new ProgressBar())
            {
                while (_xtr.Read())
                {
                    progressBar.Report((double)_xtr.LineNumber / _totalLineCount);
                    if (_xtr.NodeType == XmlNodeType.Element && _xtr.Name == "way")
                    {

                        long.TryParse(_xtr.GetAttribute("id"), out long wayId);

                        var (nodeRefs, wayTags) = ReadWayContent();

                        // skips ways without a name only if it is not a dog park
                        if (!wayTags.Any(kv => kv.Item1 == "name") && !wayTags.Any(kv => kv.Item2 == "dog_park"))
                        {
                            continue;
                        }

                        if (!wayTags.Any(kv => kv.Item2 == "dog_park" ||
                                        (kv.Item1 == "dog" && kv.Item2 == "yes") ||
                                        (kv.Item1 == "dog" && kv.Item2 == "leashed") ||
                                        (kv.Item1 == "dog" && kv.Item2 == "unleashed") ||
                                        (kv.Item1 == "leisure" && kv.Item2 == "park"))
                            ||
                            wayTags.Any(kv => kv.Item1 == "crossing" || kv.Item1 == "crossing:island" || kv.Item1 == "highway")
                            )
                        {
                            continue;
                        }

                        _targetedWayIds.Add(wayId);

                        foreach (var id in nodeRefs)
                        {
                            _targetedNodeIds.Add(id);
                        }
                    }
                }
            }
            Console.WriteLine("done.");
        }

        private (List<long>, List<(string, string)>) ReadWayContent()
        {
            var nodeRefs = new List<long>();
            var tags = new List<(string, string)>();

            while (_xtr.Read())
            {
                if (_xtr.NodeType == XmlNodeType.EndElement && _xtr.Name == "way")
                {
                    break;
                }
                else if (_xtr.NodeType == XmlNodeType.Element)
                {
                    switch (_xtr.Name)
                    {
                        case "tag":
                            var key = _xtr.GetAttribute("k");
                            var value = _xtr.GetAttribute("v");
                            tags.Add((key, value));
                            break;
                        case "nd":
                            nodeRefs.Add(long.Parse(_xtr.GetAttribute("ref")));
                            break;
                    }
                }
            }
            return (nodeRefs, tags);
        }

        private List<(string, string)> ReadNodeContent()
        {
            var tags = new List<(string, string)>();
            while (_xtr.Read())
            {
                if (_xtr.NodeType == XmlNodeType.EndElement && _xtr.Name == "node")
                {
                    break;
                }
                else if (_xtr.NodeType == XmlNodeType.Element && _xtr.Name == "tag")
                {
                    var key = _xtr.GetAttribute("k");
                    var value = _xtr.GetAttribute("v");
                    tags.Add((key, value));
                }
            }

            return tags;
        }
    }
}
