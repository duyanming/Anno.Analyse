using System;
using System.Collections.Generic;
using System.Text;
using Anno.Infrastructure;
using Anno.Plugs.AnalyseService.Dto;
using Anno.EngineData;

namespace Anno.Plugs.AnalyseService
{
    using Anno.Const.Attribute;
    public class TraceModule:LogBaseModule
    {
        [AnnoInfo(Desc ="服务访问量统计")]
        public dynamic ServiceAnalyse([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {
            //this.Fatal($"startDate:{startDate} endDate:{endDate}", "接收参数");
            string sql = @"SELECT  AppNameTarget Name,COUNT(AppNameTarget) Count FROM  sys_trace  
WHERE Timespan>=?startDate AND Timespan<?endDate
GROUP BY AppNameTarget; ";
            //this.Fatal($"startDate.ToString(yyyy-MM-dd):{startDate.ToString("yyyy-MM-dd")} endDate ShortDateString:{endDate.ToString("yyyy-MM-dd")}", "接收参数ToShortDateString");
            return DbInstance.Db.Ado.Query<ServiceAnalyseOutPutDto>(sql, new { startDate=startDate.ToString("yyyy-MM-dd"), endDate= endDate.ToString("yyyy-MM-dd") });
        }
    }
}
