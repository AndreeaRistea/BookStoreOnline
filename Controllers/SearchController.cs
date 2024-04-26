using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Proiect.Search;
using Proiect_CE.Data;
using Proiect_CE.Models;
using static Lucene.Net.Search.SimpleFacetedSearch;

namespace Proiect.Controllers
{
    public class SearchController : Controller
    {
        private readonly string _indexPath = Path.Combine(AppContext.BaseDirectory, "indexDirectory");
        private readonly BookStoreContext _dbContext; 

        public SearchController(BookStoreContext dbContext)
        {
            _dbContext = dbContext;

            if (!System.IO.Directory.Exists(_indexPath))
            {
                System.IO.Directory.CreateDirectory(_indexPath);
            }
            IndexData();

        }

        private void IndexData()
        {
            using var directory = FSDirectory.Open(new DirectoryInfo(_indexPath));

            using var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);

            using var indexWriter = new IndexWriter(directory, analyzer, true, IndexWriter.MaxFieldLength.LIMITED);

            var books = _dbContext.Books.Include(b => b.Specifications).ToList();

            foreach (var book in books)
            {
                var document = new Document();
     
                document.Add(new Field("ISBN", book.ISBN, Field.Store.YES, Field.Index.ANALYZED));
                document.Add(new Field("Title", book.Title, Field.Store.YES, Field.Index.ANALYZED));
                document.Add(new Field("Description", book.Description, Field.Store.YES, Field.Index.ANALYZED));

                foreach (var spec in book.Specifications)
                {
                    document.Add(new Field("SpecificationName", spec.Name, Field.Store.YES, Field.Index.ANALYZED));
                    document.Add(new Field("SpecificationValue", spec.Value, Field.Store.YES, Field.Index.ANALYZED));
                }

                indexWriter.AddDocument(document);
            }

            indexWriter.Commit();
        }


        public IActionResult Search(string searchTerm, string sortOrder = "relevanceAsc")
        {
            using var directory = FSDirectory.Open(new DirectoryInfo(_indexPath));
            using var analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            using var indexReader = IndexReader.Open(directory, false);
            using var indexSearcher = new IndexSearcher(indexReader);

            var queryParser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30,
                new[] { "Title", "Description", "SpecificationName", "SpecificationValue" }, analyzer);
            var query = queryParser.Parse(searchTerm);
            Sort sort;
            if (sortOrder == "relevanceAsc")
            {
                sort = new Sort(new SortField("SpecificationValue", SortField.STRING, true));
            }
            else if (sortOrder == "relevanceDesc")
            {
                sort = new Sort(new SortField("SpecificationValue", SortField.STRING, false));
            }
            //else
            //{
                sort = Sort.RELEVANCE;
            //}

            var hits = indexSearcher.Search(query, null, 10, sort);
            var foundBooks = new List<Book>();

            foreach (var hit in hits.ScoreDocs)
            {
                var doc = indexSearcher.Doc(hit.Doc);
                Console.WriteLine(hit.Score);
                var isbn = doc.Get("ISBN");

                var book = _dbContext.Books.Include(b => b.Specifications)
                    .FirstOrDefault(b => b.ISBN == isbn);

                if (book != null)
                {
                    foundBooks.Add(book);
                    
                }
            }

            return View("SearchResult", foundBooks);
        }
    }
}

