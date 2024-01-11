namespace Surveys.Models
{
    public class Survey
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<SurveyDetail>? Details { get; set; }
    }
}
