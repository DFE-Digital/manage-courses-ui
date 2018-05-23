namespace ManageCoursesUi.Claims
{
    public class Category
    {
        public string id { get; set; }
        public string name { get; set; }

        public override string ToString()
        {
            return $"Category {id} {name}";
        }
    }
}
