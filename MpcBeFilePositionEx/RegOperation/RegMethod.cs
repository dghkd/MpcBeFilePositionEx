using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Diagnostics;
using System.Reflection;

using MpcBeFilePositionEx.Common;

namespace MpcBeFilePositionEx.RegOperation
{
    public class RegMethod
    {
        /// <summary>
        /// 取得指定的副檔名關聯的MPC-BE執行檔路徑
        /// </summary>
        /// <param name="extName">指定的副檔名 EX:".mp4"</param>
        public static string GetMpcBeExePath(string extName)
        {
            string ret = "";
            RegistryKey key = RegMethod.OpenShellOpenCmdKey(extName);
            if (key != null)
            {
                string commandValue = Convert.ToString(key.GetValue(""));
                List<string> cmds = commandValue.Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                foreach (string s in cmds)
                {
                    if (s.IndexOf(RegString.MPC_BE_EXE) >= 0
                        || s.IndexOf(RegString.MPC_BE_EXE_64) >= 0)
                    {
                        ret = s;
                    }
                }
            }
            key.Close();
            return ret;
        }

        /// <summary>
        /// 附加執行到現有的MPC-BE執行指令上
        /// </summary>
        /// <param name="extName">指定的副檔名 EX:".mp4"</param>
        public static bool AttachMpcBeExt(string extName)
        {
            if (RegMethod.CheckAttached(extName))
            {
                return false;
            }

            if (RegMethod.GetMpcBeExePath(extName) == "")
            {
                return false;
            }

            RegMethod.CreateOldValue(extName);

            RegistryKey key = RegMethod.OpenShellOpenCmdKey(extName, true);
            if (key != null)
            {
                string oldValue = Convert.ToString(key.GetValue(""));
                string launcherPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + AppEnum.MPC_BE_LAUNCHER_NAME;
                string newValue = String.Format("\"{0}\" {1}", launcherPath, oldValue);
                key.SetValue("", newValue, RegistryValueKind.String);
                key.Close();
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 移除附加的執行指令
        /// </summary>
        /// <param name="extName">指定的副檔名 EX:".mp4"</param>
        public static bool DetachMpcBeExt(string extName)
        {
            if (RegMethod.GetMpcBeExePath(extName) == "")
            {
                return false;
            }

            if (!RegMethod.CheckAttached(extName))
            {
                return true;
            }

            string oldValue = RegMethod.GetOldValue(extName);
            if (oldValue != "")
            {
                RegistryKey key = RegMethod.OpenShellOpenCmdKey(extName, true);
                key.SetValue("", oldValue, RegistryValueKind.String);
                key.Close();
                RegMethod.DeleteOldValue(extName);
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 判斷是否已經有附加的啟動指定
        /// </summary>
        /// <param name="extName">指定的副檔名 EX:".mp4"</param>
        public static bool CheckAttached(string extName)
        {
            bool ret = false;
            RegistryKey key = RegMethod.OpenShellOpenCmdKey(extName, true);
            if (key != null)
            {
                string launcherPath = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\" + AppEnum.MPC_BE_LAUNCHER_NAME;
                //Check attached.
                string commandValue = Convert.ToString(key.GetValue(""));
                List<string> cmds = commandValue.Split(new string[] { "\"" }, StringSplitOptions.RemoveEmptyEntries).ToList();
                if (cmds.Count > 3)
                {
                    if (commandValue.IndexOf(launcherPath) >= 0)
                    {
                        //Already attached.
                        ret = true;
                    }
                }
            }
            key.Close();
            return ret;
        }

        /// <summary>
        /// 複製現有的shell open command值
        /// </summary>
        /// <param name="extName">指定的副檔名 EX:".mp4"</param>
        public static bool CreateOldValue(string extName)
        {
            bool ret = false;
            RegistryKey key = RegMethod.OpenShellOpenCmdKey(extName, true);
            if (key != null)
            {
                string commandValue = Convert.ToString(key.GetValue(""));
                key.SetValue(RegString.REG_VALUE_NAME_OLD_VALUE, commandValue, RegistryValueKind.String);
                ret = true;
            }
            key.Close();
            return ret;
        }

        /// <summary>
        /// 取得附加指令前的原始指令
        /// </summary>
        /// <param name="extName">指定的副檔名 EX:".mp4"</param>
        /// <returns></returns>
        public static string GetOldValue(string extName)
        {
            string ret = "";
            RegistryKey key = RegMethod.OpenShellOpenCmdKey(extName);
            if (key != null)
            {
                string commandValue = Convert.ToString(key.GetValue(RegString.REG_VALUE_NAME_OLD_VALUE));
                ret = commandValue;
            }
            key.Close();
            return ret;
        }

        /// <summary>
        /// 刪除已建立的舊shell open command值
        /// </summary>
        /// <param name="extName">指定的副檔名 EX:".mp4"</param>
        public static bool DeleteOldValue(string extName)
        {
            bool ret = false;
            RegistryKey key = RegMethod.OpenShellOpenCmdKey(extName, true);
            if (key != null)
            {
                key.DeleteValue(RegString.REG_VALUE_NAME_OLD_VALUE, false);
                ret = true;
            }
            return ret;
        }

        #region Private Method

        /// <summary>
        /// 開啟啟動命令的registry key
        /// </summary>
        /// <param name="extName">指定的副檔名 EX:".mp4"</param>
        /// <param name="writable">是否需要寫入權限</param>
        /// <returns></returns>
        private static RegistryKey OpenShellOpenCmdKey(string extName, bool writable = false)
        {
            string subKey = RegString.MPC_BE + extName + "\\" + RegString.REG_SUBKEY_SHELL_OPEN_COMMAND;
            RegistryKey key = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64).OpenSubKey(subKey, writable);
            if (key == null)
            {
                subKey = RegString.MPC_BE_64 + extName + "\\" + RegString.REG_SUBKEY_SHELL_OPEN_COMMAND;
                key = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64).OpenSubKey(subKey, writable);
            }
            return key;
        }

        #endregion Private Method
    }
}