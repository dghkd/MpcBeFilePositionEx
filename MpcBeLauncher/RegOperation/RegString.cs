using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MpcBeLauncher.RegOperation
{
    public class RegString
    {
        /// <summary>
        /// REG_KEY_MPC_BE_SETTINGS = @"Software\MPC-BE\Settings"
        /// </summary>
        public const string REG_KEY_MPC_BE_SETTINGS = @"Software\MPC-BE\Settings";

        /// <summary>
        /// REG_KEY_MPC_BE_RECENT_FILES= @"Software\MPC-BE\RecentFiles
        /// </summary>
        public const string REG_KEY_MPC_BE_RECENT_FILES = @"Software\MPC-BE\RecentFiles";

        /// <summary>
        /// REG_VALUE_NAME_MPC_BE_RECENT_FILE = "File"
        /// <para>Note:仍需要添加指定index值</para>
        /// </summary>
        public const string REG_VALUE_NAME_MPC_BE_RECENT_FILE = "File";

        /// <summary>
        /// REG_VALUE_NAME_MPC_BE_SETTINGS_FILE_NAME = "File Name "
        /// <para>Note:仍需要添加指定index值</para>
        /// </summary>
        public const string REG_VALUE_NAME_MPC_BE_SETTINGS_FILE_NAME = "File Name ";

        /// <summary>
        /// REG_VALUE_NAME_MPC_BE_SETTINGS_FILE_POSITION = "File Position "
        /// <para>Note:仍需要添加指定index值</para>
        /// </summary>
        public const string REG_VALUE_NAME_MPC_BE_SETTINGS_FILE_POSITION = "File Position ";

        /// <summary>
        /// REG_VALUE_NAME_MPC_BE_SETTINGS_RECENT_FILE_NUMBER = "RecentFilesNumber"
        /// </summary>
        public const string REG_VALUE_NAME_MPC_BE_SETTINGS_RECENT_FILE_NUMBER = "RecentFilesNumber";
    }
}