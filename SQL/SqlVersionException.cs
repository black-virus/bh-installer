using System;
using System.Data.SqlClient;

namespace blackhouse.Installer.SQL
{

    public class SqlVersionException : Exception
    {

        public SqlVersionException(SqlException sqlException, string onScript)
            : base(GetMessage(sqlException, onScript), sqlException) {
        }

        private static string GetMessage(SqlException sqlException, string onScript) {
            return "-- Severity level: " + sqlException.Class + "\r\n" +
                   "-- SQL error type number: " + sqlException.Number + "\r\n" +
                   "-- SQL error number: " + sqlException.State + "\r\n" +
                   "-- Line number: " + sqlException.LineNumber + "\r\n" +
                   "-- Kod SQL:\r\n " + onScript;
        }

    }

}
