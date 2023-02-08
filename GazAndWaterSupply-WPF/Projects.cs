using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GazAndWaterSupply_WPF
{
    internal class Projects
    {
        [BsonId]
        [BsonIgnoreIfDefault]
        public ObjectId _id;
        public string Department { get; set; }
        public string Customer { get; set; }
        public string Developer { get; set; }
        public string Designer { get; set; }
        [BsonIgnoreIfNull]
        public List<Document> Documents { get; set; }
    }
}
