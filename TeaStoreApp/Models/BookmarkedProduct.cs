using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeaStoreApp.Models
{
    public class BookmarkedProduct
    {
        [PrimaryKey,AutoIncrement]
        public int Id { get; set; }  // chiave primaria, autoincrementata

        public int ProductId { get; set; }  // prodotto tra favorites

        public string Name {  get; set; }

        public string Detail {  get; set; }

        public int Price { get; set; }

        public string ImageUrl { get; set; }

        public bool IsBookmarked { get; set; }


    }
}
