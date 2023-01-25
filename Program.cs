// See https://aka.ms/new-console-template for more information
using System.Data.SQLite;


class WhatsForDinner
{
    static void Main()
    {
        InitializeDatabase();
        bool running = true;
        Console.WriteLine("\nWelcome to your own personal cookbook!");

        while (running)
        {
            DisplayMenu();
            var input = System.Console.ReadLine();

            switch (input)
            {
                case "1":
                    ViewRecipes();
                    break;
                case "2":
                    DisplayRecipe();
                    break;
                case "3":
                    AddRecipes();
                    break;
                case "4":
                    EditRecipe();
                    break;
                case "5":
                    DeleteRecipe();
                    break;
                case "6":
                    running = false;
                    break;
                default:
                    System.Console.WriteLine("\n");
                    System.Console.WriteLine("Please enter a valid selection!");
                    System.Console.WriteLine("\n");
                    break;
            }
        }
    }

    static void InitializeDatabase()
    {
        String databaseFile = "WhatsForDinner.db";
        if (!File.Exists(databaseFile))
        {
            using (SQLiteConnection connection = new SQLiteConnection("Data Source=WhatsForDinner.db; version=3;"))
            {
                connection.Open();
                String createTableCommand = @"CREATE TABLE recipes (
                    id	INTEGER NOT NULL UNIQUE PRIMARY KEY AUTOINCREMENT,
                    name	TEXT NOT NULL UNIQUE,
                    instructions	TEXT NOT NULL,
                    time	INTEGER NOT NULL,
                    type	TEXT NOT NULL)";

                using (SQLiteCommand command = new SQLiteCommand(createTableCommand, connection))
                {
                    int rows = command.ExecuteNonQuery();

                    if (rows == 0)
                    {
                        System.Console.WriteLine("Database created!");
                    }
                    else if (rows < 0)
                    {
                        System.Console.WriteLine("Failed to create database");
                    }
                }
            }
        }
    }
    
    static void DisplayMenu()
    {
        Console.WriteLine("\nMenu:");
        Console.WriteLine("1. View Recipes");
        Console.WriteLine("2. Display Specific Recipe");
        Console.WriteLine("3. Add Recipe");
        Console.WriteLine("4. Edit Recipe");
        Console.WriteLine("5. Delete Recipe");
        Console.WriteLine("6. Exit");
    }

    static void DisplayRecipe()
    {
        System.Console.WriteLine("\nPlease select a method to search for a recipe to edit:");
        System.Console.WriteLine("1. Name\n2. ID");

        String searchMethodString = System.Console.ReadLine();
        int searchMethod = int.Parse(searchMethodString);

        System.Console.WriteLine("\nPlease enter the search value:");
        String searchValue = System.Console.ReadLine();


        using (SQLiteConnection connection = new SQLiteConnection("Data Source=WhatsForDinner.db; version=3;"))
        {
            connection.Open();

            String query = "SELECT * FROM recipes WHERE ";

            if (searchMethod == 1)
            {
                query += "name = @searchValue";
            }
            else if (searchMethod == 2)
            {
                query += "id = @searchValue";
            }

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@searchValue", searchValue);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        int id = Convert.ToInt32(reader["id"]);
                        String name = (String)reader["name"];
                        String instructions = (String)reader["instructions"];
                        int time = Convert.ToInt32(reader["time"]);
                        String type = (String)reader["type"];

                        System.Console.WriteLine("\nID: " + id);
                        System.Console.WriteLine("Name: " + name);
                        System.Console.WriteLine("Instructions:\n" + instructions);
                        System.Console.WriteLine("Mintues to Make: " + time);
                        System.Console.WriteLine("Type: " + type);
                        System.Console.WriteLine("\nPress enter to continue:");
                        System.Console.ReadLine();
                    }
                    else
                    {
                        System.Console.WriteLine("No such recipe found");
                    }
                }
            }

        }
    }

    static void ViewRecipes()
    {
        using (SQLiteConnection connection = new SQLiteConnection("Data Source=WhatsForDinner.db; version=3;"))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand("SELECT * FROM recipes", connection))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        System.Console.WriteLine("\n");
                        while (reader.Read())
                        {
                            System.Console.WriteLine("ID: " + reader["id"] + " Name: " + reader["name"] + " Instructions: " + reader["instructions"]);
                        }
                        System.Console.WriteLine("\nPress enter to continue");
                        System.Console.ReadLine();
                    }
                    else{
                        System.Console.WriteLine("\nNo rows found :(");
                    }
                }
            }
        }
    }

    static void AddRecipes()
    {
        string name;
        string instructions;
        string timeString;
        string type;

        System.Console.WriteLine("\nPlease enter the name of the recipe: ");
        name = System.Console.ReadLine();

        System.Console.WriteLine("\nPlease enter the instructions for the recipes: ");
        instructions = System.Console.ReadLine();

        System.Console.WriteLine("\nPlease enter the time need to make as a positive integer in minutes: ");
        timeString = System.Console.ReadLine();
        int time = int.Parse(timeString);

        System.Console.WriteLine("\nPlease enter the type of recipe this is (Breakfast, Lunch, Dinner, Refreshment): ");
        type = System.Console.ReadLine();

        using (SQLiteConnection connection = new SQLiteConnection("Data Source=WhatsForDinner.db; version=3;"))
        {
            connection.Open();
            using (SQLiteCommand command = new SQLiteCommand("INSERT INTO recipes(name,instructions,time,type) VALUES(@name, @instructions, @time, @type)", connection))
            {
                command.Parameters.AddWithValue("@name", name);
                command.Parameters.AddWithValue("@instructions", instructions);
                command.Parameters.AddWithValue("@time", time);
                command.Parameters.AddWithValue("@type", type);

                int rowsReturned = command.ExecuteNonQuery();
                
                if (rowsReturned > 0)
                {
                    System.Console.WriteLine("\nRecipe successfully added!");
                }
                else 
                {
                    System.Console.WriteLine("\nRecipe failed to add.");
                }
            }
        }
    }

    static void EditRecipe()
    {
        System.Console.WriteLine("\nPlease select a method to search for a recipe to edit:");
        System.Console.WriteLine("1. Name\n2. ID");

        String searchMethodString = System.Console.ReadLine();
        int searchMethod = int.Parse(searchMethodString);

        System.Console.WriteLine("\nPlease enter the search value:");
        String searchValue = System.Console.ReadLine();

        using (SQLiteConnection connection = new SQLiteConnection("Data Source=WhatsForDinner.db; version=3;"))
        {
            connection.Open();
            String query = "SELECT * FROM recipes ";

            if (searchMethod == 1)
            {
                query += "WHERE name = @searchValue";
            }
            else if (searchMethod == 2)
            {
                query += "WHERE id = @searchValue";
            }

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@searchValue", searchValue);

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        reader.Read();
                        int id = Convert.ToInt32(reader["id"]);
                        String existingName = (String)reader["name"];
                        String existingInstructions = (String)reader["instructions"];
                        int existingTime = Convert.ToInt32(reader["time"]);
                        String existingType = (String)reader["type"];

                        System.Console.WriteLine("\nPlease enter desired updates. To keep existing value, just press enter");

                        System.Console.WriteLine("Please enter an updated name. Existing name: " + existingName);
                        String newName = System.Console.ReadLine();
                        if (newName == "")
                        {
                            newName = existingName;
                        }

                        System.Console.WriteLine("Please enter updated instructions. Existing instructions: " + existingInstructions);
                        String newInstructions = System.Console.ReadLine();
                        if (newInstructions == "")
                        {
                            newInstructions = existingInstructions;
                        }

                        System.Console.WriteLine("Please enter an updated time. Existing time: " + existingTime);
                        String newTimeString = System.Console.ReadLine();
                        int newTime;
                        if (newTimeString == "")
                        {
                            newTime = existingTime;
                        }
                        else
                        {
                            newTime = int.Parse(newTimeString);
                        }

                        System.Console.WriteLine("Please enter an updated type. Existing type: " + existingType);
                        String newType = System.Console.ReadLine();
                        if (newType == "")
                        {
                            newType = existingType;
                        }

                        using (SQLiteCommand updateCommand = new SQLiteCommand("UPDATE recipes SET name = @newName, instructions = @newInstructions, time = @newTime, type = @newType WHERE id = @id", connection))
                        {
                            updateCommand.Parameters.AddWithValue("@id", id);
                            updateCommand.Parameters.AddWithValue("@newName", newName);
                            updateCommand.Parameters.AddWithValue("@newInstructions", newInstructions);
                            updateCommand.Parameters.AddWithValue("@newTime", newTime);
                            updateCommand.Parameters.AddWithValue("@newType", newType);

                            int rows = updateCommand.ExecuteNonQuery();

                            if (rows > 0)
                            {
                                System.Console.WriteLine("\nRecipe successfully updated!");
                            } 
                            else
                            {
                                System.Console.WriteLine("\nFailed to update recipe");
                            }
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("\nRecipe could not be found");
                    }
                }
            }
        }
    }

    static void DeleteRecipe()
    {
        System.Console.WriteLine("\nPlease enter the ID of the recipe you want to delete: ");
        int deleteID = int.Parse(System.Console.ReadLine());

        using (SQLiteConnection connection = new SQLiteConnection("Data Source=WhatsForDinner.db; version=3;"))
        {
            connection.Open();
            String deleteCommand = "DELETE FROM recipes WHERE id = @id";
            using (SQLiteCommand command = new SQLiteCommand(deleteCommand, connection))
            {
                command.Parameters.AddWithValue("@id", deleteID);

                int rows = command.ExecuteNonQuery();

                if (rows > 0)
                {
                    System.Console.WriteLine("\nDelete successful!");
                }
                else
                {
                    System.Console.WriteLine("\nDelete was not successful");
                }
            }
        }
    }
}

