using DynamicsAdapter.Web.PersonSearch.Models;
using System.Collections.Generic;

namespace DynamicsAdapter.Web.PersonSearch
{
    public interface ISearchResultQueue
    {
        bool Enqueue(PersonSearchCompleted completeEvent);
        PersonSearchCompleted Dequeue();
        bool IsEmpty();
    }

    public class SearchResultQueue : ISearchResultQueue
    {
        private static Queue<PersonSearchCompleted> _resultQueue;
        private static object syncObj = new object();
        public SearchResultQueue()
        {
            if (_resultQueue == null)
                _resultQueue = new Queue<PersonSearchCompleted>();
        }

        public bool Enqueue(PersonSearchCompleted completeEvent)
        {
            lock (syncObj)
            {
                _resultQueue.Enqueue(completeEvent);
                return true;
            }
        }

        public bool IsEmpty()
        {
            return _resultQueue.Count <= 0;
        }

        public PersonSearchCompleted Dequeue()
        {
            lock (syncObj)
            {
                if(_resultQueue.Count>0)
                    return _resultQueue.Dequeue();
                return null;
            }
        }
    }
}
