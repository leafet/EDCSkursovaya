namespace KursavayaECS.Data
{
    public class CourseClaim
    {
        public required Guid ID { get; set; }
        public required Student Student { get; set; }
        public required Course Course { get; set; }
    }
}
