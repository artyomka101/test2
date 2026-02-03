-- Исправление типа поля duration в таблице Courses
-- Выполните эту команду в pgAdmin

-- Сначала проверим текущий тип поля
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'courses' AND column_name = 'duration';

-- Если duration имеет тип character varying (VARCHAR), то исправим его:
ALTER TABLE Courses ALTER COLUMN duration TYPE INTEGER USING duration::INTEGER;

-- Проверим результат
SELECT column_name, data_type 
FROM information_schema.columns 
WHERE table_name = 'courses' AND column_name = 'duration';