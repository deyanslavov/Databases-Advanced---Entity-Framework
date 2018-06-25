namespace P02_VillainNames
{
    using System;
    using System.Data.SqlClient;

    public class StartUp
    {
        public static void Main()
        {
            var connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=MinionsDB;Integrated Security=True");

            connection.Open();
            using (connection)
            {
                string getAllVillainsNameAndMinionsCountQuery = "SELECT v.Name, COUNT(*) AS MinionsCount FROM Villains AS v JOIN MinionsVillains AS mv ON mv.VillainId = v.Id JOIN Minions AS m ON m.Id = mv.MinionId GROUP BY v.Name HAVING COUNT(*) > 3 ORDER BY MinionsCount DESC";
                var getAllVillainsNameAndMinionsCountQueryCmd = new SqlCommand(getAllVillainsNameAndMinionsCountQuery, connection);

                var reader = getAllVillainsNameAndMinionsCountQueryCmd.ExecuteReader();

                while (reader.Read())
                {
                    string villainName = Convert.ToString(reader[0]);
                    int minionsCount = (int)reader[1];

                    Console.WriteLine($"{villainName} - {minionsCount}");
                }
                reader.Close();
            }
        }
    }
}
