namespace P05_ChangeTownNamesCasing
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;

    public class StartUp
    {
        public static void Main()
        {
            string countryName = Console.ReadLine();

            var connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=MinionsDB;Integrated Security=True");

            connection.Open();
            using (connection)
            {
                SqlCommand getCountryIdQueryCmd;
                SqlDataReader reader;

                GetCountryId(countryName, connection, out getCountryIdQueryCmd, out reader);

                if (!reader.Read())
                {
                    Console.WriteLine("No town names were affected.");
                    return;
                }
                else
                {
                    reader.Close();
                    int countryId = (int)getCountryIdQueryCmd.ExecuteScalar();
                    reader.Close();

                    int rowsAffected = ChangeTownNamesCasing(connection, countryId);

                    SqlCommand getTownNamesQueryCmd = CreateGetTownNamesCmd(connection, countryId);

                    reader = getTownNamesQueryCmd.ExecuteReader();

                    PrintOutput(reader, rowsAffected);
                }
            }
        }

        private static void PrintOutput(SqlDataReader reader, int rowsAffected)
        {
            var towns = new List<string>();
            while (reader.Read())
            {
                towns.Add((string)reader[0]);
            }
            reader.Close();

            Console.WriteLine($"{rowsAffected} town names were affected.");
            Console.WriteLine("[" + string.Join(", ", towns) + "]");
        }

        private static SqlCommand CreateGetTownNamesCmd(SqlConnection connection, int countryId)
        {
            string getTownNamesQuery = "SELECT Name FROM Towns WHERE CountryId = @countryId";
            var getTownNamesQueryCmd = new SqlCommand(getTownNamesQuery, connection);
            getTownNamesQueryCmd.Parameters.AddWithValue("@countryId", countryId);
            return getTownNamesQueryCmd;
        }

        private static int ChangeTownNamesCasing(SqlConnection connection, int countryId)
        {
            string changeTownNamesCasingQuery = "UPDATE Towns SET Name = UPPER(Name) WHERE CountryId = @countryId";
            var changeTownNamesCasingQueryCmd = new SqlCommand(changeTownNamesCasingQuery, connection);
            changeTownNamesCasingQueryCmd.Parameters.AddWithValue("@countryId", countryId);

            int rowsAffected = changeTownNamesCasingQueryCmd.ExecuteNonQuery();
            return rowsAffected;
        }

        private static void GetCountryId(string countryName, SqlConnection connection, out SqlCommand getCountryIdQueryCmd, out SqlDataReader reader)
        {
            string getCountryIdQuery = "SELECT Id FROM Countries WHERE Name = @countryName";
            getCountryIdQueryCmd = new SqlCommand(getCountryIdQuery, connection);
            getCountryIdQueryCmd.Parameters.AddWithValue("@countryName", countryName);

            reader = getCountryIdQueryCmd.ExecuteReader();
        }
    }
}
