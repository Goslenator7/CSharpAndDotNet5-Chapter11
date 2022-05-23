using System;
using Microsoft.EntityFrameworkCore;


namespace Packt.Shared  
{
	// This manages the connection to the database
	public class Northwind : DbContext
	{
		//these properties map to tables in the database
		public DbSet<Category> Categories { get; set; }
        public DbSet<Products> Products { get; set; }

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
			string path = System.IO.Path.Combine(System.Environment.CurrentDirectory, "Northwind.db");
			optionsBuilder.UseSqlite($"Filename={path}");
        }

		protected override void OnModelCreating(ModelBuilder modelbuilder)
        {
			// example of using fluent API instead of attributes
			// to limit the Length of a category name to 15
			modelbuilder.Entity<Category>()
				.Property(category => category.CategoryName)
				.IsRequired() //NOT NULL
				.HasMaxLength(15);

			//added to "fix" the lack of decimal support in sqlite
			modelbuilder.Entity<Product>()
				.Property(product => product.Cost)
				.HasConversion<double>();
        }
    }
}

