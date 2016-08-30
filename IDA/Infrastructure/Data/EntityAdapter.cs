using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace ConceptONE.Infrastructure
{

    public class EntityAdapter<T>
    {
        private IDbConnection _Connection;
        private string _CommandText;
        private IDbDataParameter[] _Parameters;
        private bool _IsStoredProcedure;

        public IList<T> EntityList { get; private set; }

        public EntityAdapter(string connectionString, string commandText, params IDbDataParameter[] prms)
        {
            Initialize(connectionString, commandText, prms);
        }
        public EntityAdapter(IDbConnection connection, string commandText, bool isStoredProcedure, params IDbDataParameter[] prms)
        {
            Initialize(connection, commandText, isStoredProcedure, prms);
        }

        public void Initialize(string connectionString, string commandText, params IDbDataParameter[] prms)
        {
            _Connection = new System.Data.SqlClient.SqlConnection(connectionString);
            try
            {
                _CommandText = commandText;
                _Parameters = prms;
                _Connection.Open();
                EntityList = GetEntityList();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw;
            }
            finally
            {
                _Connection.Close();
            }
        }
        
        private void Initialize(IDbConnection connection, string commandText, bool isStoredProcedure, params IDbDataParameter[] prms)
        {
            _Connection = connection;
            try
            {
                _CommandText = commandText;
                _IsStoredProcedure = isStoredProcedure;
                _Parameters = prms;
                if (connection.State != ConnectionState.Open) _Connection.Open();
                EntityList = GetEntityList();
            }
            catch (Exception ex)
            {
                Logger.LogActivity("ERROR (EntityAdapter.Initialize) - ConnectionString:({0}); CommandText:({1});", connection.ConnectionString, commandText);
                Logger.LogException(ex);
                throw;
            }
        }

        private IDbCommand GetCommand(string commandText, params IDbDataParameter[] prms)
        {
            IDbCommand cmd = _Connection.CreateCommand();
            cmd.CommandText = commandText;
            cmd.CommandTimeout = 3 * 60;
            if (_IsStoredProcedure) cmd.CommandType = CommandType.StoredProcedure;
            if (prms != null)
            {
                foreach (IDbDataParameter p in prms)
                {
                    cmd.Parameters.Add(p);
                }
            }
            return cmd;
        }

        private void Map<TEntity>(IDataRecord record, TEntity entity)
        {
            List<string> tempExclude = new List<string> { "NumericPrecision".ToLower(), "CharacterMaximumLength".ToLower() };
            PropertyInfo[] props = entity.GetType().GetProperties();
            foreach (PropertyInfo prop in entity.GetType().GetProperties())
            {
                try
                {
                    if (tempExclude.Contains(prop.Name.ToLower()))
                    {
                        prop.SetValue(entity, string.IsNullOrEmpty(record[prop.Name].ToString()) ? null : record[prop.Name].ToString());
                    }
                    else
                    {
                        prop.SetValue(entity, string.IsNullOrEmpty(record[prop.Name].ToString()) ? null : record[prop.Name]);
                    }
                }
                catch (Exception ex)
                {
                    prop.SetValue(entity, string.IsNullOrEmpty(record[prop.Name].ToString()) ? null : record[prop.Name].ToString());
                    throw ex;
                }
            }
        }

        private IList<T> GetEntityList()
        {
            IList<T> result;
            try
            {
                using (IDbCommand cmd = GetCommand(_CommandText, _Parameters))
                {
                    using (IDataReader dr = cmd.ExecuteReader())
                    {
                        result = new List<T>();
                        while (dr.Read())
                        {
                            var entity = Activator.CreateInstance(typeof(T));
                            Map(dr, entity);
                            result.Add((T)entity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogActivity(string.Format("ERROR (EntityAdapter.GetEntityList): {0}", _CommandText));
                Logger.LogException(ex);
                throw;
            }
            
            return result;
        }

    }

}
