using System;

namespace UniversitySystem.Exceptions
{
    // Власний користувацький виняток для обробки помилок валідації (Демонстрація ООП успадкування)
    public class ValidationException : Exception
    {
        public ValidationException(string message) : base($"[Помилка Валідації]: {message}")
        {
        }
    }
}