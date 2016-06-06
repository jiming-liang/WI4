using System;
using System.Text;
using System.Web;
using System.ILog1;


public class GlobalAsax : HttpApplication
{
    private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

    void Application_Start(object sender, EventArgs e)
    {
        log4net.Config.XmlConfigurator.Configure();
    }

    void Application_Error(object sender, EventArgs e)
    {
        HttpContext context = ((HttpApplication)sender).Context;
        _log.Error("WIMS", (context.Server.GetLastError().InnerException == null) ? context.Server.GetLastError() : context.Server.GetLastError().InnerException);
    }

    void Session_Start(object sender, EventArgs e)
    {
    }

    void Session_End(object sender, EventArgs e)
    {
    }
}
