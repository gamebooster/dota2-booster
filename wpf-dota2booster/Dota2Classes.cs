using System;
using Dota2Booster;

namespace wpf_dota2booster {
  internal class Dota2Classes {
    public enum Items {
      ObserverWard = 42,
      SentryWard = 43,
      ShadowBlade = 152,
      GemOfTrueSight = 30,
      BlackKingBar = 115,
      Bottle = 41,
      BlinkDagger = 1,
      DivineRapier = 133,
      SmokeOfDeceit = 188,
      DustOfAppearance = 40
    }

    public class EntityList : MemoryObject {
      public EntityList(ProcessMemory memory, IntPtr address)
        : base(memory, address) {
      }

      public Item GetItemById(int entityId) {
        IntPtr item = memory.ReadInt32Ptr(baseAddress + entityId*16);
        if (item == IntPtr.Zero) return null;
        return new Item(memory, item);
      }

      public Hero GetHeroById(int entityId) {
        IntPtr hero = memory.ReadInt32Ptr(baseAddress + entityId * 16);
        if (hero == IntPtr.Zero) return null;
        return new Hero(memory, hero);
      }
    }

    public class Inventory : MemoryObject
    {
      public Inventory(ProcessMemory memory, IntPtr address)
        : base(memory, address)
      {
      }

      public int GetItemIdByIndex(int index) {
        return memory.ReadInt16(baseAddress + 0x18 + index*4);
      }
    }

    public class Hero : MemoryObject {
      public Hero(ProcessMemory memory, IntPtr address)
        : base(memory, address) {
      }

      public bool SeenByEnemy {
        get { return memory.ReadBytes(baseAddress + 0x12E0, 1)[0] == 30; }
      }

      public Inventory Inventory
      {
        get { return new Inventory(memory, baseAddress + 0x2aa8); }
      }

      public float Mana {
        get { return memory.ReadFloat(baseAddress + 0x1134); }
      }

      public float MaxMana {
        get { return memory.ReadFloat(baseAddress + 0x1138); }
      }

      public int Health {
        get { return memory.ReadInt32(baseAddress + 0xfc); }
      }

      public int MaxHealth {
        get { return memory.ReadInt32(baseAddress + 0x110c); }
      }

      public int Level {
        get { return memory.ReadInt32(baseAddress + 0x10fc); }
      }

      public int DamageMax {
        get { return memory.ReadInt32(baseAddress + 0x12d8); }
      }
    }

    public class Item : MemoryObject {
      public Item(ProcessMemory memory, IntPtr address) : base(memory, address) {
      }

      public int Charges {
        get { return memory.ReadInt32(baseAddress + 0x7e4); }
      }

      public Items ItemId {
        get {
          IntPtr itemInfo = memory.ReadInt32Ptr(baseAddress + 0x778);
          return (Items) memory.ReadInt32(itemInfo + 0x3c);
        }
      }
    }

    public class MemoryObject {
      protected IntPtr baseAddress;
      protected ProcessMemory memory;

      public MemoryObject(ProcessMemory memory, IntPtr address) {
        if (memory == null) throw new ArgumentNullException("memory");
        if (address == IntPtr.Zero) throw new ArgumentException("address");
        this.memory = memory;
        baseAddress = address;
      }
    }

    public class Player : MemoryObject {
      public Player(ProcessMemory memory, IntPtr address)
        : base(memory, address) {
      }

      public int PlayerId {
        get { return memory.ReadInt32(baseAddress + 0x1A14); }
      }

      public int EntityId {
        get { return memory.ReadInt16(baseAddress + 0x19A4); }
      }
    }

    public class PlayerList : MemoryObject {
      public PlayerList(ProcessMemory memory, IntPtr address)
        : base(memory, address) {
      }

      public Player GetPlayerById(int playerId) {
        IntPtr player = memory.ReadInt32Ptr(baseAddress + playerId*16);
        if (player == IntPtr.Zero) return null;
        return new Player(memory, player);
      }
    }

    public class PlayerResources : MemoryObject {
      public PlayerResources(ProcessMemory memory, IntPtr address)
        : base(memory, address) {
      }

      public bool IsValidPlayer(int playerId) {
        IntPtr playerName = memory.ReadInt32Ptr(baseAddress + 0x5BC0 + 4*playerId);
        if (playerName == IntPtr.Zero) return false;
        return true;
      }

      public int GetGold(int playerId) {
        return memory.ReadInt32(baseAddress + 0x4c80 + 4*playerId) + memory.ReadInt32(baseAddress + 0x4d00 + 4*playerId);
      }

      public string GetPlayerName(int playerId) {
        IntPtr playerName = memory.ReadInt32Ptr(baseAddress + 0x5BC0 + 4*playerId);
        if (playerName == IntPtr.Zero) return "";
        return memory.ReadCString(playerName);
      }
    }
  }
}