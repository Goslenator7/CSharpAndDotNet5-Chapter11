using System;
using static System.Console;
using Packt.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace WorkingWithEFCore
{
    class Program
    {
        static void QueryingProducts()
        {
            using (var db = new Northwind())
            {
                var loggerFactory = db.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(new ConsoleLoggerProvider());

                WriteLine("Products that cost more than a price, highest cost first: ");
                string input;
                decimal price;
                do
                {
                    Write("Enter a product price: ");
                    input = ReadLine();
                }
                while (!decimal.TryParse(input, out price));

                IQueryable<Product> prods = db.Products
                    .Where(product => product.Cost > price)
                    .OrderByDescending(product => product.Cost);

                foreach (Product item in prods)
                {
                    WriteLine($"{item.ProductID}: {item.ProductName} costs {item.Cost:$#,##0.00} and has {item.Stock} in stock.");
                }
            }
        }

        static void QueryingCategories()
        {
           using (var db = new Northwind())
           {
                var loggerFactory = db.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(new ConsoleLoggerProvider());

             WriteLine("Categories and how many products they have:");

             //a query to get all categories and their related products
             IQueryable<Category> cats; // = db.Categories; .Include(c => c.Products);

                db.ChangeTracker.LazyLoadingEnabled = false;

                Write("Enable eager loading? (Y/N): ");
                bool eagerLoading = (ReadKey().Key == ConsoleKey.Y);
                bool explicitLoading = false;
                WriteLine();

                if (eagerLoading)
                {
                    cats = db.Categories.Include(c => c.Products);
                }
                else
                {
                    cats = db.Categories;

                    Write("Enable explicit loading? (Y/N): ");
                    explicitLoading = (ReadKey().Key == ConsoleKey.Y);
                    WriteLine();
                }

             foreach (Category c in cats)
             {
               if (explicitLoading)
                    {
                        Write($"Explicitly load products for {c.CategoryName}? (Y/N): ");
                        ConsoleKeyInfo key = ReadKey();
                        WriteLine();
                        if(key.Key == ConsoleKey.Y)
                        {
                            var products = db.Entry(c).Collection(c2 => c2.Products);
                            if (!products.IsLoaded) products.Load();
                        }
                    }
               WriteLine($"{c.CategoryName} has {c.Products.Count} products.");
             }
           }
        }

        static void FilteredIncludes()
        {
            using (var db = new Northwind())
            {
                Write("Enter a minimum for units in stock: ");
                string unitsInStock = ReadLine();
                int stock = int.Parse(unitsInStock);

                IQueryable<Category> cats = db.Categories.Include(c => c.Products.Where(p => p.Stock >= stock));

                // output the SQL query created from the line above
                WriteLine($"ToQueryString: {cats.ToQueryString()}");

                foreach (Category c in cats)
                {
                    WriteLine($"{c.CategoryName} has {c.Products.Count} products with a minimum of {stock} units in stock");

                    foreach (Product p in c.Products)
                    {
                        WriteLine($" {p.ProductName} has {p.Stock} units in stock.");
                    }
                }
            }
        }

        static void QueryingWithLike()
        {
            using (var db = new Northwind())
            {
                var loggerFactory = db.GetService<ILoggerFactory>();
                loggerFactory.AddProvider(new ConsoleLoggerProvider());

                Write("Enter part of a product name: ");
                string input = ReadLine();

                IQueryable<Product>prods = db.Products.Where(p => EF.Functions.Like(p.ProductName, $"%{input}%"));

                foreach(Product item in prods)
                {
                    WriteLine($"{item.ProductName} has {item.Stock} in stock. Discontinued? {item.Discontinued}");
                }
            }
        }

        static void Main(string[] args)
        {
            QueryingCategories();
            //FilteredIncludes();
            //QueryingProducts();
            //QueryingWithLike();
        }
    }
}
