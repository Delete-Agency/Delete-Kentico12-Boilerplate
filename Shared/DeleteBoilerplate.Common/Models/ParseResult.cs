namespace DeleteBoilerplate.Common.Models
{
    public class ParseResult<TValue>
    {
        public TValue Value { get; set; }

        public bool Successful { get; set; }
    }
}