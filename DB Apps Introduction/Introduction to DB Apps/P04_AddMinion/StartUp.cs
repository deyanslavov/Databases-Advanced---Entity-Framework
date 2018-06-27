namespace P04_AddMinion
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            string[] minionInfo;
            string villainName;
            ReadInput(out minionInfo, out villainName);

            var connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=MinionsDB;Integrated Security=True");

            connection.Open();
            using (connection)
            {
                string minionName = minionInfo[0];
                int minionAge = int.Parse(minionInfo[1]);
                string minionTown = minionInfo[2];

                SqlDataReader reader = CheckForTownAndAddIfNotExisting(connection, minionTown);

                reader = GetVillainAndAddIfMissing(villainName, connection);

                CreateMinion(connection, minionName, minionAge, minionTown);

                AddMinionToVillain(villainName, connection, minionName);

                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
            }
        }

        private static void AddMinionToVillain(string villainName, SqlConnection connection, string minionName)
        {
            string addMinionToVillainQuery = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES ((SELECT TOP 1 Id FROM Minions WHERE Name = @minionName ORDER BY Id DESC), (SELECT Id FROM Villains WHERE Name = @villainName))";
            var addMinionToVillainQueryCmd = new SqlCommand(addMinionToVillainQuery, connection);
            addMinionToVillainQueryCmd.Parameters.AddWithValue("@minionName", minionName);
            addMinionToVillainQueryCmd.Parameters.AddWithValue("@villainName", villainName);

            addMinionToVillainQueryCmd.ExecuteNonQuery();
        }

        private static void CreateMinion(SqlConnection connection, string minionName, int minionAge, string minionTown)
        {
            string createMinionQuery = "INSERT INTO Minions (Name, Age, TownId) VALUES (@minionName, @minionAge, (SELECT Id FROM Towns WHERE Name = @minionTown))";
            var createMinionQueryCmd = new SqlCommand(createMinionQuery, connection);
            createMinionQueryCmd.Parameters.AddWithValue("@minionName", minionName);
            createMinionQueryCmd.Parameters.AddWithValue("@minionAge", minionAge);
            createMinionQueryCmd.Parameters.AddWithValue("@minionTown", minionTown);

            createMinionQueryCmd.ExecuteNonQuery();
        }

        private static SqlDataReader GetVillainAndAddIfMissing(string villainName, SqlConnection connection)
        {
            SqlDataReader reader;
            string getVillainNameQuery = "SELECT Name FROM Villains WHERE Name = @villainName";
            var getVillainNameQueryCmd = new SqlCommand(getVillainNameQuery, connection);
            getVillainNameQueryCmd.Parameters.AddWithValue("@villainName", villainName);

            reader = getVillainNameQueryCmd.ExecuteReader();
            if (!reader.Read())
            {
                reader.Close();
                string createVillainQuery = "INSERT INTO Villains (Name, EvilnessFactorId) VALUES (@villainName, 4)";
                var createVillainQueryCmd = new SqlCommand(createVillainQuery, connection);
                createVillainQueryCmd.Parameters.AddWithValue("@villainName", villainName);
                createVillainQueryCmd.ExecuteNonQuery();

                Console.WriteLine($"Villain {villainName} was added to the database.");
            }
            reader.Close();
            return reader;
        }

        private static SqlDataReader CheckForTownAndAddIfNotExisting(SqlConnection connection, string minionTown)
        {
            string checkIfTownExistsQuery = "SELECT Name FROM Towns WHERE Name = @minionTown";
            var checkIfTownExistsQueryCmd = new SqlCommand(checkIfTownExistsQuery, connection);
            checkIfTownExistsQueryCmd.Parameters.AddWithValue("@minionTown", minionTown);

            var reader = checkIfTownExistsQueryCmd.ExecuteReader();

            if (!reader.Read())
            {
                reader.Close();
                string createTownQuery = "INSERT INTO Towns (Name) VALUES (@minionTown)";
                var createTownQueryCmd = new SqlCommand(createTownQuery, connection);
                createTownQueryCmd.Parameters.AddWithValue("@minionTown", minionTown);
                createTownQueryCmd.ExecuteNonQuery();

                Console.WriteLine($"Town {minionTown} was added to the database.");
            }
            reader.Close();
            return reader;
        }

        private static void ReadInput(out string[] minionInfo, out string villainName)
        {
            minionInfo = Console.ReadLine().Split(' ').Skip(1).ToArray();
            villainName = Console.ReadLine().Split(' ').Skip(1).ToArray()[0];
        }
    }
}
