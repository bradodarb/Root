using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DAL.Root.NHibernate.Integrations;
using Model.Root;
using NHibernate;
using Util.Root;

namespace DAL.Root.NHibernate
{
	public class NHibernateSessionperRequestRepositoryBase<T> : NHibernateRepositoryBase<T> where T : class, IPersistenceEntity
	{

		public NHibernateSessionperRequestRepositoryBase(ISession session)
		{
			_Session = session;
		}

		public override bool SubmitChanges()
		{
			bool committed = false;

			if (!NHibernateSessionManager.Instance.HasOpenTransaction())
			{
				NHibernateSessionManager.Instance.BeginTransaction();
			}
			committed = NHibernateSessionManager.Instance.CommitTransaction();

			return committed;
		}

		public override bool RollBack()
		{
			bool rolled = false;

			rolled = NHibernateSessionManager.Instance.RollbackTransaction();

			return rolled;
		}

		public override void Dispose()
		{
			//Defer session disposal and let the ISession per HttpRequest provider manage it
		}
	}
}
