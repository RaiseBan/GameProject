using UnityEngine;
public interface IUserItem
{
    public void Usage<T>(T _item) where T : ItemData;
    
}