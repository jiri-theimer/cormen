﻿using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
using Telerik.Reporting;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;
using System.Collections.Generic;
using Telerik.Reporting.Services.Engine;
using System.IO;



namespace UI.Controllers
{    
    [Route("api/reports")]    
    public class ReportsController : ReportsControllerBase
    {
        
        public ReportsController(IReportServiceConfiguration reportServiceConfiguration,BL.RunningApp app) : base(reportServiceConfiguration)
        {
            
          
            var resolver = new CustomReportSourceResolver(app);
            reportServiceConfiguration.ReportSourceResolver = resolver;

        }



        

    }


    public class CustomReportSourceResolver : IReportSourceResolver
    {
        private BL.RunningApp _app;
        public CustomReportSourceResolver(BL.RunningApp app)
        {
            _app = app;
        }
        public Telerik.Reporting.ReportSource Resolve(string reportId, OperationOrigin operationOrigin, IDictionary<string, object> currentParameterValues)
        {            
            //soubor sestavy###login uživatele###j72id
            List<string> lis = BO.BAS.ConvertString2List(reportId, "###");
            reportId = lis[0];
            string strLogin = lis[1];
            int intJ72ID = 0;
            if (lis.Count > 2)
            {
                intJ72ID = BO.BAS.InInt(lis[2]);
            }
            

            string reportXml = File.ReadAllText(_app.ReportFolder + "\\" + reportId);

            if (reportXml.Contains("1=1"))
            {
                var cu = new BO.RunningUser() { j03Login = strLogin };
                BL.Factory f = new BL.Factory(cu, _app);
                if (intJ72ID > 0)
                {
                    var recJ72 = f.j72TheGridTemplateBL.Load(intJ72ID);                    
                    var mq = new BO.myQuery(recJ72.j72Entity);
                    mq.lisJ73= f.j72TheGridTemplateBL.GetList_j73(intJ72ID, recJ72.j72Entity.Substring(0, 3));

                    
                    BL.DL.FinalSqlCommand fq = BL.DL.basQuery.ParseFinalSql("", mq, cu);
                    //File.WriteAllText("c:\\temp\\hovado.txt", fq.SqlWhere);
                    reportXml = reportXml.Replace("1=1", fq.SqlWhere).Replace("#query_alias#", recJ72.j72Name);
                    
                }
                
            }
            
            


            return new Telerik.Reporting.XmlReportSource { Xml = reportXml };
        }
    }


}