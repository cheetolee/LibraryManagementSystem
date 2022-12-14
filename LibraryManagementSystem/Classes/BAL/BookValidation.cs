using LibraryManagementSystem.Interfaces.BAL;
using LibraryManagementSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagementSystem.Classes.BAL
{
    partial class Validation : IValidation
    {
        public bool ValidateBook(Book book)
        {
            if (IsValidISBN(book.ISBN) && !String.IsNullOrWhiteSpace(book.Title) && (book.AvailableCopies <= book.TotalCopies))
            {
                book.PlainISBN = book.ISBN.Replace("-", "");
                return true;
            }
            else
                return false;
        }

        public bool IsValidISBN(string iSBN)
        {
            if (String.IsNullOrWhiteSpace(iSBN))
                return false;

            string plainISBN = iSBN.Replace("-", "");
            int length = plainISBN.Length;

            if (length == 10 || length == 13)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool BookValidForCheckOut(Book book)
        {
            if (book != null && book.BookId != null && book.AvailableCopies > 0)
                return true;
            return false;
        }

        public bool BookValidForCheckIn(Book book)
        {
            if (book != null && book.BookId != null && book.AvailableCopies < book.TotalCopies)
                return true;
            return false;
        }
    }
}
