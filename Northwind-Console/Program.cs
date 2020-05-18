using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NLog;
using NorthwindConsole.Models;

namespace NorthwindConsole
{
    class MainClass
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public static void Main(string[] args)
        {
            logger.Info("Program started");
            try
            {
                string choice;
                do
                {
                    Console.WriteLine("1) Display Categories and Descriptions");
                    Console.WriteLine("2) Add Category");
                    // can give submenu options for menu choices 3 and 4 in order to preserve the previous features
                    Console.WriteLine("3) (Modify) Display Category and related active products");
                    Console.WriteLine("4) (Modify - Done) Display all Categories and their related active products");
                    Console.WriteLine("5) (New) Add new Product record");
                    Console.WriteLine("6) (New) Edit existing Product record");
                    Console.WriteLine("7) (New) Display specific Product details");
                    Console.WriteLine("8) (New) Edit a Category");
                    Console.WriteLine("9) (New) Delete a Product record");
                    Console.WriteLine("0) (New) Delete a Category record");
                    // use data annotations and handle all user errors gracefully and Nlog errors
                    Console.WriteLine("\"q\" to quit");
                    choice = Console.ReadLine();
                    Console.Clear();
                    logger.Info($"Option {choice} selected");
                    if (choice == "1")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryName);

                        Console.WriteLine($"{query.Count()} records returned");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryName} - {item.Description}");
                        }
                    } 
                    else if (choice == "2")
                    {
                        Category category = new Category();
                        Console.WriteLine("Enter Category Name:");
                        category.CategoryName = Console.ReadLine();
                        Console.WriteLine("Enter the Category Description:");
                        category.Description = Console.ReadLine();

                        ValidationContext context = new ValidationContext(category, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(category, context, results, true);
                        if (isValid)
                        {
                            var db = new NorthwindContext();
                            // check for unique name
                            if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "CategoryName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                // TODO: save category to db
                                db.Categories.Add(category);
                                db.SaveChanges();
                            }
                        }
                        if (!isValid)
                        {
                            foreach (var result in results)
                            {
                                logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
                            }
                        }
                    } else if (choice == "3")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryId);

                        Console.WriteLine("Select the category whose products you want to display:");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryId}) {item.CategoryName}");
                        }
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");

                        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == id);
                        Console.WriteLine($"{category.CategoryName} - {category.Description}");
                        
                        // give user option to select all products, active only products or discontinued products.

                        Console.WriteLine("Please select the following options: ");
                        Console.WriteLine("P) for All Products");
                        Console.WriteLine("A) for Active only Products");
                        Console.WriteLine("D) for Discontinued Products");
                        var entry = Console.ReadLine().ToUpper();
                        logger.Info("User selected option " + entry);

                        try
                        {
                            if (entry == "P")
                            {
                                foreach (Product p in category.Products)
                                {
                                    Console.WriteLine(p.ProductName);
                                }
                            } else if (entry == "A")
                            {
                                foreach (Product p in category.Products.Where(p => p.Discontinued.Equals(false)))
                                {
                                    Console.WriteLine(p.ProductName);
                                }
                            } else if (entry == "D")
                            {
                                foreach (Product p in category.Products.Where(p => p.Discontinued.Equals(true)))
                                {
                                    Console.WriteLine(p.ProductName);
                                }
                            }
                            else
                            {
                                logger.Info(entry + "is not a valid selection.");
                            }
                        }
                        catch (Exception e)
                        {
                            logger.Info("Unable to retrieve product information." + e);
                        }

                        
                    }
                    else if (choice == "4")
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);

                        // give user option to select all products, active only products or discontinued products.

                        Console.WriteLine("Please select the following options: ");
                        Console.WriteLine("P) for All Products");
                        Console.WriteLine("A) for Active only Products");
                        Console.WriteLine("D) for Discontinued Products");
                        var entry = Console.ReadLine().ToUpper();
                        logger.Info("User selected option " + entry);
                        var flag = false;

                        try
                        {
                            if (entry == "A")
                            {
                                flag = false;

                            } else if (entry == "D")
                            {
                                flag = true;
                            }

                            if (entry == "D" || entry == "A")
                            {

                                foreach (var item in query)
                                {
                                    Console.WriteLine($"{item.CategoryName} - Active Products");

                                    foreach (Product p in item.Products.Where(p => p.Discontinued.Equals(flag)))
                                    {
                                        Console.WriteLine($"\t{p.ProductName}");
                                    }
                                }
                            }
                            else if (entry == "P")
                            {
                                foreach (var item in query)
                                {
                                    Console.WriteLine($"{item.CategoryName} - Active Products");

                                    foreach (Product p in item.Products)
                                    {
                                        Console.WriteLine($"\t{p.ProductName}");
                                    }
                                }
                            }
                            else
                            {
                                logger.Info("Invalid selection: " + entry);
                            }
                        }
                        catch (Exception e)
                        {
                            logger.Info("Unable to retrieve product details: " + e);
                        }

                        
                    }
                    Console.WriteLine();

                } while (choice.ToLower() != "q");
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            logger.Info("Program ended");
        }
    }
}
