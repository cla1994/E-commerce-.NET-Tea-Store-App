using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeaStoreApp.Models;

namespace TeaStoreApp.Services
{
    public class BookmarkItemService
    {
        // servono path dove salvo database; sqlite connection che prende path; creare tabella del database

        private readonly SQLiteConnection _database;

        public BookmarkItemService() 
        {
            // combino nome cartella e nome database
            var dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "entities.db");
            // inizializzo connessione
            _database = new SQLiteConnection(dbPath);
            // creo tabella db con sue proprietà (creo nuova classe in models, bookmarkedproduct)
            _database.CreateTable<BookmarkedProduct>();
        }

        public BookmarkedProduct Read(int id)
        {
           return _database
                .Table<BookmarkedProduct>()
                .Where(p => p.ProductId == id)
                .FirstOrDefault();  
            // linq, legge singolo elemento da tabella
        }

        public List<BookmarkedProduct> ReadAll()
        {
            return _database
                 .Table<BookmarkedProduct>()
                 .ToList();            
        }

        public void Create(BookmarkedProduct bookmarkedProducts)
        {
            _database.Insert(bookmarkedProducts);  // inserisce nel db
        }

        public void Delete(BookmarkedProduct bookmarkedProducts)
        {
            _database.Delete(bookmarkedProducts); 
        }
    }
}
