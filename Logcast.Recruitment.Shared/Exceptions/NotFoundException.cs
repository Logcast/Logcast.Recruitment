using System;

namespace Logcast.Recruitment.Shared.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string s):base(s)
        {
        }
    }
}