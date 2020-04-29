using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using Dapper;

namespace BL.DL
{
    public class DbHandler
    {
        public BO.RunningUser CurrentUser { get; set; }
        private string _conString;
        private string _logDir;
        public DbHandler(string connectstring,BO.RunningUser ru,string strLogDir)
        {
            _conString = connectstring;
            this.CurrentUser = ru;
            _logDir = strLogDir;
        }

        

        public  string RunSp(string strProcName,ref Dapper.DynamicParameters pars)
        {            
            using (SqlConnection con = new SqlConnection(_conString))
            {
                try
                {
                    con.Query(strProcName,pars,null,true,null,System.Data.CommandType.StoredProcedure);
                    if (pars.Get<string>("err_ret") != "")
                    {
                        this.CurrentUser.AddMessage(pars.Get<string>("err_ret"));
                        return  pars.Get<string>("err_ret");

                    }
                    else
                    {
                        return "1";
                    }                    
                }
                catch (Exception e)
                {
                    log_error(e, strProcName, pars);
                    return e.Message;
                    
                }
                
                
            }
        }


        public  T Load<T>(string strSQL)
        {
            using (SqlConnection con = new SqlConnection(_conString))
            {
                try
                {
                    return con.Query<T>(strSQL).FirstOrDefault();
                }catch(Exception e)
                {
                    log_error(e, strSQL);
                    return default(T);
                }
                
            }
        }
        public  T Load<T>(string strSQL, object param)
        {
            using (SqlConnection con = new SqlConnection(_conString))
            {
                
                try
                {
                    return con.Query<T>(strSQL, param).FirstOrDefault();
                }
                catch (Exception e)
                {
                    log_error(e, strSQL,param);                    
                    return default(T);
                }

            }
        }
        public  T Load<T>(string strSQL, Dapper.DynamicParameters pars)
        {
            using (SqlConnection con = new SqlConnection(_conString))
            {
                try
                {
                    return con.Query<T>(strSQL, pars).FirstOrDefault();
                }catch(Exception e)
                {
                    log_error(e, strSQL, pars);
                    return default(T);
                }
                                

            }
        }
        public  IEnumerable<T> GetList<T>(string strSQL)
        {
            using (SqlConnection con = new SqlConnection(_conString))
            {
                try
                {
                    return con.Query<T>(strSQL);
                }catch(Exception e)
                {
                    log_error(e, strSQL);
                    return null;
                }
                

            }
        }
        public  IEnumerable<T> GetList<T>(string strSQL, object param)
        {
            using (SqlConnection con = new SqlConnection(_conString))
            {
                try
                {
                    return con.Query<T>(strSQL, param);
                }catch(Exception e)
                {
                    log_error(e, strSQL,param);
                    return null;
                }

            }
        }
        public  IEnumerable<T> GetList<T>(string strSQL, Dapper.DynamicParameters pars)
        {
            using (SqlConnection con = new SqlConnection(_conString))
            {
                try
                {
                    return con.Query<T>(strSQL, pars);
                } catch (Exception e)
                {
                    log_error(e, strSQL, pars);
                    return null;
                }
                
            }
        }

        public  System.Data.DataTable GetDataTable(string strSQL, List<DL.Param4DT> pars=null)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            using (SqlConnection con = new SqlConnection(_conString))
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
                try
                {
                    adapter.Fill(dt);
                }
                catch(Exception e)
                {
                    log_error(e, strSQL);
                }
                

                return dt;

            }
        }

        public  int SaveRecord(string strTable,DynamicParameters pars,BO.BaseBO rec)
        {
            var strPidField = strTable.Substring(0, 3) + "ID";
            var s = new System.Text.StringBuilder();
            bool bolInsert = true;
            if (rec.pid > 0) bolInsert = false;

            if (bolInsert)
            {
                s.Append(string.Format("INSERT INTO {0} (", strTable));
                pars.Add("DateInsert", DateTime.Now, System.Data.DbType.DateTime);
                pars.Add("UserInsert", this.CurrentUser.j03Login, System.Data.DbType.String);
            }
            else
            {
                s.Append(string.Format("UPDATE {0} SET ", strTable));
            }
            pars.Add("DateUpdate", DateTime.Now, System.Data.DbType.DateTime);
            pars.Add("UserUpdate", this.CurrentUser.j03Login, System.Data.DbType.String);
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


            using (SqlConnection con = new SqlConnection(_conString))
            {
                try
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
                catch(Exception e)
                {
                    
                    log_error(e, s.ToString(),pars);
                }
                                                        

            }


            return 0;
        }

        public  bool RunSql(string strSQL, object param=null)
        {
            using (SqlConnection con = new SqlConnection(_conString))
            {
                try
                {
                    if (con.Execute(strSQL, param) > 0)
                    {
                        return true;
                    }
                }catch(Exception e)
                {
                    log_error(e, strSQL, param);
                    return false;
                }
                
            }
            return false;
        }

        public  string GetSQL1_Ocas(string strPrefix)
        {
            return string.Format("a.{0}ID as pid,CASE WHEN GETDATE() BETWEEN a.ValidFrom AND a.ValidUntil THEN 0 ELSE 1 end as isclosed,'{0}' as entity", strPrefix);
        }


        
        private  void log_error(Exception e, string strSQL, DynamicParameters pars)
        {
            CurrentUser.AddMessage(e.Message);
            var strPath = string.Format("{0}\\sql-error-{1}.log", _logDir, DateTime.Now.ToString("yyyy.MM.dd"));

            System.IO.File.AppendAllLines(strPath, new List<string>() { "", "", "------------------------------", DateTime.Now.ToString(), "CURRENT USER-login: " + CurrentUser.j03Login, "CURRENT USER-name:" + CurrentUser.FullName, "SQL:", strSQL });

            if (pars != null)
            {
                string strVal = "";
                foreach (var strP in pars.ParameterNames)
                {

                    if (pars.Get<dynamic>(strP) != null)
                    {
                        strVal = pars.Get<dynamic>(strP).ToString();
                    }
                    System.IO.File.AppendAllLines(strPath, new List<string>() { "PARAM: " + strP + ", VALUE: " + strVal });

                }
            }
            
            System.IO.File.AppendAllLines(strPath, new List<string>() { "","ERROR: ", e.Message });
        }

        private  void log_error(Exception e,string strSQL,object param=null)
        {
            CurrentUser.AddMessage(e.Message);
            var strPath = string.Format("{0}\\sql-error-{1}.log", _logDir, DateTime.Now.ToString("yyyy.MM.dd"));
                        
            var strParams = "";
            if (param != null)
            {
                strParams = param.ToString();
            }
            System.IO.File.AppendAllLines(strPath, new List<string>() {"","","------------------------------", DateTime.Now.ToString(),"CURRENT USER-login: " + CurrentUser.j03Login, "CURRENT USER-name:" + CurrentUser.FullName, "SQL:", strSQL,"","PARAMs:",strParams,"","ERROR:", e.Message });            

        }

    }
}
