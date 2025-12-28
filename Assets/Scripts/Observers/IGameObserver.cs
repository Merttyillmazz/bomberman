public interface IGameObserver
{
 
    void OnNotify(string eventName, object data);
}