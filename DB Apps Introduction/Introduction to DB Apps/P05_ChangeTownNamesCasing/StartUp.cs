namespace P05_ChangeTownNamesCasing
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            string countryName = Console.ReadLine();

            var connection = new SqlConnection("Server=.\\SQLEXPRESS;Database=MinionsDB;Integrated Security=True");

            connection.Open();
            using (connection)
            {
                string getCountryIdQuery = "SELECT Id FROM Countries WHERE Name = @countryName";
                var getCountryIdQueryCmd = new SqlCommand(getCountryIdQuery, connection);
                getCountryIdQueryCmd.Parameters.AddWithValue("@countryName", countryName);

                var reader = getCountryIdQueryCmd.ExecuteReader();

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

                    string changeTownNamesCasingQuery = "UPDATE Towns SET Name = UPPER(Name) WHERE CountryId = @countryId";
                    var changeTownNamesCasingQueryCmd = new SqlCommand(changeTownNamesCasingQuery, connection);
                    changeTownNamesCasingQueryCmd.Parameters.AddWithValue("@countryId", countryId);

                    int rowsAffected = changeTownNamesCasingQueryCmd.ExecuteNonQuery();

                    string getTownNamesQuery = "SELECT Name FROM Towns WHERE CountryId = @countryId";
                    var getTownNamesQueryCmd = new SqlCommand(getTownNamesQuery, connection);
                    getTownNamesQueryCmd.Parameters.AddWithValue("@countryId", countryId);

                    reader = getTownNamesQueryCmd.ExecuteReader();

                    var towns = new List<string>();
                    while (reader.Read())
                    {
                        towns.Add((string)reader[0]);
                    }
                    reader.Close();

                    Console.WriteLine($"{rowsAffected} town names were affected.");
                    Console.WriteLine("[" + string.Join(", ", towns) + "]");
                }
            }
        }
    }
}
