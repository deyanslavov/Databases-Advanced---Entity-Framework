namespace BookShop
{
    using System;
    using System.Linq;
    using System.Text;
    using BookShop.Initializer;
    using BookShop.Data;
    using BookShop.Models;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;

    public class StartUp
    {
        public static void Main()
        {
            using (var db = new BookShopContext())
            {
                int booksRemoved = RemoveBooks(db);
                Console.WriteLine(booksRemoved + " books were deleted");
            }
        }
        // 15. Remove Books 

        public static int RemoveBooks(BookShopContext context)
        {
            var booksToBeRemoved = context.Books
                .Where(b => b.Copies < 4200)
                .ToArray();

            int booksRemoved = booksToBeRemoved.Length;

            context.Books.RemoveRange(booksToBeRemoved);

            context.SaveChanges();

            return booksRemoved;
        }

        // 14. Increase Prices

        public static void IncreasePrices(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.ReleaseDate.Value.Year < 2010)
                .ToList();

            books.ForEach(b => b.Price += 5);

            context.SaveChanges();
        }

        // 13. Most Recent Books 

        public static string GetMostRecentBooks(BookShopContext context)
        {
            var categoriesBooks = context.Categories
                .Select(c => new
                {
                    c.Name,
                    BookCount = c.CategoryBooks.Select(cb => cb.Book).Count(),
                    TopThreeBooks = c.CategoryBooks.Select(cb => cb.Book).OrderByDescending(b => b.ReleaseDate).Take(3).ToArray()
                })
                .OrderBy(c => c.Name)
                .ToArray();

            var sb = new StringBuilder();

            foreach (var category in categoriesBooks)
            {
                sb.AppendLine($"--{category.Name}");

                foreach (var book in category.TopThreeBooks)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDate.Value.Year})");
                }
            }
            string result = sb.ToString().Trim();
            return result;
        }

        // 12. Profit by Category 

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            var booksInfo = context.Categories
                .Select(c => new
                {
                    c.Name,
                    Profit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(c => c.Profit)
                .ThenBy(c => c.Name)
                .Select(c => $"{c.Name} ${c.Profit:F2}")
                .ToArray();

            string result = string.Join(Environment.NewLine, booksInfo);
            return result;
        }

        // 11. Total Book Copies 

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            var authorsCopies = context.Authors
                .Include(a => a.Books)
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName,
                    TotalCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(b => b.TotalCopies)
                .ToArray();

            var sb = new StringBuilder();

            foreach (var author in authorsCopies)
            {
                sb.AppendLine($"{author.FullName} - {author.TotalCopies}");
            }

            string result = sb.ToString().Trim();
            return result;
        }

        // 10. Count Books 

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            int booksCount = context.Books
                .Where(b => b.Title.Length > lengthCheck)
                .Count();

            return booksCount;
        }

        // 09. Book Search by Author 

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            var books = context.Books
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => $"{b.Title} ({b.Author.FirstName} {b.Author.LastName})")
                .ToArray();
            
            string result = string.Join(Environment.NewLine, books);
            return result;
        }

        // 08. Book Search 

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            if (input == null)
            {
                input = Console.ReadLine();
            }

            input = input.ToLower();

            var bookTitles = context.Books
                .Where(b => b.Title.ToLower().Contains(input))
                .Select(b => b.Title)
                .OrderBy(t => t);

            string result = string.Join(Environment.NewLine, bookTitles).Trim();
            return result;
        }

        // 07. Author Search 

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            var authors = context.Authors
                .Where(a => a.FirstName.EndsWith(input))
                .OrderBy(a => a.FirstName)
                .ThenBy(a => a.LastName)
                .Select(a => new
                {
                    FullName = a.FirstName + " " + a.LastName
                })
                .ToArray();

            var sb = new StringBuilder();

            foreach (var author in authors)
            {
                sb.AppendLine(author.FullName);
            }

            return sb.ToString().Trim();
        }

        // 06. Released Before Date 

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            int[] dateTokens = date.Split('-').Select(int.Parse).ToArray();
            int day = dateTokens[0];
            int month = dateTokens[1];
            int year = dateTokens[2];

            DateTime dateTime = new DateTime(year, month, day);

            var books = context.Books
                .Where(b => b.ReleaseDate < dateTime)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new
                {
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                
                .ToArray();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:F2}");
            }

            string result = sb.ToString().Trim();
            return result;
        }

        // 05. Book Titles by Category 

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            string[] categories = input.ToLower().Split();

            var bookTitles = context.Books
                .Where(b => b.BookCategories.Any(c => categories.Contains(c.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
                
        }

        // 04. Not Released In

        public static string GetBooksNotRealeasedIn(BookShopContext context, int year)
        {
            var bookTitles = context.Books
                .Where(b => b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            string result = string.Join(Environment.NewLine, bookTitles);
            return result;
        }

        // 03. Books by Price

        public static string GetBooksByPrice(BookShopContext context)
        {
            var books = context.Books
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => new
                {
                    b.Title,
                    b.Price
                })
                .ToArray();

            var sb = new StringBuilder();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }

            string result = sb.ToString().Trim();
            return result;
        }

        // 02. Golden Books

        public static string GetGoldenBooks(BookShopContext context)
        {
            var bookTitles = context.Books
                .OrderBy(b => b.BookId)
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }

        // 01. Age Restriction

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            var sb = new StringBuilder();

            var ageRestriction = (AgeRestriction)Enum.Parse(typeof(AgeRestriction), command, true);

            var books = context.Books
                .Where(b => b.AgeRestriction == ageRestriction)
                .Select(b => b.Title)
                .OrderBy(b => b)
                .ToArray();

            var result = string.Join(Environment.NewLine, books);
            return result;
        }
    }
}
