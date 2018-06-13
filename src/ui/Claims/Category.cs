namespace GovUk.Education.ManageCourses.Ui.Claims
{
    public class Category
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Category {Id} {Name}";
        }
    }
}
