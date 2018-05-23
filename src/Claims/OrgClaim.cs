namespace ManageCoursesUi.Claims
{
    public class OrgClaim
    {
        public string id { get; set; }
        public string name { get; set; }
        public Category category { get; set; }

        public override string ToString()
        {
            return $"OrgClaim {id} {name} {category}";
        }
    }
}