using System;
using System.Collections.Generic;

namespace UniversitySystem.Models
{
    /// Клас, що описує академічну групу університету та керує списком її студентів.
    public class Group
    {
        // Автоматична властивість для збереження унікального коду групи (наприклад, Б-121).
        public string GroupCode { get; private set; }

        // Узагальнена колекція (List), яка зберігає об'єкти типу Student.
        public List<Student> Students { get; set; } = new List<Student>();

        /// Конструктор класу Group. Створює новий екземпляр групи та валідує її код.
        /// <param name="groupCode">Код або назва академічної групи.</param>
        public Group(string groupCode)
        {
            // Перевірка надійності даних: якщо рядок порожній, складається з пробілів або дорівнює null,
            // генерується системний виняток ArgumentException, який зупиняє створення некоректного об'єкта.
            if (string.IsNullOrWhiteSpace(groupCode))
                throw new ArgumentException("Код групи не може бути порожнім.");

            // Приведення коду групи до верхнього регістру (наприклад, "кн-21" автоматично стане "КН-21").
            // Це потрібно для уніфікації даних та полегшення подальшого пошуку чи сортування.
            GroupCode = groupCode.ToUpper();
        }

        /// Метод для безпечного додавання студента до поточного списку групи.
        /// <param name="student">Екземпляр класу Student, якого потрібно зарахувати.</param>
        public void AddStudent(Student student)
        {
            // Захист від критичної помилки: якщо замість об'єкта студента передали null reference (порожнечу),
            // програма викидає виняток ArgumentNullException, вказуючи назву некоректного параметра.
            if (student == null) throw new ArgumentNullException(nameof(student));

            // Якщо перевірку пройдено — додаємо студента до динамічного списку колекції.
            Students.Add(student);
        }
    }
}