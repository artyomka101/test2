CREATE TABLE IF NOT EXISTS users (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    username VARCHAR(50) UNIQUE NOT NULL,
    password VARCHAR(100) NOT NULL,
    full_name VARCHAR(100) NOT NULL,
    group_name VARCHAR(20) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS subjects (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name VARCHAR(100) NOT NULL,
    credits INTEGER NOT NULL DEFAULT 3,
    semester INTEGER NOT NULL,
    teacher VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS grades (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    user_id INTEGER NOT NULL,
    subject_id INTEGER NOT NULL,
    grade INTEGER NOT NULL CHECK (grade >= 2 AND grade <= 5),
    grade_type VARCHAR(20) NOT NULL DEFAULT 'exam',
    date_received DATE NOT NULL DEFAULT CURRENT_DATE,
    FOREIGN KEY (user_id) REFERENCES users(id) ON DELETE CASCADE,
    FOREIGN KEY (subject_id) REFERENCES subjects(id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_grades_user_id ON grades(user_id);
CREATE INDEX IF NOT EXISTS idx_grades_subject_id ON grades(subject_id);
CREATE INDEX IF NOT EXISTS idx_grades_date ON grades(date_received);
CREATE INDEX IF NOT EXISTS idx_subjects_semester ON subjects(semester);

INSERT OR IGNORE INTO subjects (name, credits, semester, teacher) VALUES
('Математический анализ', 4, 1, 'Иванов И.И.'),
('Программирование', 3, 1, 'Петров П.П.'),
('Физика', 3, 1, 'Сидоров С.С.'),
('Базы данных', 4, 2, 'Козлов К.К.'),
('Алгоритмы и структуры данных', 3, 2, 'Новиков Н.Н.');

INSERT OR IGNORE INTO users (username, password, full_name, group_name) VALUES
('student1', '123456', 'Иванов Иван Иванович', 'ИТ-21'),
('student2', '123456', 'Петрова Мария Сергеевна', 'ИТ-21'),
('student3', '123456', 'Сидоров Алексей Петрович', 'ИТ-22');

INSERT OR IGNORE INTO grades (user_id, subject_id, grade, grade_type, date_received) VALUES
(1, 1, 5, 'exam', '2024-01-15'),
(1, 2, 4, 'test', '2024-01-20'),
(1, 3, 5, 'homework', '2024-01-25'),
(2, 1, 4, 'exam', '2024-01-15'),
(2, 2, 5, 'test', '2024-01-20'),
(3, 1, 3, 'exam', '2024-01-15'),
(3, 2, 4, 'test', '2024-01-20');