-- Создание таблицы employees
CREATE TABLE employees (
    employee_id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    department VARCHAR(100),
    balance NUMERIC(15,2) DEFAULT 0
);

-- Создание таблицы advances
CREATE TABLE advances (
    id SERIAL PRIMARY KEY,
    employee_id INT NOT NULL,
    date DATE NOT NULL,
    sum NUMERIC(15,2) NOT NULL,
    reported BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (employee_id) REFERENCES employees(employee_id)
);

-- Создание таблицы reports (сначала без внешнего ключа)
CREATE TABLE reports (
    id SERIAL PRIMARY KEY,
    advance_id INT NOT NULL,
    date DATE NOT NULL,
    sum NUMERIC(15,2) NOT NULL DEFAULT 0
);

-- Создание таблицы expenses
CREATE TABLE expenses (
    id SERIAL PRIMARY KEY,
    report_id INT NOT NULL,
    category VARCHAR(100) NOT NULL,
    quantity NUMERIC(10,2) NOT NULL,
    sum NUMERIC(15,2) NOT NULL,
    FOREIGN KEY (report_id) REFERENCES reports(id)
);

-- Создание таблицы balances
CREATE TABLE balances (
    id SERIAL PRIMARY KEY,
    employee_id INT NOT NULL,
    month CHAR(7) NOT NULL,
    end_balance NUMERIC(15,2) NOT NULL,
    FOREIGN KEY (employee_id) REFERENCES employees(employee_id)
);

-- Функция для обновления суммы в отчете
CREATE OR REPLACE FUNCTION update_report_sum()
RETURNS TRIGGER AS $$
BEGIN
    UPDATE reports 
    SET sum = (
        SELECT COALESCE(SUM(sum), 0)
        FROM expenses
        WHERE report_id = COALESCE(NEW.report_id, OLD.report_id)
    )
    WHERE id = COALESCE(NEW.report_id, OLD.report_id);
    
    -- Обновляем статус reported в таблице advances
    UPDATE advances a
    SET reported = EXISTS (
        SELECT 1 FROM reports r 
        WHERE r.advance_id = a.id AND r.sum > 0
    )
    WHERE a.id = (
        SELECT advance_id FROM reports 
        WHERE id = COALESCE(NEW.report_id, OLD.report_id)
    );
    
    RETURN NULL;
END;
$$ LANGUAGE plpgsql;

-- Триггеры для автоматического обновления суммы отчета
CREATE TRIGGER expenses_after_insert
AFTER INSERT ON expenses
FOR EACH ROW
EXECUTE FUNCTION update_report_sum();

CREATE TRIGGER expenses_after_update
AFTER UPDATE ON expenses
FOR EACH ROW
WHEN (OLD.sum <> NEW.sum OR OLD.report_id <> NEW.report_id)
EXECUTE FUNCTION update_report_sum();

CREATE TRIGGER expenses_after_delete
AFTER DELETE ON expenses
FOR EACH ROW
EXECUTE FUNCTION update_report_sum();

-- Теперь добавляем внешний ключ в reports
ALTER TABLE reports 
ADD CONSTRAINT reports_advance_id_fkey 
FOREIGN KEY (advance_id) REFERENCES advances(id);

-- Заполнение таблицы employees
INSERT INTO employees (name, department, balance) VALUES
('Иванов Иван Иванович', 'Отдел продаж', 25000.00),
('Петрова Светлана Викторовна', 'Бухгалтерия', 0.00),
('Сидоров Алексей Дмитриевич', 'ИТ-отдел', 15000.00),
('Кузнецова Елена Сергеевна', 'Отдел маркетинга', 5000.00),
('Васильев Дмитрий Петрович', 'Отдел логистики', 30000.00),
('Григорьев Николай Николаевич', 'ИТ-отдел', 12000.00),
('Мельникова Анастасия Павловна', 'Бухгалтерия', 7000.00),
('Тарасов Игорь Олегович', 'Отдел продаж', 16000.00),
('Егорова Марина Валерьевна', 'Отдел логистики', 21000.00),
('Зайцев Роман Андреевич', 'Отдел маркетинга', 4000.00);

-- Заполнение таблицы advances
INSERT INTO advances (employee_id, date, sum) VALUES
(1, '2025-05-10', 20000.00),
(1, '2025-05-20', 15000.00),
(2, '2025-05-15', 10000.00),
(3, '2025-05-05', 25000.00),
(4, '2025-05-12', 8000.00),
(5, '2025-05-18', 20000.00),
(6, '2025-05-05', 9000.00),
(6, '2025-05-10', 5000.00),
(7, '2025-05-12', 11000.00),
(8, '2025-05-14', 6000.00),
(9, '2025-05-15', 7000.00),
(10, '2025-05-20', 9500.00),
(3, '2025-05-22', 10500.00),
(2, '2025-05-24', 7500.00);

-- Заполнение таблицы reports (сумма будет 0, так как расходов еще нет)
INSERT INTO reports (advance_id, date) VALUES
(1, '2025-05-17'),
(3, '2025-05-22'),
(4, '2025-05-15'),
(6, '2025-05-25'),
(7, '2025-05-11'),
(8, '2025-05-13'),
(10, '2025-05-16'),
(11, '2025-05-18'),
(12, '2025-05-23'),
(14, '2025-05-25');

-- Заполнение таблицы expenses (сумма в reports обновится автоматически)
INSERT INTO expenses (report_id, category, quantity, sum) VALUES
(1, 'Командировочные расходы', 1, 12000.00),
(1, 'Транспорт', 4, 4500.00),
(1, 'Представительские расходы', 1, 2000.00),
(2, 'Офисные расходы', 1, 9500.00),
(3, 'Гостиница', 3, 15000.00),
(3, 'Питание', 5, 5000.00),
(3, 'Транспорт', 2, 3000.00),
(4, 'Логистика', 1, 15000.00),
(4, 'Упаковка', 100, 4500.00),
(5, 'Проживание', 2, 4000.00),
(5, 'Питание', 5, 2500.00),
(6, 'Офисные материалы', 10, 3000.00),
(6, 'Такси', 4, 2800.00),
(7, 'Командировочные', 1, 5800.00),
(8, 'Интернет', 1, 1000.00),
(8, 'Консультации', 2, 6000.00),
(9, 'Маркетинг', 1, 7000.00),
(10, 'Транспортировка', 3, 9300.00);

-- Заполнение таблицы balances
INSERT INTO balances (employee_id, month, end_balance) VALUES
(1, '2025-04', 5000.00),
(2, '2025-04', 0.00),
(3, '2025-04', 10000.00),
(4, '2025-04', 2000.00),
(5, '2025-04', 25000.00),
(1, '2025-05', 6500.00),
(2, '2025-05', 500.00),
(3, '2025-05', 2000.00),
(4, '2025-05', 5000.00),
(5, '2025-05', 30500.00),
(6, '2025-05', 3000.00),
(7, '2025-05', 4000.00),
(8, '2025-05', 8000.00),
(9, '2025-05', 7000.00),
(10, '2025-05', 3500.00);
