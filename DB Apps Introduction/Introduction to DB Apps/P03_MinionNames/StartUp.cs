namespace P03_MinionNames
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
                string getVillainNameQuery = "SELECT Name FROM Villains WHERE Id = @villainId";
                var getVillainNameQueryCmd = new SqlCommand(getVillainNameQuery, connection);

                getVillainNameQueryCmd.Parameters.AddWithValue("@villainId", villainId);

                var reader = getVillainNameQueryCmd.ExecuteReader();
                string villainName = null;


                while (reader.Read())
                {
                    villainName = Convert.ToString(reader[0]);
                }

                if (string.IsNullOrEmpty(villainName))
                {
                    Console.WriteLine($"No villain with ID {villainId} exists in the database.");
                    return;
                }
                else
                {
                    Console.WriteLine($"Villain: {villainName}");
                }

                reader = PrintMinions(villainId, connection, reader);
            }
        }

        private static SqlDataReader PrintMinions(int villainId, SqlConnection connection, SqlDataReader reader)
        {
            string getAllVillainMinionsQuery = "SELECT m.Name, m.Age FROM Minions AS m JOIN MinionsVillains AS mv ON mv.MinionId = m.Id JOIN Villains AS v ON v.Id = mv.VillainId WHERE v.Id = @villainId ORDER BY m.Name";
            var getAllVillainMinionsQueryCmd = new SqlCommand(getAllVillainMinionsQuery, connection);
            getAllVillainMinionsQueryCmd.Parameters.AddWithValue("@villainId", villainId);

            reader.Close();
            reader = getAllVillainMinionsQueryCmd.ExecuteReader();
            int minionPosition = 1;

            if (!reader.Read())
            {
                Console.WriteLine("(no minions)");
            }
            else
            {
                while (reader.Read())
                {
                    string minionName = (string)reader[0];
                    var minionAge = reader[1];

                    Console.WriteLine($"{minionPosition++}. {minionName} {minionAge}");
                }
            }
            reader.Close();
            return reader;
        }
    }
}