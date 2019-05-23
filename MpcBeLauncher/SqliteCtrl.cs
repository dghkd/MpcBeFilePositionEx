using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
using Dapper;
using System.Data.SQLite;
using System.Data.SQLite.Generic;

namespace MpcBeLauncher
{
    public class SqliteCtrl
    {
        #region Private Member

        private const string _dbFileName = "FileHistory.sqlite";
        private readonly string _dbFilePath = "";
        private readonly string _sourceStr = "";
        private SQLiteConnection _dbConnectiong;

        #endregion Private Member

        #region Constructor

        public SqliteCtrl(string folderPath)
        {
            _dbFilePath = folderPath + "\\" + _dbFileName;
            _sourceStr = "data source=" + _dbFilePath;
        }

        #endregion Constructor

        #region Public Method

        public void Init()
        {
            if (!File.Exists(_dbFilePath))
            {
                using (var cn = new SQLiteConnection(_sourceStr))
                {
                    FilePosData tmp = new FilePosData();
                    /*
                     * @"CREATE TABLE FilePosData (
                     * FullPath TEXT,
                     * Position INTEGER,
                     * ...
                     * CONSTRAINT FilePosData_PK PRIMARY KEY (FullPath)
                     * )"
                     */
                    string cmdStr = @"CREATE TABLE " + nameof(FilePosData)
                        + "("
                        + nameof(tmp.FullPath) + " TEXT,"
                        + nameof(tmp.Position) + " INTEGER,"
                        + nameof(tmp.Name) + " TEXT,"
                        + nameof(tmp.FileSize) + " INTERGER,"
                        + nameof(tmp.AudioTrack) + " INTERGER,"
                        + nameof(tmp.Subtitle) + " INTERGER,"
                        + "CONSTRAINT FilePosData_PK PRIMARY KEY (FullPath)"
                        + ")";
                    cn.Execute(cmdStr);
                }
            }

            _dbConnectiong = new SQLiteConnection(_sourceStr);
        }

        public void InsertData(FilePosData data)
        {
            /*
             * INSERT OR REPLACE INTO FilePosData
             * (FullPath, Position, ...)
             * VALUES
             * ('c:\xxx\xx', 123456, ...)
             */
            string cmdInsert = "INSERT OR REPLACE INTO " + nameof(FilePosData)
                + " ("
                + nameof(data.FullPath) + ", "
                + nameof(data.Position) + ", "
                + nameof(data.Name) + ", "
                + nameof(data.FileSize) + ", "
                + nameof(data.AudioTrack) + ", "
                + nameof(data.Subtitle)
                + ")"
                + " VALUES "
                + "("
                + "\"" + data.FullPath + "\"" + ", "
                + data.Position + ", "
                + "\"" + data.Name + "\"" + ", "
                + data.FileSize + ", "
                + data.AudioTrack + ", "
                + data.Subtitle
                + ")";

            _dbConnectiong.Execute(cmdInsert);
        }

        public FilePosData GetDataByFullPath(string path)
        {
            FilePosData tmp = new FilePosData();
            /*
             * SELECT * FROM FilePosData WHERE FullPath LIKE "c:\xxx\"
             */
            string cmdGet = "SELECT * FROM " + nameof(FilePosData) + " WHERE " + (nameof(tmp.FullPath)) + " LIKE "
                + "\"" + path + "\"";
            try
            {
                tmp = _dbConnectiong.QueryFirst<FilePosData>(cmdGet);
            }
            catch (Exception e)
            {
                Debug.WriteLine($"[GetDataByFullPath] {e.Message}");
                return null;
            }

            return tmp;
        }

        public List<FilePosData> GetDataByName(string name)
        {
            List<FilePosData> ret;
            FilePosData tmp = new FilePosData();

            /*
             * SELECT * FROM FilePosData WHERE FullPath LIKE "c:\xxx\"
             */
            string cmdGet = "SELECT * FROM " + nameof(FilePosData) + " WHERE " + (nameof(tmp.Name)) + " LIKE "
                + "\"" + name + "\"";
            try
            {
                ret = _dbConnectiong.Query<FilePosData>(cmdGet).ToList();
            }
            catch (Exception e)
            {
                Debug.WriteLine(String.Format($"[GetDataByName] {e.Message}"));
                return null;
            }

            return ret;
        }

        public void RemoveDataByFullPath(string fullPath)
        {
            FilePosData tmp = new FilePosData();

            /*
             * DELETE FROM FilePosData WHERE FullPath LIKE "C:\xx\oo"
             */
            string cmdRm = "DELETE FROM " + nameof(FilePosData)
                + " WHERE " + nameof(tmp.FullPath)
                + " LIKE " + String.Format($"\"{fullPath}\"");

            _dbConnectiong.Execute(cmdRm);
        }

        #endregion Public Method
    }
}