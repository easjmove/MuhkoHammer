using MuhkoHammerApi;
using MuhkoHammerApi.ModelClasses;
using System.Data.SqlClient;
using System.Drawing;

namespace MuhkoHammerApi.Managers
{
    public class GameSystemsManager
    {
        private DBReader myDB;
        private static List<GameSystem> gameSystems = ReadGameSystems(false);
        private static List<GameSystem> allGameSystems = ReadGameSystems(true);

        public GameSystemsManager()
        {
            myDB = new DBReader(new SqlConnection("Data source=mssql8.unoeuro.com;Initial Catalog=muhkohammer_dk_db_muhkohammer;User ID=muhkohammer_dk;Password=d6xHfa9mF2GE"));
        }

        private static List<GameSystem> ReadGameSystems(bool includeAll)
        {
            DBReader myDB = new DBReader(new SqlConnection("Data source=mssql8.unoeuro.com;Initial Catalog=muhkohammer_dk_db_muhkohammer;User ID=muhkohammer_dk;Password=d6xHfa9mF2GE"));
            List<GameSystem> gameSystems = myDB.ReadOrgData(includeAll);
            return gameSystems;
        }

        public static void ReloadData()
        {
            gameSystems = ReadGameSystems(false);
            allGameSystems = ReadGameSystems(true);
        }

        public IEnumerable<GameSystem> GetAll(bool includeAll)
        {
            if (includeAll)
            {
                return new List<GameSystem>(allGameSystems);
            }
            else
            {
                return new List<GameSystem>(gameSystems);
            }
        }

        public GameSystem GetByID(int id, bool includeAll)
        {
            if (includeAll)
            {
                return allGameSystems.Find(gameSystem => gameSystem.Id == id);
            }
            else
            {
                return gameSystems.Find(gameSystem => gameSystem.Id == id);
            }
        }

        public IEnumerable<Faction> GetFactions(int gameSystemID, bool includeAll)
        {
            return GetByID(gameSystemID, includeAll).Factions;
        }

        public Faction GetFactionByID(int gameSystemID, int id, bool includeAll)
        {
            return GetByID(gameSystemID, includeAll).Factions.Find(faction => faction.Id == id);
        }

        public IEnumerable<Unit> GetUnits(int GameSystemID, int FactionID, bool includeAll)
        {
            return GetFactionByID(GameSystemID, FactionID, includeAll).Units;
        }

        public Unit GetUnitByID(int gameSystemId, int factionId, int id, bool includeAll)
        {
            return GetFactionByID(gameSystemId, factionId, includeAll).Units.Find(unit => unit.Id == id);
        }

        public IEnumerable<UnitImage> GetUnitImages(int gameSystemId, int factionId, int unitId, bool includeAll)
        {
            return GetUnitByID(gameSystemId, factionId, unitId, includeAll).Images;
        }

        public byte[] GetImageByID(int id, bool Thumbnail)
        {
            byte[] result = myDB.ReadImage(id, Thumbnail);
            return result;
        }

        public void AddImage(IFormFile data, int? unitId)
        {
            Image myImage = Image.FromStream(data.OpenReadStream());
            Image thumbNail = CreateThumbnailImage(myImage);

            UnitImage newImage = myDB.InsertImage(ImageToBytes(myImage), ImageToBytes(thumbNail), unitId);
            if (newImage != null && unitId != null && unitId != -1)
            {
                foreach (GameSystem gs in gameSystems)
                {
                    foreach (Faction fac in gs.Factions)
                    {
                        foreach (Unit unit in fac.Units)
                        {
                            if (unit.Id == unitId)
                            {
                                unit.Images.Add(newImage);
                                return;
                            }
                        }
                    }
                }
            }
        }

        public void CreateThumbs()
        {
            Dictionary<int, byte[]> allImages = myDB.ReadAllImages();

            foreach (int ImageID in allImages.Keys)
            {
                byte[] currentBytes = allImages[ImageID];
                Image currentImage = null;
                using (var ms = new MemoryStream(currentBytes))
                {
                    currentImage = Image.FromStream(ms);
                }
                Image newThumb = CreateThumbnailImage(currentImage);
                myDB.UpdateThumbnail(ImageID, ImageToBytes(newThumb));
            }
        }

        public static byte[] ImageToBytes(Image x)
        {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] xByte = (byte[])_imageConverter.ConvertTo(x, typeof(byte[]));
            return xByte;
        }

        private static Image CreateThumbnailImage(Image image)
        {
            const int thumbnailSize = 150;

            var imageHeight = image.Height;
            var imageWidth = image.Width;
            if (imageHeight > imageWidth)
            {
                imageWidth = (int)(((float)imageWidth / (float)imageHeight) * thumbnailSize);
                imageHeight = thumbnailSize;
            }
            else
            {
                imageHeight = (int)(((float)imageHeight / (float)imageWidth) * thumbnailSize);
                imageWidth = thumbnailSize;
            }

            Image thumb = image.GetThumbnailImage(imageWidth, imageHeight, () => false, IntPtr.Zero);

            return thumb;
        }
    }
}
