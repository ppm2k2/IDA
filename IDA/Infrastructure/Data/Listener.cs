using System;
using System.Data;
using System.Data.SqlClient;

namespace ConceptONE.Infrastructure.Data
{

    /// <summary>
    /// For the SqlDependency object to work the BROKER must be enabled on the Database
    /// ALTER DATABASE [Sandbox] SET ENABLE_BROKER WITH ROLLBACK IMMEDIATE
    /// </summary>
    public class Listener : IDisposable
    {
        public delegate void EventHandler(DataSet dataSet);

        #region Fields

        private SqlConnection _Connection;
        private SqlCommand _Command;

        #endregion

        #region Events

        public event EventHandler OnChange;

        #endregion

        #region Constructor

        public Listener(string connectionString, SqlCommand command)
        {
            _Connection = new SqlConnection(connectionString);
            _Command = command;
            _Command.Connection = _Connection;
            _Connection.Open();

            SqlDependency.Start(_Connection.ConnectionString);
        }

        #endregion

        #region Methods

        public void Dispose()
        {
            Stop();
        }

        public void Start()
        {
            Listen();
        }

        public void Stop()
        {
            SqlDependency.Stop(_Connection.ConnectionString);

            _Connection.Close();
            _Command.Dispose();
            _Connection.Dispose();
        }

        private void Listen()
        {
            _Command.Notification = null;

            SqlDependency dependency = GetSqlDependency();
            DataSet dataAffected = GetDataset();

            OnChange(dataAffected);
        }

        private SqlDependency GetSqlDependency()
        {
            SqlDependency result = new SqlDependency(_Command);
            result.OnChange += new OnChangeEventHandler(Handle_OnChange);

            return result;
        }

        private DataSet GetDataset()
        {
            DataSet result = new DataSet();

            using (SqlDataAdapter adapter = new SqlDataAdapter(_Command))
                adapter.Fill(result);

            return result;
        }

        private void Handle_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                SqlDependency dependency = (SqlDependency)sender;
                dependency.OnChange -= Handle_OnChange;

                Listen();
            }
            else
            {
                throw new Exception("Failed to create queue notification subscription!");
            }
        }

        #endregion
    }
}
