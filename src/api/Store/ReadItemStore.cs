using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Laobian.Share.Blog;
using Laobian.Share.Read;
using Laobian.Share.Util;

namespace Laobian.Api.Store
{
    public class ReadItemStore
    {
        private readonly ConcurrentDictionary<int, List<ReadItem>> _readItems;

        public ReadItemStore(IDictionary<int, string> val)
        {
            _readItems = new ConcurrentDictionary<int, List<ReadItem>>();
            foreach (var item in val)
            {
                _readItems.TryAdd(item.Key, JsonUtil.Deserialize<List<ReadItem>>(item.Value));
            }
        }

        public IDictionary<int, List<ReadItem>> GetAll()
        {
            return _readItems;
        }

        public List<ReadItem> Get(int year)
        {
            if (_readItems.TryGetValue(year, out var val))
            {
                return val;
            }

            return null;
        }

        public void Add(ReadItem item)
        {
            _readItems.AddOrUpdate(item.StartTime.Year,
                link => new List<ReadItem> { item },
                (link, val) =>
                {
                    val.Add(item);
                    return val;
                });
        }

        public void Remove(string id)
        {
            foreach (var readItem in _readItems)
            {
                var result = readItem.Value.FirstOrDefault(x => x.Id == id);
                if (result != null)
                {
                    readItem.Value.Remove(result);
                }
            }
        }

        public void Update(ReadItem item)
        {
            var existingItem = _readItems.Values.SelectMany(x => x).FirstOrDefault(x => x.Id == item.Id);
            if (existingItem == null)
            {
                return;
            }

            existingItem.PublishTime = item.PublishTime;
            existingItem.BookName = item.BookName;
            existingItem.IsCompleted = item.IsCompleted;
            existingItem.StartTime = item.StartTime;
            existingItem.AuthorCountry = item.AuthorCountry;
            existingItem.BlogPostLink = item.BlogPostLink;
            existingItem.BookName2 = item.BookName2;
            existingItem.EndTime = item.EndTime;
            existingItem.AuthorName = item.AuthorName;
            existingItem.PublisherName = item.PublisherName;
            existingItem.Grade = item.Grade;
            existingItem.ShortComment = item.ShortComment;
        }
    }
}
