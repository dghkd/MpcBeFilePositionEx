using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Diagnostics;

namespace MpcBeLauncher.RegOperation
{
    public class RegMethod
    {
        #region Public Method

        /// <summary>
        /// 設定MPC-BE的檔案歷史紀錄
        /// </summary>
        /// <param name="filePath">影片檔路徑</param>
        /// <param name="position">影片檔撥放時間</param>
        /// <param name="audioTrack">影片檔聲道</param>
        /// <param name="subIndex">影片檔字幕</param>
        /// <param name="regIndex">要設定的歷史紀錄位址。預設值為1</param>
        /// <returns></returns>
        public static bool SetMpcBeRecentFile(string filePath, long position, int audioTrack, int subIndex, int regIndex = 1)
        {
            string formatValue = String.Format($"{filePath}|{position.ToString()}|{audioTrack.ToString()}|{subIndex.ToString()}");
            try
            {
                RegistryKey key = Registry.CurrentUser.CreateSubKey(RegString.REG_KEY_MPC_BE_RECENT_FILES);
                key.SetValue(RegString.REG_VALUE_NAME_MPC_BE_RECENT_FILE + regIndex.ToString(), formatValue, RegistryValueKind.String);
                key.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[SetMpcBeRecentFile] {e.Message}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 透過檔案路徑取得MPC-BE歷史檔案資料
        /// </summary>
        /// <param name="path">指定的檔案路徑</param>
        /// <returns>
        /// 回傳Tuple型別，其中Item1為歷史檔案資訊、Item2為取得歷史檔案的位置(index)。
        /// <para>若指定的路徑無法找到對應的歷史檔案資訊，則Item1為null，Item2為-1</para>
        /// </returns>
        public static Tuple<FilePosData, int> GetMpcBeRecentFileByPath(string path)
        {
            FilePosData recentData = new FilePosData();
            Tuple<FilePosData, int> ret = new Tuple<FilePosData, int>(null, -1);

            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(RegString.REG_KEY_MPC_BE_SETTINGS);
                if (key == null)
                {
                    return ret;
                }
                int recentNum = Convert.ToInt32(key.GetValue(RegString.REG_VALUE_NAME_MPC_BE_SETTINGS_RECENT_FILE_NUMBER, 0));

                RegistryKey keyRecent = Registry.CurrentUser.OpenSubKey(RegString.REG_KEY_MPC_BE_RECENT_FILES);
                for (int i = 1; i <= recentNum; i++)
                {
                    string fileValueName = RegString.REG_VALUE_NAME_MPC_BE_RECENT_FILE + i.ToString();
                    string rawValue = Convert.ToString(keyRecent.GetValue(fileValueName));
                    recentData = ParseRecentFile(rawValue);
                    if (recentData != null
                        && recentData.FullPath == path)
                    {
                        ret = new Tuple<FilePosData, int>(recentData, i);
                        break;
                    }
                }

                key.Close();
                keyRecent.Close();
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[GetMpcBeRecentFileByPath] {e.Message}");
            }

            return ret;
        }

        /// <summary>
        /// 取得MPC-BE登錄檔中所有歷史檔案資料
        /// </summary>
        /// <returns></returns>
        public static List<FilePosData> GetRecentFilePostDataList()
        {
            List<FilePosData> ret = new List<FilePosData>();

            RegistryKey key = Registry.CurrentUser.OpenSubKey(RegString.REG_KEY_MPC_BE_SETTINGS);
            if (key == null)
            {
                return ret;
            }
            int recentNum = Convert.ToInt32(key.GetValue(RegString.REG_VALUE_NAME_MPC_BE_SETTINGS_RECENT_FILE_NUMBER, 0));

            RegistryKey keyRecent = Registry.CurrentUser.OpenSubKey(RegString.REG_KEY_MPC_BE_RECENT_FILES);
            for (int i = 1; i <= recentNum; i++)
            {
                FilePosData recentData = new FilePosData();
                string fileValueName = RegString.REG_VALUE_NAME_MPC_BE_RECENT_FILE + i.ToString();
                string rawValue = Convert.ToString(keyRecent.GetValue(fileValueName));
                recentData = ParseRecentFile(rawValue);
                if (recentData != null)
                {
                    ret.Add(recentData);
                }
            }

            key.Close();
            keyRecent.Close();

            return ret;
        }

        #endregion Public Method

        #region Private Method

        private static FilePosData ParseRecentFile(string value)
        {
            FilePosData ret = new FilePosData();
            string[] datas = value.Split('|');
            if (datas.Length < 4)
            {
                return null;
            }

            ret.FullPath = datas[0];
            ret.Position = Convert.ToInt64(datas[1]);
            ret.AudioTrack = Convert.ToInt32(datas[2]);
            ret.Subtitle = Convert.ToInt32(datas[3]);

            return ret;
        }

        #endregion Private Method
    }
}