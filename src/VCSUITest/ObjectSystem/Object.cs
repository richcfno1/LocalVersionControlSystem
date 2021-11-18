namespace LocalVersionControlSystem.ObjectSystem
{
    class Object
    {
        public string NameSHA256 { get; set; }
        public string ContentSHA256 { get; set; }
        public string Name { get; set; }
        public byte[] Content { get; set; }

        public Object(string nameSHA256, string contentSHA256, string name, byte[] content)
        {
            NameSHA256 = nameSHA256;
            ContentSHA256 = contentSHA256;
            Name = name;
            Content = content;
        }
    }
}
