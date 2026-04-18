using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections;

namespace ISBase
{
    [Serializable]
    public abstract class BaseClass
    {
        //[NonSerialized]
        //protected SqlConnection mConnection;
        //[NonSerialized]
        //protected SqlTransaction mTransaction;

        public bool IsLoaded { get; set; }
        public bool IsCopied { get; set; }
        private bool? mIsActive = null;
        public virtual bool IsActive { get; set; }

        private ArrayList mQuery { get; set; }
        public ArrayList Query
        {
            get
            {
                if (mQuery == null) mQuery = new ArrayList();
                return mQuery;
            }
            set
            {
                mQuery = value;
            }

        }

        public BaseClass()
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

        public virtual bool Copy()
        {
            IsLoaded = false;
            IsCopied = true;
            //if (!IsLoaded) throw new Exception("Copy() cannot be performed because object is not loaded from constructors");
            return true;
        }

        public virtual bool Copy(SqlConnection objConn, SqlTransaction objTran)
        {
            IsLoaded = false;
            //if (!IsLoaded) throw new Exception("Copy() cannot be performed because object is not loaded from constructors");
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
