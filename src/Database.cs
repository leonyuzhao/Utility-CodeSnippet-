using System;
using System.Data;
using System.Data.OleDb;

/// <summary>
/// Wrapped Quick Database Utilities
/// <summary>
namespace Utility.Database
{   
    interface IDatabase
    {
        void SetConnection(string connectStr);

        void EnsureTable(string name, string query, bool forceCreate);

        bool HasTable(string name);

        DataTable GetData(string query);

        void ExecuteQuery(string query);
    }

    public class AccessDB : IDatabase
    {
        private OleDbConnection i_connection;

        public AccessDB(string connectStr)
        {
            SetConnection(connectStr);
        }

        public void SetConnection(string connectStr)
        {
            i_connection = new OleDbConnection(connectStr);
        }

        public void EnsureTable(string name, string query, bool forceCreate)
        {
            bool hasTable = HasTable(name);
            if (forceCreate && hasTable)
            {
                ExecuteQuery(string.Format(" DROP TABLE [{0}] ", name));
            }
            if (!hasTable) { ExecuteQuery(query); }
        }

        public bool HasTable(string name)
        {
            bool hasValue = false;
            string[] restrictions = new string[3];
            restrictions[2] = name;
            try
            {
                i_connection.Open();
                hasValue = (i_connection.GetSchema("Tables", restrictions).Rows.Count > 0);
            }
            catch (System.Data.DataException ex)
            {
                hasValue = false;
                throw ex;
            }
            finally
            {
                if (i_connection.State != ConnectionState.Closed) 
                {
                    i_connection.Close(); 
                } 
            }
            return hasValue;
        }

        public DataTable GetData(string query)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = query;
            cmd.Connection = i_connection;
            OleDbDataAdapter adapter = new OleDbDataAdapter(cmd);
            DataTable resultTable = new DataTable();
            try
            {
                i_connection.Open();
                adapter.Fill(resultTable);
            }
            catch (System.Data.DataException ex)
            {
                throw ex; 
            }
            finally
            {
                if (i_connection.State != ConnectionState.Closed) 
                {
                    i_connection.Close(); 
                } 
            }
            return resultTable;
        }

        public void ExecuteQuery(string query)
        {
            OleDbCommand cmd = new OleDbCommand();
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.CommandText = query;
            cmd.Connection = i_connection;
            try
            {
                i_connection.Open();
                cmd.ExecuteNonQuery();
            }
            catch (System.Data.DataException ex)
            {
                throw ex;
            }
            finally
            {
                if (i_connection.State != ConnectionState.Closed)
                {
                    i_connection.Close();
                }
            }
        }

        public OleDbConnection Connection
        {
            get { return i_connection; }
        }
    }
}