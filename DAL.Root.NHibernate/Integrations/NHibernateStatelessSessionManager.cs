using System.Diagnostics;
using System.Runtime.Remoting.Messaging;
using System.Web;
using NHibernate;
using NHibernate.Cache;
using NHibernate.Cfg;

namespace DAL.Root.NHibernate.Integrations
{
    /// <summary>
    /// Handles creation and management of sessions and transactions.  It is a singleton because 
    /// building the initial session factory is very expensive.
    /// </summary>
    public sealed class NHibernateStatelessSessionManager
    {
        #region Thread-safe, lazy Singleton

        /// <summary>
        /// This is a thread-safe, lazy singleton.  See http://www.yoda.arachsys.com/csharp/singleton.html
        /// for more details about its implementation.
        /// </summary>
        public static NHibernateStatelessSessionManager Instance
        {
            get
            {
                return Nested.NHibernateSessionManager;
            }
        }

        /// <summary>
        /// Initializes the NHibernate session factory upon instantiation.
        /// </summary>
        private NHibernateStatelessSessionManager()
        {
            InitSessionFactory();
        }

        /// <summary>
        /// Assists with ensuring thread-safe, lazy singleton
        /// </summary>
        internal class Nested
        {
            static Nested()
            {

            }
            private static NHibernateStatelessSessionManager _NHibernateSessionManager;
            internal static NHibernateStatelessSessionManager NHibernateSessionManager
            {
                get
                {
                    if (_NHibernateSessionManager == null)
                    {
                        _NHibernateSessionManager = new NHibernateStatelessSessionManager();
                    }
                    return _NHibernateSessionManager;
                }
            }
        }

        #endregion

        private void InitSessionFactory()
        {
            sessionFactory = SessionFactoryProvider.SessionFactory.GetFactory();
        }

        /// <summary>
        /// Allows you to register an interceptor on a new session.  This may not be called if there is already
        /// an open session attached to the HttpContext.  If you have an interceptor to be used, modify
        /// the HttpModule to call this before calling BeginTransaction().
        /// </summary>



        /// <summary>
        /// Gets a session with or without an interceptor.  This method is not called directly; instead,
        /// it gets invoked from other public methods.
        /// </summary>
        public IStatelessSession GetSession()
        {
            IStatelessSession session = ContextSession;

            if (session == null)
            {
                session = sessionFactory.OpenStatelessSession();

                ContextSession = session;
            }

            Debug.Assert(session != null, "session was null");

            return session;
        }

        /// <summary>
        /// Flushes anything left in the session and closes the connection.
        /// </summary>
        public void CloseSession()
        {
            IStatelessSession session = ContextSession;

            if (session != null && session.IsOpen)
            {
                session.Close();
                session.Dispose();
            }

            ContextSession = null;
        }

        public void BeginTransaction()
        {
            ITransaction transaction = ContextTransaction;

            if (transaction == null)
            {
                transaction = GetSession().BeginTransaction();
                ContextTransaction = transaction;
            }
        }

        public bool CommitTransaction()
        {
            ITransaction transaction = ContextTransaction;
            bool result = false;
            try
            {
                if (HasOpenTransaction())
                {
                    transaction.Commit();
                    result = transaction.WasCommitted;
                    ContextTransaction = null;
                }
            }
            catch (HibernateException)
            {
                RollbackTransaction();
                throw;
            }
            return result;
        }


        public bool HasOpenTransaction()
        {
            ITransaction transaction = ContextTransaction;

            return transaction != null && !transaction.WasCommitted && !transaction.WasRolledBack;
        }

        public bool RollbackTransaction()
        {
            ITransaction transaction = ContextTransaction;
            bool result = false;

            try
            {

                if (HasOpenTransaction())
                {
                    transaction.Rollback();
                    result = transaction.WasRolledBack;
                }

                ContextTransaction = null;
            }
            finally
            {
                CloseSession();
            }
            return result;
        }

        /// <summary>
        /// If within a web context, this uses <see cref="HttpContext" /> instead of the WinForms 
        /// specific <see cref="CallContext" />.  Discussion concerning this found at 
        /// http://forum.springframework.net/showthread.php?t=572.
        /// </summary>
        private ITransaction ContextTransaction
        {
            get
            {
                if (IsInWebContext())
                {
                    return (ITransaction)HttpContext.Current.Items[TRANSACTION_KEY];
                }
                else
                {
                    return (ITransaction)CallContext.GetData(TRANSACTION_KEY);
                }
            }
            set
            {
                if (IsInWebContext())
                {
                    HttpContext.Current.Items[TRANSACTION_KEY] = value;
                }
                else
                {
                    CallContext.SetData(TRANSACTION_KEY, value);
                }
            }
        }

        /// <summary>
        /// If within a web context, this uses <see cref="HttpContext" /> instead of the WinForms 
        /// specific <see cref="CallContext" />.  Discussion concerning this found at 
        /// http://forum.springframework.net/showthread.php?t=572.
        /// </summary>
        private IStatelessSession ContextSession
        {
            get
            {
                if (IsInWebContext())
                {
                    return (IStatelessSession)HttpContext.Current.Items[SESSION_KEY];
                }
                else
                {
                    return (IStatelessSession)CallContext.GetData(SESSION_KEY);
                }
            }
            set
            {
                if (IsInWebContext())
                {
                    HttpContext.Current.Items[SESSION_KEY] = value;
                }
                else
                {
                    CallContext.SetData(SESSION_KEY, value);
                }
            }
        }

        private bool IsInWebContext()
        {
            return HttpContext.Current != null;
        }

        private const string TRANSACTION_KEY = "CONTEXT_TRANSACTION";
        private const string SESSION_KEY = "CONTEXT_SESSION";
        private ISessionFactory sessionFactory;
    }
}
