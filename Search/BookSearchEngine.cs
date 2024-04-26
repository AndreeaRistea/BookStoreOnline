using Lucene.Net;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using Microsoft.AspNetCore.Http.HttpResults;
using Proiect.Models;
using Proiect_CE.Models;
namespace Proiect.Search
{
    public class BookSearchEngine
    {
        private readonly Lucene.Net.Util.Version appLuceneVersion;
        private readonly StandardAnalyzer analyzer;
        private readonly RAMDirectory directory;
        private readonly IndexWriter writer;
        public BookSearchEngine()
        {
             appLuceneVersion = Lucene.Net.Util.Version.LUCENE_30;
             analyzer = new StandardAnalyzer(appLuceneVersion);
             directory = new RAMDirectory();
            //var config = new IndexWriterConfig(appLuceneVersion, analyzer);
            writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
        }
        public void AddBooksToIndex(IEnumerable<Book> books)
        {
            foreach (var book in books)
            {
                var document = new Document();

                // Indexarea titlului și descrierii
                document.Add(new Field("Title", book.Title, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                document.Add(new Field("Description", book.Description, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));

                // Indexarea specificațiilor
                foreach (var spec in book.Specifications)
                {
                    document.Add(new Field("SpecificationName", spec.Name, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                    document.Add(new Field("SpecificationValue", spec.Value, Field.Store.YES, Field.Index.NOT_ANALYZED_NO_NORMS));
                }

                writer.AddDocument(document);
            }
            writer.Commit();
        }

        public IEnumerable<Book> Search(string searchTerm)
        {
            var directoryReader = DirectoryReader.Open(directory,false);

            var indexSearcher = new IndexSearcher(directoryReader);

            // Definirea câmpurilor în care se va căuta
            string[] fields = { "SpecificationName", "SpecificationValue", "Title", "Description" };

            var queryParser = new MultiFieldQueryParser(appLuceneVersion, fields, analyzer);
            var query = queryParser.Parse(searchTerm);

            // Căutare în index
            var hits = indexSearcher.Search(query, 10).ScoreDocs;
            var books = new List<Book>();

            foreach (var hit in hits)
            {
                var document = indexSearcher.Doc(hit.Doc);

                // Crearea unui obiect Book pentru fiecare document găsit
                var book = new Book
                {
                    Title = document.Get("Title"),
                    Description = document.Get("Description")
                };

                // Puteți adăuga și alte câmpuri în obiectul Book, cum ar fi Specificațiile
                var specifications = new List<Specification>();
                specifications.Add(new Specification
                {
                    Name = document.Get("SpecificationName"),
                    Value = document.Get("SpecificationValue")
                });
                book.Specifications = specifications;

                books.Add(book);
            }

            return books;
        }
    }
}
