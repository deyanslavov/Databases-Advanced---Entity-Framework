namespace P08_IncreaseMinionAge
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            int[] minionsIds = Console.ReadLine().Split().Select(int.Parse).ToArray();

            var connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=MinionsDB;Integrated Security=True");

            connection.Open();
            using (connection)
            {
                IncreaseMinionsAge(minionsIds, connection);
                PrintMinionsInfo(connection);
            }
        }

        private static void PrintMinionsInfo(SqlConnection connection)
        {
            string minionsInfoSql = "SELECT Name, Age FROM Minions";

            using(var command = new SqlCommand(minionsInfoSql, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string minionName = (string)reader[0];
                        int minionAge = (int)reader[1];

                        Console.WriteLine($"{ minionName} { minionAge}");
                    }
                }
            }
        }

        private static void IncreaseMinionsAge(int[] minionsIds, SqlConnection connection)
        {
            string updateMinionsSql = "UPDATE Minions SET Age += 1 WHERE Id = @id";

            for (int i = 0; i < minionsIds.Length; i++)
            {
                using (var command = new SqlCommand(updateMinionsSql, connection))
                {
                    command.Parameters.AddWithValue("@id", minionsIds[i]);
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
