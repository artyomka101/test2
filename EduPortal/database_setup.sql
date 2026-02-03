-- Создание базы данных EduPortal для PostgreSQL
-- Выполните эти команды в pgAdmin

-- 1. Создайте базу данных EduPortal
-- CREATE DATABASE "EduPortal";

-- 2. Подключитесь к базе данных EduPortal и выполните следующие команды:

-- Таблица преподавателей
CREATE TABLE Teachers (
    teacher_id SERIAL PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    subject VARCHAR(100) NOT NULL,
    email VARCHAR(100),
    phone VARCHAR(20)
);

-- Таблица курсов
CREATE TABLE Courses (
    course_id SERIAL PRIMARY KEY,
    course_name VARCHAR(100) NOT NULL,
    duration INTEGER NOT NULL, -- продолжительность в часах
    teacher_id INTEGER NOT NULL,
    credits INTEGER DEFAULT 3,
    semester INTEGER NOT NULL,
    description TEXT,
    FOREIGN KEY (teacher_id) REFERENCES Teachers(teacher_id) ON DELETE CASCADE
);

-- Таблица студентов
CREATE TABLE Students (
    student_id SERIAL PRIMARY KEY,
    first_name VARCHAR(50) NOT NULL,
    last_name VARCHAR(50) NOT NULL,
    email VARCHAR(100) UNIQUE NOT NULL,
    course_id INTEGER,
    group_name VARCHAR(20),
    enrollment_date DATE DEFAULT CURRENT_DATE,
    FOREIGN KEY (course_id) REFERENCES Courses(course_id) ON DELETE SET NULL
);

-- Таблица для связи многие-ко-многим между студентами и курсами
CREATE TABLE Student_Courses (
    id SERIAL PRIMARY KEY,
    student_id INTEGER NOT NULL,
    course_id INTEGER NOT NULL,
    enrollment_date DATE DEFAULT CURRENT_DATE,
    grade INTEGER CHECK (grade >= 2 AND grade <= 5),
    grade_type VARCHAR(20) DEFAULT 'exam',
    FOREIGN KEY (student_id) REFERENCES Students(student_id) ON DELETE CASCADE,
    FOREIGN KEY (course_id) REFERENCES Courses(course_id) ON DELETE CASCADE,
    UNIQUE(student_id, course_id)
);

-- Таблица для учебных материалов
CREATE TABLE Course_Materials (
    material_id SERIAL PRIMARY KEY,
    course_id INTEGER NOT NULL,
    material_name VARCHAR(200) NOT NULL,
    file_path VARCHAR(500),
    upload_date DATE DEFAULT CURRENT_DATE,
    file_size INTEGER,
    FOREIGN KEY (course_id) REFERENCES Courses(course_id) ON DELETE CASCADE
);

-- Индексы для оптимизации
CREATE INDEX idx_courses_teacher_id ON Courses(teacher_id);
CREATE INDEX idx_students_course_id ON Students(course_id);
CREATE INDEX idx_student_courses_student_id ON Student_Courses(student_id);
CREATE INDEX idx_student_courses_course_id ON Student_Courses(course_id);
CREATE INDEX idx_course_materials_course_id ON Course_Materials(course_id);

-- Тестовые данные
INSERT INTO Teachers (first_name, last_name, subject, email) VALUES
('Иван', 'Иванов', 'Математический анализ', 'ivanov@edu.ru'),
('Петр', 'Петров', 'Программирование', 'petrov@edu.ru'),
('Сергей', 'Сидоров', 'Физика', 'sidorov@edu.ru'),
('Константин', 'Козлов', 'Базы данных', 'kozlov@edu.ru'),
('Николай', 'Новиков', 'Алгоритмы и структуры данных', 'novikov@edu.ru');

INSERT INTO Courses (course_name, duration, teacher_id, credits, semester, description) VALUES
('Математический анализ', 120, 1, 4, 1, 'Основы математического анализа'),
('Программирование на C#', 90, 2, 3, 1, 'Изучение языка программирования C#'),
('Общая физика', 80, 3, 3, 1, 'Курс общей физики'),
('Базы данных', 100, 4, 4, 2, 'Проектирование и работа с базами данных'),
('Алгоритмы и структуры данных', 85, 5, 3, 2, 'Изучение алгоритмов и структур данных');

INSERT INTO Students (first_name, last_name, email, group_name) VALUES
('Иван', 'Иванов', 'student1@edu.ru', 'ИТ-21'),
('Мария', 'Петрова', 'student2@edu.ru', 'ИТ-21'),
('Алексей', 'Сидоров', 'student3@edu.ru', 'ИТ-22'),
('Елена', 'Козлова', 'student4@edu.ru', 'ИТ-22'),
('Дмитрий', 'Новиков', 'student5@edu.ru', 'ИТ-21');

-- Записываем студентов на курсы
INSERT INTO Student_Courses (student_id, course_id, grade, grade_type) VALUES
(1, 1, 5, 'exam'),
(1, 2, 4, 'test'),
(1, 3, 5, 'homework'),
(2, 1, 4, 'exam'),
(2, 2, 5, 'test'),
(3, 1, 3, 'exam'),
(3, 2, 4, 'test'),
(4, 4, 5, 'exam'),
(5, 5, 4, 'test');

-- Добавляем учебные материалы
INSERT INTO Course_Materials (course_id, material_name, file_path) VALUES
(1, 'Лекции по математическому анализу', '/materials/math_lectures.pdf'),
(2, 'Примеры кода C#', '/materials/csharp_examples.zip'),
(3, 'Лабораторные работы по физике', '/materials/physics_labs.pdf'),
(4, 'Руководство по SQL', '/materials/sql_guide.pdf'),
(5, 'Алгоритмы сортировки', '/materials/sorting_algorithms.pdf');