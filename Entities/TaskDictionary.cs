namespace Entities
{
    public class TaskDictionary
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Date { get; set; }
        public string? Time { get; set; }
        public bool IsCompleted { get; set; }
        public string? Category { get; set; }
        public Guid UserId { get; set; }
    }
}