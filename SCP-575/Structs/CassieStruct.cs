namespace SCP_575.Structs
{
    public struct CassieStruct
    {
        public CassieStruct(string message, string translation)
        {
            Message = message;
            Translation = translation;
        }
        public string Message { get; set; }
        public string Translation { get; set; }
    }
}