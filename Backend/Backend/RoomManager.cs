using System.Collections.Generic;
using System.Linq;
using DarkRift.Server;
using GameModels;

namespace Backend
{
    public sealed class RoomManager
    {
        private readonly Dictionary<uint, Room> _rooms = new Dictionary<uint, Room>();
        private readonly Dictionary<IClient, HashSet<uint>> _clientsRooms = new Dictionary<IClient, HashSet<uint>>();

        private const int ClientMaxRooms = 10;

        public Room GetRoom(uint id)
        {
            if (!_rooms.TryGetValue(id, out var room))
            {
                room = new Room(id);
                _rooms.Add(id, room);
            }

            return room;
        }

        public bool ClientJoinRoom(IClient client, RoomJoin roomJoin)
        {
            if (!_clientsRooms.TryGetValue(client, out var clientRooms))
            {
                clientRooms = new HashSet<uint>();
                _clientsRooms.Add(client, clientRooms);
            }

            if (clientRooms.Count >= ClientMaxRooms)
                return false;

            var room = GetRoom(roomJoin.RoomId);
            clientRooms.Add(room.RoomId);
            if (!room.Clients.Add(client))
                return false;

            client.SendMessage(room.Objects.Select(o => new ObjectInit
            {
                Id = o.Id,
                Location = o.Location,
                Type = o.Type,
                RoomId = o.Room.RoomId,
                OwnerId = o.Owner.ID,
            }).Package(), ObjectInit.StaticSendMode);

            return true;
        }

        public bool ClientLeaveRoom(IClient client, RoomLeave roomLeave)
        {
            if (!_clientsRooms.TryGetValue(client, out var clientRooms))
                return false;

            if (!clientRooms.Remove(roomLeave.RoomId))
                return false;

            var room = GetRoom(roomLeave.RoomId);
            client.SendMessage(room.Objects.Select(o => new ObjectRemove
            {
                Id = o.Id,
            }).Package(), ObjectRemove.StaticSendMode);

            return true;
        }

        public bool InitObjectInRoom(IClient client, ObjectInit init, NetworkObject networkObject)
        {
            if (!_clientsRooms.TryGetValue(client, out var clientRooms) || !clientRooms.Contains(init.RoomId))
                return false;

            var room = GetRoom(init.RoomId);
            room.Objects.Add(networkObject);
            networkObject.Room = room;

            room.SendMessageToAll(init.Package(), init.SendMode);

            return true;
        }

        public bool TransferObjectToRoom(IClient client, ObjectTransfer transfer, NetworkObject networkObject)
        {
            if (networkObject.Room.RoomId == transfer.RoomId)
                return false;

            if (!_clientsRooms.TryGetValue(client, out var clientRooms) || !clientRooms.Contains(transfer.RoomId))
                return false;

            var oldRoom = networkObject.Room;
            oldRoom.Objects.Remove(networkObject);

            var room = GetRoom(transfer.RoomId);
            room.Objects.Add(networkObject);
            networkObject.Room = room;

            var message = transfer.Package();
            oldRoom.SendMessageToAllExcept(message, ObjectTransfer.StaticSendMode, client);
            room.SendMessageToAllExcept(message, ObjectTransfer.StaticSendMode, oldRoom.Clients);

            return true;
        }

        public bool RemoveObjectFromRoom(IClient client, ObjectRemove remove, NetworkObject networkObject)
        {
            var room = networkObject.Room;
            room.Objects.Remove(networkObject);
            networkObject.Room = null;

            room.SendMessageToAllExcept(remove.Package(), remove.SendMode, client);

            return true;
        }
    }
}
