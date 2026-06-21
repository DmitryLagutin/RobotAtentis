![Trading](https://github.com/DmitryLagutin/RobotAtentis/blob/master/img/1.jpg "Пример работы робота в высокочастотном режиме")

# 🤖 TradingBot (RobotAtentis)

C# торговый робот для высокочастотной торговли на срочном рынке Московской биржи.

## 🎯 Краткое описание

Профессиональный алгоритмический торговый бот с поддержкой:
- **Фьючерсы и опционы** Московской биржи (MOEX)
- **HFT-алгоритмы** - высокая скорость исполнения транзакций
- **Статистические стратегии** и машинное обучение
- **Бэктестинг** стратегий на исторических данных
- **Telegram уведомления** о торговых операциях

### Основные преимущества:

⭐ **Высокая скорость** - оптимизированный код для HFT операций  
⭐ **Гибкая архитектура** - простая разработка собственных стратегий  
⭐ **Интеграция с брокером** - коннектор Atentis от Алор Брокер  
⭐ **Визуализация** - мониторинг торгов в реальном времени  
⭐ **Тестирование** - бэктестинг перед реальным использованием  

---

## 📋 Содержание

- [🔧 Технологии](#-технологии)
- [🚀 Установка](#-установка)
- [⚙️ Конфигурация](#️-конфигурация)
- [💡 Использование](#-использование)
- [📊 Структура проекта](#-структура-проекта)
- [🌐 Полезные ссылки](#-полезные-ссылки)

---

## 🔧 Технологии

| Компонент | Описание |
|-----------|----------|
| **Язык программирования** | C# (.NET Framework) |
| **Коннектор** | Atentis Library (Alor Broker) |
| **База данных** | SQLite / MySQL для хранения данных |
| **Интерфейс** | Windows Forms (WinForms) |
| **HFT** | Оптимизированные алгоритмы высокой частоты |

---

## 🚀 Установка

### Требования

- **.NET Framework 4.6+** или **Unity Profiler**
- Visual Studio 2017+ (для сборки из исходников)
- Аккаунт у брокера **Алор Брокер** с доступом к API
- Подключенный терминал Alor Trade Terminal

### Шаги установки

1. **Скопируйте проект:**
   ```bash
   git clone https://github.com/DmitryLagutin/RobotAtentis.git
   cd RobotAtentis
   ```

2. **Откройте решение в Visual Studio:**
   ```
   Откройте файл .sln и восстановите NuGet пакеты
   ```

3. **Настройте конфигурацию:**
   - Откройте `App.config` или создайте `settings.json`
   - Укажите параметры подключения к API брокера
   
4. **Скомпилируйте проект:**
   ```bash
   dotnet build
   # или в Visual Studio: Ctrl+Shift+B
   ```

---

## ⚙️ Конфигурация

### Параметры подключения

```json
{
  "broker": {
    "name": "Alor",
    "apiKey": "your_api_key_here",
    "environment": "sandbox" // or "production"
  },
  "trading": {
    "market": "MOEX",
    "instruments": ["RTS"], // фьючерсы РТС, СПБИ и др.
    "riskLimits": {
      "maxPosition": 100,
      "maxDailyVolume": 500
    }
  }
}
```

### Настройка торгового алгоритма

Основные настройки в коде:

- **Strategy Type**: Trend following, Mean reversion, Arbitrage
- **Risk Management**: Stop-loss, Take-profit levels
- **Timeframes**: M1, M5, M15 для анализа
- **Data Feed**: Real-time quotes из MOEX

---

## 💡 Использование

### Запуск робота

1. Запустите приложение из Visual Studio или выполните:
   ```bash
   dotnet run --project TradingBot.exe
   ```

2. Подключитесь к торговой платформе брокера

3. Выберите инструмент для торговли (фьючерс, опцион)

4. Загрузите стратегию или выберите из шаблонов

5. Запустите тестовые торги (Paper Trading)

### Торговые операции

#### Вход в позицию:
```csharp
// Открыть длинную позицию
await trader.OpenPosition(symbol, quantity, orderType);

// Открыть короткую позицию
await trader.OpenShortPosition(symbol, quantity, orderType);
```

#### Выход из позиции:
```csharp
// Закрыть позицию
await trader.ClosePosition(symbol, positionId);

// Частичная закрытие
await trader.PartialClose(symbol, partialQuantity);
```

#### Управление рисками:
```csharp
// Установка стоп-лосса
await trader.SetStopLoss(positionId, stopPrice);

// Установка тейк-профита
await trader.SetTakeProfit(positionId, takePrice);
```

### Telegram уведомления

Подключите Telegram бот для получения оповещений:

```csharp
var telegramNotifier = new TelegramNotifier(botToken, chatId);
telegramNotifier.SubscribeTradeEvents(trader);
```

Получайте уведомления о:
- Открытии/закрытии позиций
- Изменении баланса
- Срабатывании стоп-лосс/тейк-профит
- Ошибках исполнения ордеров

---

## 📊 Структура проекта

```
RobotAtentis/
├── ApplicationContext.cs        # Глобальное состояние приложения
├── DataOrderBook.cs           # Обработка стакана заявок
├── Form1.cs                   # Основной UI интерфейс
├── Form1.Designer.cs          # Дизайн формы
├── InfoDataIn.cs              # Структуры входящих данных
├── LogData.cs                 # Логирование операций
├── Order.cs                   # Модель ордера
├── Position.cs                # Модель позиции
├── Program.cs                 # Точка входа
├── TradeHelper.cs             # Вспомогательные функции торгов
├── Trading.cs                 # Основная логика торговли
├── Properties/                # Метаданные проекта
│   ├── AssemblyInfo.cs
│   ├── Resources.Designer.cs
│   └── Settings.Designer.cs
├── img/                       # Изображения проекта
│   └── 1.jpg                  # Пример интерфейса
└── README.md                  # Эта документация
```

---

## 🌐 Полезные ссылки

### Официальные ресурсы

- [Московская биржа](https://www.moex.com/)
- [Alor Broker API](https://alorbroker.ru/trading/tools/)
- [Atentis Documentation](https://github.com/alor-broker/atentis)

### Обучение и документация

- [Начало работы](https://github.com/DmitryLagutin/RobotAtentis/wiki/%D0%9D%D0%B0%D1%87%D0%B0%D0%BB%D0%BE-%D1%80%D0%B0%D0%B1%D0%BE%D1%82%D1%8B)
- [Настройка торгового алгоритма](https://github.com/DmitryLagutin/RobotAtentis/wiki/%D0%9D%D0%B0%D1%81%D1%82%D1%80%D0%BE%D0%B9%D0%BA%D0%B0-%D1%82%D0%BE%D1%80%D0%B3%D0%BE%D0%B2%D0%BE%D0%B3%D0%BE-%D0%B0%D0%BB%D0%B3%D0%BE%D1%80%D0%B8%D1%82%D0%BC%D0%B0)
- [Использование базы данных](https://github.com/DmitryLagutin/RobotAtentis/wiki/%D0%98%D1%81%D0%BF%D0%BE%D0%BB%D1%8C%D0%B7%D0%BE%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5-%D0%B1%D0%B0%D0%B7%D1%8B-%D0%B4%D0%B0%D0%BD%D0%BD%D1%8B%D1%85)
- [Визуализация](https://github.com/DmitryLagutin/RobotAtentis/wiki/%D0%92%D0%B8%D0%B7%D1%83%D0%B0%D0%BB%D0%B8%D0%B7%D0%B0%D1%86%D0%B8%D1%8F)

### Стратегии

- Тренд-следование (Trend Following)
- Возврат к среднему (Mean Reversion)
- Арбитраж между рынками
- Статистический арбитраж

---

## ⚠️ Важные предупреждения

1. **Риск потери средств** - алгоритмическая торговля связана с финансовыми рисками
2. **Тестируйте на демо** - всегда запускайте сначала в Paper Trading режиме
3. **Мониторьте систему** - постоянно следите за работой робота
4. **Диверсифицируйте риски** - не используйте весь капитал в одной стратегии

---

## 👥 Конкуренты и вклад

Top Contributors:
- @DmitryLagutin (35 contributions)
- @dimalaga (17 contributions)

---

## 📝 License

Данный проект предназначен для образовательных и коммерческих целей. Используйте на свой страх и риск.

---

## 👤 Автор

Разработано [@DmitryLagutin](https://github.com/DmitryLagutin)

*Happy trading! 📈*
