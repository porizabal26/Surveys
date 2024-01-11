namespace Surveys.Models
{
    public class Field
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Placeholder { get; set; }
        public bool Mandatory { get; set; }
        public long DataTypeId { get; set; }
        public DataType? DataType { get; set; }
    }
}
