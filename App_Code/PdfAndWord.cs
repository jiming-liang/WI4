using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Drawing;
public class PdfAndWord
{
    public DataSet ds;
    public string BaseUrl;
    public string LessThanEncode;
    public string AmpEncode;
    public string Rig;
    public string SelectedStatus;
    public string Prefix = "WIT-";

    public string[] Headers;
    public string WiNo;

    protected Color orange = Color.FromArgb(232, 109, 31);
    protected Color deepblue = Color.FromArgb(0, 70, 127);
    protected Color lightblue = Color.FromArgb(61, 183, 228);
    public virtual void Start(System.Web.HttpResponse Response, string userName) {
    }
    protected string GetFileName() {
        string fileName = ds.Tables[0].Rows[0]["WiNo"].ToString();
        if (this.Rig != null)
            fileName = this.Prefix + this.Rig + "-" + this.ds.Tables[0].Rows[0]["WICategoryName"] + "-" + this.ds.Tables[0].Rows[0]["JobDescriptionNumber"];
        this.WiNo = fileName;
        fileName += " " + this.ds.Tables[0].Rows[0]["jobDescriptionName"];
        fileName = fileName.Replace(@"/", " ");
        return fileName;
    }
    protected  string GetImageNumber(ref int i, ref int j, string type, string prompt) {
        if (type == "Local" || prompt != "") {
            j++;
            char character = (char)(j + 64);
            return character.ToString();
        }
        return (++i).ToString();
    }
    private static string GetHeader2ListResources(int index, DataSet ds)
    {
        ArrayList list = new ArrayList();
        ArrayList list0 = new ArrayList();
        string s = "";
        int prevRowId = -1, curRowId = 0;
        foreach (DataRow dr in ds.Tables[index].Rows)
        {
            curRowId = int.Parse(dr["row"].ToString());
            if (curRowId != prevRowId)
            {
                if (prevRowId != -1)
                {
                    string[] arr0 = (string[])list.ToArray(Type.GetType("System.String"));
                    list0.Add(string.Join(" / ", arr0));
                    list.Clear();

                }
                prevRowId = curRowId;
            }
            s = "" + dr["name"];
            if (dr.Table.Columns.Contains("other") && dr["other"].ToString().Trim() != "undefined" && dr["other"].ToString().Trim() != "")
            {
                s = dr["other"] + "";
            }
            s += "(" + dr["number"] + ")";
            list.Add(s);
        }
        if (list.Count > 0)
            list0.Add(string.Join(" / ", (string[])list.ToArray(Type.GetType("System.String"))));

        string[] arr = (string[])list0.ToArray(Type.GetType("System.String"));
        return string.Join(", ", arr);
    }

    public static string GetHeader2List(string index, DataSet ds)
    {
        int i = int.Parse(index);
        if (i == 3)
            return GetHeader2ListResources(i, ds);
        ArrayList list = new ArrayList();
        string s = "";
        foreach (DataRow dr in ds.Tables[i].Rows)
        {
            s = "" + dr["name"];
            if (dr.Table.Columns.Contains("other") && dr["other"].ToString().Trim() != "undefined" && dr["other"].ToString().Trim() != "")
            {
                //s += "(" + dr["other"] + ")";
                s = dr["other"] + "";
            }
            list.Add(s);
        }
        string[] arr = (string[])list.ToArray(Type.GetType("System.String"));
        return string.Join(", ", arr);
    }

}