using System;
using System.Web.Script.Serialization;
using System.Web.Services;
using Microsoft.ApplicationBlocks.Data;
using mUtilities.Data;
using System.Data.SqlClient;
using System.Data;
using System.Web.Script.Services;
using System.DirectoryServices.AccountManagement;
using System.Threading;
using System.Security;
using System.Security.Principal;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Web;
using System.Web.UI;
using System.Diagnostics;
using System.DirectoryServices;
using Newtonsoft.Json;
using System.Configuration;
public partial class Canvas : Page {
    protected void Page_Load(object sender, EventArgs e) {


    }
    [WebMethod]
    public static string GetData2( string empId, string phones) {
        //https://payzonekpiqa.enscoplc.com:81/canvas.aspx?empid={{customer.custom_empid}}
        //https://desksso-dev.enscoplc.com/sso/canvas.aspx?empid={{customer.custom_empid}}
        DataAccessor da = new DataAccessor(ConfigurationManager.AppSettings["ConnectionStringPS"]);
        string s = @"
select top 1 -- j.EFFDT,j.EFFSEQ, c.EFFDT,  
e.EMPLID [Employee ID]
,NAME 'Employee Name'
,c.DESCR 'Employee Classification'
,JOB_DESCR 'Job Description'
,DEPT_DESCR 'Department Description'
,LOC_DESCR ' Work Location'
,e.BUSINESS_UNIT 'Ensco BU'

, convert(varchar, p.BIRTHDATE, 101) DOB
, case when i.NATIONAL_ID IS null OR LEN(i.NATIONAL_ID)<4 then '' else RIGHT(NATIONAL_ID, 4) end SSN
, e.EMPL_STATUS  'Employee Status'
, ph.PHONE 'Work Phone Number'
--,JOBCODE 'Job Code'
from ENS_EMPLOYEE_INFO_VW e
join PS_PERSON p on e.EMPLID=p.EMPLID
join PS_JOB j on e.EMPLID=j.EMPLID-- and j.EFFDT=MAX(
join PS_SET_CNTRL_REC r on j.REG_REGION=r.SETCNTRLVALUE and r.RECNAME='EMPL_CLASS_TBL' 
join PS_EMPL_CLASS_TBL c on j.EMPL_CLASS=c.EMPL_CLASS and c.EFFDT<=j.EFFDT and r.SETID=c.SETID
left join PS_PERS_NID i on e.EMPLID=i.EMPLID AND i.COUNTRY='USA' and i.NATIONAL_ID_TYPE in ('PR', 'NID')
left join PS_PERSONAL_PHONE ph on e.EMPLID=ph.EMPLID and PHONE_TYPE='BUSN'
where e.EMPLID='" + empId + @"' 
order by j.EFFDT desc, EFFSEQ, c.EFFDT desc
";
        DataSet ds = da.GetDataSet(s);
        return ds.GetXml();

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        return "PS testing";
        //return serializer.Serialize(list);
    }
    [WebMethod]
    public static string GetData(string empId, string phones) {
        //https://payzonekpiqa.enscoplc.com:81/canvas.aspx?empid={{customer.custom_empid}}
        //https://desksso-dev.enscoplc.com/sso/canvas.aspx?empid={{customer.custom_empid}}
        DataAccessor da = new DataAccessor(ConfigurationManager.AppSettings["ConnectionStringPS"]);
        string s = @"
select top 1
e.EMPLID [Employee ID]
,NAME 'Employee Name'
,c.DESCR 'Employee Classification'
,JOB_DESCR 'Job Description'
,DEPT_DESCR 'Department Description'
,LOC_DESCR ' Work Location'
,e.BUSINESS_UNIT 'Ensco BU'
, convert(varchar, p.BIRTHDATE, 101) DOB
, case when i.NATIONAL_ID IS null OR LEN(i.NATIONAL_ID)<4 then '' else RIGHT(NATIONAL_ID, 4) end SSN
, e.EMPL_STATUS  'Employee Status'
, ph.PHONE 'Work Phone Number'
--, case when ph.PHONE in (" + phones + @") then 1 else 0 end Flag
from ENS_EMPLOYEE_INFO_VW e
join PS_PERSON p on e.EMPLID=p.EMPLID
join PS_JOB j on e.EMPLID=j.EMPLID
join PS_SET_CNTRL_REC r on j.REG_REGION=r.SETCNTRLVALUE and r.RECNAME='EMPL_CLASS_TBL' 
join PS_EMPL_CLASS_TBL c on j.EMPL_CLASS=c.EMPL_CLASS and c.EFFDT<=j.EFFDT and r.SETID=c.SETID
left join PS_PERS_NID i on e.EMPLID=i.EMPLID AND i.COUNTRY='USA' and i.NATIONAL_ID_TYPE in ('PR', 'NID')
left join PS_PERSONAL_PHONE ph on e.EMPLID=ph.EMPLID and PHONE_TYPE='BUSN'
where e.EMPLID='" + empId + @"' -- " + (phones == "" ? "" : " or ph.PHONE in (" + phones + ")") + @" 
order by j.EFFDT desc, EFFSEQ, c.EFFDT desc
";
        using (StreamReader sr = new StreamReader( HttpContext.Current.Server.MapPath("sql.txt"))) {
            s = sr.ReadToEnd();
        }
        s = s.Replace("011311", empId);
        DataSet ds = da.GetDataSet(s);
        return ds.GetXml();

        JavaScriptSerializer serializer = new JavaScriptSerializer();
        return "PS testing";
        //return serializer.Serialize(list);
    }
}


