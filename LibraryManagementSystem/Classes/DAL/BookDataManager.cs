using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using MySql.Data.MySqlClient;
using Dapper;
using LibraryManagementSystem.Interfaces.DAL;
using Newtonsoft.Json;
using System.Windows;
using Newtonsoft.Json.Linq;

namespace LibraryManagementSystem.Classes.DAL
{
    public class BookDataManager : IBookDataManager
    {
        //private readonly string connectionString = ConfigurationManager.ConnectionStrings["lms-localDB"].ConnectionString;
        private readonly string connectionString;

        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "Nfjq4tAqxBmSO8zzrafd10JJVtPUSiIaXIh6mr2J",
            BasePath = "https://library-a972e-default-rtdb.firebaseio.com/"

        };

        IFirebaseClient client;

        public bool CreateConnection()
        {
            client = new FireSharp.FirebaseClient(config);
            if (client != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> CreateBook(Book book)
        {
            bool success = true;
            try
            {
                client = new FireSharp.FirebaseClient(config);
                var data = book;
                PushResponse response = client.Push("books/", data);
                data.BookId = response.Result.name;
                SetResponse setResponse = client.Set("books/" + data.BookId, data);
            }
            catch (Exception e)
            {
                success = false;
                Console.WriteLine(e.Message);
            }

            return success;
        }

        public async Task<bool> UpdateBook(Book book)
        {
            bool success = true;

            try
            {
                client = new FireSharp.FirebaseClient(config);
                SetResponse response = client.Set("books/" + book.BookId, book);
            }
            catch (Exception e)
            {
                success = false;
                Console.WriteLine(e.Message);
            }

            return success;
        }

        public async Task<bool> DeleteBook(string bookId)
        {
            bool success = true;

            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Delete("books/" + bookId);

            }
            catch (Exception e)
            {
                success = false;
                Console.WriteLine(e.Message);
            }

            return success;
        }

        public async Task<Book> GetBook(string bookId)
        {
            Book data = null;

            try
            {
                client = new FireSharp.FirebaseClient(config);
                FirebaseResponse response = client.Get("books/" + bookId);
                data = JsonConvert.DeserializeObject<Book>(response.Body);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return data;
        }

        public async Task<List<Book>> SearchBooks(string query)
        {
            List<Book> data = new List<Book>();
            try
            {
                
                client = new FireSharp.FirebaseClient(config);

                FirebaseResponse response = client.Get("books/");
              //  data = JsonConvert.DeserializeObject<List<Book>>(response.Body);

                var dd = JsonConvert.DeserializeObject<dynamic>(response.Body);

                foreach (dynamic item in dd)
                {
                    foreach (dynamic prop in item)
                    {
                        JEnumerable<JToken> tokenList = prop.Children();
                        Book b = new Book();
                        foreach (JToken token in tokenList)
                        {
                            JObject obj = JObject.Parse("{" + token.ToString() + "}");
                            foreach (var pair in obj)
                            {
                                string key = pair.Key;
                                string value = pair.Value.ToString().Replace("{", "").Replace("}","");
                                if (key.Equals("Authors"))
                                {
                                    b.Authors = value;
                                }
                                else if (key.Equals("AvailableCopies"))
                                {
                                    b.AvailableCopies = uint.Parse(value);
                                }
                                else if (key.Equals("BookId"))
                                {
                                    b.BookId = value;
                                }
                                else if (key.Equals("DateAdded"))
                                {
                                    b.DateAdded = Convert.ToDateTime(value);
                                }
                                else if (key.Equals("ISBN"))
                                {
                                    b.ISBN = value;
                                }
                                else if (key.Equals("PlainISBN"))
                                {
                                    b.PlainISBN = value;
                                }
                                else if (key.Equals("PublishingYear"))
                                {
                                    b.PublishingYear = ushort.Parse(value);
                                }
                                else if (key.Equals("Title"))
                                {
                                    b.Title = value;
                                }
                                else if (key.Equals("TotalCheckOuts"))
                                {
                                    b.TotalCheckOuts = uint.Parse(value);
                                }
                                else if (key.Equals("TotalCopies"))
                                {
                                    b.TotalCopies = uint.Parse(value);
                                }
                            }
                        }

                        if (b.Title.Contains(query))
                        {
                            data.Add(b);
                        }
                    }
                }

                        /*foreach (dynamic item in dd)
                        {
                            foreach (dynamic prop in item)
                            {
                                Newtonsoft.Json.Linq.JObject test = prop;
                                int length = test.ToString().Length;
                                JEnumerable<JToken> hh = test.Children();
                                foreach(JToken token in hh)
                                {

                                }
                                *//*Newtonsoft.Json.Linq.JValue test2 = (Newtonsoft.Json.Linq.JValue) test.ToString().Remove(0,1);
                                test2 = (Newtonsoft.Json.Linq.JValue) test2.ToString().Remove(test2.ToString().Length - 1, 1);
                                BookTest fff = JsonConvert.DeserializeObject<BookTest>((string)test2);*//*
                                break;
                            }
                            break;
                        }*/

                        Console.WriteLine("hello");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            /*string sql = @"SELECT 
	                            BookId,
	                            ISBN,
                                PlainISBN,
                                Title,
                                Authors,
                                PublishingYear,
                                DateAdded,
                                TotalCopies,
                                AvailableCopies
                            FROM books
                            WHERE Title Like @Query
                            AND IsActive = true;";

            var parameters = new
            {
                Query = "%" + query + "%"
            };

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    result = await conn.QueryAsync<Book>(sql, parameters).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }*/

            return data;
        }

        public async Task<bool> CheckOutBook(BookTransaction transaction)
        {
            bool success = true;
            /*string sql = @"INSERT INTO book_transaction (
	                            CustomerId,
                                BookId,
                                CheckOut
                            ) Select
	                            @CustomerId,
                                @BookId,
                                @CheckOut
                            FROM DUAL
                            WHERE NOT EXISTS (
	                            SELECT TransactionId
                                FROM book_transaction
                                WHERE CustomerId = @CustomerId AND BookId = @BookId AND CheckIn IS NULL
                            );";

            string updateCopiesSql = @"UPDATE books 
                                       SET AvailableCopies = AvailableCopies - 1 
                                       WHERE BookId = {0}";

            updateCopiesSql = String.Format(updateCopiesSql, transaction.Book.BookId);

            var parameters = new
            {
                CustomerId = transaction.Customer.CustomerId,
                BookId = transaction.Book.BookId,
                CheckOut = transaction.CheckOut
            };

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    int rowsAffected = await conn.ExecuteAsync(sql, parameters).ConfigureAwait(false);
                    if (rowsAffected == 0)
                        success = false;
                    else
                        rowsAffected = await conn.ExecuteAsync(updateCopiesSql).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                success = false;
                Console.WriteLine(e.Message);
            }*/

            return success;
        }

        public async Task<bool> CheckInBook(BookTransaction transaction)
        {
            bool success = true;
            /*string sql = @"UPDATE book_transaction
                           SET CheckIn = @CheckIn
                           WHERE CustomerId = @CustomerId 
	                             AND BookId = @BookId
	                             AND CheckIn IS NULL;";

            string updateCopiesSql = @"UPDATE books 
                                       SET AvailableCopies = AvailableCopies + 1 
                                       WHERE BookId = {0}";

            updateCopiesSql = String.Format(updateCopiesSql, transaction.Book.BookId);

            var parameters = new
            {
                CustomerId = transaction.Customer.CustomerId,
                BookId = transaction.Book.BookId,
                CheckIn = transaction.CheckIn.Value
            };

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    int rowsAffected = await conn.ExecuteAsync(sql, parameters).ConfigureAwait(false);
                    if (rowsAffected == 0)
                        success = false;
                    else
                        rowsAffected = await conn.ExecuteAsync(updateCopiesSql).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                success = false;
                Console.WriteLine(e.Message);
            }*/

            return success;
        }

        public async Task<IEnumerable<BookTransaction>> GetBookTransactionHistory(string bookId)
        {
            IEnumerable<BookTransaction> result = null;

            /*string sql = @"SELECT 
	                            bt.TransactionId,
                                bt.CheckOut,
                                bt.CheckIn,
	                            b.BookId,
	                            b.ISBN,
                                b.PlainISBN,
                                b.Title,
                                b.Authors,
                                b.PublishingYear,
                                b.DateAdded,
                                b.TotalCopies,
                                b.AvailableCopies,
                                c.CustomerId,
                                c.FirstName,
                                c.LastName,
                                c.Email,
                                c.DateOfBirth,
                                c.AccountCreatedOn
                            FROM book_transaction bt
                            INNER JOIN books b
	                            ON bt.BookId = b.BookId AND bt.BookId = @BookId AND b.IsActive = true
                            INNER JOIN customers c
	                            ON bt.CustomerId = c.CustomerId AND c.IsActive = true
                            ORDER BY bt.TransactionId DESC;";

            var parameters = new
            {
                BookId = bookId
            };

            try
            {
                using (var conn = new MySqlConnection(connectionString))
                {
                    result = await conn.QueryAsync<BookTransaction, Book, Customer, BookTransaction>(sql,
                                         param: parameters,
                                         map: (bTransaction, book, customer) => {
                                             bTransaction.Book = book;
                                             bTransaction.Customer = customer;
                                             return bTransaction;
                                         },
                                         splitOn: "BookId,CustomerId")
                                         .ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }*/

            return result;
        }
    }
}