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
                    Console.WriteLine("3) (Modify - Done) Display Category and related active products");
                    Console.WriteLine("4) (Modify - Done) Display all Categories and their related active products");
                    Console.WriteLine("5) (New - Done) Add new Product record");
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
                    }
                    
                    else if (choice == "3")
                    
                    {
                        var db = new NorthwindContext();
                        var query = db.Categories.OrderBy(p => p.CategoryID);

                        Console.WriteLine("Select the category whose products you want to display:");
                        foreach (var item in query)
                        {
                            Console.WriteLine($"{item.CategoryID}) {item.CategoryName}");
                        }
                        int id = int.Parse(Console.ReadLine());
                        Console.Clear();
                        logger.Info($"CategoryId {id} selected");

                        Category category = db.Categories.FirstOrDefault(c => c.CategoryID == id);
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
                                Console.WriteLine("All Statuses");
                                foreach (Product p in category.Products)
                                {
                                    Console.WriteLine(p.ProductName);
                                }
                            } else if (entry == "A")
                            {
                                Console.WriteLine("Active Status Only");
                                foreach (Product p in category.Products.Where(p => p.Discontinued.Equals(false)))
                                {
                                    Console.WriteLine(p.ProductName);
                                }
                            } else if (entry == "D")
                            {
                                Console.WriteLine("Discontinued Status Only");
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
                        var query = db.Categories.Include("Products").OrderBy(p => p.CategoryID);

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
                                    if (entry == "A")
                                    {
                                        Console.WriteLine($"{item.CategoryName} - Active Status Only");
                                    } else if (entry == "D")

                                    {
                                        Console.WriteLine($"{item.CategoryName} - Discontinued Status Only");
                                    }

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
                                    Console.WriteLine($"{item.CategoryName} - All Statuses");

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
                    
                    else if (choice == "5")
                    {
                        Product product = new Product();
                        Console.WriteLine("Enter Product Name:");
                        product.ProductName = Console.ReadLine();

                        ValidationContext context = new ValidationContext(product, null, null);
                        List<ValidationResult> results = new List<ValidationResult>();

                        var isValid = Validator.TryValidateObject(product, context, results, true);
                       
                        if (isValid)
                        {
                            var db = new NorthwindContext();
                            // check for unique name
                            if (db.Products.Any(p => p.ProductName == product.ProductName))
                            {
                                // generate validation error
                                isValid = false;
                                results.Add(new ValidationResult("Name exists", new string[] { "ProductName" }));
                            }
                            else
                            {
                                logger.Info("Validation passed");
                                var queryC = db.Categories.OrderBy(p => p.CategoryID);

                                foreach (var item in queryC)
                                {
                                    Console.WriteLine($"{item.CategoryID}) {item.CategoryName}");
                                }

                                try
                                {
                                    int value = 0;
                                    Console.WriteLine("Please enter the Category number item belongs to: ");
                                    bool valid = int.TryParse(Console.ReadLine(), out value);
                                    bool existing = db.Categories.Any(c => c.CategoryID == value);

                                    if (valid && existing)
                                    {
                                        product.CategoryID = value;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Category ID must be a number value listed.");
                                    }

                                }
                                catch (Exception e)
                                {
                                    logger.Info("Category ID error : " + e);
                                }

                                var queryS = db.Suppliers.OrderBy(s => s.SupplierID);

                                foreach (var item in queryS)
                                {
                                    Console.WriteLine($"{item.SupplierID}) {item.CompanyName}");
                                }

                                try
                                {
                                    int value = 0;
                                    Console.WriteLine("Please select the product Supplier ID: ");
                                    bool valid = int.TryParse(Console.ReadLine(), out value);
                                    bool existing = db.Suppliers.Any(s => s.SupplierID == value);

                                    if (valid && existing)
                                    {
                                        product.SupplierID = value;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Supplier ID must be a number value listed");
                                    }

                                }
                                catch (Exception e)
                                {
                                    logger.Info("Supplier ID error: " + e);
                                }

                                //quantity per unit
                                Console.WriteLine("What is the quantity per unit?");
                                product.QuantityPerUnit = Console.ReadLine();
                                Console.Clear();

                                //unit price
                                try
                                {
                                    decimal value = 0;
                                    Console.WriteLine("What is the unit price?");
                                    bool valid = decimal.TryParse(Console.ReadLine(), out value);
                                    if (valid)
                                    {
                                        product.UnitPrice = value;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Unit price must be a number value.");
                                    }
                                    Console.Clear();
                                }
                                catch (Exception e)

                                {
                                    logger.Info("Unit Price error : " + e);
                                }

                                ////whoesale price
                                //try
                                //{
                                //    decimal value = 0;
                                //    Console.WriteLine("What is the wholesale price?");
                                //    bool valid = decimal.TryParse(Console.ReadLine(), out value);

                                //    if (valid)
                                //    {
                                //        product.WholesalePrice = value;
                                //    }
                                //    else
                                //    {
                                //        Console.WriteLine("Unit price must be a number value.");
                                //    }

                                //    Console.Clear();
                                //}
                                //catch (Exception e)
                                //{
                                //    logger.Info("Wholesale Price error : " + e);
                                //}

                                //units in stock
                                try
                                {
                                    Int16 value = 0;
                                    Console.WriteLine("What are the units in stock?");
                                    bool valid = Int16.TryParse(Console.ReadLine(), out value);

                                    if (valid)
                                    {
                                        product.UnitsInStock = value;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Units in stock must be a number value.");
                                    }

                                    Console.Clear();
                                }
                                catch (Exception e)
                                {
                                    logger.Info("Units in stock error: " + e);
                                }

                                //units on order
                                try
                                {
                                    Int16 value = 0;
                                    Console.WriteLine("What are the units on order?");
                                    bool valid = Int16.TryParse(Console.ReadLine(), out value);

                                    if (valid)
                                    {
                                        product.UnitsOnOrder = value;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Units on order must be a number value.");
                                    }

                                    Console.Clear();
                                }
                                catch (Exception e)
                                {
                                    logger.Info("Units in stock error " + e);
                                }

                                //reorder level
                                try
                                {
                                    Int16 value = 0;
                                    Console.WriteLine("What is the reorder level?");
                                    bool valid = Int16.TryParse(Console.ReadLine(), out value);

                                    if (valid)
                                    {
                                        product.ReorderLevel = value;
                                    }
                                    else
                                    {
                                        Console.WriteLine("Reorder level must be a number value.");
                                    }

                                    Console.Clear();
                                }
                                catch (Exception e)
                                {
                                    logger.Info("Units in stock error: " + e);
                                }

                                // TODO: save category to db
                                db.Products.Add(product);
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
