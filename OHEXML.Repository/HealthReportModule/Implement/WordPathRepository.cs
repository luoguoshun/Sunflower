using OHEXML.Entity.Entities;

namespace OHEXML.Repository.HealthReportModule.Implement
{
    public class WordPathRepository : SugalRepository<WordPathInfo>
    {
        public bool SaveWord(string UnitName, string WordPath, string wordName, string SaveTime)
        {
            WordPathInfo pathInfo = new WordPathInfo()
            {
                UnitName = UnitName,
                WordName = wordName,
                BasePath = WordPath,
                SaveTime = SaveTime,
            };
            //ExecuteCommand返回插入行数
            return Context.Insertable(pathInfo).ExecuteCommand() > 0;
        }
    }
}

