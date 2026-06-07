using System;
using System.Text.RegularExpressions;
using UniversitySystem.Exceptions;

namespace UniversitySystem.Models
{
    /// Клас, що описує конкретне заняття (пару) у розкладі університету.

    public class ScheduleItem
    {
        // Закриті поля (backing fields) для безпечного збереження внутрішнього стану об'єкта.
        // Доступ до них можливий лише через відповідні публічні властивості.
        private string _classroom;
        private int _lessonNumber;
        private string _teacherName;

        // Автоматичні властивості для збереження прив'язки до групи та предмета.
        // Оскільки вони не мають додаткової логіки у set, C# сам створює для них приховані поля.
        public string GroupCode { get; set; }
        public string SubjectName { get; set; }

        /// Номер пари протягом навчального дня.
        /// Має вбудовану валідацію обмеження кількості пар
        public int LessonNumber
        {
            get => _lessonNumber;
            set
            {
                // Бізнес-правило: в університеті не може бути менше 1 або більше 4 пар на день.
                // Якщо умова порушена — викидається кастомний виняток валідації.
                if (value < 1 || value > 4)
                    throw new ValidationException("Номер пари повинен бути в межах від 1 до 4.");

                _lessonNumber = value;
            }
        }

        /// ПІБ викладача, який проводить заняття.
        public string TeacherName
        {
            get => _teacherName;
            set
            {
                // Перевірка на те, щоб поле не було пустим або заповненим одними лише пробілами.
                if (string.IsNullOrWhiteSpace(value))
                    throw new ValidationException("Ім'я викладача не може бути порожнім.");

                _teacherName = value;
            }
        }

        /// Аудиторія, де проходить заняття.
        /// Формат перевіряється за допомогою регулярного виразу (Regex).
        public string Classroom
        {
            get => _classroom;
            set
            {
                // Розбір регулярного виразу (шаблону):
                // ^ — початок рядка.
                // (1[0-2]|[1-9]) — шукає число від 1 до 12 (номери навчальних корпусів).
                // \. — екранована крапка (символ крапки у тексті).
                // \d{3} — рівно три цифри підряд (номер кабінету, наприклад 202 чи 005).
                // $ — кінець рядка (забороняє введення зайвих символів після номера).
                string pattern = @"^(1[0-2]|[1-9])\.\d{3}$";

                // Статичний метод Regex.IsMatch перевіряє, чи відповідає введений рядок нашому шаблону.
                if (!Regex.IsMatch(value, pattern))
                    throw new ValidationException("Некоректний формат аудиторії! Формат: [Корпус 1-12].[3 цифри]. Приклад: 6.202 або 1.001");

                _classroom = value;
            }
        }

        /// Конструктор класу ScheduleItem для ініціалізації розкладу.
        /// <param name="groupCode">Код академічної групи.</param>
        /// <param name="subjectName">Назва навчального предмета.</param>
        /// <param name="lessonNumber">Номер пари (від 1 до 4).</param>
        /// <param name="teacherName">ПІБ викладача.</param>
        /// <param name="classroom">Аудиторія у форматі Х.ХХХ.</param>
        public ScheduleItem(string groupCode, string subjectName, int lessonNumber, string teacherName, string classroom)
        {
            // Передача значень через публічні властивості. 
            // Завдяки цьому валідація у сетерах відпрацює навіть під час створення об'єкта через конструктор.
            GroupCode = groupCode;
            SubjectName = subjectName;
            LessonNumber = lessonNumber;
            TeacherName = teacherName;
            Classroom = classroom;
        }
    }
}