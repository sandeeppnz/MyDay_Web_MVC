namespace SANSurveyWebAPI.Models
{
    public class CheckBoxListItem
    {
        public int ID { get; set; }
        public string Display { get; set; }
        public string Description { get; set; }
        public bool IsChecked { get; set; }
        public bool IsSelectedBefore { get; set; }
        //public bool? IsWardRoundIndicator { get; set; }
    }
}