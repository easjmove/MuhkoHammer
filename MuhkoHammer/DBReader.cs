using MuhkoHammer.ModelClasses;
using System.Data.SqlClient;

namespace MuhkoHammer
{
    public static class DBReader
    {
        public static List<GameSystem> ReadOrgData(SqlConnection sqlConn)
        {
            List<GameSystem> GameSystems = new List<GameSystem>();

            Dictionary<int, GameSystem> dictGameSystems = new Dictionary<int, GameSystem>();
            Dictionary<int, Faction> dictFactions = new Dictionary<int, Faction>();
            Dictionary<int, Unit> dictUnits = new Dictionary<int, Unit>();

            SqlCommand sqlComm = new SqlCommand("SELECT GameSystems.ID AS 'GSID', GameSystems.Name AS 'GSName'," +
                                                "Factions.ID AS 'FID', Factions.Name AS 'FName', Factions.GameSystemID AS 'FGSID', " +
                                                "Units.ID AS 'UID', Units.Name AS 'UName', Units.FactionID AS 'UFID', " +
                                                "Images.ID AS 'IID', Images.UploadDate, Images.UnitID AS 'IUID' " +
                                                "FROM GameSystems " +
                                                "LEFT OUTER JOIN Factions ON GameSystems.ID = Factions.GameSystemID " +
                                                "LEFT OUTER JOIN Units ON Factions.ID = Units.FactionID " +
                                                "LEFT OUTER JOIN Images ON Units.ID = Images.UnitID", sqlConn);
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

            return GameSystems;
        }
    }
}
