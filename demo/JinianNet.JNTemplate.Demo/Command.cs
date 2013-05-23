using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace JinianNet.JNTemplate.Demo
{
    public class Command
    {
        HttpContext ctx;

        public Command(HttpContext context)
        {
            ctx = context;
        }

        private DbHelper db = new SQLiteHelper();
        public DataTable Execute(string sql)
        {
            return db.ExecuteTable(sql);
        }

        public string QueryString(string key)
        {
            return ctx.Request.QueryString[key];
        }

        public Hashtable GetArticle(string key)
        {
            int id;
            if (!string.IsNullOrEmpty(ctx.Request.QueryString[key]) && int.TryParse(ctx.Request.QueryString[key], out id) && id > 0)
            {
            
                using (DataTable dt = db.ExecuteTable("select * from JNC_Article where iD=" + id.ToString()))
                {
                    if (dt.Rows.Count > 0)
                    {
                        Hashtable hash = new Hashtable();
                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            hash[dt.Columns[i].ColumnName] = dt.Rows[0][dt.Columns[i].ColumnName];
                        }
                        return hash;
                    }
                }
            }

            return null;
        }

        public bool IsNull(object value)
        {
            return value == null;
        }
    }
}