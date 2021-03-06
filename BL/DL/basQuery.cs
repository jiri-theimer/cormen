﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using BO;

namespace BL.DL
{
    public class basQuery
    {
        private static void ParseJ73Query(ref List<DL.QueryRow> lis, BO.myQuery mq)
        {
            int x = 0; string ss = ""; string strField = ""; string strAndOrZleva = "";

            foreach (var c in mq.lisJ73)
            {
                x += 1;
                ss = x.ToString();
                strField = c.j73Column;
                if (c.FieldSqlSyntax != null)
                {
                    strField = c.FieldSqlSyntax;
                }
                strAndOrZleva = c.j73Op;

                switch (c.j73Operator)
                {
                    case "ISNULL":
                        AQ(ref lis, strField + " IS NULL", "", null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        break;
                    case "NOT-ISNULL":
                        AQ(ref lis, strField + " IS NOT NULL", "", null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        break;
                    case "GREATERZERO":
                        AQ(ref lis, "ISNULL(" + strField + ",0)>0", "", null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        break;
                    case "ISNULLORZERO":
                        AQ(ref lis, "ISNULL(" + strField + ",0)=0", "", null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        break;
                    case "CONTAINS":
                        AQ(ref lis, strField + " LIKE '%" + BO.BAS.GSS(c.j73Value) + "+%'", null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        break;
                    case "STARTS":
                        AQ(ref lis, strField + " LIKE '" + BO.BAS.GSS(c.j73Value) + "+%'", null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        break;
                    case "INTERVAL":
                        if (c.FieldType == "date")
                        {
                            if (c.j73DatePeriodFlag > 0)
                            {
                                c.j73Date1 = mq.lisPeriods.Where(p => p.pid == c.j73DatePeriodFlag).First().d1;
                                c.j73Date2 = Convert.ToDateTime(mq.lisPeriods.Where(p => p.pid == c.j73DatePeriodFlag).First().d2).AddDays(1).AddMinutes(-1);
                            }
                            if (c.j73Date1 != null && c.j73Date2 != null)
                            {
                                AQ(ref lis, c.WrapFilter(strField + " BETWEEN " + BO.BAS.GD(c.j73Date1) + " AND " + BO.BAS.GD(c.j73Date2)), null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                            }
                            else
                            {
                                if (c.j73Date1 != null)
                                {
                                    AQ(ref lis, c.WrapFilter(strField + ">=" + BO.BAS.GD(c.j73Date1)), null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                                }
                                if (c.j73Date2 != null)
                                {
                                    AQ(ref lis, c.WrapFilter(strField + "<=" + BO.BAS.GD(c.j73Date2)), null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                                }
                            }

                        }
                        if (c.FieldType == "number")
                        {
                            AQ(ref lis, c.WrapFilter(strField + " BETWEEN " + BO.BAS.GN(c.j73Num1) + " AND " + BO.BAS.GN(c.j73Num2)), null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        }
                        break;
                    case "EQUAL":
                    case "NOT-EQUAL":
                        string strOper = "=";
                        if (c.j73Operator == "NOT-EQUAL")
                        {
                            strOper = "<>";
                        }
                        if (c.FieldType == "bool")
                        {
                            AQ(ref lis, strField + " " + strOper + " " + c.j73Value, "", null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        }
                        if (c.FieldType == "string")
                        {
                            AQ(ref lis, strField + " " + strOper + " '" + BO.BAS.GSS(c.j73Value) + "'", null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        }
                        if (c.FieldType == "combo")
                        {
                            AQ(ref lis, c.WrapFilter(strField + " " + strOper + " " + c.j73ComboValue.ToString()), null, null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);

                        }
                        if (c.FieldType == "multi")
                        {
                            strOper = "IN";
                            if (c.j73Operator == "NOT-EQUAL")
                            {
                                strOper = "NOT IN";
                            }
                            AQ(ref lis, c.WrapFilter(strField + " " + strOper + " (" + c.j73Value + ")"), "", null, strAndOrZleva, c.j73BracketLeft, c.j73BracketRight);
                        }
                        break;
                }

            }
        }
        public static DL.FinalSqlCommand ParseFinalSql(string strPrimarySql, BO.myQuery mq, BO.RunningUser ru, bool bolPrepareParam4DT = false)
        {
            var lis = new List<DL.QueryRow>();
            if (mq.lisJ73 != null)
            {
                ParseJ73Query(ref lis, mq);
            }
            if (mq.IsRecordValid == true)
            {
                AQ(ref lis, "a.ValidUntil>GETDATE()", "", null);
            }
            if (mq.IsRecordValid == false)
            {
                AQ(ref lis, "GETDATE() NOT BETWEEN a.ValidFrom AND a.ValidUntil", "", null);
            }
            if (mq.global_d2 != null && Convert.ToDateTime(mq.global_d2).Hour==0)
            {
                mq.global_d2 = Convert.ToDateTime(mq.global_d2).AddDays(1).AddMinutes(-1);
            }
            if (mq.global_d1 != null && mq.global_d2 != null)
            {
                if (mq.Prefix == "p41") AQ(ref lis, "(a.p41PlanStart BETWEEN @d1 AND @d2 OR a.p41PlanEnd BETWEEN @d1 AND @d2 OR (@d1 >= a.p41PlanStart AND @d2<=a.p41PlanEnd))", "d1", mq.global_d1, "AND", null, null, "d2", mq.global_d2);
                if (mq.Prefix == "p44") AQ(ref lis, "a.p41ID IN (select p41ID FROM p41Task WHERE p41PlanStart BETWEEN @d1 AND @d2 OR p41PlanEnd BETWEEN @d1 AND @d2 OR (@d1 >= p41PlanStart AND @d2<=p41PlanEnd))", "d1", mq.global_d1, "AND", null, null, "d2", mq.global_d2);
                if (mq.Prefix=="y02") AQ(ref lis, "p33Date BETWEEN @d1 AND @d2", "d1", mq.global_d1, "AND", null, null, "d2", mq.global_d2);
            }

            if (mq.pids != null && mq.pids.Any())
            {
                AQ(ref lis, mq.PkField + " IN (" + String.Join(",", mq.pids) + ")", "", null);
            }
            if (mq.b03id > 0)
            {
                if (mq.Prefix == "b02") AQ(ref lis, "a.b02ID IN (select b02ID FROM b04StatusGroupBinding WHERE b03ID=@b03id)", "b03id", mq.b03id);
            }
            if (mq.b02id > 0)
            {
                AQ(ref lis, "a.b02ID=@b02id", "b02id", mq.b02id);
            }
            if (mq.b02ids != null && mq.b02ids.Count() > 0)
            {
                switch (mq.Prefix)
                {
                    case "z01":
                    case "z02":
                    
                        break;
                    case "p44":
                        AQ(ref lis, "a.p41ID IN (select p41ID FROM p41Task WHERE b02ID IN (" + string.Join(",", mq.b02ids) + "))", "", null);
                        break;
                    default:
                        AQ(ref lis, "a.b02ID IN (select b02ID FROM b02Status WHERE b02ID IN (" + string.Join(",", mq.b02ids) + "))", "", null);
                        break;
                }

            }
            if (mq.j04id > 0)
            {
                AQ(ref lis, "a.j04ID=@j04id", "j04id", mq.j04id);
            }
            if (mq.j72id > 0)
            {
                if (mq.Prefix == "j04") AQ(ref lis, "a.j04ID IN (select j04ID FROM j74TheGridReceiver WHERE j72ID=@j72id)", "j72id", mq.j72id);
            }

            if (mq.DateBetween != null && mq.DateBetweenDays > 0)
            {
                AQ(ref lis, string.Format("(a.p41PlanStart BETWEEN @d1 AND DATEADD(DAY,{0},@d1) OR a.p41PlanEnd BETWEEN @d1 AND DATEADD(DAY,{0},@d1) OR @d1 BETWEEN a.p41PlanStart AND a.p41PlanEnd OR DATEADD(DAY,{0},@d1) BETWEEN a.p41PlanStart AND a.p41PlanEnd)", mq.DateBetweenDays), "d1", mq.DateBetween.Value);
            }
            if (mq.p19id > 0)
            {
                if (mq.Prefix == "p10") AQ(ref lis, "a.p10ID IN (select xa.p10ID FROM p10MasterProduct xa INNER JOIN p13MasterTpv xb ON xa.p13ID=xb.p13ID INNER JOIN p14MasterOper xc ON xb.p13ID=xc.p13ID WHERE xc.p19ID=@p19id)", "p19id", mq.p19id);
                if (mq.Prefix == "p13") AQ(ref lis, "a.p13ID IN (select xa.p13ID FROM p10MasterProduct xa INNER JOIN p13MasterTpv xb ON xa.p13ID=xb.p13ID INNER JOIN p14MasterOper xc ON xb.p13ID=xc.p13ID WHERE xc.p19ID=@p19id)", "p19id", mq.p19id);
            }
            if (mq.p21id > 0)
            {
                if (mq.Prefix == "p10") AQ(ref lis, "a.p10ID IN (select p10ID FROM p22LicenseBinding WHERE p21ID=@p21id)", "p21id", mq.p21id);
                if (mq.Prefix == "p13") AQ(ref lis, "a.p13ID IN (select xb.p13ID FROM p22LicenseBinding xa INNER JOIN p10MasterProduct xb ON xa.p10ID=xb.p10ID WHERE xa.p21ID=@p21id)", "p21id", mq.p21id);
                if (mq.Prefix == "p11" || mq.Prefix == "p12") AQ(ref lis, "a.p21ID=@p21id", "p21id", mq.p21id);
                if (mq.Prefix == "o23") AQ(ref lis, "a.o23Entity LIKE 'p21License' AND a.o23RecordPid=@p21id", "p21id", mq.p21id);
            }
            if (mq.p21id_missing > 0)
            {
                if (mq.Prefix == "p10") AQ(ref lis, "a.p10ID NOT IN (select p10ID FROM p22LicenseBinding WHERE p21ID=@p21id)", "p21id", mq.p21id_missing);

            }
            if (mq.p10id > 0)
            {
                if (mq.Prefix == "p21") AQ(ref lis, "a.p21ID IN (select p21ID FROM p22LicenseBinding WHERE p10ID=@p10id)", "p10id", mq.p10id);
                if (mq.Prefix == "p11") AQ(ref lis, "a.p10ID_Master=@p10id", "p10id", mq.p10id);
                if (mq.Prefix == "o23") AQ(ref lis, "a.o23Entity LIKE 'p10MasterProduct' AND a.o23RecordPid=@p10id", "p10id", mq.p10id);
                if (mq.Prefix == "p14") AQ(ref lis, "a.p13ID IN (select p13ID FROM p10MasterProduct WHERE p10ID=@p10id)", "p10id", mq.p10id);
            }
            if (mq.p12id > 0)
            {
                if (mq.Prefix == "p11" || mq.Prefix == "p15") AQ(ref lis, "a.p12ID=@p12id", "p12id", mq.p12id);
                if (mq.Prefix == "o23") AQ(ref lis, "a.o23Entity LIKE 'p12ClientTpv' AND a.o23RecordPid=@p12id", "p12id", mq.p12id);
            }
            if (mq.p11id > 0)
            {
                if (mq.Prefix == "o23") AQ(ref lis, "a.o23Entity LIKE 'p11ClientProduct' AND a.o23RecordPid=@p10id", "p11id", mq.p11id);
                if (mq.Prefix == "p21") AQ(ref lis, "a.p21ID IN (select p21ID FROM p11ClientProduct WHERE p11ID=@p11id)", "p11id", mq.p11id);
                if (mq.Prefix == "p51") AQ(ref lis, "a.p51ID IN (select p51ID FROM p52OrderItem WHERE p11ID=@p11id)", "p11id", mq.p11id);
                if (mq.Prefix == "p52") AQ(ref lis, "a.p11ID=@p11id", "p11id", mq.p11id);
                if (mq.Prefix == "p41") AQ(ref lis, "a.p52ID IN (select p52ID FROM p52OrderItem WHERE p11ID=@p11id)", "p11id", mq.p11id);
                if (mq.Prefix == "p15") AQ(ref lis, "a.p12ID IN (select p12ID FROM p11ClientProduct WHERE p11ID=@p11id)", "p11id", mq.p11id);
                //if (mq.Prefix == "p44") AQ(ref lis, "a.p41ID IN (select xa.p41ID FROM p41Task xa INNER JOIN p52OrderItem xb ON xa.p52ID=xb.p52ID WHERE xb.p11ID=@p11id)", "p11id", mq.p11id);
                if (mq.Prefix == "p44") AQ(ref lis, "a.p19ID IN (select xa.p19ID FROM p19Material xa INNER JOIN p11ClientProduct xb ON xa.p19Code=xb.p11Code WHERE xb.p11ID=@p11id)", "p11id", mq.p11id);

            }
            if (mq.p13id > 0)
            {
                if (mq.Prefix == "p10" || mq.Prefix == "p14") AQ(ref lis, "a.p13ID=@p13id", "p13id", mq.p13id);
                if (mq.Prefix == "p12") AQ(ref lis, "a.p13ID_Master=@p13id", "p13id", mq.p13id);
                if (mq.Prefix == "p11") AQ(ref lis, "a.p11ID IN (select xa.p11ID FROM p11ClientProduct xa INNER JOIN p12ClientTpv xb ON xa.p12ID=xb.p12ID WHERE xb.p13ID_Master=@p13id)", "p13id", mq.p13id);
                if (mq.Prefix == "o23") AQ(ref lis, "a.o23Entity LIKE 'p13MasterTpv' AND a.o23RecordPid=@p13id", "p13id", mq.p13id);
            }

            if (mq.p28id > 0)
            {
                if (mq.Prefix == "j02" || mq.Prefix == "p26" || mq.Prefix == "p21") AQ(ref lis, "a.p28ID=@p28id", "p28id", mq.p28id);
                if (mq.Prefix == "p11" || mq.Prefix == "p12") AQ(ref lis, "a.p21ID IN (select p21ID FROM p21License WHERE p28ID=@p28id)", "p28id", mq.p28id);
                if (mq.Prefix == "o23") AQ(ref lis, "a.o23Entity LIKE 'p28Company' AND a.o23RecordPid=@p28id", "p28id", mq.p28id);
                if (mq.Prefix == "p51") AQ(ref lis, "a.p28ID=@p28id", "p28id", mq.p28id);
                if (mq.Prefix == "p41") AQ(ref lis, "a.p52ID IN (select xa.p52ID FROM p52OrderItem xa INNER JOIN p51Order xb ON xa.p51ID=xb.p51ID WHERE xb.p28ID=@p28id)", "p28id", mq.p28id);
            }
            if (mq.j02id > 0)
            {
                if (mq.Prefix == "o23") AQ(ref lis, "a.o23Entity LIKE 'j02Person' AND a.o23RecordPid=@j02id", "j02id", mq.j02id);
                if (mq.Prefix == "p51" || mq.Prefix == "p41") AQ(ref lis, "a.j02ID_Owner=@j02id", "j02id", mq.j02id);
                if (mq.Prefix == "j90" || mq.Prefix == "j92" || mq.Prefix == "x40") AQ(ref lis, "a.j03ID IN (select j03ID FROM j03User WHERE j02ID=@j02id)", "j02id", mq.j02id);

            }
            if (mq.p25id > 0)
            {
                if (mq.Prefix == "p26") AQ(ref lis, "a.p25ID=@p25id", "p25id", mq.p25id);
                if (mq.Prefix == "p18") AQ(ref lis, "a.p25ID=@p25id", "p25id", mq.p25id);
                if (mq.Prefix == "p27") AQ(ref lis, "a.p27ID IN (select xa.p27ID FROM p29MszUnitBinding xa INNER JOIN p26Msz xb ON xa.p26ID=xb.p26ID WHERE xb.p25ID=@p25id)", "p25id", mq.p25id);
            }
            //if (mq.p25ids !=null && mq.p25ids.Count > 0)
            //{
            //    if (mq.Prefix == "p26") AQ(ref lis, "a.p25ID IN (" + string.Join(",", mq.p25ids) + ")", "", null);
            //    if (mq.Prefix == "p18") AQ(ref lis, "a.p25ID IN (" + string.Join(",", mq.p25ids) + ")", "", null);
            //    if (mq.Prefix == "p27") AQ(ref lis, "a.p27ID IN (select xa.p27ID FROM p29MszUnitBinding xa INNER JOIN p26Msz xb ON xa.p26ID=xb.p26ID WHERE xb.p25ID IN (" + string.Join(",", mq.p25ids) + "))", "", null);
            //}
            if (mq.p18flag > 0)
            {
                if (mq.Prefix == "p18") AQ(ref lis, "a.p18Flag=@p18flag", "p18flag", mq.p18flag);
            }
            if (mq.p18flags != null && mq.p18flags.Count() > 0)
            {
                if (mq.Prefix == "p18") AQ(ref lis, "a.p18Flag IN (" + string.Join(",", mq.p18flags) + ")", "", null);
                if (mq.Prefix == "p41") AQ(ref lis, "a.p41ID IN (select xa.p41ID FROM p44TaskOperPlan xa INNER JOIN p18OperCode xb ON xa.p18ID=xb.p18ID WHERE xb.p18Flag IN (" + string.Join(",", mq.p18flags) + "))", "", null);
            }
            if (mq.p26id > 0)
            {
                if (mq.Prefix == "o23") AQ(ref lis, "a.o23Entity LIKE 'p26Msz' AND a.o23RecordPid=@p26id", "p26id", mq.p26id);
                if (mq.Prefix == "p27") AQ(ref lis, "a.p27ID IN (select p27ID FROM p29MszUnitBinding WHERE p26ID=@p26id)", "p26id", mq.p26id);
                if (mq.Prefix == "p41") AQ(ref lis, "a.p27ID IN (select p27ID FROM p29MszUnitBinding WHERE p26ID=@p26id)", "p26id", mq.p26id);

            }
            if (mq.p26ids != null && mq.p26ids.Count() > 0)
            {
                if (mq.Prefix == "p41") AQ(ref lis, "a.p27ID IN (select p27ID FROM p29MszUnitBinding WHERE p26ID IN (" + string.Join(",", mq.p26ids) + "))", "", null);
                if (mq.Prefix == "p27") AQ(ref lis, "a.p27ID IN (select p27ID FROM p29MszUnitBinding WHERE p26ID IN (" + string.Join(",", mq.p26ids) + "))", "", null);
            }
            if (mq.p27id > 0)
            {
                if (mq.Prefix == "p18") AQ(ref lis, "a.p25ID IN (select p25ID_HW FROM p27MszUnit WHERE p27ID=@p27id)", "p27id", mq.p27id);  //zde se musí pracovat s kmenovým typem zařízení definovaným ve středisku!
            }
            if (mq.p27ids != null && mq.p27ids.Count() > 0)
            {
                if (mq.Prefix == "p41") AQ(ref lis, "a.p27ID IN (" + string.Join(",", mq.p27ids) + ")", "", null);
            }

            if (mq.p41id > 0)
            {
                if (mq.Prefix == "o23") AQ(ref lis, "a.o23Entity LIKE 'p41Task' AND a.o23RecordPid=@p41id", "p41id", mq.p41id);
                if (mq.Prefix == "p15") AQ(ref lis, "a.p12ID IN (select xb.p12ID FROM p52OrderItem xa INNER JOIN p11ClientProduct xb ON xa.p11ID=xb.p11ID INNER JOIN p41Task xc ON xa.p52ID=xc.p52ID WHERE xc.p41ID=@p41id)", "p41id", mq.p41id);
                if (mq.Prefix == "p44") AQ(ref lis, "a.p41ID=@p41id", "p41id", mq.p41id);
                if (mq.Prefix == "p45") AQ(ref lis, "a.p41ID=@p41id", "p41id", mq.p41id);

                //zde se musí pracovat s kmenovým typem zařízení definovaným ve středisku!
                if (mq.Prefix == "p18") AQ(ref lis, "a.p25ID IN (select xa.p25ID_HW FROM p27MszUnit xa INNER JOIN p41Task xb ON xa.p27ID=xb.p27ID WHERE xb.p41ID=@p41id)", "p41id", mq.p41id);
            }
            if (mq.p51id > 0)
            {
                if (mq.Prefix == "p52") AQ(ref lis, "a.p51ID=@p51id", "p51id", mq.p51id);
                if (mq.Prefix == "o23") AQ(ref lis, "a.o23Entity LIKE 'p51Order' AND a.o23RecordPid=@p51id", "p51id", mq.p51id);
                if (mq.Prefix == "p41") AQ(ref lis, "a.p52ID IN (select p52ID FROM p52OrderItem WHERE p51ID=@p51id)", "p51id", mq.p51id);

            }
            if (mq.p52id > 0)
            {
                if (mq.Prefix == "p41") AQ(ref lis, "a.p52ID=@p52id", "p52id", mq.p52id);
            }
            if (mq.o53id > 0)
            {
                if (mq.Prefix == "o51") AQ(ref lis, "a.o53ID=@o53id", "o53id", mq.o53id);
            }

            if (mq.Prefix == "b02" && !string.IsNullOrEmpty(mq.query_by_entity_prefix))
            {
                AQ(ref lis, "a.b02Entity=@prefix", "prefix", mq.query_by_entity_prefix);    //filtr seznamu stavů podle druhu entity
            }
            if (mq.Prefix == "o12" && !string.IsNullOrEmpty(mq.query_by_entity_prefix))
            {
                AQ(ref lis, "a.o12Entity=@prefix", "prefix", mq.query_by_entity_prefix);    //filtr seznamu kategorií podle druhu entity
            }

            if (mq.TheGridFilter != null)
            {
                ParseSqlFromTheGridFilter(mq, ref lis);  //složit filtrovací podmínku ze sloupcového filtru gridu
            }
            if (ru.j03EnvironmentFlag == 2)    //odstínit data pouze pro držitele licence, tj. firmu s licencí na p10SwLicenseFlag>0
            {
                //if (mq.Prefix == "j02")
                //{
                //    AQ(ref lis, "(a.p28ID=@p28id_my OR a.j02ID_Owner IN (select j02ID FROM j02Person WHERE p28ID=@p28id_my)", "p28id_my", ru.p28ID);
                //}
                if (mq.Prefix == "o23" || mq.Prefix == "p41" || mq.Prefix == "p31" || mq.Prefix == "p51")
                {

                    AQ(ref lis, "a.j02ID_Owner IN (select j02ID FROM j02Person WHERE p28ID=@p28id_my)", "p28id_my", ru.p28ID);
                }
                if (mq.Prefix == "p52")
                {
                    AQ(ref lis, "a.p51ID IN (select xa.p51ID FROM p51Order xa INNER JOIN j02Person xb ON xa.j02ID_Owner=xb.j02ID WHERE xb.p28ID=@p28id_my)", "p28id_my", ru.p28ID);    //pouze objednávky cloud klienta
                }
                if (mq.Prefix == "p28" || mq.Prefix == "j02")
                {

                    AQ(ref lis, "(a.p28ID=@p28id_my OR a.j02ID_Owner IN (select j02ID FROM j02Person WHERE p28ID=@p28id_my))", "p28id_my", ru.p28ID);
                }
                if (mq.Prefix == "p11" || mq.Prefix == "p12")
                {
                    AQ(ref lis, "a.p21ID IN (SELECT p21ID FROM p21License WHERE p28ID = @p28id_my)", "p28id_my", ru.p28ID);    //pouze produkty v licenci klienta
                }
                if (mq.Prefix == "p26" || mq.Prefix == "p21")
                {
                    AQ(ref lis, "a.p28ID = @p28id", "p28id", ru.p28ID);    //pouze licence a stroje klienta
                }
                if (mq.Prefix == "p25")
                {
                    AQ(ref lis, "a.p25ID IN (select p25ID FROM p26Msz WHERE p28ID=@p28id)", "p28id", ru.p28ID);    //pouze typy zařízení za klientovi stroje
                }
                if (mq.Prefix == "p27")
                {
                    AQ(ref lis, "a.p27ID IN (select xa.p27ID FROM p29MszUnitBinding xa INNER JOIN p26Msz xb ON xa.p26ID=xb.p26ID WHERE xb.p28ID=@p28id_my)", "p28id_my", ru.p28ID);    //pouze klientovi stroje
                }
                if (mq.Prefix == "j04")
                {
                    AQ(ref lis, "a.j04IsClientRole=1", "", null); //pouze klientské role
                }
                if (mq.Prefix == "p19" || mq.Prefix == "p20") //klientský materiál nebo měrné jednotky
                {
                    AQ(ref lis, "(a.p28ID IS NULL OR a.p28ID=@p28id)", "p28id", ru.p28ID);
                }
            }




            if (String.IsNullOrEmpty(mq.SearchString) == false && mq.SearchString.Length > 1)
            {
                if (mq.Prefix == "p28")
                {
                    AQ(ref lis, "(a.p28Name LIKE '%'+@expr+'%' OR a.p28ShortName LIKE '%'+@expr+'%' OR a.p28RegID LIKE '%'+@expr+'%' OR a.p28VatID LIKE '%'+@expr+'%' OR a.p28Code LIKE '%'+@expr+'%' OR a.p28ID IN (select p28ID FROM j02Person WHERE p28ID IS NOT NULL AND j02LastName like '%'+@expr+'%'))", "expr", mq.SearchString);
                }
                if (mq.Prefix == "j02")
                {
                    AQ(ref lis, "(a.j02LastName LIKE '%'+@expr+'%' OR a.j02FirstName LIKE '%'+@expr+'%' OR a.j02Email LIKE '%'+@expr+'%' OR p28.p28Name LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
                if (mq.Prefix == "p51")
                {
                    AQ(ref lis, "(a.p51Name LIKE '%'+@expr+'%' OR a.p51Code LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
                if (mq.Prefix == "p52")
                {
                    AQ(ref lis, "a.p52ID IN (select xa.p52ID FROM p52OrderItem xa INNER JOIN p51Order xb ON xa.p51ID=xb.p51ID INNER JOIN p11ClientProduct xc ON xa.p11ID=xc.p11ID LEFT OUTER JOIN p28Company xd ON xb.p28ID=xd.p28ID WHERE xb.p51Name LIKE '%'+@expr+'%' OR xc.p11Name LIKE '%'+@expr+'%' OR xd.p28Name LIKE '%'+@expr+'%' OR xb.p51Code LIKE '%'+@expr+'%' OR xc.p11Code LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
                if (mq.Prefix == "p10")
                {
                    AQ(ref lis, "(a.p10Name LIKE '%'+@expr+'%' OR a.p10Code LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
                if (mq.Prefix == "p13")
                {
                    AQ(ref lis, "(a.p13Name LIKE '%'+@expr+'%' OR a.p13Code LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
                if (mq.Prefix == "o23")
                {
                    AQ(ref lis, "(a.o23Name LIKE '%'+@expr+'%' OR a.o23Code LIKE '%'+@expr+'%' OR a.o23Memo LIKE '%'+@expr+'%')", "expr", mq.SearchString);
                }
            }


            var ret = new DL.FinalSqlCommand();

            if (lis.Count > 0)
            {
                ret.Parameters = new Dapper.DynamicParameters();
                if (bolPrepareParam4DT) ret.Parameters4DT = new List<DL.Param4DT>();
                foreach (var c in lis.Where(p => String.IsNullOrEmpty(p.ParName) == false))
                {
                    ret.Parameters.Add(c.ParName, c.ParValue);
                    if (bolPrepareParam4DT) ret.Parameters4DT.Add(new DL.Param4DT() { ParName = c.ParName, ParValue = c.ParValue });

                }
                foreach (var c in lis.Where(p => String.IsNullOrEmpty(p.Par2Name) == false))
                {
                    ret.Parameters.Add(c.Par2Name, c.Par2Value);
                    if (bolPrepareParam4DT) ret.Parameters4DT.Add(new DL.Param4DT() { ParName = c.Par2Name, ParValue = c.Par2Value });
                }


                ret.SqlWhere = String.Join(" ", lis.Select(p => p.AndOrZleva + " " + p.BracketLeft + p.StringWhere + p.BracketRight)).Trim();    //složit závěrčnou podmínku
                //System.IO.File.WriteAllText("c:\\temp\\hovado"+mq.Prefix+".txt", ret.SqlWhere);

                ret.SqlWhere = String.Join(" ", lis.Select(p => p.AndOrZleva + " " + p.StringWhere)).Trim();    //složit závěrčnou podmínku

            }



            if (!string.IsNullOrEmpty(ret.SqlWhere))
            {
                strPrimarySql += " WHERE " + ret.SqlWhere;
            }
            if (!string.IsNullOrEmpty(mq.explicit_orderby))
            {
                strPrimarySql += " ORDER BY " + mq.explicit_orderby;
            }

            if (strPrimarySql.Contains("@gd1"))
            {   //view s napevno navrženou podmínkou časového filtru                            
                if (mq.global_d1 == null) mq.global_d1 = new DateTime(2000, 1, 1);
                if (mq.global_d2 == null)
                {
                    mq.global_d2 = new DateTime(3000, 1, 1);
                }
                
                if (ret.Parameters == null)
                {
                    ret.Parameters = new Dapper.DynamicParameters();
                    if (bolPrepareParam4DT) ret.Parameters4DT = new List<DL.Param4DT>();
                }
                ret.Parameters.Add("gd1", mq.global_d1, System.Data.DbType.DateTime);
                if (bolPrepareParam4DT) ret.Parameters4DT.Add(new DL.Param4DT() { ParName = "gd1", ParValue = mq.global_d1 });
                ret.Parameters.Add("gd2", mq.global_d2, System.Data.DbType.DateTime);
                if (bolPrepareParam4DT) ret.Parameters4DT.Add(new DL.Param4DT() { ParName = "gd2", ParValue = mq.global_d2 });
            }

            ret.FinalSql = strPrimarySql;
            return ret;

        }

        private static void AQ(ref List<DL.QueryRow> lis, string strWhere, string strParName, object ParValue, string strAndOrZleva = "AND", string strBracketLeft = null, string strBracketRight = null, string strPar2Name = null, object Par2Value = null)
        {
            if (lis.Count == 0)
            {
                strAndOrZleva = ""; //první podmínka zleva
            }
            if (String.IsNullOrEmpty(strParName) == false && lis.Where(p => p.ParName == strParName).Count() > 0)
            {
                return; //parametr strParName již byl dříve přidán
            }
            lis.Add(new DL.QueryRow() { StringWhere = strWhere, ParName = strParName, ParValue = ParValue, AndOrZleva = strAndOrZleva, BracketLeft = strBracketLeft, BracketRight = strBracketRight, Par2Name = strPar2Name, Par2Value = Par2Value });
        }

        private static object get_param_value(string colType, string colValue)
        {
            if (String.IsNullOrEmpty(colValue) == true)
            {
                return null;
            }
            if (colType == "num")
            {
                return BO.BAS.InDouble(colValue);
            }
            if (colType == "date")
            {
                return Convert.ToDateTime(colValue);
            }
            if (colType == "bool")
            {
                if (colValue == "1")
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

            return colValue;
        }
        private static void ParseSqlFromTheGridFilter(BO.myQuery mq, ref List<DL.QueryRow> lisAQ)
        {

            int x = 0;
            foreach (var filterrow in mq.TheGridFilter)
            {
                var col = filterrow.BoundColumn;
                var strF = col.getFinalSqlSyntax_WHERE();

                x += 1;
                string parName = "par" + x.ToString();

                int endIndex = 0;
                string[] arr = new string[] { filterrow.value };
                if (filterrow.value.IndexOf(";") > -1)  //v podmnínce sloupcového filtru může být středníkem odděleno více hodnot!
                {
                    arr = filterrow.value.Split(";");
                    endIndex = arr.Count() - 1;
                }

                switch (filterrow.oper)
                {
                    case "1":   //IS NULL
                        AQ(ref lisAQ, strF + " IS NULL", "", null);
                        break;
                    case "2":   //IS NOT NULL
                        AQ(ref lisAQ, strF + " IS NOT NULL", "", null);
                        break;
                    case "10":   //větší než nula
                        AQ(ref lisAQ, strF + " > 0", "", null);
                        break;
                    case "11":   //je nula nebo prázdné
                        AQ(ref lisAQ, "ISNULL(" + strF + ",0)=0", "", null);
                        break;
                    case "8":   //ANO
                        AQ(ref lisAQ, strF + " = 1", "", null);
                        break;
                    case "9":   //NE
                        AQ(ref lisAQ, strF + " = 0", "", null);
                        break;
                    case "3":   //obsahuje                
                        for (var i = 0; i <= endIndex; i++)
                        {
                            if (arr[i].Trim() != "")
                            {
                                AQ(ref lisAQ, leva_zavorka(i, endIndex) + string.Format(strF + " LIKE '%'+@{0}+'%'", parName + "i" + i.ToString()) + prava_zavorka(i, endIndex), parName + "i" + i.ToString(), arr[i], i == 0 ? "AND" : "OR"); ;
                            }

                        }

                        break;
                    case "5":   //začíná na 
                        for (var i = 0; i <= endIndex; i++)
                        {
                            if (arr[i].Trim() != "")
                            {
                                AQ(ref lisAQ, leva_zavorka(i, endIndex) + string.Format(strF + " LIKE @{0}+'%'", parName + "i" + i.ToString()) + prava_zavorka(i, endIndex), parName + "i" + i.ToString(), arr[i], i == 0 ? "AND" : "OR");
                            }

                        }

                        break;
                    case "6":   //je rovno
                        for (var i = 0; i <= endIndex; i++)
                        {
                            if (arr[i].Trim() != "")
                            {
                                AQ(ref lisAQ, leva_zavorka(i, endIndex) + string.Format(strF + " = @{0}", parName + "i" + i.ToString()) + prava_zavorka(i, endIndex), parName + "i" + i.ToString(), get_param_value(col.NormalizedTypeName, arr[i]), i == 0 ? "AND" : "OR");
                            }

                        }

                        break;
                    case "4":   //interval
                        AQ(ref lisAQ, string.Format(strF + " >= @{0}", parName + "c1"), parName + "c1", get_param_value(col.NormalizedTypeName, filterrow.c1value));
                        AQ(ref lisAQ, string.Format(strF + " <= @{0}", parName + "c2"), parName + "c2", get_param_value(col.NormalizedTypeName, filterrow.c2value));
                        break;
                    case "7":   //není rovno
                        for (var i = 0; i <= endIndex; i++)
                        {
                            if (arr[i].Trim() != "")
                            {
                                AQ(ref lisAQ, leva_zavorka(i, endIndex) + string.Format(strF + " <> @{0}", parName + "i" + i.ToString()) + prava_zavorka(i, endIndex), parName + "i" + i.ToString(), get_param_value(col.NormalizedTypeName, arr[i]), i == 0 ? "AND" : "OR");
                            }
                        }

                        break;
                }

            }


            string leva_zavorka(int i, int intEndIndex)
            {
                if (intEndIndex > 0 && i == 0)
                {
                    return "(";
                }
                else
                {
                    return "";
                }
            }
            string prava_zavorka(int i, int intEndIndex)
            {
                if (intEndIndex > 0 && i == intEndIndex)
                {
                    return ")";
                }
                else
                {
                    return "";
                }
            }

        }

    }
}
