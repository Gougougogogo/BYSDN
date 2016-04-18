using BYSDN.Entity;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Search.Highlight;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Version = Lucene.Net.Util.Version;

namespace BYSDN.Lucene
{
    public class Lucener
    {
        private static string indexerPath 
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory+"/Indexer";
            }
        }
        public static bool IsInitalized
        {
            get
            {
                if (System.IO.Directory.Exists(indexerPath))
                {
                    return System.IO.Directory.EnumerateFiles(indexerPath).Count() > 0 ? true : false;
                }
                else
                {
                    return false;
                }
            }
        }
        /// <summary>
        /// Import all data from data base
        /// </summary>
        public void ImportData()
        {
            IndexWriter writer = new IndexWriter(FSDirectory.Open(indexerPath), new StandardAnalyzer(Version.LUCENE_30), true, IndexWriter.MaxFieldLength.LIMITED);
            writer.UseCompoundFile = true;

            BYSDNEntities entities = new BYSDNEntities();
            var result = (from a in entities.Table_Question
                          select new
                          {
                              ID = a.ID,
                              Title = a.Tittle,
                              Content = a.Content,
                              Tags = a.Tags
                          }).ToList();

            foreach (var obj in result)
            {
                Document doc = new Document();

                doc.Add(new Field("content", ParseHtml(obj.Content) + " " + obj.Title + " " + obj.Tags, Field.Store.YES, Field.Index.ANALYZED));
                doc.Add(new Field("contentid", obj.ID.ToString(), Field.Store.YES, Field.Index.NO));
                doc.Add(new Field("type", "BBS", Field.Store.YES, Field.Index.NO));

                writer.AddDocument(doc);
            }

            writer.Optimize();
            writer.Commit();
            writer.Dispose();
        }

        public List<SearchResult> Search(string key, int resultCount)
        {
            List<SearchResult> result = new List<SearchResult>();

            //init search engine
            var analyzer = new StandardAnalyzer(Version.LUCENE_30);
            IndexSearcher searcher = new IndexSearcher(FSDirectory.Open(indexerPath));

            // parse the query, "content" is the default field to search
            var parser = new QueryParser(Version.LUCENE_30, "content", analyzer);
            Query query = parser.Parse(key);

            //Get highlighter 
            IFormatter formatter = new SimpleHTMLFormatter("<span style=\"font-weight:bold;\">", "</span>");
            SimpleFragmenter fragmenter = new SimpleFragmenter(360);
            QueryScorer scorer = new QueryScorer(query);
            Highlighter highlighter = new Highlighter(formatter, scorer);
            highlighter.TextFragmenter = fragmenter;

            // search
            TopDocs hits = searcher.Search(query, 200);
            for (int i = 0; i < hits.TotalHits; i++)
            {
                // get the document from index
                Document doc = searcher.Doc(hits.ScoreDocs[i].Doc);

                TokenStream stream = analyzer.TokenStream("", new StringReader(doc.Get("content")));
                String sample = highlighter.GetBestFragments(stream, doc.Get("content"), 2, "...");

                result.Add(new SearchResult(doc,sample,hits.ScoreDocs[i].Score));
            }

            searcher.Dispose();

            return result;
        }

        public void AddData(string Id, string content, string tags, string title, string type)
        {
            IndexWriter writer = new IndexWriter(FSDirectory.Open(indexerPath), new StandardAnalyzer(Version.LUCENE_30), false, IndexWriter.MaxFieldLength.LIMITED);
            Document doc = new Document();

            doc.Add(new Field("content", ParseHtml(content) + title + tags, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("contentid", Id, Field.Store.YES, Field.Index.NO));
            doc.Add(new Field("type", type, Field.Store.YES, Field.Index.NO));

            writer.AddDocument(doc);
            writer.Commit();
            writer.Dispose();
        }

        /// <summary>
        /// Very simple, inefficient, and memory consuming HTML parser. Take a look at Demo/HtmlParser in DotLucene package for a better HTML parser.
        /// </summary>
        /// <param name="html">HTML document</param>
        /// <returns>Plain text.</returns>
        private string ParseHtml(string html)
        {
            string temp = Regex.Replace(html, "<[^>]*>", "");
            temp = temp.Replace(',',' ');
            temp = temp.Replace('.', ' '); 
            temp = temp.Replace('?', ' ');
            temp = temp.Replace('，',' ');
            temp = temp.Replace('。', ' ');
            return temp.Replace("&nbsp;", " ");
        }
    }
}