using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LapLich.Models
{
    public class Room
    {
        public int RoomID { get; set; }
        public string RoomName { get; set; }

        public Room(int roomID, string roomName)
        {
            RoomID = roomID;
            RoomName = roomName;
        }

        public override string ToString()
        {
            return $"RoomID: {RoomID}, RoomName: {RoomName}";
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Room))
                return false;

            Room otherRoom = (Room)obj;
            return RoomID == otherRoom.RoomID;
        }

        public override int GetHashCode()
        {
            return RoomID.GetHashCode();
        }
    }
}