using System;
using static System.Console;
using Packt.Shared;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace WorkingWithEFCore
{
    class Program
    {

        static void QueryingCategories()
        {
           using (var db = new Northwind())
           {
             WriteLine("Categories and how many products they have:");

             //a query to get all categories and their related products
             IQueryable<Category> cats = db.Categories.Include(c => c.Products);

             foreach (Category c in cats)
             {
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

        static void Main(string[] args)
        {
            //QueryingCategories();
            FilteredIncludes();
        }
    }
}
