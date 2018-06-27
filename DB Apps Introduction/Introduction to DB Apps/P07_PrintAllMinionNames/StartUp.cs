namespace P07_PrintAllMinionNames
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public class StartUp
    {
        public static void Main()
        {
            var connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=MinionsDB;Integrated Security=True");

            connection.Open();
            using (connection)
            {
                List<string> minionsNames = GetMinionsNames(connection);
                PrintMinionsNames(minionsNames);
            }
        }

        private static void PrintMinionsNames(List<string> minionsNames)
        {
            for (int i = 0; i < minionsNames.Count / 2; i++)
            {
                Console.WriteLine(minionsNames[i]);
                Console.WriteLine(minionsNames[minionsNames.Count - 1 - i]);
            }

            if (minionsNames.Count % 2 == 1)
            {
                Console.WriteLine(minionsNames[minionsNames.Count / 2]);
            }
        }

        private static List<string> GetMinionsNames(SqlConnection connection)
        {
            List<string> minionsNames = new List<string>();

            string minionsNamesSql = "SELECT Name FROM Minions";

            using(var command = new SqlCommand(minionsNamesSql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string minionName = (string)reader[0];
                        minionsNames.Add(minionName);
                    }
                }
            }
            return minionsNames;
        }
    }
}
