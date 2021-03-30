using System;

namespace Anno.Infrastructure
{
    public static class DbInstance
    {
        private static string _connectinStr = Const.AppSettings.ConnStr;
        private static IFreeSql fsql = null;
        private static object locker = new object();
        public static IFreeSql Db
        {
            get
            {
                if (fsql == null)
                {
                    lock (locker)
                    {
                        if (fsql == null)
                        {
                            fsql = new FreeSql.FreeSqlBuilder()
                                  .UseConnectionString(FreeSql.DataType.MySql, _connectinStr)
                                  .Build(); //请务必定义成 Singleton 单例模式
                        }
                    }
                }
                return fsql;
            }
        }

    }
}
