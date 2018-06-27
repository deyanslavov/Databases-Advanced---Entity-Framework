namespace P06_RemoveVillain
{
    using System;
    using System.Data.SqlClient;

    public class StartUp
    {
        public static void Main()
        {
            int villainId = int.Parse(Console.ReadLine());

            var connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=MinionsDB;Integrated Security=True");

            connection.Open();
            using (connection)
            {
                string villainName = GetVillainName(villainId, connection);

                if (villainName == null)
                {
                    Console.WriteLine("No such villain was found.");
                    return;
                }
                else
                {
                    int minionsReleasedCount = ReleaseMinions(villainId, connection);
                    DeleteVillain(villainId, connection);

                    PrintOutput(villainName, minionsReleasedCount);
                }
            }
        }

        private static void PrintOutput(string villainName, int minionsReleasedCount)
        {
            Console.WriteLine($"{villainName} was deleted.\n\r{minionsReleasedCount} minions were released.");
        }

        private static void DeleteVillain(int villainId, SqlConnection connection)
        {
            string deleteVillainSql = "DELETE FROM Villains WHERE Id = @id";

            using (var command = new SqlCommand(deleteVillainSql, connection))
            {
                command.Parameters.AddWithValue("@id", villainId);
                command.ExecuteNonQuery();
            }
        }

        private static int ReleaseMinions(int villainId, SqlConnection connection)
        {
            int minionsReleasedCount;
            string deleteVillainRelatedMinionsSql = "DELETE FROM MinionsVillains WHERE VillainId = @villainId";
            
            using(var command = new SqlCommand(deleteVillainRelatedMinionsSql, connection))
            {
                command.Parameters.AddWithValue("@villainId", villainId);
                minionsReleasedCount = command.ExecuteNonQuery();
            }
            return minionsReleasedCount;
        }

        private static string GetVillainName(int villainId, SqlConnection connection)
        {
            string villainName;
            string findVillainNameSql = "SELECT Name FROM Villains WHERE Id = @id";

            using(var command = new SqlCommand(findVillainNameSql, connection))
            {
                command.Parameters.AddWithValue("@id", villainId);
                villainName = (string)command.ExecuteScalar();
            }
            return villainName;
        }
    }
}
