using System;

namespace UniversityManagementSystem.Application.Exceptions
{
    public class CustomException : Exception
    {
        public CustomException(string message) : base(message) { }
        public CustomException(string message, Exception inner) : base(message, inner) { }
    }

    public class NotFoundException : CustomException
    {
        public NotFoundException(string entityName, int id) : base($"{entityName} with ID {id} was not found.") { }
    }

    public class DuplicateException : CustomException
    {
        public DuplicateException(string message) : base(message) { }
    }

    public class UnauthorizedAccessException : CustomException
    {
        public UnauthorizedAccessException(string message) : base(message) { }
    }
}