using System;

namespace WebApi.Models
{
    public class InfoResponse
    {
        public Guid Id { get; }

        public InfoResponse(Guid id)
        {
            Id = id;
        }
    }
}