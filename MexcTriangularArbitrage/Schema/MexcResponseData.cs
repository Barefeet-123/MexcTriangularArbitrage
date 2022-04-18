namespace MexcTriangularArbitrage.Schema
{
    public class MexcResponseData<T>
    {
        public int code { get; set; }
        public T data { get; set; }
    }
}
