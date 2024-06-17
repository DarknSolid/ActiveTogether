using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseSeeding.Dataextractors.Dogparks
{
    public class OsmNode
    {
        public long Id { get; set; }
        public float Lat { get; set; }
        public float Lon { get; set; }

        public virtual List<(string,string)>? GetTags()
        {
            return null;
        }
    }

    public class OsmNodeTagged : OsmNode
    {
        public List<(string, string)> Tags { get; set; }

        public override List<(string,string)>? GetTags()
        {
            return Tags;
        }

    }

    public class OsmWay
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<OsmNode> Nodes { get; set; }
        public List<(string, string)> Tags { get; set; }
    }
}
