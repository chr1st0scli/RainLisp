using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace RainLispTests
{
    /// <summary>
    /// Allows the JSON serialization of a class's private fields only.
    /// </summary>
    internal class PrivateFieldsContractResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            MemberInfo[] membersInfo = objectType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

            return membersInfo.ToList();
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            return base.CreateProperties(type, MemberSerialization.Fields);
        }
    }
}
