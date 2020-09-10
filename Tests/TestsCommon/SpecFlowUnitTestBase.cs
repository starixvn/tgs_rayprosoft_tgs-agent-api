using System;
using TechTalk.SpecFlow;

namespace TGS.Tests.TestsCommon
{
    public abstract class SpecFlowUnitTestBase : Steps
    {
        private readonly ScenarioContext _context;

        public SpecFlowUnitTestBase(ScenarioContext context)
        {
            _context = context;
        }

        protected T Get<T>(string key, T defaultValue = default)
        {
            return _context.TryGetValue<T>(key, out var value) ?
                value :
                defaultValue;
        }

        protected T GetOrCreate<T>(string key, Func<T> create)
        {
            return _context.ContainsKey(key) ?
                _context.Get<T>(key) :
                Set(key, create());
        }

        protected T Set<T>(string key, T value)
        {
            _context.Add(key, value);
            return value;
        }

        protected bool Remove(string key)
        {
            return _context.Remove(key);
        }
    }
}