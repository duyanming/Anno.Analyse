using System;
using System.Collections.Generic;
using System.Text;
using Anno.Infrastructure;
using Anno.Plugs.AnalyseService.Dto;
using Anno.EngineData;

namespace Anno.Plugs.AnalyseService
{
    using Anno.Const.Attribute;
    public class TraceModule : LogBaseModule
    {
        [AnnoInfo(Desc = "服务访问量统计 Top 10")]
        public dynamic ServiceAnalyse([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {
            string sql = @"SELECT  AppNameTarget name,COUNT(AppNameTarget) value FROM  sys_trace  
WHERE Timespan>=?startDate AND Timespan<?endDate
GROUP BY AppNameTarget ORDER BY value desc LIMIT 10; ";
            return DbInstance.Db.Ado.Query<NameValueOutPutDto>(sql, new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") });
        }


        [AnnoInfo(Desc = "服务访问错误量统计 Top 10")]
        public dynamic ServiceErrorAnalyse([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {
            string sql = @"SELECT  AppNameTarget name,COUNT(AppNameTarget) value FROM  sys_trace  
WHERE Rlt=0 AND  Timespan>=?startDate AND Timespan<?endDate
GROUP BY AppNameTarget ORDER BY value desc LIMIT 10; ";
            return DbInstance.Db.Ado.Query<NameValueOutPutDto>(sql, new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") });
        }

        [AnnoInfo(Desc = "服务管道访问统计 Top 10")]
        public dynamic ServiceModuleAnalyse([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {
            return GetServiceData(startDate,endDate, "Askchannel");
        }

        [AnnoInfo(Desc = "服务模块访问统计 Top 10")]
        public dynamic ServiceRouterAnalyse([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {
            return GetServiceData(startDate, endDate, "Askrouter");
        }

        [AnnoInfo(Desc = "服务方法访问统计 Top 10")]
        public dynamic ServiceMethodAnalyse([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {
            return GetServiceData(startDate, endDate, "Askmethod");
        }

        [AnnoInfo(Desc = "服务方法Error访问统计 Top 10")]
        public dynamic ServiceMethodErrorAnalyse([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {
            string sql = @"SELECT  Askmethod name,COUNT(Askmethod) value FROM  sys_trace  
WHERE Rlt=0 AND  Timespan>=?startDate AND Timespan<?endDate
GROUP BY Askmethod ORDER BY value desc LIMIT 10; ";
            return DbInstance.Db.Ado.Query<NameValueOutPutDto>(sql, new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") });
        }

        /// <summary>
        /// Trace分析数据
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private List<NameValueOutPutDto> GetServiceData(DateTime startDate, DateTime endDate,string type) {

            string sql = $@"SELECT  {type} name,COUNT({type}) value FROM  sys_trace  
WHERE  Rlt=1 AND  Timespan>=?startDate AND Timespan<?endDate
GROUP BY {type} ORDER BY value desc LIMIT 10; ";
            return DbInstance.Db.Ado.Query<NameValueOutPutDto>(sql, new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") });
        }
    }
}
