using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using BencodeLibrary;
using System.Text.RegularExpressions;

namespace MoveFile
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            list = new ArrayList();
            tdDownloadList = new ArrayList();

            list.Add("D:\\TDDOWNLOAD20160214");
            list.Add("E:\\tdDownloadY");
            list.Add("E:\\TDDOWNLOADE");
            list.Add("E:\\ThunderDownloadRemote");
            list.Add("E:\\ThunderDownloadRemoteA");
            list.Add("E:\\tdDownloadA");
            list.Add("E:\\TDDOWNLOADADMIN");
            list.Add("E:\\迅雷下载");
            list.Add("E:\\tdDownloadz");
            list.Add("D:\\迅雷下载");
            list.Add("Z:\\tddownloada");
            list.Add("Z:\\tddownloadb");
            list.Add("Z:\\tddownloadc");
            list.Add("Z:\\迅雷下载");
            list.Add("E:\\tdDownloadAdmin");
            list.Add("E:\\dlA");
            list.Add("F:\\迅雷下载");
            //list.Add("E:\\utorrentDownload");
            //list.Add("E:\\utorrentRemote");
            tdDownloadList.AddRange(list);
            tdDownloadList.Add("e:\\abcd");
            tdDownloadList.Add("d:\\abcd");
            tdDownloadList.Add("f:\\abcd");
            tdDownloadList.Add("F:\\迅雷下载");
            tdDownloadList.Add("G:\\121\\20220120");
            list.Add("Z:\\tdDownloadd1");

        }

        ArrayList list;
        ArrayList moveList;
        ArrayList moveList1;
        ArrayList singleFileList;
        ArrayList tdDownloadList;

        void init()
        {
            moveList1 = new ArrayList();
            moveList = new ArrayList();
        
            singleFileList = new ArrayList();
        }

        private void Insert_Click(object sender, EventArgs e)
        {
            init();
            totalLength = 0;

            processUtorrent();
            process();
            moveFile();



        }

        void test()
        {
            DirectoryInfo TheFolder = new DirectoryInfo("D:\\新建文件夹 (2)\\新建文件夹");
            Directory.Move(TheFolder.FullName, "d:\\abcd");
            Console.WriteLine(TheFolder.Root.FullName);
        }
        double totalLength = 0;
        private void process()
        {

            foreach (string s in list)
            {
                process(s);
            }
            label1.Text = totalLength.ToString();
            dataGridView1.DataSource = moveList;
            dataGridView1.Refresh();
        }

        private void processUtorrent()
        {
            DirectoryInfo TheFolder = new DirectoryInfo("D:\\VuzeMove");
            if (!TheFolder.Exists)
                return;
            bool hasIncomplete = false;
            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                hasIncomplete = false;
                FileInfo[] fileInfos = NextFolder.GetFiles("*", SearchOption.AllDirectories);
                foreach (FileInfo f in fileInfos)
                {
                    if(f.Extension == ".bt!" || f.Extension == ".!ut" || f.Extension == ".bc!" || f.Extension == ".az!" || f.Extension == ".td" || f.Extension == ".tdl"||f.Extension== ".xltd")
                    {
                        hasIncomplete = true;
                        break;
                    }
                }
                if(!hasIncomplete)
                {
                    moveList.Add(new MyFileInfo(null, NextFolder.FullName));
                }
            }
        }

        private void process(string folderPath)
        {


            DirectoryInfo TheFolder = new DirectoryInfo(folderPath);
            if (TheFolder.Exists)
            {
                foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories("*", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        int torrentNumber = 0;
                        bool hasIncomplete = false;
                        bool hasBigFile = false;
                        FileInfo[] fileInfos = NextFolder.GetFiles("*", SearchOption.AllDirectories);
                        double folderLength = 0;
                        foreach (FileInfo f in fileInfos)
                        {

                            if (f.Length / 1024 / 1024 > 60 && (f.Extension == ".bt!" || f.Extension == ".!ut" || f.Extension == ".bc!" || f.Extension == ".az!" || f.Extension == ".td" || f.Extension == ".tdl"|| f.Extension == ".xltd"))
                            {
                                hasIncomplete = true;
                                continue;
                            }
                            else if (f.Length / 1024 / 1024 > 35)
                            {
                                hasBigFile = true;
                            }
                            if (f.Extension.ToLower() == ".torrent")
                            {
                                torrentNumber++;
                            }
                            folderLength += f.Length / 1024 / 1024;
                        }
                        if (!hasIncomplete && hasBigFile)
                        {
                            if (torrentNumber > 1)
                            {
                                moveList1.Add(new MyFileInfo(null, NextFolder.FullName));
                            }
                            else
                                moveList.Add(new MyFileInfo(null, NextFolder.FullName));
                            totalLength += folderLength;

                        }

                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }

                }
                foreach (FileInfo f in TheFolder.GetFiles("*", SearchOption.TopDirectoryOnly))
                {
                    if (f.Length / 1024 / 1024 > 60 && (f.Extension != ".bt!" && f.Extension != ".!ut" && f.Extension != ".bc!" && f.Extension != ".az!" && f.Extension != ".td" && f.Extension != ".tdl"&& f.Extension != ".xltd"))
                    {

                        singleFileList.Add(f.FullName);
                    }
                }
            }

             
         

        }

        private void moveFile()
        {
            foreach (string path in singleFileList)
            {
                string newPath = path[0] + ":\\abcd\\finish\\" + Path.GetFileName(path);
                try
                {
                    File.Move(path, newPath);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("已存在"))
                    {
                        Random rand = new Random();//生成随机数  
                        string prefix = "";

                        prefix += rand.Next();
                        string fileName = Path.GetFileName(newPath);
                        fileName = rand.Next() + "_" + fileName;
                        newPath = path[0] + ":\\abcd\\finish\\" + fileName;
                        File.Move(path, newPath);
                    }
                    else
                        MessageBox.Show("old:" + path + "       new:" + newPath + "     " + e.Message);
                }
            }
            foreach (MyFileInfo myfileInfo in moveList)
            {
                string newDir = "";
                bool hasTorrent = false;
                string torrentName = "";
                string folder = myfileInfo.Path;
                DirectoryInfo TheFolder = new DirectoryInfo(folder);
                foreach (FileInfo file in TheFolder.GetFiles())
                {
                    if (file.Extension == ".torrent")
                    {
                        hasTorrent = true;
                        torrentName = getName(file.FullName);
                    }
                }
                if (hasTorrent)
                    newDir = folder[0] + ":\\abcd\\finish\\" + torrentName;
                else
                    newDir = folder[0] + ":\\abcd\\finish\\" + new DirectoryInfo(folder).Name;
                try
                {

                    Directory.Move(folder, newDir);
                }
                catch (Exception e)
                {
                    if (e.Message.Contains("已存在"))
                    {
                        string dirName = new DirectoryInfo(newDir).Name;
                        Random rand = new Random();//生成随机数  
                        string prefix = "";

                        prefix += rand.Next();

                        dirName = prefix + "_" + dirName;

                        dirName = Path.Combine(folder[0] + ":\\abcd\\finish\\", dirName);
                        Directory.Move(folder, dirName);
                    } else
                        MessageBox.Show("old:" + folder + "       new:" + newDir + "     " + e.Message);
                }

            }
            foreach (MyFileInfo myfileInfo in moveList1)
            {
                string folder = myfileInfo.Path;
                string newDir = folder[0] + ":\\abcd\\muti-torrents\\" + new DirectoryInfo(folder).Name;
                try
                {

                    Directory.Move(folder, newDir);
                }
                catch (Exception e)
                {

                    MessageBox.Show("old:" + folder + "       new:" + newDir + "     " + e.Message);
                }

            }
        }

        private string getName(string torrentPath)
        {
            BDict torrentFile = BencodingUtils.DecodeFile(torrentPath) as BDict;

            return ((BString)((torrentFile["info"] as BDict)["name"])).Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            init();
            DirectoryInfo TheFolder = new DirectoryInfo("d:\\utorrentFinish");
            bool isPicFolder;

            foreach (DirectoryInfo NextFolder in TheFolder.GetDirectories("*", SearchOption.TopDirectoryOnly))
            {
                try
                {
                    isPicFolder = true;
                    FileInfo[] fileInfos = NextFolder.GetFiles("*", SearchOption.AllDirectories);
                    foreach (FileInfo fileInfo in fileInfos)
                    {
                        if (fileInfo.Length / 1024 / 2024 > 25)
                        {
                            isPicFolder = false;
                            break;
                        }

                    }
                    if (isPicFolder)
                    {
                        moveList.Add(NextFolder);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }


            }
            foreach (DirectoryInfo info in moveList)
            {
                Directory.Move(info.FullName, Path.Combine("d:\\abcd\\pic", info.ToString()).ToString());
            }


            dataGridView1.DataSource = moveList;
            dataGridView1.Refresh();
        }

        Dictionary<string, ArrayList> dic = new Dictionary<string, ArrayList>();
        Regex idRegex1 = new Regex("[A-Z]{1,}-[0-9]{1,}|[A-Z]{1,}[0-9]{1,}|[A-Z]{1,}‐[0-9]{1,}");
        ArrayList resultList = new ArrayList();
        ArrayList arrayList = new ArrayList() { "-1.", "-2.", "-3.", "-4.", "-5.", "-6.", "-7.", "-8.", "A.", "C.", "D.", "E.", "F.", "G.", "H.", "I.", "-A-", "-B-", "-C-", "-D-", "-E-", "-F-", "-G-", "-H-","_1.", "_2.", "_3.", "_4.", "_5.", "_6.", "_7.", "_8.", "_9." };
        private void button3_Click(object sender, EventArgs e)
        {
            foreach (string path in tdDownloadList)
            {
                DirectoryInfo TheFolder = new DirectoryInfo(path);
                if(TheFolder.Exists)
          
                    foreach (FileInfo fileInfo in TheFolder.GetFiles("*", SearchOption.AllDirectories))
                    {
     
                        

                            if (fileInfo.Length / 1024 / 1024 > 60)
                        {
                            string fileName = Path.GetFileName(fileInfo.Name.ToUpper());
                            bool ifContinue = false;
                            foreach (string term in arrayList)
                            {
                                if (fileName.Contains(term))
                                {
                                    Console.WriteLine(fileName);
                                    ifContinue = true;
                                    break;
                                }
                            }
                            if (ifContinue) continue;
                            string vid = idRegex1.Match(Path.GetFileNameWithoutExtension(fileInfo.Name.ToUpper().Replace("U15XX.",""))).Value.Replace("-", "").Replace("_", "");
                        string letter = "";
                        string number = "";
                        bool isEndofLetter = false;
                        for (int i = 0; i < vid.Length; i++)      
                                //修改   对于出现KIDM235A  KIDM235B
                            if (reg1.IsMatch(vid[i].ToString()))
                            {
                                if (isEndofLetter)
                                    break;
                                else
                                    letter += vid[i];
                            }
                            else
                            {
                                number += vid[i];
                                isEndofLetter = true;
                            }
                        if(number.StartsWith("00")&&number.Length>=5)
                            {
                                number = number.Substring(2);
                                vid = letter + number;
                            }
                        if (letter.ToUpper().EndsWith("VR") || letter.ToUpper().EndsWith("VS"))
                            continue;
                        if (dic.ContainsKey(vid))
                            {
                                dic[vid].Add(fileInfo);
                            }
                            else
                            {
                                dic.Add(vid, new ArrayList { fileInfo });
                            }
                        }
                    }
      
            }
            foreach (var item in dic)

            {
                if ( item.Value.Count > 1)
                {
                    string path="";
                    bool isValid = false;
                    foreach (FileInfo fileInfo in item.Value)
                    {



                        isValid = false;
                        if (!fileInfo.Extension.EndsWith("td"))
                        {
                            isValid = true;
                        }
                        else
                        {
                            path = fileInfo.FullName;
                        }

                     
                        
                    }
                    if (isValid)
                    {
                        MyFileInfo myFileInfo = new MyFileInfo(item.Key, path);
                        resultList.Add(myFileInfo);
                    }

                }

            }
            resultList.Sort(new myReverserClass());
            dataGridView2.DataSource = resultList;
            dataGridView2.Refresh();
        }
        Regex reg1 = new Regex("[A-Z]");

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            string id = dataGridView2.CurrentCell.Value.ToString();
            string letter = "";
            string number = "";
            bool isEndofLetter = false;
            for (int i = 0; i < id.Length; i++)                        //修改   对于出现KIDM235A  KIDM235B
                if (reg1.IsMatch(id[i].ToString()))
                {
                    if (isEndofLetter)
                        break;
                    else
                        letter += id[i];
                }
                else
                {
                    number += id[i];
                    isEndofLetter = true;
                }
            string str = "C:\\Progra~1\\Everything\\Everything.exe -search \"" + letter + " " + number + "\"";
            System.Diagnostics.Process p = new System.Diagnostics.Process();
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.UseShellExecute = false;    //是否使用操作系统shell启动
            p.StartInfo.RedirectStandardInput = true;//接受来自调用程序的输入信息
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息
            p.StartInfo.RedirectStandardError = true;//重定向标准错误输出
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口
            p.Start();//启动程序
            p.StandardInput.WriteLine(str + "&exit");

            p.StandardInput.AutoFlush = true;

            Clipboard.SetText(letter + " " + number);
        }
    }


    public class MyFileInfo
    {

        public MyFileInfo(string fileName, string path)
        {
            this.FileName = fileName;
            this.Path = path;
        }
        string path;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public string FileName { get => fileName; set => fileName = value; }

        string fileName;

    }

    public class myReverserClass : IComparer
    {

        // Calls CaseInsensitiveComparer.Compare with the parameters reversed.
        int IComparer.Compare(Object x, Object y)
        {
            MyFileInfo myFileInfoX = (MyFileInfo)x;
            MyFileInfo myFileInfoY = (MyFileInfo)y;
            return ((new CaseInsensitiveComparer()).Compare(myFileInfoY.Path, myFileInfoX.Path));
        }

    }

}
