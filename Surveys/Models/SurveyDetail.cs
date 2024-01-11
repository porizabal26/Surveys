namespace Surveys.Models
{
    public class SurveyDetail
    {
        public long Id { get; set; }
        public long? SurveyId { get; set; }
        public long? FieldId { get; set; }
        public Survey? Survey { get; set; }
        public Field? Field { get; set; }
    }
}
