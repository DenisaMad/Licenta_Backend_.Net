namespace Backend.DataAbstraction.Security
{
    public class EncryptedRequestPayload
    {
        public string EncryptedKey { get; set; }
        public string Iv { get; set; }
        public string Payload { get; set; }
    }

    public class EncryptedResponsePayload
    {
        public string Iv { get; set; }
        public string Payload { get; set; }
    }
}
