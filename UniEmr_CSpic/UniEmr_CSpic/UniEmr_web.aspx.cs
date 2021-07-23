using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace UniEmr_CSpic
{
    //超声报告图片
    public partial class UniEmr_web : Page
    {
        public int pic_count;
        public string strLocalIP = "";

        public List<string> l_txlj = new List<string>();
        public List<string> l_txmc = new List<string>();
        public DirectoryInfo theFolder;
        public bool status = false;
        string filename;
        string bgbs = "";
        string djbs = "";

        bool correct = true;

        #region  川沙服务器
        public void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    djbs = Request.Params["bgbs"].Replace("US_", "");
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('参数有误')</script>");
                    correct = false;
                }

                if (correct)
                {
                    pic_count = 0;
                    ViewState["pic_count"] = pic_count;

                    string conn = ConfigurationManager.ConnectionStrings["connectionString"].ConnectionString;
                    SqlConnection sqlconn = new SqlConnection(conn);
                    SqlCommand sqlcommand = sqlcommand = new SqlCommand();
                    sqlcommand.CommandType = System.Data.CommandType.Text;
                    sqlcommand.Connection = sqlconn;
                    try
                    {
                        sqlconn.Open();

                        sqlcommand.CommandText = string.Format("select bgbs from u_us..t_us_csbg where djbs='{0}'", djbs);
                        SqlDataReader dr = sqlcommand.ExecuteReader();
                        while (dr.Read())
                        {
                            bgbs = dr["bgbs"].ToString();
                        }
                        dr.Close();

                        sqlcommand.CommandText = string.Format("select txlj,txmc from u_us..t_us_csbgtx where bgbs='{0}'", bgbs);
                        dr = sqlcommand.ExecuteReader();
                        while (dr.Read())
                        {
                            l_txlj.Add(dr["txlj"].ToString());
                            l_txmc.Add(dr["txmc"].ToString());
                        }
                        dr.Close();
                    }
                    catch (Exception ex)
                    {
                        Response.Write("<script>alert('" + ex.ToString() + "')</script>");
                    }
                    finally
                    {
                        sqlcommand.Dispose();
                        sqlconn.Close();

                        if (l_txmc.Count != 0 && l_txlj.Count != 0)
                        {
                            ViewState["l_txlj"] = l_txlj;
                            ViewState["l_txmc"] = l_txmc;
                        }
                    }

                    Label1.Text = "共" + l_txmc.Count.ToString() + "张";

                    if (l_txlj.Count != 0 && l_txmc.Count != 0)
                    {
                        IPHostEntry ipHost = Dns.Resolve(Dns.GetHostName());
                        IPAddress ipAddress = ipHost.AddressList[0];
                        strLocalIP = ipAddress.ToString();
                        ViewState["strLocalIP"] = strLocalIP;

                        status = connectState(l_txlj[0], "tjuser", "tjuser", strLocalIP);
                        if (status)
                        {
                            for (int i = 0; i < l_txmc.Count; i++)
                            {
                                System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                                img.ID = "img_" + i.ToString();
                                div_image.Controls.Add(img);

                                HtmlGenericControl div = new HtmlGenericControl("div");
                                div.ID = "div_img_" + i.ToString();

                                div.InnerHtml = "<h3 style=\"color:white\">第 " + (i + 1).ToString() + " 张<h3/><hr/>";
                                div_image.Controls.Add(div);

                                theFolder = new DirectoryInfo(l_txlj[i]);
                                filename = theFolder.ToString() + @"\" + l_txmc[i];
                                Transport_All(filename, img);
                            }

                            //theFolder = new DirectoryInfo(l_txlj[0]);
                            //filename = theFolder.ToString() + @"\" + l_txmc[0];
                            //Transport(filename);
                        }
                        else
                        {
                            Response.Write("<script>alert('error')</script>");
                        }
                    }
                    else if (l_txlj.Count == 0 && l_txmc.Count == 0)
                    {
                        Response.Write("<script>alert('无图像')</script>");
                    }
                }
                
            }
        }
        #endregion

        #region 青浦服务器
        //public void Page_Load(object sender, EventArgs e)
        //{
        //    if (!IsPostBack)
        //    {
        //        pic_count = 0;
        //        ViewState["pic_count"] = pic_count;

        //        string djh = Request.Params["djh"];
        //        string djbs = "";
        //        string bgbs = "";

        //        string conn = "Server = 168.168.168.139; Database = u_us1; UID = sa; Pwd = qpzs; Connect Timeout = 8;";
        //        SqlConnection sqlconn = new SqlConnection(conn);
        //        SqlCommand sqlcommand = sqlcommand = new SqlCommand();
        //        sqlcommand.CommandType = System.Data.CommandType.Text;
        //        sqlcommand.Connection = sqlconn;
        //        try
        //        {
        //            sqlcommand.CommandText = string.Format("select djbs from u_us..t_us_csdj where djh='{0}'", djh);
        //            sqlconn.Open();
        //            SqlDataReader dr = sqlcommand.ExecuteReader();
        //            while (dr.Read())
        //            {
        //                djbs = dr["djbs"].ToString();
        //            }
        //            dr.Close();

        //            sqlcommand.CommandText = string.Format("select bgbs from u_us..t_us_csbg where djbs='{0}'", djbs);
        //            dr = sqlcommand.ExecuteReader();
        //            while (dr.Read())
        //            {
        //                bgbs = dr["bgbs"].ToString();
        //            }
        //            dr.Close();

        //            sqlcommand.CommandText = string.Format("select txlj,txmc from u_us..t_us_csbgtx where bgbs='{0}'", bgbs);
        //            dr = sqlcommand.ExecuteReader();
        //            while (dr.Read())
        //            {
        //                l_txlj.Add(dr["txlj"].ToString());
        //                l_txmc.Add(dr["txmc"].ToString());
        //            }
        //            dr.Close();
        //        }
        //        catch (Exception ex)
        //        {
        //            Response.Write("<script>alert('" + ex.ToString() + "')</script>");
        //        }
        //        finally
        //        {
        //            sqlcommand.Dispose();
        //            sqlconn.Close();

        //            if (l_txmc.Count != 0 && l_txlj.Count != 0)
        //            {
        //                ViewState["l_txlj"] = l_txlj;
        //                ViewState["l_txmc"] = l_txmc;
        //            }
        //        }

        //        Label1.Text = "共" + l_txmc.Count.ToString() + "张";

        //        if (l_txlj.Count != 0 && l_txmc.Count != 0)
        //        {
        //            IPHostEntry ipHost = Dns.Resolve(Dns.GetHostName());
        //            IPAddress ipAddress = ipHost.AddressList[0];
        //            strLocalIP = ipAddress.ToString();
        //            ViewState["strLocalIP"] = strLocalIP;

        //            status = connectState(l_txlj[0], "administrator", "wl3115", strLocalIP);
        //            if (status)
        //            {
        //                //共享文件夹的目录
        //                theFolder = new DirectoryInfo(l_txlj[0]);
        //                filename = theFolder.ToString() + @"\" + l_txmc[0];
        //                Transport(filename);
        //            }
        //            else
        //            {
        //                Response.Write("<script>alert('error')</script>");
        //            }
        //        }
        //        else if (l_txlj.Count == 0 && l_txmc.Count == 0)
        //        {
        //            Response.Write("<script>alert('无图像')</script>");
        //        }
        //    }
        //}
        #endregion

        /// <summary>
        /// 图片输出
        /// </summary>
        /// <param name="fileName"></param>
        public void Transport(string fileName)
        {
            FileStream Stream = null;

            try
            {
                Stream = new FileStream(fileName, FileMode.Open);
                MemoryStream ms = new MemoryStream();

                Bitmap map = new Bitmap(Stream);
                map.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                //Response.ClearContent();
                //Response.ContentType = "image/jpg";
                //Response.BinaryWrite(ms.ToArray());

                string base64 = Convert.ToBase64String(ms.ToArray());
                //Image1.ImageUrl = "data:image/jpg;base64," + base64;

                Stream.Flush();
            }
            catch(Exception ex)
            {
                Response.Write("<script>alert('" + ex.ToString() + "')</script>");
            }
            finally
            {
                if(Stream != null)
                {
                    Stream.Close();
                    Stream.Dispose();
                }
            }
        }

        /// <summary>
        /// 所有图片输出
        /// </summary>
        /// <param name="fileName"></param>
        public void Transport_All(string fileName, System.Web.UI.WebControls.Image img)
        {
            FileStream Stream = null;

            try
            {
                Stream = new FileStream(fileName, FileMode.Open);
                MemoryStream ms = new MemoryStream();

                Bitmap map = new Bitmap(Stream);
                map.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);

                //Response.ClearContent();
                //Response.ContentType = "image/jpg";
                //Response.BinaryWrite(ms.ToArray());

                string base64 = Convert.ToBase64String(ms.ToArray());
                img.ImageUrl = "data:image/jpg;base64," + base64;

                ms.Close();
                ms.Dispose();

                Stream.Flush();
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.ToString() + "')</script>");
            }
            finally
            {
                if (Stream != null)
                {
                    Stream.Close();
                    Stream.Dispose();
                }

                Disconnect();
            }
        }

        /// <summary>
        /// 连接共享文件夹
        /// </summary>
        /// <param name="path"></param>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <returns></returns>
        public bool connectState(string path, string userName, string passWord, string ipaddress)
        {
            bool Flag = false;
            Process proc = new Process();
            try
            {               
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                proc.StandardInput.WriteLine("net use * /del /y");
                //string dosLine = "net use " + path + " " + passWord + " /user:" + ipaddress + @"\" + userName;
                string dosLine = "net use " + path + " " + passWord + " /user:domain\\" + userName;
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();

                if (string.IsNullOrEmpty(errormsg))
                {
                    Flag = true;
                }
                else
                {
                    Response.Write("<script>alert('" + errormsg + "')</script>");
                }
            }
            catch (Exception ex)
            {
                Response.Write("<script>alert('" + ex.ToString() + "')</script>");
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }

            return Flag;
        }

        //断开共享文件夹
        public void Disconnect()
        {
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                proc.StandardInput.WriteLine("net use * /del /y");
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
        }

        /// <summary>
        /// 下一张按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //public void Button1_Click(object sender, EventArgs e)
        //{
        //    l_txlj.Clear();
        //    l_txlj = ViewState["l_txlj"] as List<string>;
        //    l_txmc.Clear();
        //    l_txmc = ViewState["l_txmc"] as List<string>;
        //    pic_count = (int)ViewState["pic_count"];
        //    strLocalIP = (string)ViewState["strLocalP"];

        //    if (pic_count + 1 == l_txlj.Count)
        //    {
        //        pic_count = 0;
        //    }
        //    else
        //    {
        //        pic_count++;
        //    }

        //    status = false;
        //    status = connectState(l_txlj[pic_count], "tjuser", "tjuser", strLocalIP);
        //    if (status)
        //    {
        //        theFolder = new DirectoryInfo(l_txlj[pic_count]);
        //        string filename = theFolder.ToString() + @"\" + l_txmc[pic_count];
        //        Transport(filename);

        //        Label2.Text = "第" + (pic_count + 1).ToString() + "张";
        //    }
        //    else
        //    {
        //        Response.Write("<script>alert('error')</script>");
        //    }

        //    ViewState["pic_count"] = pic_count;
        //}

        /// <summary>
        /// 上一张按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //public void Button2_Click(object sender, EventArgs e)
        //{
        //    l_txlj.Clear();
        //    l_txlj = ViewState["l_txlj"] as List<string>;
        //    l_txmc.Clear();
        //    l_txmc = ViewState["l_txmc"] as List<string>;
        //    pic_count = (int)ViewState["pic_count"];
        //    strLocalIP = (string)ViewState["strLocalP"];

        //    if (pic_count == 0)
        //    {
        //        pic_count = l_txlj.Count - 1;
        //    }
        //    else
        //    {
        //        pic_count--;
        //    }

        //    status = false;
        //    status = connectState(l_txlj[pic_count], "tjuser", "tjuser", strLocalIP);
        //    if (status)
        //    {
        //        theFolder = new DirectoryInfo(l_txlj[pic_count]);
        //        string filename = theFolder.ToString() + @"\" + l_txmc[pic_count];
        //        Transport(filename);

        //        Label2.Text = "第" + (pic_count + 1).ToString() + "张";
        //    }
        //    else
        //    {
        //        Response.Write("<script>alert('error')</script>");
        //    }

        //    ViewState["pic_count"] = pic_count;
        //}


        /// <summary>
        /// ftp方式访问
        /// </summary>
        /// <param name="path"></param>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public void Ftp_FileRead(string path, string user, string password)
        {
            FtpWebRequest reqFTP = null;
            FtpWebResponse response = null;
            Stream stream = null;
            MemoryStream ms = new MemoryStream();
            try
            {
                //创建FtpWebRequest对象
                reqFTP = (FtpWebRequest)WebRequest.Create(new Uri(path));
                reqFTP.Method = WebRequestMethods.Ftp.DownloadFile;

                // 指定数据传输类型  
                reqFTP.UseBinary = true;

                // ftp用户名和密码             
                reqFTP.Credentials = new NetworkCredential(user, password);
                response = (FtpWebResponse)reqFTP.GetResponse();
                // 把下载的文件写入流
                stream = response.GetResponseStream();

                Bitmap map = new Bitmap(stream);
                map.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                Response.ClearContent();
                Response.ContentType = "image/jpg";
                Response.BinaryWrite(ms.ToArray());
            }
            catch (Exception ex)
            {
                reqFTP.Abort();
                Response.Write("错误:" + ex.ToString());
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }

                if (response != null)
                {
                    response.Close();
                }
            }

        }
    }
}