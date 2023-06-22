using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using Casino;
using Casino.TwentyOne;

namespace TwentyOne
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //string text = "Here is some text.";
            //File.WriteAllText(@"D:\Logs\log.txt", text); //Write text
            //string text = File.ReadAllText(@"D:\Logs\log.txt"); // Read text

            const string casinoName = "Grand Hotel Casino";

            Guid identifier = Guid.NewGuid(); // Globally Unique Identifier

            Player newPlayer = new Player("Norman"); // C# now allows to implicitely declare variables. eg.   var newPlayer = new Player("Norman"); would  work. it is good practice to explicitly  declare variables for  readability.

            Console.WriteLine("Welcome to the {0}.Let's start by telling your name.", casinoName);
            string playerName =Console.ReadLine();

            if (playerName.ToLower()  ==  "admin")
            {
                List<ExceptionEntity> Exceptions = ReadExceptions();
                foreach  (var exception in Exceptions)
                {
                    Console.Write(exception.Id + "  | ");
                    Console.Write(exception.ExceptionType + "  | ");
                    Console.Write(exception.ExceptionMessage + "  | ");
                    Console.Write(exception.TimeStamp + "  | ");
                    Console.WriteLine();
                }
                Console.ReadLine();
                return;
            }

            // Exception handling
            bool validAnswer  = false;
            int bank = 0;
            while (!validAnswer)
            {
                Console.WriteLine("And how much money did you bring today?");
                validAnswer = int.TryParse(Console.ReadLine(), out bank); // if output is not  int  or not valid.
                if (!validAnswer) Console.WriteLine("Please enter valid amount. No decimals.");
            }

            //Console.WriteLine("And how much money did you bring today?");
            //int bank =  Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Hello, {0}. Would  you like to join a game of 21 right now?", playerName);
            string answer = Console.ReadLine().ToLower();
            if (answer == "yes" ||  answer == "yeah" || answer == "y") 
            {
                Player player = new Player(playerName, bank);
                player.Id = Guid.NewGuid();
                // Log player GUID
                using (StreamWriter file = new StreamWriter(@"D:\Logs\log.txt", true))
                {
                    file.WriteLine(player.Id);
                    
                }
                Game game = new TwentyOneGame();
                game += player;
                player.isActivelyPlaying = true;
                while (player.isActivelyPlaying  && player.Balance > 0)
                {
                    try
                    {
                    game.Play();
                    }
                    catch (FraudException ex)
                    {
                        Console.WriteLine(ex.Message);
                        UpdateDbWithException(ex);
                        Console.ReadLine();
                        return;
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("An  error occured. Please contact your System Administrator.");
                        Console.ReadLine(); 
                        return;
                    }
                }
                game -= player;
                Console.WriteLine("Thank you for playing!");
            }
            Console.WriteLine("Feel free to look around the casino. Bye for now.");
            Console.ReadLine(); 
        }
        private static void UpdateDbWithException(Exception ex) 
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TwentyOneGame;
                                       Integrated Security=True;Connect Timeout=30;Encrypt=False;
                                       TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

            string queryString = @"INSERT INTO Exceptions (ExceptionType, ExceptionMessage, TimeStamp) VALUES
                                   (@ExceptionType, @ExceptionMessage, @TimeStamp)";  // Parameterized queries. Used to avoid SQL Injection attacks. ADO.Net will help 

            // "using" is used to control unmanaged code. wrap connection in curly braces to automatically shutoff the resource. Frees up memory.
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, connection);
                command.Parameters.Add("@ExceptionType", System.Data.SqlDbType.VarChar);
                command.Parameters.Add("@ExceptionMessage", System.Data.SqlDbType.VarChar);
                command.Parameters.Add("@TimeStamp", System.Data.SqlDbType.DateTime);

                command.Parameters["@ExceptionType"].Value = ex.GetType().ToString();
                command.Parameters["@ExceptionMessage"].Value = ex.Message;
                command.Parameters["@TimeStamp"].Value = DateTime.Now;

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();

            }
        }
        private static List<ExceptionEntity> ReadExceptions()
        {
            string connectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TwentyOneGame;
                                       Integrated Security=True;Connect Timeout=30;Encrypt=False;
                                       TrustServerCertificate=False;ApplicationIntent=ReadWrite;
                                       MultiSubnetFailover=False";

            string queryString = @"Select Id, ExceptionType, ExceptionMessage, TimeStamp From Exceptions";

            List<ExceptionEntity> Exceptions = new List<ExceptionEntity>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(queryString, conn);

                conn.Open();
                
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    ExceptionEntity exception = new ExceptionEntity();  
                    exception.Id = Convert.ToInt32(reader["Id"]);
                    exception.ExceptionType = reader["ExceptionType"].ToString();
                    exception.ExceptionMessage = reader["ExceptionMessage"].ToString();
                    exception.TimeStamp = Convert.ToDateTime(reader["TimeStamp"]);
                    Exceptions.Add(exception);
                }
                conn.Close();
            }
            return Exceptions;
        }
    }
}
