namespace DosinisSDK.Rest
{
    public struct Header
    {
        public Header(string key, string value)
        {
            this.key = key;
            this.value = value;
        }
        
        public readonly string key;
        public readonly string value;
    }
}
