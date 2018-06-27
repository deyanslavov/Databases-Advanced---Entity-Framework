namespace P09_IncreaseAgeStoredProcedure
{
    using System;
    using System.Data.SqlClient;

    public class StartUp
    {
        public static void Main()
        {
            int minionId = int.Parse(Console.ReadLine());

            var connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=MinionsDB;Integrated Security=True");

            connection.Open();
            using (connection)
            {
                IncreaseMinionAge(minionId, connection);
                PrintOutput(minionId, connection);
            }
        }

        private static void PrintOutput(int minionId, SqlConnection connection)
        {
            string getMinionInfoSql = "SELECT Name, Age FROM Minions WHERE Id = @id";

            using(var command = new SqlCommand(getMinionInfoSql, connection))
            {
                command.Parameters.AddWithValue("@id", minionId);

                using (var reader = command.ExecuteReader())
                {
                    reader.Read();

                    string minionName = (string)reader[0];
                    int minionAge = (int)reader[1];

                    Console.WriteLine($"{minionName} – {minionAge} years old");
                }
            }
        }

        private static void IncreaseMinionAge(int minionId, SqlConnection connection)
        {
            string execProcedureSql = "EXEC usp_GetOlder {0}";

            using(var command = new SqlCommand(string.Format(execProcedureSql, minionId), connection))
            {
                command.ExecuteNonQuery();
            }
        }
    }
}
