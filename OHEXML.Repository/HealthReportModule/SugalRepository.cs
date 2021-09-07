using OHEXML.Common.SiteConfig;
using SqlSugar;
namespace OHEXML.Repository.HealthReportModule
{
    public class SugalRepository<T> : SimpleClient<T> where T : class, new()
    {
        public SugalRepository(ISqlSugarClient context = null) : base(context)//注意这里要有默认值等于null
        {
            if (context == null)
            {
                Context = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = SiteConfigHelper.GetSectioValue("ConnectionStrings", "SQLConnection2"),
                    DbType = DbType.SqlServer,
                    InitKeyType = InitKeyType.Attribute,
                    IsAutoCloseConnection = true,
                });
            }
        }

    }
}
