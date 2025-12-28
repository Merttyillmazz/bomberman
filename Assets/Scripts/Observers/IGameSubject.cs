public interface IGameSubject
{
    void AddObserver(IGameObserver observer);
    void RemoveObserver(IGameObserver observer);
    void NotifyObservers(string eventName);
}