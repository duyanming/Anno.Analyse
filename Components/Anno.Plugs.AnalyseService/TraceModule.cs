using System;
using System.Collections.Generic;
using System.Text;
using Anno.Infrastructure;
using Anno.Plugs.AnalyseService.Dto;
using Anno.EngineData;

namespace Anno.Plugs.AnalyseService
{
    using Anno.Const.Attribute;
    public class TraceModule:BaseModule
    {
        [AnnoInfo(Desc ="服务访问量统计")]
        public dynamic ServiceAnalyse([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {

            string sql = @"SELECT  AppNameTarget Name,COUNT(AppNameTarget) Count FROM  sys_trace  
WHERE Timespan>=?startDate AND Timespan<?endDate
GROUP BY AppNameTarget; ";
            return DbInstance.Db.Ado.Query<ServiceAnalyseOutPutDto>(sql, new { startDate=startDate.ToShortDateString(), endDate= endDate.ToShortDateString() });
        }
    }
}
