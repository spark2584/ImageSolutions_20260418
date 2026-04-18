using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Dynamic;

namespace ISBase
{
    [Serializable]
    public abstract class DynamicBaseClass : DynamicObject
    {
        protected Dictionary<string, object> properties = new Dictionary<string, object>();

        //[NonSerialized]
        //protected SqlConnection mConnection;
        //[NonSerialized]
        //protected SqlTransaction mTransaction;

        public bool IsLoaded { get; protected set; }
        public virtual bool IsActive { get; set; }

        public DynamicBaseClass()
        {
            IsActive = true;
        }

        protected virtual void Load()
        {
            IsLoaded = true;
        }

        protected virtual void Load(SqlConnection objConn, SqlTransaction objTran)
        {
            IsLoaded = true;
            //mConnection = objConn;
            //mTransaction = objTran;
        }

        public virtual bool Create()
        {
            if (IsLoaded) throw new Exception("Create() cannot be performed because object is loaded from constructors");
            return true;
        }

        public virtual bool Create(SqlConnection objConn, SqlTransaction objTran)
        {
            if (IsLoaded) throw new Exception("Create() cannot be performed because object is loaded from constructors");
            //mConnection = objConn;
            //mTransaction = objTran;
            return true;
        }

        public virtual bool Update()
        {
            if (!IsLoaded) throw new Exception("Update() cannot be performed because object is not loaded from constructors");
            return true;
        }

        public virtual bool Update(SqlConnection objConn, SqlTransaction objTran)
        {
            if (!IsLoaded) throw new Exception("Update() cannot be performed because object is not loaded from constructors");
            //mConnection = objConn;
            //mTransaction = objTran;
            return true;
        }

        public virtual bool Delete()
        {
            if (!IsLoaded) throw new Exception("Delete() cannot be performed because object is not loaded from constructors");
            return true;
        }

        public virtual bool Delete(SqlConnection objConn, SqlTransaction objTran)
        {
            if (!IsLoaded) throw new Exception("Delete() cannot be performed because object is not loaded from constructors");
            //mConnection = objConn;
            //mTransaction = objTran;
            return true;
        }
    }
}
