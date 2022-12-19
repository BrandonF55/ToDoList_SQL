using System.Collections.Generic;
using MySqlConnector;

namespace ToDoList.Models
{
    public class Item
    {
        public string Description { get; set; }
        public int Id { get; set;}


        public Item(string description)
        {
            Description = description;

        }

        public Item(string description, int id)
        {
            Description = description;
            Id = id;
        }

        public override bool Equals(System.Object otherItem)  //equals is built-in to C#. Must use System.Object
        {
            if (!(otherItem is Item))  //checks if argument passed into parameter is an Item object
            {
                return false;
            }
            else
            {
                Item newItem = (Item)otherItem;  //converts argument passed in, to an Item object
                bool idEquality = (this.Id == newItem.Id);
                bool descriptionEquality = (this.Description == newItem.Description);
                return (idEquality && descriptionEquality);
            }
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();  //generates a hash code for each item
        }


        public static List<Item> GetAll()
        {
            List<Item> allItems = new List<Item> { }; //make new list of item objects

            MySqlConnection conn = new MySqlConnection(DBConfiguration.ConnectionString); //MySqlConnection object returns a connection
            conn.Open();  //opening sql connection

            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;  //creates sql command query
            cmd.CommandText = "SELECT * FROM items;"; //this is what the command is set to

            MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader; //data reader
            while (rdr.Read())  //reads results from database one at a time
            {
                int itemId = rdr.GetInt32(0); //int at the 0nth index of the array returned by the reader
                string itemDescription = rdr.GetString(1);  //returns strings
                Item newItem = new Item(itemDescription, itemId); //each row in database is an Item stored in this list
                allItems.Add(newItem);
            }
            conn.Close(); //close connection
            if (conn != null)
            {
                conn.Dispose();   //disposes connection
            }
            return allItems;
        }

        public static void ClearAll()
        {
            MySqlConnection conn = new MySqlConnection(DBConfiguration.ConnectionString); //call new MySqlConnection(DBConfiguration.ConnectionString) to create connection
            conn.Open();  //open connection 

            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
            cmd.CommandText = "DELETE FROM items;";   //clears entire items table in database
            cmd.ExecuteNonQuery();  //modifies data instead of querying
            conn.Close();
            if (conn != null)
            {
                conn.Dispose();
            }
        }
        public static Item Find(int searchId)
        {
            Item placeholderItem = new Item("placeholder item");
            return placeholderItem;

        }

        public void Save()
        {
            MySqlConnection conn = new MySqlConnection(DBConfiguration.ConnectionString);
            conn.Open();  //opens connection

            MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;

            // Begin new code

            cmd.CommandText = "INSERT INTO items (description) VALUES (@ItemDescription);"; //@ItemDescription is a parameter placeholder
            MySqlParameter param = new MySqlParameter();
            param.ParameterName = "@ItemDescription";  //.ParameterName is a buildin method for giving parameters names for the parameter placeholder 
            param.Value = this.Description; //adds value to the above parameter
            cmd.Parameters.Add(param);
            cmd.ExecuteNonQuery();  //executes nonquery command (modifies)
            Id = (int) cmd.LastInsertedId; //sets the item's Id property equal to the value of teh id of the new row in the database

            // End new code

            conn.Close();   //closes connection
            if (conn != null)
            {
                conn.Dispose();
            }
        }

    }
}
