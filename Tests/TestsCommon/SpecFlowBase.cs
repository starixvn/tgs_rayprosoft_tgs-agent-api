using System;
using System.Linq;
using TechTalk.SpecFlow;

namespace TGS.Tests.TestsCommon
{
    public abstract class SpecFlowBase : Steps
    {
        private readonly ScenarioContext _context;

        protected SpecFlowBase(ScenarioContext context)
        {
            _context = context;
        }

        protected T Get<T>(string key, T defaultValue = default) =>
            _context.TryGetValue<T>(key, out var value) ? value : defaultValue;

        protected T GetOrCreate<T>(string key, Func<T> create) =>
            _context.ContainsKey(key) ? _context.Get<T>(key) : Set(key, create());

        protected T Set<T>(string key, T value)
        {
            _context[key] = value;
            return value;
        }

        protected bool Remove(string key) =>
            _context.Remove(key);

        protected bool HasTag(string tag) =>
            _context.ScenarioInfo.Tags.Contains(tag);
    }
}