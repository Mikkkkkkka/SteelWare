# SteelWare

Backend для работы со складом рулонов металла на `ASP.NET Core Web API`.

## Реализовано

### Основные возможности

- добавление рулона на склад
- удаление рулона по `id` через soft delete
- получение списка рулонов
- получение статистики по складу

### Хранение данных

- данные хранятся в `PostgreSQL`
- используется `Entity Framework Core`
- схема БД создается автоматически при старте приложения

### Обработка ошибок

- валидация тела запроса и query-параметров
- `404 Not Found` при удалении несуществующего рулона
- `ProblemDetails` для HTTP-ошибок

### Выполненные бонусные пункты

- фильтрация списка рулонов работает по комбинации нескольких диапазонов сразу
- статистика дополнительно отдает:
  - день с минимальным количеством рулонов
  - день с максимальным количеством рулонов
  - день с минимальным суммарным весом
  - день с максимальным суммарным весом
- проект запускается в Docker
- конфигурация подключения к БД настраивается через `.env` или системные переменные окружения
- проект покрыт тестами без зависимости от реальной БД

## Стек

- `.NET 9`
- `ASP.NET Core`
- `Entity Framework Core`
- `PostgreSQL`
- `Swagger / OpenAPI`
- `xUnit`
- `Docker Compose`

## Конфигурация

Приложение загружает `.env` из текущей директории или родительских каталогов. Для локального запуска создай `.env` в корне проекта на основе `.env.example`.

Пример:

```env
ASPNETCORE_ENVIRONMENT=Development
POSTGRES_DB=steelware
POSTGRES_USER=postgres
POSTGRES_PASSWORD=postgres
POSTGRES_PORT=5432
STEELWARE_PORT=8080
ConnectionStrings__SteelWare=Host=localhost;Port=5432;Database=steelware;Username=postgres;Password=postgres
```

## Локальный запуск

1. Подними PostgreSQL.
2. Создай `.env` в корне проекта.
3. Запусти приложение:

```powershell
dotnet run --project src/SteelWare
```

Локальный профиль по умолчанию использует `http://localhost:5033`.

Swagger в `Development`:

- `http://localhost:5033/swagger`

## Запуск через Docker

1. Создай `.env` в корне проекта.
2. Выполни:

```powershell
docker compose up --build
```

API будет доступен на `http://localhost:8080`.

## API

### Рулоны

- `POST /api/steel-rolls`
- `DELETE /api/steel-rolls/{id}`
- `GET /api/steel-rolls`

Тело `POST /api/steel-rolls`:

```json
{
  "length": 12.5,
  "weight": 150
}
```

Поддерживаемые query-параметры для `GET /api/steel-rolls`:

- `idsFrom`, `idsTo`
- `weightsFrom`, `weightsTo`
- `addedFrom`, `addedTo`
- `deletedFrom`, `deletedTo`

Пример:

```text
/api/steel-rolls?idsFrom=10&idsTo=100&weightsFrom=50&weightsTo=200
```

### Статистика

- `GET /api/storage-statistics/count-added`
- `GET /api/storage-statistics/count-deleted`
- `GET /api/storage-statistics/average-length?from=2026-04-01&to=2026-04-30`
- `GET /api/storage-statistics/average-weight?from=2026-04-01&to=2026-04-30`
- `GET /api/storage-statistics/max-length?from=2026-04-01&to=2026-04-30`
- `GET /api/storage-statistics/min-length?from=2026-04-01&to=2026-04-30`
- `GET /api/storage-statistics/max-weight?from=2026-04-01&to=2026-04-30`
- `GET /api/storage-statistics/min-weight?from=2026-04-01&to=2026-04-30`
- `GET /api/storage-statistics/total-weight?from=2026-04-01&to=2026-04-30`
- `GET /api/storage-statistics/min-roll-count-day?from=2026-04-01&to=2026-04-30`
- `GET /api/storage-statistics/max-roll-count-day?from=2026-04-01&to=2026-04-30`
- `GET /api/storage-statistics/min-total-weight-day?from=2026-04-01&to=2026-04-30`
- `GET /api/storage-statistics/max-total-weight-day?from=2026-04-01&to=2026-04-30`

## Тесты

Запуск:

```powershell
dotnet test
```

Сейчас тестами покрыты:

- статистика
- валидация контрактов
- поведение контроллера при `POST` и `DELETE`
