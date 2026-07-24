namespace Common.Events.Interfaces
{
    public interface IFileProcessor
    {
        public void writeFile(string fileName, string content);
    }
}
