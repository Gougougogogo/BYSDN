using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BYSDN.Lucene
{
    public class SearchResult
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public string Sample { get; set; }
        public string Type { get; set; }
        public float Score { get; set; }
        public SearchResult(Document searchDoc,string sample,float score)
        {
            this.Id = searchDoc.Get("contentid");
            this.Type = searchDoc.Get("type");
            this.Sample = sample;
            this.Score = score;
        }
    }
}