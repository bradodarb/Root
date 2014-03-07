using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Web.Root.Caching
{
	public class DynamicWebCache<T>
	{
		#region Private Members
		private string _Key;
        private Func<DynamicWebCache<T>, T> _Load;
		private Action<CacheItemRemovedReason> _OnRemove;
		private DateTime _AbsoluteExpiration;
		private TimeSpan _SlidingExpiration;
		private CacheItemPriority _Priority;
		private Cache _Cache;
		#endregion

        public TimeSpan SlidingExpiration
        {
            get
            {
                return _SlidingExpiration;
            }
            set
            {
                _SlidingExpiration = value;
                this.Refresh();
            }
        }
        public DateTime AbsoluteExpiration
        {
            get
            {
                return _AbsoluteExpiration;
            }
            set
            {
                _AbsoluteExpiration = value;
            }
        }

		#region Constructors


        private DynamicWebCache(string key, Func<DynamicWebCache<T>, T> load, Action<CacheItemRemovedReason> onRemove = null, CacheItemPriority priority = CacheItemPriority.Normal, bool refreshOnRemove = false, HttpContext context = null)
		{
			if (string.IsNullOrEmpty(key))
				throw new ArgumentNullException("name");
			if (load == null)
				throw new ArgumentNullException("load");
            if (context != null)
            {
                _Cache = context.Cache;
            }
            else
            {
                _Cache = HttpContext.Current.Cache;
            }
           
			_Key = key;
			_Load = load;
			_OnRemove = onRemove;
			_AbsoluteExpiration = Cache.NoAbsoluteExpiration;
			_SlidingExpiration = Cache.NoSlidingExpiration;
			_Priority = priority;
		}

        public DynamicWebCache(HttpContext context, string key, Func<DynamicWebCache<T>, T> load, DateTime expiration, Action<CacheItemRemovedReason> onRemove = null, CacheItemPriority priority = CacheItemPriority.Normal)
            : this(key, load, onRemove, priority, false, context)
        {
            _AbsoluteExpiration = expiration;
        }
        public DynamicWebCache(string key, Func<DynamicWebCache<T>, T> load, DateTime expiration, Action<CacheItemRemovedReason> onRemove = null, CacheItemPriority priority = CacheItemPriority.Normal)
			: this(key, load, onRemove, priority)
		{
			_AbsoluteExpiration = expiration;
		}

        public DynamicWebCache(string key, Func<DynamicWebCache<T>, T> load, TimeSpan expiration, Action<CacheItemRemovedReason> onRemove = null, CacheItemPriority priority = CacheItemPriority.Normal)
			: this(key, load, onRemove, priority)
		{
			_SlidingExpiration = expiration;
		}


		#endregion

		public T Value
		{
			get
			{
				if (_Cache.Get(_Key) == null)
				{
					Refresh();
				}

				return (T)_Cache.Get(_Key);
			}
		}

        public DynamicWebCache<T> Invalidate()
		{
			_Cache.Remove(_Key);

			return this;
		}

        public DynamicWebCache<T> Refresh()
		{
			var value = _Load(this);
			Invalidate();
			
			if (value != null)
			{
				_Cache.Add(_Key, value, null, _AbsoluteExpiration, _SlidingExpiration, CacheItemPriority.Normal, OnRemoved);
			}
			return this;
		}

        public DynamicWebCache<T> Load()
		{
			if (_Cache.Get(_Key) != null)
			{
				Refresh();
			}

			return this;
		}

		private void OnRemoved(string key, object value, CacheItemRemovedReason reason)
		{
			if (_OnRemove != null)
			{
				_OnRemove(reason);
			}
		}
	}
}
