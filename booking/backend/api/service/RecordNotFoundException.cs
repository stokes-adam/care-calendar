namespace service;

public class RecordNotFoundException<T> : Exception
{
    public RecordNotFoundException(Guid id)
        : base($"No {typeof(T).Name} record found with id {id}")
    {
    }
}