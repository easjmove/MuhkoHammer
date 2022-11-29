using MuhkoHammerApi.ModelClasses;
using System.Data.SqlClient;

namespace MuhkoHammerApi
{
    public class DBReader
    {
        private SqlConnection SqlConn;

        public DBReader(SqlConnection sqlConn)
        {
            SqlConn = sqlConn;
        }

        public List<GameSystem> ReadOrgData(bool includeAll)
        {
            SqlConn.Open();

            List<GameSystem> GameSystems = new List<GameSystem>();

            Dictionary<int, GameSystem> dictGameSystems = new Dictionary<int, GameSystem>();
            Dictionary<int, Faction> dictFactions = new Dictionary<int, Faction>();
            Dictionary<int, Unit> dictUnits = new Dictionary<int, Unit>();

            string joinType = "JOIN";
            if (includeAll) joinType = "LEFT OUTER JOIN";
 
            SqlCommand sqlComm = new SqlCommand("SELECT GameSystems.ID AS 'GSID', GameSystems.Name AS 'GSName'," +
                                                "Factions.ID AS 'FID', Factions.Name AS 'FName', Factions.GameSystemID AS 'FGSID', " +
                                                "Units.ID AS 'UID', Units.Name AS 'UName', Units.FactionID AS 'UFID', " +
                                                "Images.ID AS 'IID', Images.UploadDate, Images.UnitID AS 'IUID' " +
                                                "FROM GameSystems " +
                                                joinType + " Factions ON GameSystems.ID = Factions.GameSystemID " +
                                                joinType + " Units ON Factions.ID = Units.FactionID " +
                                                joinType + " Images ON Units.ID = Images.UnitID " +
                                                "ORDER BY GameSystems.Name, Factions.Name, Units.Name, Images.UploadDate DESC", SqlConn);
            SqlDataReader sqlReader = sqlComm.ExecuteReader();
            while (sqlReader.Read())
            {
                int GSID = int.Parse(sqlReader["GSID"].ToString());
                if (!dictGameSystems.ContainsKey(GSID))
                {
                    GameSystem newGameSystem = new GameSystem() { Id = GSID, Name = sqlReader["GSName"].ToString() };
                    dictGameSystems.Add(GSID, newGameSystem);
                    GameSystems.Add(newGameSystem);
                }

                if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("FGSID")))
                {
                    int FID = int.Parse(sqlReader["FID"].ToString());
                    int FGSID = int.Parse(sqlReader["FGSID"].ToString());
                    if (!dictFactions.ContainsKey(FID))
                    {
                        Faction newFaction = new Faction() { Id = FID, Name = sqlReader["FName"].ToString() };
                        dictGameSystems[FGSID].Factions.Add(newFaction);
                        dictFactions.Add(FID, newFaction);
                    }
                }

                if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("UFID")))
                {
                    int UID = int.Parse(sqlReader["UID"].ToString());
                    int UFID = int.Parse(sqlReader["UFID"].ToString());
                    if (!dictUnits.ContainsKey(UID))
                    {
                        Unit newUnit = new Unit() { Id = UID, Name = sqlReader["UName"].ToString() };
                        dictFactions[UFID].Units.Add(newUnit);
                        dictUnits.Add(UID, newUnit);
                    }
                }

                if (!sqlReader.IsDBNull(sqlReader.GetOrdinal("IUID")))
                {
                    UnitImage newImage = new UnitImage() { Id = int.Parse(sqlReader["IID"].ToString()), UploadedDate = DateTime.Parse(sqlReader["UploadDate"].ToString()) };
                    dictUnits[int.Parse(sqlReader["IUID"].ToString())].Images.Add(newImage);
                }
            }

            SqlConn.Close();
            return GameSystems;
        }

        public byte[] ReadImage(int ImageID, bool Thumbnail)
        {
            SqlConn.Open();

            string column = "data";
            if (Thumbnail) column = "Thumbnail";

            SqlCommand sqlComm = new SqlCommand("SELECT [" + column + "] FROM [Images] WHERE [ID] = " + ImageID, SqlConn);
            SqlDataReader sqlReader = sqlComm.ExecuteReader();
            byte[] result = null;
            if (sqlReader.Read())
            {
                result = (byte[])sqlReader[column];
            }
            SqlConn.Close();
            return result;
        }

        public Dictionary<int,byte[]> ReadAllImages()
        {
            Dictionary<int, byte[]> result = new Dictionary<int, byte[]>();
            SqlConn.Open();

            SqlCommand sqlComm = new SqlCommand("SELECT [Data],[ID] FROM [Images] WHERE [Thumbnail] IS NULL", SqlConn);
            SqlDataReader sqlReader = sqlComm.ExecuteReader();
            while (sqlReader.Read())
            {
                result.Add((int)sqlReader["ID"], (byte[])sqlReader["Data"]);
            }
            SqlConn.Close();
            return result;
        }

        public void UpdateThumbnail(int ImageID, byte[] data)
        {
            SqlConn.Open();

            SqlCommand sqlComm = new SqlCommand("UPDATE Images SET [Thumbnail] = @Data WHERE [ID] = @ImageID", SqlConn);
            SqlParameter sqlIDParam = sqlComm.Parameters.AddWithValue("@ImageID", ImageID);
            sqlIDParam.DbType = System.Data.DbType.Int32;

            SqlParameter sqlDataParam = sqlComm.Parameters.AddWithValue("@Data", data);
            sqlDataParam.DbType = System.Data.DbType.Binary;

            sqlComm.ExecuteNonQuery();
            SqlConn.Close();
        }

        public UnitImage InsertImage(byte[] data, byte[] thumbnail, int? unitID)
        {
            SqlConn.Open();

            SqlCommand sqlComm = new SqlCommand("INSERT INTO Images (UnitID,UploadDate,[Data],[Thumbnail]) " +
                                                "OUTPUT INSERTED.ID, INSERTED.UploadDate " +
                                                "VALUES (@UnitID,GETDATE(), @Data, @Thumbnail)", SqlConn);

            SqlParameter sqlIDParam = new SqlParameter("@UnitID", DBNull.Value);
            sqlIDParam.DbType = System.Data.DbType.Int32;
            if (unitID != null && unitID != -1)
            {
                sqlIDParam.Value = unitID;
            } 
            sqlIDParam = sqlComm.Parameters.Add(sqlIDParam);

            SqlParameter sqlDataParam = sqlComm.Parameters.AddWithValue("@Data", data);
            sqlDataParam.DbType = System.Data.DbType.Binary;

            SqlParameter sqlThumbParam = sqlComm.Parameters.AddWithValue("@Thumbnail", thumbnail);
            sqlThumbParam.DbType = System.Data.DbType.Binary;

            SqlDataReader sqlReader = sqlComm.ExecuteReader();

            sqlReader.Read();
            int newID = int.Parse(sqlReader["ID"].ToString());
            DateTime unitDate = DateTime.Parse(sqlReader["UploadDate"].ToString());

            SqlConn.Close();

            UnitImage newImage = null;

            if (unitID != null && unitID != -1)
            {
                newImage = new UnitImage() { Id = newID, UploadedDate = unitDate };
            }

            return newImage;
        }
    }
}
