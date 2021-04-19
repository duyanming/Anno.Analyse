using System;
using System.Collections.Generic;
using System.Text;
using Anno.Infrastructure;
using Anno.Plugs.AnalyseService.Dto;
using Anno.EngineData;

namespace Anno.Plugs.AnalyseService
{
    using Anno.Const.Attribute;
    using System.Linq;

    public class TraceModule : LogBaseModule
    {
        private static IP2Region.DbSearcher _search = new IP2Region.DbSearcher(System.IO.Path.Combine(Environment.CurrentDirectory, "DB", "ip2region.db"));
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
            return GetServiceData(startDate, endDate, "Askchannel");
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
        private List<NameValueOutPutDto> GetServiceData(DateTime startDate, DateTime endDate, string type)
        {

            string sql = $@"SELECT  {type} name,COUNT({type}) value FROM  sys_trace  
WHERE  Rlt=1 AND  Timespan>=?startDate AND Timespan<?endDate
GROUP BY {type} ORDER BY value desc LIMIT 10; ";
            return DbInstance.Db.Ado.Query<NameValueOutPutDto>(sql, new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") });
        }

        #region 用户访问全国分布
        [AnnoInfo(Desc = "用户访问全国分布 IP2Region 未知数据归北京 默认最近6个月数据 数据缓存20分钟")]
        [CacheLRU(100, 1200, false)]
        public dynamic UserAnalyse([AnnoInfo(Desc = "开始时间（2021-03-30）")] DateTime startDate, [AnnoInfo(Desc = "结束时间（2021-03-31）")] DateTime endDate)
        {
           var nameValues= InitData();

            if (startDate == null||startDate.Equals(DateTime.MinValue))
            {
                startDate = DateTime.Now.AddMonths(-6).AddDays(1);
            }
            if (endDate == null || endDate.Equals(DateTime.MinValue))
            {
                endDate = DateTime.Now.AddDays(1);
            }
            string sql = @"SELECT SUBSTRING_INDEX(Ip,':',1) Ip FROM  sys_trace WHERE   Timespan>=?startDate AND Timespan<?endDate;  ";
            var ips = DbInstance.Db.Ado.Query<string>(sql, new { startDate = startDate.ToString("yyyy-MM-dd"), endDate = endDate.ToString("yyyy-MM-dd") });
            foreach (var item in ips)
            {
                if (!string.IsNullOrWhiteSpace(item))
                {
                    var data = _search.BtreeSearch(item).Region;
                    var datas = data.Split('|');
                    if (datas.Length >= 3)
                    {
                        var nv = nameValues.Find(n => n.name.Equals(datas[2]));
                        if (nv == null)
                        {
                            nv = nameValues.Find(n => n.name.Equals("北京"));
                        }
                        nv.value = nv.value + 1;
                    }
                }
            }
            nameValues = nameValues.OrderByDescending(it => it.value).ToList();
            return nameValues;
        }

        private List<NameValueOutPutDto> InitData()
        {
            List<NameValueOutPutDto> nameValues = new List<NameValueOutPutDto>() {
           new NameValueOutPutDto() { name= "西藏", value= 0 },
        new NameValueOutPutDto() { name= "青海", value= 0 },
        new NameValueOutPutDto() { name= "宁夏", value= 0 },
        new NameValueOutPutDto() { name= "海南", value= 0 },
        new NameValueOutPutDto() { name= "甘肃", value= 0 },
        new NameValueOutPutDto() { name= "贵州", value= 0 },
        new NameValueOutPutDto() { name= "新疆", value= 0 },
        new NameValueOutPutDto() { name= "云南", value= 0 },
        new NameValueOutPutDto() { name= "重庆", value= 0 },
        new NameValueOutPutDto() { name= "吉林", value= 0 },
        new NameValueOutPutDto() { name= "山西", value= 0 },
        new NameValueOutPutDto() { name= "天津", value= 0 },
        new NameValueOutPutDto() { name= "江西", value= 0 },
        new NameValueOutPutDto() { name= "广西", value= 0 },
        new NameValueOutPutDto() { name= "陕西", value= 0 },
        new NameValueOutPutDto() { name= "黑龙江", value= 0 },
        new NameValueOutPutDto() { name= "内蒙古", value= 0 },
        new NameValueOutPutDto() { name= "安徽", value= 0 },
        new NameValueOutPutDto() { name= "北京", value= 0 },
        new NameValueOutPutDto() { name= "福建", value= 0 },
        new NameValueOutPutDto() { name= "上海", value= 0 },
        new NameValueOutPutDto() { name= "湖北", value= 0 },
        new NameValueOutPutDto() { name= "湖南", value= 0 },
        new NameValueOutPutDto() { name= "四川", value= 0 },
        new NameValueOutPutDto() { name= "辽宁", value= 0 },
        new NameValueOutPutDto() { name= "河北", value= 0 },
        new NameValueOutPutDto() { name= "河南", value= 0 },
        new NameValueOutPutDto() { name= "浙江", value= 0 },
        new NameValueOutPutDto() { name= "山东", value= 0 },
        new NameValueOutPutDto() { name= "江苏", value= 0 },
        new NameValueOutPutDto() { name= "广东", value= 0 },
        new NameValueOutPutDto() { name= "台湾", value= 0 },
        new NameValueOutPutDto() { name= "香港", value= 0 },
        new NameValueOutPutDto() { name= "澳门", value= 0 },
        new NameValueOutPutDto() { name= "南海诸岛", value= 0 }
            };

            return nameValues;
        }
        #endregion
    }
}
