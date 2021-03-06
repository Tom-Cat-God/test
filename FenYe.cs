﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using System.Configuration;


namespace ykx
{
    public class FenYe
    {
        public static string FY(string ashxName,string templateName, string tableName, string requestNum,string id,string adminId)
        {
            int yeShu = int.Parse(ConfigurationManager.AppSettings["pagecount"].ToString());
            int PageNumber = 1;
            if (requestNum != null)
            {
                //前后端用正则表达式的形式有点不一样，道理是一样的。
                Regex r = new Regex(@"^\d*$");
                if (r.IsMatch(requestNum))
                    PageNumber = int.Parse(requestNum);
            }
            DataTable dt = SqlHelper.ExecuteDataTable("select * from (select *,ROW_NUMBER() over( order by "+id+" asc) as num from "+ tableName + " p where IS_DELETE = 0 and  Admin_ID = "+ adminId + ") s where s.num>@Start and s.num<@End ",
                new SqlParameter("@Start", (PageNumber - 1) * yeShu),
                new SqlParameter("@End", PageNumber * yeShu +1));
            // var Data = new { Products = dt.Rows };

            ////////////////////////////////////////////
            int totalCount = (int)SqlHelper.ExecuteScalar("select count(*) from "+ tableName + " where IS_DELETE = 0");
            int pageCount = (int)Math.Ceiling(totalCount / (float)yeShu);
            object[] pageData = new object[pageCount];
            for (int i = 0; i < pageCount; i++)
            {
                pageData[i] = new { Href = ""+ ashxName + "?Action=Search&PageNumber=" + (i + 1), Title = i + 1 };
            }
            var Data = new { Products = dt.Rows, PageData = pageData, TotalCount = totalCount, PageNumber = PageNumber, PageCount = pageCount };
            ////////////////////////////////////////////
            string html = CommonHelper.RenderHtml(templateName, Data);
            return html;
        }
    }
}