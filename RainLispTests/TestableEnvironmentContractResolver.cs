using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using RainLisp.Evaluation.Results;
using System.Reflection;

namespace RainLispTests
{
    internal class TestableEnvironmentContractResolver : DefaultContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            string valuePropName = nameof(PrimitiveDatum<bool>.Value);
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            var props = type.GetProperties(flags)
                .Where(p => p.Name == valuePropName)
                .Select(p => base.CreateProperty(p, memberSerialization));

            var fields = type.GetFields(flags)
                .Select(f => base.CreateProperty(f, memberSerialization));

            var jsonProps = props.Concat(fields).ToList();
            foreach (var jsonProp in jsonProps)
                jsonProp.Readable = true;

            return jsonProps;
        }
    }
}
