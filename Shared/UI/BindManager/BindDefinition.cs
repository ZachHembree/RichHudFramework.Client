using System.Xml.Serialization;
using VRage;
using BindDefinitionData = VRage.MyTuple<string, string[]>;

namespace RichHudFramework
{
    namespace UI
    {
        [XmlType(TypeName = "Alias")]
        public struct BindAliasDefinition
        {
            [XmlArray("Controls")]
            public string[] controlNames;
        }

        /// <summary>
        /// Stores data for serializing individual key binds to XML.
        /// </summary>
        [XmlType(TypeName = "Bind")]
        public struct BindDefinition
        {
            [XmlAttribute]
            public string name;

            [XmlArray("Controls")]
            public string[] controlNames;

            [XmlArray("Aliases")]
            public BindAliasDefinition[] aliases;

            public BindDefinition(string name, string[] controlNames, BindAliasDefinition[] aliases = null)
            {
                this.name = name;
                this.controlNames = controlNames;
                this.aliases = aliases;
            }

            public static implicit operator BindDefinition(BindDefinitionData value)
            {
                return new BindDefinition(value.Item1, value.Item2);
            }

            public static implicit operator BindDefinitionData(BindDefinition value)
            {
                return new BindDefinitionData(value.name, value.controlNames);
            }
        }
    }
}