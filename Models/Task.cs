namespace toDoList
{
    public class Task
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public TaskType Type { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}