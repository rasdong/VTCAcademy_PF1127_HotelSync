using MySql.Data.MySqlClient;

namespace HotelManagementSystem
{
    public class DataHelper
    {
        private static DataHelper? instance; 
        private readonly string connectionString;

        private DataHelper()
        {
            connectionString = "Server=localhost;Database=hotel_management;Uid=root;Pwd=@1406;";
        }

        public static DataHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataHelper();
                }
                return instance;
            }
        }

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(connectionString);
        }
    }
}