namespace Surveys.Models
{
    public class SurveyDetailResponse
    {
        public long Id { get; set; }
        public long? SurveyId { get; set; }
        public long? SurveyDetailId { get; set; }
        public string? TextValue { get; set; } = "";
        public int? NumValue { get; set; }
        public DateTime? DateValue { get; set; }
        public string? FieldName { get; set; }
    }
}
