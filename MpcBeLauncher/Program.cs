using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.CodeDom;

using MpcBeLauncher.RegOperation;
using RegistryUtils;

namespace MpcBeLauncher
{
    internal class Program
    {
        private static Process _mpcbeProcess = new Process();
        private static SqliteCtrl _sqlCtrl;
        private static RegWatcher _regWatcher;

        private static void Main(string[] args)
        {
            Console.WriteLine(args[0]);
            Console.WriteLine(args[1]);

            Console.OutputEncoding = System.Text.Encoding.Unicode;

            if (args.Length == 0)
            {
                //無參數 nothing to do.
                return;
            }
            else if (args.Length == 1)
            {
                //只有一個參數，直接執行該參數
                Process.Start(args[0]);
                return;
            }
            else if (args.Length == 2)
            {
                //取出各參數
                String mpcbeExePath = args[0];
                String filePath = args[1];
                String fileName = Path.GetFileName(filePath);

                if (mpcbeExePath != null
                    && mpcbeExePath != ""
                    && filePath != null
                    && filePath != "")
                {
                    //初始化資料庫
                    string launcherPath = Assembly.GetEntryAssembly().Location;
                    _sqlCtrl = new SqliteCtrl(Path.GetDirectoryName(launcherPath));
                    _sqlCtrl.Init();

                    _regWatcher = new RegWatcher();
                    _regWatcher.RegRecentFileChanged += On_MpcBe_RegChanged;

                    //讀取資料庫是否存在該檔案紀錄
                    FilePosData curData = _sqlCtrl.GetDataByFullPath(filePath);
                    if (curData != null)
                    {
                        //資料庫存在該檔案紀錄
                        //插入MPC-BE登錄檔歷史資料
                        RegMethod.SetMpcBeRecentFile(curData.FullPath, curData.Position, curData.AudioTrack, curData.Subtitle);
                    }
                    else
                    {
                        //讀取資料庫是否存在同檔名紀錄
                        List<FilePosData> nameList = _sqlCtrl.GetDataByName(fileName);

                        if (nameList != null
                            && nameList.Count > 0)
                        {
                            //比對檔案大小
                            long fileSize = (new FileInfo(filePath).Length);
                            foreach (FilePosData data in nameList)
                            {
                                if (data.FileSize == fileSize)
                                {
                                    //替換資料庫該檔案紀錄
                                    _sqlCtrl.RemoveDataByFullPath(data.FullPath);
                                    data.FullPath = filePath;
                                    _sqlCtrl.InsertData(data);

                                    //相同檔名、相同檔案大小
                                    //插入MPC-BE登陸檔歷史資料
                                    RegMethod.SetMpcBeRecentFile(data.FullPath, data.Position, data.AudioTrack, data.Subtitle);

                                    break;
                                }
                            }
                        }
                        else
                        {
                            //無歷史資料 直接啟動MPC-BE
                            //Nothing to do.
                        }
                    }
                }

                //啟動MPC-BE
                _regWatcher.Start();
                _mpcbeProcess = StartMpcBe(mpcbeExePath, filePath);

                //等待MPC-BE關閉
                _mpcbeProcess.WaitForExit();

                _regWatcher.Stop();
            }
        }

        #region Private Method

        private static void On_MpcBe_RegChanged(RegWatcher sender, List<FilePosData> changedData)
        {
            foreach (FilePosData saveData in changedData)
            {
                FileInfo fileInfo = new FileInfo(saveData.FullPath);
                saveData.FileSize = fileInfo.Length;
                saveData.Name = fileInfo.Name;

                //寫入資料庫
                _sqlCtrl.InsertData(saveData);
            }
        }

        /// <summary>
        /// 執行MPC-BE
        /// </summary>
        /// <param name="mpcbeExePath">MPC-BE執行檔路徑</param>
        /// <param name="filePath">指定要撥放的檔案路徑</param>
        /// <returns>MPC-BE實體Process</returns>
        private static Process StartMpcBe(string mpcbeExePath, string filePath)
        {
            filePath = FormatFilePath(filePath);

            return Process.Start(mpcbeExePath, filePath);
        }

        /// <summary>
        /// 格式化路徑參數，將指定的路徑字串加上雙引號
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static String FormatFilePath(String filePath)
        {
            if (filePath.Length > 0
                && filePath.ElementAt(0) != '\"')
            {
                return String.Format("\"{0}\"", filePath);
            }
            else
            {
                return filePath;
            }
        }

        /// <summary>
        /// MPC-BE關閉後，讀取Registry資訊並儲存置資料庫
        /// </summary>
        private static void SaveMpcBeFileHistoryToDB(string savePath)
        {
            //讀取MPC-BE登錄檔Recent File [x]資料
            Tuple<FilePosData, int> tuple = RegMethod.GetMpcBeRecentFileByPath(savePath);
            if (tuple.Item2 == -1)
            {
                return;
            }

            FilePosData saveData = tuple.Item1;
            FileInfo fileInfo = new FileInfo(savePath);
            saveData.FileSize = fileInfo.Length;
            saveData.Name = fileInfo.Name;

            //寫入資料庫
            _sqlCtrl.InsertData(saveData);
        }

        #endregion Private Method
    }
}