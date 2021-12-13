using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Newtonsoft.Json;

namespace Pikapikachu.CrystalReport.Client.Models
{
    public static class CrystalReportFactory
    {
        /// <summary>
        /// 反序列化為DataTable
        /// </summary>
        /// <param name="report">報表名稱</param>
        /// <param name="jsonString">json字串</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static DataTable CreateReport(string report, string jsonString)
        {
            switch (report)
            {
                case "shpbah":
                    return ToDataTable(JsonConvert.DeserializeObject<List<Shpbah>>(jsonString));
                default:
                    throw new Exception("報表名稱輸入不正確");
            }
        }


        private static DataTable ToDataTable<T>(IEnumerable<T> self)
        {
            var properties = typeof(T).GetProperties();

            var dataTable = new DataTable();
            foreach (var info in properties)
                dataTable.Columns.Add(info.Name, Nullable.GetUnderlyingType(info.PropertyType)
                                                 ?? info.PropertyType);

            foreach (var entity in self)
                dataTable.Rows.Add(properties.Select(p => p.GetValue(entity)).ToArray());


            return dataTable;
        }
    }

    public enum ReportName
    {
        shpbah
    }
}