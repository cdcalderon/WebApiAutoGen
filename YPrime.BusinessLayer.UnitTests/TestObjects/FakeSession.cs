using System.Collections.Generic;
using System.Web;

namespace YPrime.BusinessLayer.UnitTests.TestObjects
{
    public class FakeSession : HttpSessionStateBase
    {
        private readonly IDictionary<string, object> _sessionDictionary = new Dictionary<string, object>();

        public override object this[string name]
        {
            get => _sessionDictionary.ContainsKey(name) ? _sessionDictionary[name] : null;
            set => _sessionDictionary[name] = value;
        }
    }
}