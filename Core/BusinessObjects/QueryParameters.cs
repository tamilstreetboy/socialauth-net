using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Brickred.SocialAuth.NET.Core
{
    public class QueryParameter
    {
        private string name = null;
        private string value = null;

        public QueryParameter(string name, string value)
        {
            this.name = name;
            this.value = value;
        }

        public string Name
        {
            get { return name; }
        }

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }
    }

    /// <summary>
    /// Comparer class used to perform the sorting of the query parameters
    /// </summary>
    public class QueryParameterComparer : IComparer<QueryParameter>
    {

        #region IComparer<QueryParameter> Members

        public int Compare(QueryParameter x, QueryParameter y)
        {
            if (x.Name == y.Name)
            {
                return string.Compare(x.Value, y.Value);
            }
            else
            {
                return string.Compare(x.Name, y.Name);
            }
        }

        #endregion
    }

    public class QueryParameters : ICollection<QueryParameter>
    {
        List<QueryParameter> queryparameters = new List<QueryParameter>();

     
        public string this[string key]
        {
            get { return queryparameters.Find(x => x.Name == key).Value; }
            set { queryparameters.RemoveAll(x => x.Name == key); queryparameters.Add(new QueryParameter(key, value)); }
        }

        public void AddRange(QueryParameters range, bool shouldOverride)
        {
            foreach (var item in range)
            {
                if (shouldOverride && queryparameters.Exists(x => x.Name == item.Name))
                    queryparameters.RemoveAll(x => x.Name == item.Name);
                queryparameters.Add(new QueryParameter(item.Name, item.Value));
            }
        }

        public void Add(string name, string value)
        {
            queryparameters.Add(new QueryParameter(name, value));
        }

        public  void Sort()
        {
            queryparameters.Sort(new QueryParameterComparer());
        }

        public bool HasName(string name)
        {
            return queryparameters.Exists(x => x.Name == name);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            QueryParameter p = null;
            for (int i = 0; i < queryparameters.Count; i++)
            {
                p = queryparameters[i];
                sb.AppendFormat("{0}={1}", p.Name, p.Value);

                if (i < queryparameters.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        #region ICollection<QueryParameter> Members

        public void Add(QueryParameter item)
        {
            queryparameters.Add(item);
        }

        public void Clear()
        {
            queryparameters.Clear();
        }

        public bool Contains(QueryParameter item)
        {
           return queryparameters.Contains(item);
        }

        public void CopyTo(QueryParameter[] array, int arrayIndex)
        {
            queryparameters.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return queryparameters.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(QueryParameter item)
        {
            return queryparameters.Remove(item);
        }

        #endregion

        #region IEnumerable<QueryParameter> Members

        public IEnumerator<QueryParameter> GetEnumerator()
        {
            return queryparameters.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return queryparameters.GetEnumerator();
        }

        #endregion
    }


}
