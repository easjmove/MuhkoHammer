using MuhkoHammer.ModelClasses;
using System.Data.SqlClient;

namespace MuhkoHammer.Managers
{
    public class GameSystemsManager
    {
        private static List<GameSystem> gameSystems = ReadGameSystems();
        public static List<GameSystem> ReadGameSystems()
        {
            SqlConnection sqlConn = new SqlConnection("Data source=(localdb)\\MSSQLLocalDB;Initial Catalog=MuhkoHammer");
            sqlConn.Open();
            List<GameSystem> gameSystems = DBReader.ReadOrgData(sqlConn);
            sqlConn.Close();
            return gameSystems;
        }

        public IEnumerable<GameSystem> GetAll()
        {
            return new List<GameSystem>(gameSystems);
        }
    }
}
