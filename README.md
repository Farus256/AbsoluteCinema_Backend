#  AbsoluteCinema

Система управління кінотеатром з повним функціоналом для бронювання квитків, управління фільмами та сеансами, реалізована на сучасному технологічному стекі.

## Зміст

- [Опис проєкту](#опис-проєкту)
- [Технології](#технології)
- [Функціонал](#функціонал)
- [Архітектура](#архітектура)
- [Вимоги](#вимоги)
- [Інструкція до запуску](#інструкція-до-запуску)
- [Налаштування](#налаштування)
- [API Документація](#api-документація)
- [Особливості реалізації](#особливості-реалізації)

## Опис проєкту

AbsoluteCinema — це веб-додаток для управління кінотеатром, який дозволяє користувачам:
- Переглядати каталог фільмів з інформацією з The Movie Database (TMDB)
- Бронювати квитки на сеанси в реальному часі
- Управляти своїм профілем та квитками
- Адміністраторам — повний контроль над системою

Додаток використовує кілька протоколів комунікації: REST API, GraphQL, gRPC та SignalR для real-time оновлень.

## Технології

### Backend
- **.NET 8.0** — основна платформа
- **ASP.NET Core Web API** — REST API сервер
- **Entity Framework Core 9.0** — ORM для роботи з базою даних
- **PostgreSQL** — реляційна база даних
- **ASP.NET Core Identity** — система аутентифікації та авторизації
- **JWT Bearer Authentication** — токени для безпеки
- **SignalR 1.2.0** — real-time комунікація через WebSocket
- **HotChocolate GraphQL 14.0.0** — GraphQL API
- **gRPC 2.65.0** — високопродуктивна RPC комунікація
- **AutoMapper 13.0.1** — маппінг об'єктів
- **FluentValidation** — валідація даних
- **Swagger/OpenAPI** — документація API

### Frontend
- **React 18.3.1** — UI бібліотека
- **Vite 6.1.0** — збірщик та dev-сервер
- **React Router DOM 7.1.3** — маршрутизація
- **Bootstrap 5.3.3** — стилізація
- **Apollo Client 4.0.11** — GraphQL клієнт
- **@microsoft/signalr 8.0.0** — SignalR клієнт
- **@connectrpc/connect-web 1.7.0** — gRPC-Web клієнт
- **jwt-decode 4.0.0** — декодування JWT токенів
- **Swiper 11.2.4** — слайдери

### Інші інструменти
- **The Movie Database (TMDB) API** — інтеграція для отримання даних про фільми
- **ESLint** — лінтер для JavaScript/React

## Функціонал

### Для користувачів
- Реєстрація та авторизація (JWT токени)
- Перегляд каталогу фільмів з фільтрацією та пошуком
- Детальна інформація про фільми (актори, жанри, опис, трейлери)
- Бронювання квитків на сеанси з real-time оновленнями доступності місць
- Перегляд своїх квитків
- Редагування профілю
- Персоналізовані рекомендації фільмів

### Для адміністраторів
- CRUD операції для фільмів, акторів, жанрів, залів, сеансів
- Управління користувачами
- Статистика системи (через SignalR)
- Моніторинг сервера в реальному часі

### Технічні можливості
- REST API з повним CRUD функціоналом
- GraphQL API для гнучких запитів
- gRPC для високопродуктивної комунікації
- SignalR Hub для real-time оновлень (бронювання місць, чат, статистика)
- Автоматичне заповнення бази даних з TMDB при запуску
- Seeders для початкових даних (ролі, статуси квитків, зали)
- Валідація даних на рівні сервісів
- Обробка помилок через фільтри
- CORS налаштування для безпеки

## Архітектура

Проєкт використовує **Clean Architecture** з розділенням на шари:

```
AbsoluteCinema_Backend/
├── AbsoluteCinema.Domain/          # Доменна модель, сутності, інтерфейси
├── AbsoluteCinema.Application/     # Бізнес-логіка, сервіси, DTO, валідатори
├── AbsoluteCinema.Infrastructure/  # Реалізація: EF Core, репозиторії, Identity
└── AbsoluteCinema.WebAPI/          # Контролери, GraphQL, SignalR, gRPC
```

**Frontend:**
```
AbsoluteCinema_Frontend/
└── AbsoluteCinema_Frontend/
    └── src/
        ├── components/     # React компоненти
        ├── pages/          # Сторінки додатку
        ├── routing/        # Маршрутизація
        └── services/       # API клієнти
```

## Вимоги

### Системні вимоги
- **.NET 8.0 SDK** або новіша версія
- **Node.js 18+** та **npm**
- **PostgreSQL 12+**
- **Git**

### Зовнішні сервіси
- **The Movie Database (TMDB) API Key** — для отримання даних про фільми
  - Отримати можна на [https://www.themoviedb.org/settings/api](https://www.themoviedb.org/settings/api)

## Інструкція до запуску

### 1. Клонування репозиторію

```bash
git clone <repository-url>
cd AbsoluteCinema
```

### 2. Налаштування бази даних PostgreSQL

1. Встановіть PostgreSQL, якщо ще не встановлено
2. Створіть базу даних:
```sql
CREATE DATABASE postgres;
```
3. Запам'ятайте дані для підключення (host, port, username, password)

### 3. Налаштування Backend

1. Перейдіть до директорії Backend:
```bash
cd AbsoluteCinema/AbsoluteCinema_Backend
```

2. Відкрийте файл `AbsoluteCinema.WebAPI/appsettings.json` та налаштуйте:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=ВАШ_ПАРОЛЬ"
  },
  "ClientAddress": "http://localhost:5173",
  "Jwt": {
    "Key": "ВАШ_СЕКРЕТНИЙ_КЛЮЧ_ДЛЯ_JWT",
    "TokenIssuer": "AbsoluteCinema.watch",
    "TokenLifeMinutes": "15"
  },
  "TheMovieDatabase": {
    "ApiKey": "ВАШ_TMDB_API_KEY",
    "BaseUrl": "https://api.themoviedb.org/3"
  }
}
```

3. Відновіть залежності та запустіть міграції:
```bash
dotnet restore
dotnet ef database update --project AbsoluteCinema.Infrastructure --startup-project AbsoluteCinema.WebAPI
```

4. Запустіть Backend:
```bash
dotnet run --project AbsoluteCinema.WebAPI
```

Backend буде доступний на `http://localhost:5000` (або іншому порту, вказаному в launchSettings.json)

**Примітка:** При першому запуску автоматично виконуються seeders:
- Створення ролей (Admin, User)
- Створення статусів квитків
- Створення залів
- Завантаження фільмів з TMDB
- Створення тестових сеансів

### 4. Налаштування Frontend

1. Відкрийте новий термінал та перейдіть до директорії Frontend:
```bash
cd AbsoluteCinema/AbsoluteCinema_Frontend/AbsoluteCinema_Frontend
```

2. Встановіть залежності:
```bash
npm install
```

3. Переконайтеся, що в `appsettings.json` Backend вказано правильну адресу клієнта (`ClientAddress`)

4. Запустіть Frontend:
```bash
npm run dev
```

Frontend буде доступний на `http://localhost:5173`

### 5. Перевірка роботи

1. Відкрийте браузер та перейдіть на `http://localhost:5173`
2. Swagger документація доступна на `http://localhost:5000/swagger` (якщо запущено в режимі Development)
3. GraphQL playground доступний на `http://localhost:5000/graphql`

## Налаштування

### Зміна портів

**Backend:** Відредагуйте `Properties/launchSettings.json` в проєкті `AbsoluteCinema.WebAPI`

**Frontend:** Відредагуйте `vite.config.js` або використайте змінну середовища `PORT`

### Налаштування CORS

CORS налаштований в `Program.cs` Backend. За замовчуванням дозволено підключення з `http://localhost:5173`. Для зміни відредагуйте `ClientAddress` в `appsettings.json`.

### JWT Налаштування

- `TokenLifeMinutes` — час життя токену в хвилинах (за замовчуванням 15)
- `Key` — секретний ключ для підпису токенів (має бути достатньо довгим та безпечним)

## API Документація

### REST API

Після запуску Backend, Swagger документація доступна за адресою:
```
http://localhost:5000/swagger
```

**Основні endpoints:**

- `GET /api/Movie` — отримати всі фільми
- `GET /api/Movie/{id}` — отримати фільм за ID
- `POST /api/Auth/SignUp` — реєстрація
- `POST /api/Auth/SignIn` — авторизація
- `GET /api/Session` — отримати сеанси
- `POST /api/Ticket` — створити квиток
- `GET /api/Ticket/user/{userId}` — квитки користувача

### GraphQL

GraphQL endpoint: `http://localhost:5000/graphql`

Приклад запиту:
```graphql
query {
  movies {
    id
    title
    score
    releaseDate
  }
}
```

### gRPC

gRPC сервіси доступні через gRPC-Web. Протобуф файли знаходяться в `AbsoluteCinema.WebAPI/Protos/`

### SignalR

SignalR Hub endpoint: `/realtimehub`

**Методи:**
- `Ping()` — перевірка з'єднання
- `JoinSessionGroup(sessionId)` — приєднатися до групи сесії
- `GetBookedSeats(sessionId)` — отримати зайняті місця
- `CheckSeatAvailability(sessionId, row, place)` — перевірити доступність місця
- `GetServerStatistics()` — статистика сервера

**Події:**
- `SeatBooked` — місце заброньовано
- `SeatReleased` — місце звільнено
- `BookedSeatsLoaded` — завантажені зайняті місця

## Особливості реалізації

### Real-time бронювання

Система використовує SignalR для синхронізації стану місць між клієнтами:
- При бронюванні квитка всі клієнти в групі сесії отримують оновлення
- Перевірка доступності місця відбувається в реальному часі
- Автоматичне перепідключення при втраті з'єднання

### Стратегії завантаження даних

Використовується патерн Strategy для різних способів завантаження фільмів:
- Завантаження з включенням пов'язаних сутностей
- Фільтрація за жанрами, акторами
- Сортування та пагінація

### Unit of Work

Використовується патерн Unit of Work для управління транзакціями:
- Всі зміни зберігаються одним запитом до БД
- Забезпечує атомарність операцій

### Валідація

FluentValidation використовується для валідації DTO на рівні Application:
- Автоматична валідація при отриманні запитів
- Детальні повідомлення про помилки

### Seeders

При першому запуску автоматично виконуються:
- `RoleSeeder` — створює ролі (Admin, User)
- `TicketStatusSeeder` — створює статуси квитків
- `HallSeeder` — створює зали кінотеатру
- `TmdbSeeder` — завантажує фільми з TMDB API
- `SessionSeeder` — створює тестові сеанси

**Важливо:** `TmdbSeeder` має виконатися ПЕРЕД `SessionSeeder`, оскільки сеанси посилаються на фільми.

## Вирішення проблем

### Помилка підключення до БД

Перевірте:
- Чи запущено PostgreSQL
- Чи правильні дані в `ConnectionStrings` в `appsettings.json`
- Чи створено базу даних

### CORS помилки

Переконайтеся, що:
- `ClientAddress` в `appsettings.json` відповідає адресі Frontend
- CORS middleware додано в правильному порядку в `Program.cs`

### Помилки при завантаженні з TMDB

Перевірте:
- Чи валідний API ключ TMDB
- Чи є інтернет-з'єднання
- Чи доступний TMDB API

### Помилки міграцій

Якщо міграції не застосовуються:
```bash
dotnet ef migrations remove --project AbsoluteCinema.Infrastructure --startup-project AbsoluteCinema.WebAPI
dotnet ef database update --project AbsoluteCinema.Infrastructure --startup-project AbsoluteCinema.WebAPI
```


<h1 align="center">Database Schema</h1>

![image](https://github.com/user-attachments/assets/2f7a09db-3cf9-4b0b-b486-e581c637a979)
