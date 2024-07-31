using MVCJobSearchApp.Models;

namespace JobSearchApp.Models
{
    public class CV
    {
        public int CVId { get; set; }
        public int ApplicantId { get; set; }
        public string Location { get; set; }
        public string Skill { get; set; }
        public string Education { get; set; }
        public string Description { get; set; }
    }

}
