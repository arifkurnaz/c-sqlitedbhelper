using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Data.SQLite;
using System.IO;
using System.Windows.Forms;

namespace DataControllers.DataBases
{
    public class SQLite
    {

        public SQLiteConnection Connection;
        public SQLiteConnectionStringBuilder SQLiteConnectionStringBuilder;
        public SQLiteDataAdapter DataAdapter;
        public SQLiteDataReader DataReader;
        public SQLiteException Exception;
        public SQLiteCommand Command;
        public DataTable DataTable;
        public int InsertID;
        public bool CheckConnection()
        {
            if (File.Exists(@"SQLite\ihale.s3db"))
            {
                return true;

            }
            else
            {
                return false;
            }
        }

        public SQLiteDataAdapter CreateAdapter()
        {
            return this.DataAdapter = new SQLiteDataAdapter();
        }



        public SQLiteConnection CreateConnection()
        {
            string baglanti = "Data Source =" + Environment.CurrentDirectory + "/ihale.s3db";
            this.Connection = new SQLiteConnection(baglanti);

            return this.Connection;
        }

        public SQLiteConnectionStringBuilder CreateConnectionStringBuilder()
        {
            this.SQLiteConnectionStringBuilder = new SQLiteConnectionStringBuilder();
            this.SQLiteConnectionStringBuilder.DataSource = "Data Source=Db/ihale.s3db";
            return this.SQLiteConnectionStringBuilder;

        }
        public void Insert(string table, List<string> keys, List<string> values)
        {
            List<string> parameterKeys = new List<string>();

            foreach (string key in keys)
            {
                parameterKeys.Add("@" + key);

            }
            string sql = "insert into " + table + " (" + String.Join(",", keys.ToArray()) + ")values(" + String.Join(",", parameterKeys.ToArray()) + ")";

            using (this.CreateConnection())
            {
                this.Connection.Open();

                this.Command = new SQLiteCommand(sql, this.Connection);
                int i = 0;
                foreach (string value in values)
                {
                    this.Command.Parameters.AddWithValue(parameterKeys.ElementAt(i), values.ElementAt(i));
                    i++;
                }
                this.Command.ExecuteNonQuery();
                this.Command.Dispose();
                
                this.Command = new SQLiteCommand("SELECT last_insert_rowid()", this.Connection);
                this.InsertID = int.Parse(this.Command.ExecuteScalar().ToString());
               
                MessageBox.Show(this.InsertID.ToString());
                this.Command.Dispose();
                this.Connection.Dispose();
            }


        }
        public bool Update(string table, string column, string val, List<string> keys, List<string> values)
        {
            string sql = "UPDATE " + table + " SET ";
            List<string> parameterKeys = new List<string>();
            int z = 0;
            foreach (string key in keys)
            {

                if (keys.Count > 1)
                {
                    if (z == keys.Count - 1)
                    {
                        sql += key + "=@" + key + " ";
                    }
                    else
                    {
                        sql += key + "=@" + key + ", ";

                    }

                }
                else
                {
                    sql += key + "=@" + key + " ";
                }
                z++;

                parameterKeys.Add("@" + key);
            }
            sql += " WHERE " + column + "=@" + column + " ";
            MessageBox.Show(sql);
            using (this.CreateConnection())
            {
                this.Connection.Open();

                this.Command = new SQLiteCommand(sql, this.Connection);
                int i = 0;
                foreach (string value in values)
                {
                    this.Command.Parameters.AddWithValue(parameterKeys.ElementAt(i), values.ElementAt(i));
                    i++;
                }
                this.Command.Parameters.AddWithValue("@" + column, val);
                try
                {
                    this.Command.ExecuteNonQuery();
                    this.Command.Dispose();
                    this.Connection.Dispose();
                    return true;
                }
                catch (Exception)
                {
                    
                    return false;
                }


            }


        }
        public bool Delete(string table, string column, string val)
        {
            string sql = "DELETE FROM " + table + " WHERE " + column + "=@" + column;
            using (this.CreateConnection())
            {
                this.Connection.Open();
                this.Command = new SQLiteCommand(sql, this.Connection);
                this.Command.Parameters.AddWithValue("@" + column, val);
                try
                {
                    this.Command.ExecuteNonQuery();
                    this.Command.Dispose();
                    this.Connection.Dispose();
                    return true;
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                    return false;
                }

            }
        }

        public int Num_Rows(string sql)
        {
            using (this.CreateConnection())
            {
                this.Connection.Open();

                try
                {
                    this.Command = new SQLiteCommand(sql, this.Connection);
                    this.DataTable = new DataTable();
                    this.DataAdapter = new SQLiteDataAdapter(this.Command);
                    this.DataAdapter.Fill(this.DataTable);
                    this.Command.Dispose();
                    this.DataAdapter.Dispose();
                    this.Connection.Dispose();
                    int count= this.DataTable.Rows.Count;
                    this.DataTable.Dispose();
                    return count;
                }
                catch (Exception)
                {

                    return 0;
                }
            }

        }

        public SQLiteDataReader Result(string sql)
        {
            using (this.CreateConnection())
            {
                this.Connection.Open();

                try
                {
                    this.Command = new SQLiteCommand(sql, this.Connection);
                    SQLiteDataReader rd = this.Command.ExecuteReader();
                    rd.Read();
                    this.Command.Dispose();
                    this.Connection.Dispose();
                    return rd;
                }
                catch (Exception)
                {

                    return null;
                }
            }
        }

        public DataTable Table(string sql)
        {
            using (this.CreateConnection())
            {
                this.Connection.Open();

                try
                {
                    this.Command = new SQLiteCommand(sql, this.Connection);
                    this.DataTable = new DataTable();
                    this.DataAdapter = new SQLiteDataAdapter(this.Command);
                    this.DataAdapter.Fill(this.DataTable);
                    this.Command.Dispose();
                    this.DataAdapter.Dispose();
                    this.Connection.Dispose();
                    return this.DataTable;
                }
                catch (Exception)
                {

                    return null;
                }
            }
        }

        public bool Query(string sql)
        {
            using (this.CreateConnection())
            {
                this.Connection.Open();

                try
                {
                    this.Command = new SQLiteCommand(sql, this.Connection);
                    this.Command.ExecuteNonQuery();
                    this.Command.Dispose();
                    this.Connection.Dispose();
                    return true;
                }
                catch (Exception)
                {

                    return false;
                }

            }
        }
    }
}
