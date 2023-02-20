﻿namespace Mango.Services.OrderAPI.Messaging
{
    public interface IRabbitMQConnectionSettings
    {
        public string HostName { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
    }
}