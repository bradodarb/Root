using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Caching;

namespace Web.Root.Caching
{
	public class WebCache<T>
	{
		#region Private Members
		private string _Key;
		private Func<T> _Load;
		private Action<CacheItemRemovedReason> _OnRemove;
		private DateTime _AbsoluteExpiration;
		private TimeSpan _SlidingExpiration;
		private CacheItemPriority _Priority;
		private Cache _Cache;
        private HttpContext context;
        private string p;
        private DateTime dateTime;
		private bool _throwIfCacheNotAvailable;
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


        private WebCache(string key, Func<T> load, Action<CacheItemRemovedReason> onRemove = null, CacheItemPriority priority = CacheItemPriority.Normal, bool refreshOnRemove = false, HttpContext context = null, bool throwIfCacheNotAvailable = false)
		{
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("name");
            }
            if (load == null)
            {
                throw new ArgumentNullException("load");
            }

            if (context != null)
            {
                _Cache = context.Cache;
            }
            else
            {
				if (HttpContext.Current != null)
				{
					_Cache = HttpContext.Current.Cache;
				}
            }
           
			_Key = key;
			_Load = load;
			_OnRemove = onRemove;
			_AbsoluteExpiration = Cache.NoAbsoluteExpiration;
			_SlidingExpiration = Cache.NoSlidingExpiration;
			_Priority = priority;
			_throwIfCacheNotAvailable = throwIfCacheNotAvailable;
		}

		public WebCache(HttpContext context, string key, Func<T> load, DateTime expiration, Action<CacheItemRemovedReason> onRemove = null, CacheItemPriority priority = CacheItemPriority.Normal, bool throwIfCacheNotAvailable = false)
			: this(key, load, onRemove, priority, false, context, throwIfCacheNotAvailable: throwIfCacheNotAvailable)
        {
            _AbsoluteExpiration = expiration;
        }
		public WebCache(string key, Func<T> load, DateTime expiration, Action<CacheItemRemovedReason> onRemove = null, CacheItemPriority priority = CacheItemPriority.Normal, bool throwIfCacheNotAvailable = false)
			: this(key, load, onRemove, priority, throwIfCacheNotAvailable: throwIfCacheNotAvailable)
		{
			_AbsoluteExpiration = expiration;
		}

		public WebCache(string key, Func<T> load, TimeSpan expiration, Action<CacheItemRemovedReason> onRemove = null, CacheItemPriority priority = CacheItemPriority.Normal, bool throwIfCacheNotAvailable = false)
			: this(key, load, onRemove, priority, throwIfCacheNotAvailable: throwIfCacheNotAvailable)
		{
			_SlidingExpiration = expiration;
		}

		#endregion

		public T Value
		{
			get
			{
				if (_Cache != null)
				{
					if (_Cache.Get(_Key) == null)
					{
						Refresh();
					}

					return (T)_Cache.Get(_Key);
				}
				else
				{
					if (_throwIfCacheNotAvailable)
					{
						throw new NullReferenceException("Cache is not available");
					}
					else
					{
						return _Load();
					}
				}
			}
		}

		public WebCache<T> Invalidate()
		{
			_Cache.Remove(_Key);

			return this;
		}

		public WebCache<T> Refresh()
		{
			var value = _Load();

			Invalidate();
			if (value != null)
			{
				_Cache.Add(_Key, value, null, _AbsoluteExpiration, _SlidingExpiration, CacheItemPriority.Normal, OnRemoved);
			}
			return this;
		}

		public WebCache<T> Load()
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
