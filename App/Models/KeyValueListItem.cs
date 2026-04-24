namespace TOFF.Models
{
    internal class KeyValueListItem
    {
        public string Key { get; set; }
        public string Value { get; set; }

        public KeyValueListItem(string key, string value)
        {
            Key = key;
            Value = value;
        }

        public KeyValueListItem(KeyValuePair<string, string> entry)
        {
            Key = entry.Key;
            Value = entry.Value;
        }

        public override string ToString()
        {
            return $"{Key} -> {Value}";
            //maybe replace with this at some point, idk
            return String.Format("{0, -50} {1, -5} {2}", Key, "->", Value);
        }
    }
}
