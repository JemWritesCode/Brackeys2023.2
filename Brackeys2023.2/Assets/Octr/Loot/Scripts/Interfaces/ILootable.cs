using JadePhoenix.Gameplay;
using System;

namespace octr.Loot
{
    /// <summary>
    /// This interface is for anything the player picks up / collects from the ground
    /// It will have a method that can be called with different logic when collected
    /// It also has a reference to the ItemDrop that it is attatched to
    /// </summary>
    /// 
    public interface ILootable
    {
        void Collect<T>(T item, Character character);
    }
}


