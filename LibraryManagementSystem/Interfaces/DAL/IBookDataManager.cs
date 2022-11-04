using LibraryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Interfaces.DAL
{
    public interface IBookDataManager
    {
        Task<bool> CreateBook(Book book);

        Task<bool> UpdateBook(Book book);

        Task<bool> DeleteBook(string bookId);

        Task<Book> GetBook(string bookId);

        Task<List<Book>> SearchBooks(string query);

        Task<bool> CheckOutBook(BookTransaction transaction);

        Task<bool> CheckInBook(BookTransaction transaction);

        Task<IEnumerable<BookTransaction>> GetBookTransactionHistory(string bookId);
    }
}
