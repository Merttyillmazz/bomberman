using UnityEngine;

public interface IBombStats
{
    int GetRadius(); 
}

public class BasicBombStats : IBombStats
{
    public int GetRadius()
    {
        return 1; 
    }
}

public abstract class BombDecorator : IBombStats
{
    protected IBombStats _wrappedBomb;

    public BombDecorator(IBombStats bomb)
    {
        _wrappedBomb = bomb;
    }

    public virtual int GetRadius()
    {
        return _wrappedBomb.GetRadius();
    }
}

public class RadiusEnhancer : BombDecorator
{
    public RadiusEnhancer(IBombStats bomb) : base(bomb) { }

    public override int GetRadius()
    {
        return _wrappedBomb.GetRadius() + 1;
    }
}
