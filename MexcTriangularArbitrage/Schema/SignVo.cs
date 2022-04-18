namespace MexcTriangularArbitrage.Schema
{
    public class SignVo
    {
        public long ReqTime { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        /// <summary>
        /// Get the request parameters are sorted in dictionary order, with & concatenated strings, POST should be a JSON string
        /// </summary>
        public string RequestParam { get; set; } = "";
    }
}
