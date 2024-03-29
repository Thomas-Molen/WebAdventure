﻿using System;
using System.Collections.Generic;
using textadventure_backend.Models.Entities;
using textadventure_backend.Models.Session;

namespace textadventure_backend.Services
{
    public interface ISessionManager
    {
        void AddSession(string connectionId, SessionAdventurer adventurer, string group);
        Session GetSession(string connectionId);
        void RemoveSession(string connectionId);
        void UpdateSessionRoom(string connectionId, Rooms room);
    }

    public class SessionManager : ISessionManager
    {
        private readonly List<Session> Sessions;

        public SessionManager()
        {
            Sessions = new List<Session>();
        }

        public void AddSession(string connectionId, SessionAdventurer adventurer, string group)
        {
            Session sessionToAdd = new Session
            {
                Adventurer = adventurer,
                ConnectionId = connectionId,
                Group = group
            };
            Sessions.Add(sessionToAdd);
        }

        public void RemoveSession(string connectionId)
        {
            var sessionToRemove = GetSessionFromConnectionId(connectionId);
            Sessions.Remove(sessionToRemove);
        }

        public Session GetSession(string connectionId)
        {
            var sessionToGet = GetSessionFromConnectionId(connectionId);
            return sessionToGet;
        }

        public void UpdateSessionRoom(string connectionId, Rooms room)
        {
            var sessionToUpdate = GetSessionFromConnectionId(connectionId);
            sessionToUpdate.Room = new SessionRoom
            {
                Id = room.Id,
                Event = room.Event,
                EventCompleted = room.EventCompleted,
                NorthInteraction = room.NorthInteraction,
                EastInteraction = room.EastInteraction,
                SouthInteraction = room.SouthInteraction,
                WestInteraction = room.WestInteraction
            };
        }

        private Session GetSessionFromConnectionId(string connectionId)
        {
            var session = Sessions.Find(s => s.ConnectionId == connectionId);
            if (session == null)
            {
                throw new ArgumentException("No active adventurer connected to given connectionId");
            }
            return session;
        }
    }
}
