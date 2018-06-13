namespace GovUk.Education.ManageCourses.Ui.Claims
{
    public class OrgClaim
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Category Category { get; set; }

        public override string ToString()
        {
            return $"OrgClaim {Id} {Name} {Category}";
        }
    }
}