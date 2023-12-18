using System.ComponentModel;
using System.Xml.Serialization;

namespace NP.Avalonia.Visuals.Behaviors.DataGridBehaviors
{
    public class ColumnSerializationData
    {
        [XmlAttribute]
        public string? HeaderId { get; set; }

        [XmlAttribute]
        public bool IsVisible { get; set; }

        [XmlAttribute]
        public string? WidthStr { get; set; }
    }
}
