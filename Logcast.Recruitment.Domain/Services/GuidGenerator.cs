using System;

namespace Logcast.Recruitment.Domain.Services
{
    public interface IIdGenerator
    {
        Guid NewId();
    }

    public class IdGenerator : IIdGenerator
    {
        public Guid NewId()
        {
            return Guid.NewGuid();
        }
    }
}
