-- Основная информация о сотрудниках

CREATE TABLE employees (
    employee_id SERIAL PRIMARY KEY,
    name VARCHAR(150) NOT NULL,
    department VARCHAR(100),
    balance NUMERIC(15,2) DEFAULT 0
);

-- Учет выданных сумм

CREATE TABLE advances (
    id SERIAL PRIMARY KEY,
    employee_id INT NOT NULL,
    date DATE NOT NULL,
    sum NUMERIC(15,2) NOT NULL,
    reported BOOLEAN DEFAULT FALSE,
    FOREIGN KEY (employee_id) REFERENCES employees(employee_id)
);

-- Учет предоставленных отчетов

CREATE TABLE reports (
    id SERIAL PRIMARY KEY,
    advance_id INT NOT NULL,
    date DATE NOT NULL,
    sum NUMERIC(15,2) NOT NULL,
    FOREIGN KEY (advance_id) REFERENCES advances(id)
);

-- Детализация расходов по авансовому отчету

CREATE TABLE expenses (
    id SERIAL PRIMARY KEY,
    report_id INT NOT NULL,
    category VARCHAR(100) NOT NULL,
    quantity NUMERIC(10,2) NOT NULL, 
    sum NUMERIC(15,2) NOT NULL,
    FOREIGN KEY (report_id) REFERENCES reports(id)
);

-- История остатков для отчетности

CREATE TABLE balances (
    id SERIAL PRIMARY KEY,
    employee_id INT NOT NULL,
    month CHAR(7) NOT NULL, -- YYYY-MM
    end_balance NUMERIC(15,2) NOT NULL,
    FOREIGN KEY (employee_id) REFERENCES employees(employee_id)
);

-- Заполнение таблицы employees
INSERT INTO employees (name, department, balance) VALUES
('Иванов Иван Иванович', 'Отдел продаж', 25000.00),
('Петрова Светлана Викторовна', 'Бухгалтерия', 0.00),
('Сидоров Алексей Дмитриевич', 'ИТ-отдел', 15000.00),
('Кузнецова Елена Сергеевна', 'Отдел маркетинга', 5000.00),
('Васильев Дмитрий Петрович', 'Отдел логистики', 30000.00);

-- Дополнительные сотрудники
INSERT INTO employees (name, department, balance) VALUES
('Григорьев Николай Николаевич', 'ИТ-отдел', 12000.00),
('Мельникова Анастасия Павловна', 'Бухгалтерия', 7000.00),
('Тарасов Игорь Олегович', 'Отдел продаж', 16000.00),
('Егорова Марина Валерьевна', 'Отдел логистики', 21000.00),
('Зайцев Роман Андреевич', 'Отдел маркетинга', 4000.00);

-- Заполнение таблицы advances
INSERT INTO advances (employee_id, date, sum, reported) VALUES
(1, '2025-05-10', 20000.00, TRUE),
(1, '2025-05-20', 15000.00, FALSE),
(2, '2025-05-15', 10000.00, TRUE),
(3, '2025-05-05', 25000.00, TRUE),
(4, '2025-05-12', 8000.00, FALSE),
(5, '2025-05-18', 20000.00, TRUE);

-- Дополнительные авансы
INSERT INTO advances (employee_id, date, sum, reported) VALUES
(6, '2025-05-05', 9000.00, FALSE),
(6, '2025-05-10', 5000.00, TRUE),
(7, '2025-05-12', 11000.00, TRUE),
(8, '2025-05-14', 6000.00, FALSE),
(9, '2025-05-15', 7000.00, TRUE),
(10, '2025-05-20', 9500.00, TRUE),
(3, '2025-05-22', 10500.00, FALSE),
(2, '2025-05-24', 7500.00, TRUE);

-- Заполнение таблицы reports
INSERT INTO reports (advance_id, date, sum) VALUES
(1, '2024-05-17', 18500.00),
(3, '2024-05-22', 9500.00),
(4, '2024-05-15', 23000.00),
(6, '2024-05-25', 19500.00);

-- Дополнительные отчеты
INSERT INTO reports (advance_id, date, sum) VALUES
(7, '2025-05-11', 4900.00),
(8, '2025-05-13', 10000.00),
(10, '2025-05-16', 5800.00),
(11, '2025-05-18', 7000.00),
(12, '2025-05-23', 9300.00),
(14, '2025-05-25', 7400.00);

-- Заполнение таблицы expenses
INSERT INTO expenses (report_id, category, quantity, sum) VALUES
(1, 'Командировочные расходы', 1, 12000.00),
(1, 'Транспорт', 4, 4500.00),
(1, 'Представительские расходы', 1, 2000.00),
(2, 'Офисные расходы', 1, 9500.00),
(3, 'Гостиница', 3, 15000.00),
(3, 'Питание', 5, 5000.00),
(3, 'Транспорт', 2, 3000.00),
(4, 'Логистика', 1, 15000.00),
(4, 'Упаковка', 100, 4500.00);

-- Дополнительные расходы
INSERT INTO expenses (report_id, category, quantity, sum) VALUES
(5, 'Проживание', 2, 4000.00),
(5, 'Питание', 5, 2500.00),
(6, 'Офисные материалы', 10, 3000.00),
(6, 'Такси', 4, 2800.00),
(7, 'Командировочные', 1, 5800.00),
(8, 'Интернет', 1, 1000.00),
(8, 'Консультации', 2, 6000.00),
(9, 'Маркетинг', 1, 7000.00),
(10, 'Транспортировка', 3, 9300.00),
(11, 'Прочие расходы', 2, 7400.00);

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
(5, '2025-05', 30500.00);

-- Дополнительные остатки
INSERT INTO balances (employee_id, month, end_balance) VALUES
(6, '2025-05', 3000.00),
(7, '2025-05', 4000.00),
(8, '2025-05', 8000.00),
(9, '2025-05', 7000.00),
(10, '2025-05', 3500.00);
