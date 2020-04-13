using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Dapper;


namespace BL.DL
{
    public class DbHandler
    {
        private static string _connectString = "";
        private static string _log = "c:\\temp\\hovado.txt";

         private static string GetConString()
        {
            if (_connectString == "")
            {
                var relativePath = @"../UI/appsettings.json";
                var s = System.IO.File.ReadAllText(System.IO.Path.GetFullPath(relativePath));

                var lis = s.Split("\"");

                _connectString = lis.Where(p => p.Contains("server=") == true).First().Replace("\\\\", "\\");
            }            
            return _connectString;
        }

        public static string RunSp(string strProcName, Dapper.DynamicParameters pars)
        {            
            using (SqlConnection con = new SqlConnection(GetConString()))
            {
                try
                {
                    con.Query(strProcName,pars,null,true,null,System.Data.CommandType.StoredProcedure);
                    if (pars.Get<string>("err_ret") != "")
                    {
                        return  pars.Get<string>("err_ret");

                    }
                    else
                    {
                        return "1";
                    }                    
                }
                catch (Exception e)
                {
                    return e.Message;
                    
                }
                
                
            }
        }


        public static T Load<T>(string strSQL)
        {
            using (SqlConnection con = new SqlConnection(GetConString()))
            {
                return con.Query<T>(strSQL).FirstOrDefault();
                
            }
        }
        public static T Load<T>(string strSQL, object param)
        {
            using (SqlConnection con = new SqlConnection(GetConString()))
            {
                
                try
                {
                    return con.Query<T>(strSQL, param).FirstOrDefault();
                }
                catch (Exception e)
                {
                    System.IO.File.WriteAllText(_log, e.Message);
                    return default(T);
                }

            }
        }
        public static T Load<T>(string strSQL, Dapper.DynamicParameters pars)
        {
            using (SqlConnection con = new SqlConnection(GetConString()))
            {                            
                return con.Query<T>(strSQL, pars).FirstOrDefault();
                

            }
        }
        public static IEnumerable<T> GetList<T>(string strSQL)
        {
            using (SqlConnection con = new SqlConnection(GetConString()))
            {
                try
                {
                    return con.Query<T>(strSQL);
                }catch(Exception e)
                {
                    System.IO.File.WriteAllText(_log, e.Message);
                    return null;
                }
                

            }
        }
        public static IEnumerable<T> GetList<T>(string strSQL, object param)
        {
            using (SqlConnection con = new SqlConnection(GetConString()))
            {
                return con.Query<T>(strSQL, param);

            }
        }
        public static IEnumerable<T> GetList<T>(string strSQL, Dapper.DynamicParameters pars)
        {
            using (SqlConnection con = new SqlConnection(GetConString()))
            {
                return con.Query<T>(strSQL, pars);
                
            }
        }

        public static System.Data.DataTable GetDataTable(string strSQL, List<DL.Param4DT> pars=null)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            using (SqlConnection con = new SqlConnection(GetConString()))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = strSQL;
                cmd.Connection = con;
                if (pars != null)
                {
                    foreach (var p in pars)
                    {
                        cmd.Parameters.AddWithValue(p.ParName, p.ParValue);                        
                    }
                }
                
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(dt);

                return dt;

            }
        }

        public static int SaveRecord(BO.RunningUser u, string strTable,DynamicParameters pars,BO.BaseBO rec)
        {
            var strPidField = strTable.Substring(0, 3) + "ID";
            var s = new System.Text.StringBuilder();
            bool bolInsert = true;
            if (rec.pid > 0) bolInsert = false;

            if (bolInsert)
            {
                s.Append(string.Format("INSERT INTO {0} (", strTable));
                pars.Add("DateInsert", DateTime.Now, System.Data.DbType.DateTime);
                pars.Add("UserInsert", u.j02Login, System.Data.DbType.String);
            }
            else
            {
                s.Append(string.Format("UPDATE {0} SET ", strTable));
            }
            pars.Add("DateUpdate", DateTime.Now, System.Data.DbType.DateTime);
            pars.Add("UserUpdate", u.j02Login, System.Data.DbType.String);
            if (rec.ValidFrom == null) rec.ValidFrom = System.DateTime.Now;
            pars.Add("ValidFrom", rec.ValidFrom, System.Data.DbType.DateTime);
            if (rec.ValidUntil == null) rec.ValidUntil = new DateTime(3000, 1, 1);
            pars.Add("ValidUntil", rec.ValidUntil, System.Data.DbType.DateTime);


            string strF = "", strV = "";
            foreach(var strP in pars.ParameterNames.Where(p=>p != "pid"))
            {
                if (bolInsert)
                {
                    strF += "," + strP;
                    strV += ",@" + strP;
                }
                else
                {
                    strV += ","+ strP + " = @" + strP;
                }
            }            
            strV = strV.Substring(1, strV.Length - 1);
            if (bolInsert)
            {
                strF = strF.Substring(1, strF.Length - 1);
                s.Append(strF + ") VALUES (" + strV + ")");
            }
            else
            {
                s.Append(strV);
                s.Append(" WHERE " + strPidField + " = @pid");
            }


            using (SqlConnection con = new SqlConnection(GetConString()))
            {
                if (bolInsert)
                {
                    s.Append("; SELECT CAST(SCOPE_IDENTITY() as int) as Value");
                    return con.Query<BO.COM.GetInteger>(s.ToString(), pars).FirstOrDefault().Value;
                }
                else
                {
                    if (con.Execute(s.ToString(), pars) > 0)
                    {
                        return pars.Get<int>("pid");                        
                    }
                }                                                

            }


            return 0;
        }

        public static string GetSQL1_Ocas(string strPrefix)
        {
            return string.Format("a.{0}ID as pid,CASE WHEN GETDATE() BETWEEN a.ValidFrom AND a.ValidUntil THEN 0 ELSE 1 end as isclosed,'{0}' as entity", strPrefix);
        }

       

    }
}
