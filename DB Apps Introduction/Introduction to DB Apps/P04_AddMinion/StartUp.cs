namespace P04_AddMinion
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;

    public class StartUp
    {
        public static void Main()
        {
            string[] minionInfo = Console.ReadLine().Split(' ').Skip(1).ToArray();
            string villainName = Console.ReadLine().Split(' ').Skip(1).ToArray()[0];

            var connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=MinionsDB;Integrated Security=True");

            connection.Open();
            using (connection)
            {
                string minionName = minionInfo[0];
                int minionAge = int.Parse(minionInfo[1]);
                string minionTown = minionInfo[2];

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

                string createMinionQuery = "INSERT INTO Minions (Name, Age, TownId) VALUES (@minionName, @minionAge, (SELECT Id FROM Towns WHERE Name = @minionTown))";
                var createMinionQueryCmd = new SqlCommand(createMinionQuery, connection);
                createMinionQueryCmd.Parameters.AddWithValue("@minionName", minionName);
                createMinionQueryCmd.Parameters.AddWithValue("@minionAge", minionAge);
                createMinionQueryCmd.Parameters.AddWithValue("@minionTown", minionTown);

                createMinionQueryCmd.ExecuteNonQuery();

                string addMinionToVillainQuery = "INSERT INTO MinionsVillains (MinionId, VillainId) VALUES ((SELECT TOP 1 Id FROM Minions WHERE Name = @minionName ORDER BY Id DESC), (SELECT Id FROM Villains WHERE Name = @villainName))";
                var addMinionToVillainQueryCmd = new SqlCommand(addMinionToVillainQuery, connection);
                addMinionToVillainQueryCmd.Parameters.AddWithValue("@minionName", minionName);
                addMinionToVillainQueryCmd.Parameters.AddWithValue("@villainName", villainName);

                addMinionToVillainQueryCmd.ExecuteNonQuery();

                Console.WriteLine($"Successfully added {minionName} to be minion of {villainName}.");
            }
        }
    }
}
