namespace TOFF.Models
{
    internal class ConfigItemValue
    {
        public string Name;
        public string Value;
        public List<string>? Presets { get; set; }

        public ConfigItemValue(string name, string value, List<string>? presets)
        {
            Name = name;
            Value = value;
            Presets = presets;
        }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }

    }
}
