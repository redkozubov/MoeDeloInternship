namespace MyThreadPool
{
    public interface IDataGenerator
    {
        event DataGenerator.DataGeneratedEventHandler DataGeneratedEvent;
    }
}