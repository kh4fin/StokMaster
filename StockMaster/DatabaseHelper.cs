using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;

public class DatabaseHelper
{
    private static string dbFile = "stockmaster.db";

    public static void InitializeDatabase()
    {
        if (!File.Exists(dbFile))
        {
            SQLiteConnection.CreateFile(dbFile);
        }

        CreateTables();
    }

    private static void CreateTables()
    {
        using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
        {
            conn.Open();

            string sqlProducts = @"
            CREATE TABLE IF NOT EXISTS products (
                productId INTEGER PRIMARY KEY AUTOINCREMENT,
                productName TEXT NOT NULL,
                category TEXT,
                price REAL
            );";

            string sqlInventory = @"
            CREATE TABLE IF NOT EXISTS inventory (
                inventoryId INTEGER PRIMARY KEY AUTOINCREMENT,
                productId INTEGER NOT NULL,
                quantity INTEGER NOT NULL,
                date TEXT NOT NULL,
                type TEXT NOT NULL,
                FOREIGN KEY (productId) REFERENCES products(productId)
            );";

            using (var cmd = new SQLiteCommand(sqlProducts, conn))
            {
                cmd.ExecuteNonQuery();
            }

            using (var cmd = new SQLiteCommand(sqlInventory, conn))
            {
                cmd.ExecuteNonQuery();
            }
        }
    }

    // ====== PRODUCTS ======

    public static List<Product> GetProducts()
    {
        List<Product> products = new();

        using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
        {
            conn.Open();
            string sql = "SELECT * FROM products";
            using (var cmd = new SQLiteCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    products.Add(new Product
                    {
                        ProductId = Convert.ToInt32(reader["productId"]),
                        ProductName = reader["productName"].ToString(),
                        Category = reader["category"].ToString(),
                        Price = reader["price"] != DBNull.Value ? Convert.ToDouble(reader["price"]) : 0
                    });
                }
            }
        }

        return products;
    }

    public static void InsertProduct(Product p)
    {
        using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
        {
            conn.Open();
            string sql = "INSERT INTO products (productName, category, price) VALUES (@name, @cat, @price)";
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@name", p.ProductName);
                cmd.Parameters.AddWithValue("@cat", p.Category);
                cmd.Parameters.AddWithValue("@price", p.Price);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static void UpdateProduct(Product p)
    {
        using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
        {
            conn.Open();
            string sql = "UPDATE products SET productName=@name, category=@cat, price=@price WHERE productId=@id";
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", p.ProductId);
                cmd.Parameters.AddWithValue("@name", p.ProductName);
                cmd.Parameters.AddWithValue("@cat", p.Category);
                cmd.Parameters.AddWithValue("@price", p.Price);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static void DeleteProduct(int productId)
    {
        using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
        {
            conn.Open();
            string sql = "DELETE FROM products WHERE productId=@id";
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", productId);
                cmd.ExecuteNonQuery();
            }
        }
    }

    // ====== INVENTORY ======

    public static List<Inventory> GetInventory()
    {
        List<Inventory> inventories = new();

        using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
        {
            conn.Open();
            string sql = "SELECT * FROM inventory";
            using (var cmd = new SQLiteCommand(sql, conn))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    inventories.Add(new Inventory
                    {
                        InventoryId = Convert.ToInt32(reader["inventoryId"]),
                        ProductId = Convert.ToInt32(reader["productId"]),
                        Quantity = Convert.ToInt32(reader["quantity"]),
                        Date = reader["date"].ToString(),
                        Type = reader["type"].ToString()
                    });
                }
            }
        }

        return inventories;
    }

    public static void InsertInventory(Inventory i)
    {
        using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
        {
            conn.Open();
            string sql = "INSERT INTO inventory (productId, quantity, date, type) VALUES (@pid, @qty, @date, @type)";
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@pid", i.ProductId);
                cmd.Parameters.AddWithValue("@qty", i.Quantity);
                cmd.Parameters.AddWithValue("@date", i.Date);
                cmd.Parameters.AddWithValue("@type", i.Type);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static void UpdateInventory(Inventory i)
    {
        using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
        {
            conn.Open();
            string sql = "UPDATE inventory SET productId=@pid, quantity=@qty, date=@date, type=@type WHERE inventoryId=@id";
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@pid", i.ProductId);
                cmd.Parameters.AddWithValue("@qty", i.Quantity);
                cmd.Parameters.AddWithValue("@date", i.Date);
                cmd.Parameters.AddWithValue("@type", i.Type);
                cmd.Parameters.AddWithValue("@id", i.InventoryId);
                cmd.ExecuteNonQuery();
            }
        }
    }

    public static void DeleteInventory(int inventoryId)
    {
        using (var conn = new SQLiteConnection($"Data Source={dbFile};Version=3;"))
        {
            conn.Open();
            string sql = "DELETE FROM inventory WHERE inventoryId=@id";
            using (var cmd = new SQLiteCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@id", inventoryId);
                cmd.ExecuteNonQuery();
            }
        }
    }

    // ==== Model Join ====
    

    // ==== Model Classes ====

    public class Product
    {
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public string? Category { get; set; }
        public double Price { get; set; }
    }

    public class Inventory
    {
        public int InventoryId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public string? Date { get; set; }
        public string? Type { get; set; }
    }
}
