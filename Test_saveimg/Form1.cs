using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Linq;
//using System.Text;
using System.Net; // for WebClient
using System.IO; // for MemoryStream
using System.Text;// for Encoding
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;//for jsaon
using System.Drawing;// for image
// using ThunderAgentLib;
using System.Threading;
using System.Runtime.InteropServices;
using System.Reflection;



namespace Test_saveimg
{
    public partial class Form1 : Form
    {
        public Form1()
        {            
            InitializeComponent();
        }

        public void Draw(object parain)
        {
            this.Refresh();               
        }




        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog DigOpen = new OpenFileDialog();
            DigOpen.ShowDialog();            
            string Path = DigOpen.FileName;
            if (Path.Length == 0)
            {
                MessageBox.Show("Please select Txt file");
                return;
            }

       //     MessageBox.Show(Path);

            //downloading image from urls

            WebClient wc = new WebClient();// key member of downloading
            Thunder_SDK.XLInitDownloadEngine();        

            MemoryStream ms;
            StreamReader m_StreamReader = new StreamReader(Path);// open the txt file with C# form     

            while (true)
            {
                String temp_line = m_StreamReader.ReadLine();// read one line of txt file 用的挺爽
                if (temp_line == null)
                {
                    break;// break when reach the end of file;
                }
                String[] parts = temp_line.Split('\t');// split the readed line by '\t'
                if ((parts[0].IndexOf('#')) != -1)
                {
                    continue;// the first and second  row of image is useless so discard them

                }

                String ImgSavePath = "Y:\\GitHub\\Test_saveimg\\Test_saveimg\\Test_saveimg\\bin\\Release\\" + parts[0] + "\\";
                if (!Directory.Exists(ImgSavePath))
                {
                    Directory.CreateDirectory(ImgSavePath);// create the Directiory when necessary 
                }


                ImgSavePath += parts[0] + "_" + parts[1] + ".jpg";

                // replace the download by xunlei engine
                String filename = parts[0] + "_" + parts[1] + ".jpg";
       
             
                label1.Text = parts[0] + "_" + parts[1] + ".jpg";
            //    label1.Font = new System.Drawing.Font()
                this.Refresh();


                MyObj_para para = new MyObj_para();

                para.ImgSavePath = ImgSavePath;
                para.parts2 = parts[2];
                para.temp_line = temp_line;
                para.label = label2;
                para.progressbar = progressBar1;

                
          

                Int32 lTaskId = 0;
                Int32 dwRet = Thunder_SDK.XLURLDownloadToFile(ImgSavePath, parts[2], "", ref lTaskId);
                if (Thunder_SDK.XL_SUCCESS != dwRet)
                {
                    //ThunderSDK.Program.XLUninitDownloadEngine();
                    //      MessageBox.Show (string.Format("Create new task failed, error code:%d\n", dwRet));                     
                }
                float last = 0;
                do
                {
                    Thread.Sleep(500);
                    last += 0.5F;



                    Int64 ullFileSize = 0;
                    Int64 ullRecvSize = 0;
                    int lStatus = -1;

                    dwRet = Thunder_SDK.XLQueryTaskInfo(lTaskId, ref lStatus, ref ullFileSize, ref ullRecvSize);
                    if (Thunder_SDK.XL_SUCCESS == dwRet)
                    {
                        // 输出进度信息
                        if (0 != ullFileSize)
                        {
                            double douProgress = (double)ullRecvSize / (double)ullFileSize;
                            douProgress *= 100.0;

                            progressBar1.Step = (Int32)douProgress;
                            progressBar1.PerformStep();
                        }
                        else
                        {

                        }

                        if (last > 5 && 0 == ullFileSize)
                        {
                            lStatus = 12;
                        }

                        if (11 == lStatus)
                        {
                            //   MessageBox.Show(string.Format("Download successfully.\n"));
                            progressBar1.Value = 0;
                            // this.Refresh();
                            break;
                        }

                        if (12 == lStatus)
                        {
                            //  MessageBox.Show(string.Format("Download failed.\n"));
                            progressBar1.Value = 0;
                            //  this.Refresh();

                            StreamWriter m_StreamWriter = new StreamWriter(".\\dev_urls_failed.txt", true);
                            m_StreamWriter.Write(temp_line);
                            m_StreamWriter.Write("\n");
                            m_StreamWriter.Close();

                            break;
                        }
                        label2.Text = string.Format("Last {0} Sec!", last);

                    }

                 //   System.Delegate a = new System.Delegate()
                  //  Control.Invoke(500, null, null);
                 //   this.Refresh();
                    Thread thread = new Thread(new ParameterizedThreadStart(Draw));
                    thread.Start(para); 
                    thread.Join();

                } while (Thunder_SDK.XL_SUCCESS == dwRet);
                Thunder_SDK.XLStopTask(lTaskId);//搜索程序
     
        


       
             

          

    //                  while(true)
    //                  {
    //                       String status = agent.GetTaskInfo(parts[2],"Status");
    //                      if ((string.Compare(status,"failed"))==0||(string.Compare(status,"success"))==0)
    //                      {
    //                          break;
    //                      }
    //                  }

    //                  ImgSavePath += parts[0] + "_" + parts[1] + ".jpg";// path of saving image
    // 
    //                  try
    //                  {
    //                      wc.DownloadFile(parts[2], ImgSavePath); // using the DownloadFile function instead of DownloadData
    //                //      ms = new MemoryStream(wc.DownloadData(parts[2]));
    //                //      Image img = Image.FromStream(ms);
    //                //      img.Save(ImgSavePath); 
    //                  }
    //                  catch (System.Exception ex)
    //                  {
    //                   //    MessageBox.Show(ex.Message + parts[2]);// catch the exception when the image is invalid
    // 
    //                  }



            }

            Thunder_SDK.XLUninitDownloadEngine();





            //      MessageBox.Show("Debug Here!!");
            //       WebClient wc = new WebClient();
       //     string shitu_url = "http://stu.baidu.com/i?objurl=http://www.hinews.cn/pic/0/16/31/87/16318729_751468.jpg&rt=0&rn=10&ftn=searchstu&ct=1&stt=1&tn=infacejson";
            //    shitu_url += "pn=0";


            //    http://stu.baidu.com/i?ct=3&tn=facejson&pn=0&rn=10&querysign=1267449575,1337723570&shituRetNum=586&similarRetNum=600&faceRetNum=1000&setnum=0
       //     byte[] aryByte = wc.DownloadData(shitu_url);
       //     ms = new MemoryStream(aryByte);
       //     string html = string.Empty;
       //     using (StreamReader sr = new StreamReader(ms, Encoding.Default))
       //     {
       //         html = sr.ReadToEnd();
       //     }

            // baidujason jsonObj = JArray.Parse(html);

       //     baidujason ja = (baidujason)JsonConvert.DeserializeObject(html, typeof(baidujason));


            System.Diagnostics.Process.Start(@"http://blog.csdn.net/likely_zhao/article/details/38424959");
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

    }





     public class baidujasondata
    {
       //  public string is{get;set;}
         public int bdSetImgNum{get;set;}
         public string thumbURL { get;set;}
          public string flowURL { get;set;}
          public string middleURL { get;set;}
          public string largeTnImageUrl { get;set;}
          public string hoverURL { get;set;}
          public string faceURL { get;set;}
          public int hasLarge { get;set;}
          public int simi { get;set;}
         public string objURLKey { get;set;}
         public int pageNum { get;set;}
         public string objURL { get;set;}
         public string fromURL { get;set;}
         public string fromURLEnc { get;set;}
         public string fromURLHost { get;set;}

         public int width { get;set;}
         public int height { get;set;}
         public string objId { get;set;}
         public string objType { get;set;}
         public string time { get;set;}
         public string fileSize { get;set;}

         public string fromPageTitle { get;set;}
         public string fromPageTitleEnc { get;set;}
         public string textHost { get;set;}
    }

     public class baidujason
     {
         public string imgName { get; set; }
         public string imgQuerySign { get; set; }
         public string displayNum { get; set; }
         public string listNum { get; set; }
         public string rank { get; set; }
         public string beforeCt { get; set; }
         public string keyword { get; set; }
         public baidujasondata []data{ get; set; }

     }

     class Thunder_SDK
         {
             enum enumTaskStatus
             {
                 enumTaskStatus_Connect = 0,                 // 已经建立连接
                 enumTaskStatus_Download = 2,                // 开始下载 
                 enumTaskStatus_Pause = 10,                  // 暂停
                 enumTaskStatus_Success = 11,                // 成功下载
                 enumTaskStatus_Fail = 12,                   // 下载失败
             };
             public const int XL_SUCCESS = 0;
             public const int XL_ERROR_FAIL = 0x10000000;

             // 尚未进行初始化
             public const int XL_ERROR_UNINITAILIZE = XL_ERROR_FAIL + 1;

             // 不支持的协议，目前只支持HTTP
             public const int XL_ERROR_UNSPORTED_PROTOCOL = XL_ERROR_FAIL + 2;

             // 初始化托盘图标失败
             public const int XL_ERROR_INIT_TASK_TRAY_ICON_FAIL = XL_ERROR_FAIL + 3;

             // 添加托盘图标失败
             public const int XL_ERROR_ADD_TASK_TRAY_ICON_FAIL = XL_ERROR_FAIL + 4;

             // 指针为空
             public const int XL_ERROR_POINTER_IS_NULL = XL_ERROR_FAIL + 5;

             // 字符串是空串
             public const int XL_ERROR_STRING_IS_EMPTY = XL_ERROR_FAIL + 6;

             // 传入的路径没有包含文件名
             public const int XL_ERROR_PATH_DONT_INCLUDE_FILENAME = XL_ERROR_FAIL + 7;

             // 创建目录失败
             public const int XL_ERROR_CREATE_DIRECTORY_FAIL = XL_ERROR_FAIL + 8;

             // 内存不足
             public const int XL_ERROR_MEMORY_ISNT_ENOUGH = XL_ERROR_FAIL + 9;

             // 参数不合法
             public const int XL_ERROR_INVALID_ARG = XL_ERROR_FAIL + 10;

             // 任务不存在
             public const int XL_ERROR_TASK_DONT_EXIST = XL_ERROR_FAIL + 11;

             // 文件名不合法
             public const int XL_ERROR_FILE_NAME_INVALID = XL_ERROR_FAIL + 12;

             // 没有实现
             public const int XL_ERROR_NOTIMPL = XL_ERROR_FAIL + 13;

             // 已经创建的任务数达到最大任务数，无法继续创建任务
             public const int XL_ERROR_TASKNUM_EXCEED_MAXNUM = XL_ERROR_FAIL + 14;

             // 任务类型未知
             public const int XL_ERROR_INVALID_TASK_TYPE = XL_ERROR_FAIL + 15;

             // 文件已经存在
             public const int XL_ERROR_FILE_ALREADY_EXIST = XL_ERROR_FAIL + 16;

             // 文件不存在
             public const int XL_ERROR_FILE_DONT_EXIST = XL_ERROR_FAIL + 17;

             // 读取cfg文件失败
             public const int XL_ERROR_READ_CFG_FILE_FAIL = XL_ERROR_FAIL + 18;

             // 写入cfg文件失败
             public const int XL_ERROR_WRITE_CFG_FILE_FAIL = XL_ERROR_FAIL + 19;

             // 无法继续任务，可能是不支持断点续传，也有可能是任务已经失败
             // 通过查询任务状态，确定错误原因。
             public const int XL_ERROR_CANNOT_CONTINUE_TASK = XL_ERROR_FAIL + 20;

             // 无法暂停任务，可能是不支持断点续传，也有可能是任务已经失败
             // 通过查询任务状态，确定错误原因。
             public const int XL_ERROR_CANNOT_PAUSE_TASK = XL_ERROR_FAIL + 21;

             // 缓冲区太小
             public const int XL_ERROR_BUFFER_TOO_SMALL = XL_ERROR_FAIL + 22;

             // 调用XLInitDownloadEngine的线程，在调用XLUninitDownloadEngine之前已经结束。
             // 初始化下载引擎线程，在调用XLUninitDownloadEngine之前，必须保持执行状态。
             public const int XL_ERROR_INIT_THREAD_EXIT_TOO_EARLY = XL_ERROR_FAIL + 23;

             [DllImport("XLDownload.dll", EntryPoint = "XLInitDownloadEngine")]
             public static extern bool XLInitDownloadEngine();
             [DllImport("XLDownload.dll", EntryPoint = "XLURLDownloadToFile", CharSet = CharSet.Unicode)]
             public static extern Int32 XLURLDownloadToFile(string pszFileName, string pszUrl, string pszRefUrl, ref Int32 lTaskId);
             [DllImport("XLDownload.dll")]
             public static extern Int32 XLQueryTaskInfo(Int32 lTaskId, ref Int32 plStatus, ref Int64 pullFileSize, ref Int64 pullRecvSize);
             [DllImport("XLDownload.dll")]
             public static extern Int32 XLPauseTask(Int32 lTaskId, ref Int32 lNewTaskId);
             [DllImport("XLDownload.dll")]
             public static extern Int32 XLContinueTask(Int32 lTaskId);
             [DllImport("XLDownload.dll")]
             public static extern void XLStopTask(Int32 lTaskId);
             [DllImport("XLDownload.dll")]
             public static extern bool XLUninitDownloadEngine(); 
         }


    public class MyObj_para
    {
        public string ImgSavePath{get;set;}
        public string parts2 { get; set; }
        public string temp_line { get; set; }
        public Label label { get; set; }
        public ProgressBar progressbar { get; set;}
    }


   
}

