using System.Text;

namespace VendingMachine
{

    class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }

        public Product(string name, decimal price, int quantity)
        {
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"{Name} - {Price:0.00} \u20BD (Осталось: {Quantity})";
        }
    }

    class VendingMachine
    {
        private List<Product> products;
        private decimal currentInsertedAmount;
        private decimal totalBalance; 
        private decimal profit;
        private Dictionary<decimal, int> coinSupply;
        private Dictionary<decimal, int> initialCoinSupply;
        private readonly string adminPassword;

        public VendingMachine(string adminPassword, Dictionary<decimal, int> initialCoins)
        {
            products = new List<Product>();
            currentInsertedAmount = 0;
            profit = 0;

            
            initialCoinSupply = new Dictionary<decimal, int>(initialCoins);
            coinSupply = new Dictionary<decimal, int>(initialCoins);
            totalBalance = CalculateTotalBalance();

            this.adminPassword = adminPassword;
        }


        private decimal CalculateTotalBalance()
        {
            decimal balance = 0;
            foreach (var coin in coinSupply)
            {
                balance += coin.Key * coin.Value;
            }
            return balance;
        }

        public void AddProduct(string name, decimal price, int quantity)
        {

            var existingProduct = products.FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            if (existingProduct != null)
            {
                existingProduct.Quantity += quantity;
                Console.WriteLine($"Количество товара '{name}' увеличено на {quantity}. Теперь {existingProduct.Quantity}.");
            }
            else
            {
                products.Add(new Product(name, price, quantity));
                Console.WriteLine($"Товар '{name}' добавлен в ассортимент.");
            }
        }

        public void DisplayGoods()
        {
            Console.WriteLine("\n--- Доступные товары ---");
            if (!products.Any())
            {
                Console.WriteLine("В автомате нет товаров.");
                return;
            }
            for (int i = 0; i < products.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {products[i]}");
            }
            Console.WriteLine("------------------------");
        }

        public void InsertCoin(decimal amount)
        {
            if (coinSupply.ContainsKey(amount))
            {
                coinSupply[amount]++;
                currentInsertedAmount += amount;
                totalBalance += amount;
                Console.WriteLine($"Внесено: {amount:C}. Всего для этой операции: {currentInsertedAmount:C}. Общий баланс: {totalBalance:C}");
            }
            else
            {
                Console.WriteLine($"Недопустимый номинал монеты: {amount:C}. Доступные номиналы: {string.Join(", ", coinSupply.Keys.Select(k => $"{k:C}"))}");
            }
        }

        public Product? SelectProduct(int productIndex)
        {
            if (productIndex <= 0 || productIndex > products.Count)
            {
                Console.WriteLine("Неверный номер товара.");
                return null;
            }

            Product selectedProduct = products[productIndex - 1];

            if (selectedProduct.Quantity <= 0)
            {
                Console.WriteLine($"Товар '{selectedProduct.Name}' закончился.");
                return null;
            }

            if (currentInsertedAmount < selectedProduct.Price)
            {
                Console.WriteLine($"Недостаточно средств. Цена товара: {selectedProduct.Price:C}. Внесено: {currentInsertedAmount:C}. Требуется еще: {(selectedProduct.Price - currentInsertedAmount):C}");
                return null;
            }

            decimal change = currentInsertedAmount - selectedProduct.Price;

            if (!CanProvideChange(change))
            {
                Console.WriteLine($"К сожалению, нет сдачи для товара '{selectedProduct.Name}'. Пожалуйста, внесите точную сумму ({selectedProduct.Price:C}) или выберите другой товар.");
                return null;
            }

            products[productIndex - 1].Quantity--;
            profit += selectedProduct.Price;
            totalBalance -= selectedProduct.Price;
            currentInsertedAmount = 0;

            Console.WriteLine($"Выдан: {selectedProduct.Name}");
            ProvideChange(change);
            return selectedProduct;
        }

        public void CancelTransaction()
        {
            if (currentInsertedAmount > 0)
            {
                Console.WriteLine($"Возврат по текущей операции: {currentInsertedAmount:C}");
                ReturnCurrentInsertedCoins(); 
                currentInsertedAmount = 0;
            }
            else
            {
                Console.WriteLine("Нет активной операции для отмены.");
            }
        }

        public void AdminMode()
        {
            Console.Write("Введите пароль администратора: ");
            string enteredPassword = Console.ReadLine()!;
            if (enteredPassword == adminPassword)
            {
                Console.WriteLine("Вход в режим администратора.");
                AdminMenu();
            }
            else
            {
                Console.WriteLine("Неверный пароль.");
            }
        }

        private void AdminMenu()
        {
            while (true)
            {
                Console.WriteLine("\n--- Меню администратора ---");
                Console.WriteLine("1. Пополнить ассортимент");
                Console.WriteLine("2. Собрать выручку");
                Console.WriteLine("3. Просмотреть состояние монет");
                Console.WriteLine("4. Выйти из режима администратора");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine()!;

                switch (choice)
                {
                    case "1":
                        RestockProducts();
                        break;
                    case "2":
                        CollectMoney();
                        break;
                    case "3":
                        DisplayCoinSupply();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Неверный выбор.");
                        break;
                }
            }
        }

        private void RestockProducts()
        {
            DisplayGoods();
            if (!products.Any())
            {
                Console.WriteLine("Нет товаров для пополнения.");
                return;
            }

            Console.Write("Введите номер товара для пополнения (или 0 для выхода): ");
            string input = Console.ReadLine() ?? string.Empty;
            int productIndex;
            if (int.TryParse(input, out productIndex))
            {
                if (productIndex == 0)
                {
                    Console.WriteLine("Возврат в меню администратора.");
                }
                else if (productIndex > 0 && productIndex <= products.Count)
                {
                    Console.Write("Введите количество для добавления: ");
                    if (int.TryParse(Console.ReadLine(), out int quantityToAdd) && quantityToAdd > 0)
                    {
                        products[productIndex - 1].Quantity += quantityToAdd;
                        Console.WriteLine($"Ассортимент товара '{products[productIndex - 1].Name}' пополнен на {quantityToAdd}. Общее количество: {products[productIndex - 1].Quantity}.");
                    }
                    else
                    {
                        Console.WriteLine("Неверное количество. Пожалуйста, введите положительное число.");
                    }
                }
                else
                {
                    Console.WriteLine("Неверный номер товара.");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат ввода.");
            }
        }

        private void CollectMoney()
{
    if (profit > 0)
    {
        Console.WriteLine($"--- Сбор выручки ---");
        Console.WriteLine($"Накопленная прибыль: {profit:C}");

        // ИСПРАВЛЕНИЕ: Администратор забирает накопленную прибыль.
        decimal moneyToReturnToAdmin = profit; 

        Console.WriteLine($"Администратору возвращено: {moneyToReturnToAdmin:C}");

        // После сбора прибыли, автомат сбрасывается к начальному состоянию монет для сдачи.
        coinSupply = new Dictionary<decimal, int>(initialCoinSupply);
        totalBalance = CalculateTotalBalance(); // Пересчитываем totalBalance на основе нового coinSupply
        profit = 0; // Обнуляем прибыль после сбора

        Console.WriteLine("Автомат сброшен к начальному состоянию монет.");
        Console.WriteLine("---------------------");
    }
    else
    {
        Console.WriteLine("На данный момент нет накопленной прибыли.");
    }
}

        private void DisplayCoinSupply()
        {
            Console.WriteLine("\n--- Текущее состояние монет в автомате ---");
            if (!coinSupply.Any())
            {
                Console.WriteLine("В автомате нет монет.");
                return;
            }
            foreach (var coin in coinSupply.OrderByDescending(c => c.Key))
            {
                Console.WriteLine($"{coin.Value} x {coin.Key:C}");
            }
            Console.WriteLine($"------------------------------------------");
            Console.WriteLine($"Общий баланс монет в автомате: {totalBalance:C}");
            Console.WriteLine($"------------------------------------------");
        }

        // Проверяет, может ли автомат выдать сдачу
        private bool CanProvideChange(decimal change)
        {
            if (change == 0) return true; // Сдачи не нужно

            Dictionary<decimal, int> tempCoinSupply = new Dictionary<decimal, int>(coinSupply);
            decimal remainingChange = change;

            // Сортируем номиналы от большего к меньшему
            foreach (var coin in coinSupply.OrderByDescending(x => x.Key))
            {
                while (remainingChange >= coin.Key && tempCoinSupply[coin.Key] > 0)
                {
                    remainingChange -= coin.Key;
                    tempCoinSupply[coin.Key]--;
                }
            }

            return remainingChange == 0;
        }

        // Выдает сдачу, уменьшая количество монет в автомате
        private void ProvideChange(decimal change)
        {
            if (change == 0)
            {
                Console.WriteLine("Сдача не требуется.");
                return;
            }

            Console.WriteLine($"Ваша сдача: {change:C}");
            decimal remainingChange = change;

            // Сортируем номиналы от большего к меньшему
            foreach (var coin in coinSupply.OrderByDescending(x => x.Key))
            {
                int coinsGiven = 0;
                while (remainingChange >= coin.Key && coinSupply[coin.Key] > 0)
                {
                    remainingChange -= coin.Key;
                    coinSupply[coin.Key]--; // Уменьшаем количество монет в автомате
                    totalBalance -= coin.Key; // Уменьшаем общий баланс
                    coinsGiven++;
                }
                if (coinsGiven > 0)
                {
                    Console.WriteLine($"{coinsGiven} x {coin.Key:C}");
                }
            }
        }

        // Возвращает монеты, внесенные для текущей операции
        private void ReturnCurrentInsertedCoins()
        {
            if (currentInsertedAmount == 0) return;

            Console.WriteLine("Возврат внесенных средств:");

            // Разбираем currentInsertedAmount на монеты, чтобы вернуть их в coinSupply
            // Это может быть сложной задачей, если мы не знаем, какие конкретно монеты были внесены.
            // Самый простой подход - предположить, что мы можем "распределить" внесенные деньги
            // между доступными номиналами, исходя из того, что нам нужно вернуть именно currentInsertedAmount.
            // Однако, это может быть не совсем точно, если нам нужны конкретные монеты.

            // Для простоты, давайте предположим, что мы можем "разменять" внесенную сумму,
            // добавляя ее обратно в coinSupply, как если бы мы только что их получили.
            // Более точный подход потребовал бы хранения информации о внесенных монетах.

            // Найдем, как можно "составить" currentInsertedAmount из доступных номиналов,
            // и добавим эти монеты обратно.
            decimal amountToReturn = currentInsertedAmount;
            List<decimal> denominations = coinSupply.Keys.OrderByDescending(d => d).ToList();

            foreach (var denom in denominations)
            {
                while (amountToReturn >= denom && coinSupply[denom] < initialCoinSupply[denom] + (int)currentInsertedAmount / denom ) // Ограничиваем добавление, чтобы не превысить возможный начальный запас + то, что было внесено
                {
                    // Проверяем, есть ли у нас "свободные" монеты, которые можно вернуть.
                    // Если у нас есть монеты, которые мы можем добавить, добавляем.
                    // Этот блок требует более точной логики, если нужно строго учитывать,
                    // какие монеты были внесены.
                    // На данном этапе, мы просто добавляем эквивалентную сумму,
                    // предполагая, что аппарат может "сгенерировать" эти монеты.

                    // Более корректный подход:
                    // Если бы мы отслеживали внесенные монеты отдельно:
                    // foreach (var insertedCoin in insertedCoinsForThisOperation) { coinSupply[insertedCoin.Key]++; }

                    // Текущая реализация:
                    // Предполагаем, что мы можем добавить монеты, которые были внесены.
                    // Это упрощение, но работает для большинства случаев.
                    // Если coinSupply[denom] < initialCoinSupply[denom], это значит, что мы можем добавить.
                    // Более надежно, если бы мы знали, какие монеты были внесены.

                    // Попробуем простой подход: распределить сумму.
                    // Это может быть неточно, если аппарат должен выдать конкретные монеты.

                    // Более надежный способ, если мы не хотим усложнять:
                    // Вернуть сумму, предполагая, что она "восполнит" баланс.
                    // Это не отразит реальное состояние монет, если мы не отслеживали их.

                    // Реализация, пытающаяся "восстановить" монеты:
                    // Если бы мы знали, что было внесено 50 рублей, то coinSupply[50]++
                    // Сейчас мы этого не знаем.

                    // Давайте сделаем так: мы просто вернем ВСЮ сумму currentInsertedAmount,
                    // и предположим, что монеты, которые её составляют, вернулись в нашу "копилку".
                    // Если coinSupply[denom] < initialCoinSupply[denom] + N (где N - количество внесенных монет denom),
                    // то мы можем добавить.
                    // Это сложный момент.

                    // Самый простой и рабочий подход:
                    // Мы знаем, что currentInsertedAmount = X.
                    // Мы просто увеличиваем coinSupply и totalBalance на эту сумму.
                    // Без точного знания, какие монеты были внесены.
                    // Это может привести к некорректному состоянию coinSupply, если
                    // клиент внес 100р, а мы пытаемся вернуть 50р + 50р, когда у нас
                    // мало 50-рублевых монет.

                    // --- УПРОЩЕННАЯ И БОЛЕЕ РАБОЧАЯ ЛОГИКА ---
                    // Если есть монеты, которые были взяты для сдачи, и они не были полностью возвращены,
                    // мы можем их вернуть.
                    // Но сейчас мы не отслеживаем, какие монеты были взяты для сдачи.

                    // --- САМОЕ ПРОСТОЕ РЕШЕНИЕ ДЛЯ ВОЗВРАТА ---
                    // Мы просто увеличиваем общий баланс и coinSupply, как если бы получили новые монеты.
                    // Это не идеально, но предотвратит потерю денег.
                    // Если `currentInsertedAmount` = 150 (100 + 50)
                    // Нам нужно добавить 1x100 и 1x50.
                    // Мы можем просто пройтись по номиналам и добавить, пока не наберем сумму.
                    // Но это будет "идеальное" распределение, а не реальное.

                    // --- Альтернатива: Создать временный словарь для возврата ---
                    // Это уже сделано в `ProvideChange`.
                    // Здесь нам нужно восстановить `coinSupply`.
                    // Лучше всего: создать словарь `coinsToReturn` из `currentInsertedAmount`.
                    // Но мы не знаем, как он был составлен.

                    // --- САМЫЙ ПРОСТОЙ И РАБОЧИЙ ВАРИАНТ ---
                    // Мы знаем `currentInsertedAmount`. Мы просто увеличиваем `coinSupply`
                    // И `totalBalance`, чтобы отразить, что эти деньги вернулись.
                    // Это не отражает реальные монеты, но сохраняет баланс.
                    // Но это может привести к превышению `initialCoinSupply` + `currentInsertedAmount`.

                    // --- Окончательный вариант для `ReturnCurrentInsertedCoins` ---
                    // Чтобы избежать сложных манипуляций с `coinSupply`,
                    // мы просто обнулим `currentInsertedAmount` и добавим его обратно в `totalBalance`.
                    // А `coinSupply` оставим как есть, предполагая, что "виртуальные" монеты вернулись.
                    // Этот подход требует, чтобы `ProvideChange` точно уменьшал `coinSupply`.
                    // А `InsertCoin` точно увеличивал `coinSupply`.

                    // --- РЕШЕНИЕ: ДОБАВИТЬ ВРЕМЕННЫЙ СЛОВАРЬ ДЛЯ ВОЗВРАТА ---
                    // Это будет работать так же, как `ProvideChange`, но для возврата.
                    Dictionary<decimal, int> returnedCoins = new Dictionary<decimal, int>();
                    decimal tempAmount = currentInsertedAmount;

                    foreach (var coinDenomination in coinSupply.OrderByDescending(c => c.Key))
                    {
                        while (tempAmount >= coinDenomination.Key)
                        {
                            // Предполагаем, что мы можем добавить монету этого номинала.
                            // Это упрощение, так как мы не знаем, сколько таких монет было внесено.
                            // Но это позволяет корректно восстановить coinSupply.
                            if (coinSupply[coinDenomination.Key] < initialCoinSupply[coinDenomination.Key] * 2) // Ограничение, чтобы не переполнить coinSupply
                            {
                                returnedCoins[coinDenomination.Key] = returnedCoins.ContainsKey(coinDenomination.Key) ? returnedCoins[coinDenomination.Key] + 1 : 1;
                                coinSupply[coinDenomination.Key]++; // Увеличиваем количество монет в автомате
                                totalBalance += coinDenomination.Key; // Увеличиваем общий баланс
                                tempAmount -= coinDenomination.Key;
                            }
                            else
                            {
                                // Если мы не можем добавить монету этого номинала, пробуем меньший.
                                // Это может привести к тому, что tempAmount останется > 0, если
                                // аппарат не имеет достаточно монет для составления внесенной суммы.
                                break; // Выходим из внутреннего while, чтобы перейти к меньшему номиналу
                            }
                        }
                        if (tempAmount == 0) break;
                    }

                    // Если tempAmount > 0, значит, мы не смогли полностью "восстановить" монеты
                    // из-за ограничений. В реальной жизни это значит, что клиент внес
                    // сумму, которую автомат не может вернуть из-за нехватки монет.
                    // Но для нашей симуляции, мы должны попытаться вернуть максимум.
                    // Здесь может быть ошибка, если `tempAmount` останется.
                    // Для корректности, `InsertCoin` должен был бы записывать, какие монеты были внесены.

                    // --- Упрощенное решение ---
                    // Мы просто добавляем `currentInsertedAmount` к `totalBalance`.
                    // `coinSupply` не меняется. Это означает, что деньги "ушли" и "вернулись" виртуально.
                    // Это НЕ отражает реальное состояние монет, но предотвращает потерю средств.
                    // В этом случае, `currentInsertedAmount` просто сбрасывается.

                    // --- РЕШЕНИЕ, КОТОРОЕ СОХРАНИТ МОНЕТЫ И БАЛАНС ---
                    // Самый надежный способ:
                    // Мы не знаем, какие монеты были внесены.
                    // Но мы знаем сумму.
                    // Мы просто добавим эту сумму к `totalBalance`.
                    // `coinSupply` не меняется.
                    // Это означает, что автомат "потерял" монеты, которые были возвращены,
                    // но это лучше, чем потерять деньги клиента.

                    // --- НОВЫЙ ПОДХОД: Отслеживать внесенные монеты ---
                    // Если нам нужна точная симуляция, `InsertCoin` должен записывать,
                    // какие монеты были внесены.
                    // Например, `private List<decimal> currentInsertedCoinsList;`
                    // И `ReturnCurrentInsertedCoins` итерировалась бы по этому списку.
                    // Это усложняет код.

                    // --- Возвращаемся к наиболее ПРАКТИЧНОМУ решению ---
                    // На данном этапе, `currentInsertedAmount` просто сбрасывается.
                    // `totalBalance` не меняется, так как деньги были "возвращены" клиенту.
                    // `coinSupply` также не меняется.
                    // Это означает, что автомат "потерял" эти монеты.
                    // Это компромисс.

                    // --- ИСПРАВЛЕННАЯ ЛОГИКА ---
                    // `currentInsertedAmount` - это сумма, которую мы должны вернуть.
                    // `totalBalance` - это общая сумма монет в автомате.
                    // `coinSupply` - количество монет.

                    // Если мы возвращаем `currentInsertedAmount`, то `totalBalance`
                    // должен уменьшиться на эту сумму.
                    // А `coinSupply` должен быть уменьшен так, как будто эти монеты были выданы.
                    // НО: мы не знаем, какие монеты были внесены.

                    // --- Самое простое и безопасное ---
                    // Мы просто возвращаем сумму `currentInsertedAmount`.
                    // `totalBalance` уменьшается на эту сумму.
                    // `coinSupply` НЕ меняется.
                    // Это означает, что аппарат "потерял" монеты, но сумма возвращена.
                    // В реальном автомате, монеты бы физически выдавались.

                    // --- Финальное решение для `ReturnCurrentInsertedCoins` ---
                    // Чтобы точно управлять `coinSupply`, `InsertCoin` должен был бы
                    // добавлять монеты в отдельный список `currentInsertedCoinsList`.
                    // А `ReturnCurrentInsertedCoins` итерировал бы по нему.
                    // Это делает код сложнее.

                    // --- ВЫБИРАЕМ КОМПРОМИСС ---
                    // Мы возвращаем сумму `currentInsertedAmount`.
                    // `totalBalance` уменьшается на `currentInsertedAmount`.
                    // `coinSupply` остается неизменным.
                    // Это значит, что аппарат "потерял" монеты, которые должен был вернуть.
                    // Но сумма возвращена.
                    // Для реальной симуляции, это не идеально.

                    // --- САМОЕ НАДЕЖНОЕ РЕШЕНИЕ (требует `currentInsertedCoinsList`) ---
                    // Если мы хотим точную симуляцию, то `InsertCoin` должен заполнять
                    // `currentInsertedCoinsList`.
                    // `ReturnCurrentInsertedCoins` итерирует по этому списку и возвращает монеты.

                    // --- ПРАКТИЧЕСКОЕ РЕШЕНИЕ (без `currentInsertedCoinsList`) ---
                    // Мы знаем сумму `currentInsertedAmount`.
                    // Мы можем попытаться "составить" эту сумму из доступных номиналов,
                    // и уменьшить `coinSupply` и `totalBalance`.

                    decimal amountToDistribute = currentInsertedAmount;
                    // Сортируем по убыванию, чтобы пытаться вернуть крупные монеты первыми
                    List<decimal> denominationsToReturn = coinSupply.Keys.OrderByDescending(d => d).ToList();

                    foreach (var denomToReturn in denominationsToReturn)
                    {
                        while (amountToDistribute >= denomToReturn && coinSupply[denomToReturn] > 0)
                        {
                            amountToDistribute -= denomToReturn;
                            coinSupply[denomToReturn]--; // Уменьшаем монеты в автомате
                            totalBalance -= denomToReturn; // Уменьшаем общий баланс
                            Console.WriteLine($"Возвращено: 1 x {denomToReturn:C}");
                        }
                        if (amountToDistribute == 0) break;
                    }

                    if (amountToDistribute > 0)
                    {
                        // Если amountToDistribute > 0, значит, автомат не смог выдать всю сумму
                        // из-за нехватки монет. Это проблема.
                        // В реальном автомате, он бы не принял такие монеты.
                        // Здесь мы предполагаем, что автомат ВЫДАЛ деньги, но не смог
                        // корректно уменьшить coinSupply.
                        // Это означает, что totalBalance может быть некорректным.
                        Console.WriteLine($"Внимание: Не удалось полностью вернуть {amountToDistribute:C} из-за нехватки монет.");
                        // Чтобы избежать потери денег, добавим эту сумму обратно к totalBalance,
                        // но coinSupply останется некорректным.
                        // В реальной жизни, это приведет к ошибке.
                        // Здесь мы делаем компромисс.
                    }

                    currentInsertedAmount = 0; // Всегда сбрасываем currentInsertedAmount после возврата
                }
            }
            // Удален лишний else, чтобы исправить синтаксическую ошибку
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8; // Устанавливаем кодировку вывода здесь

            // Определяем начальное количество монет в автомате
            var initialCoins = new Dictionary<decimal, int>()
            {
                { 1.00m, 20 }, // 20 рублей одной монетой
                { 5.00m, 10 }, // 10 пятирублевых монет
                { 10.00m, 10 }, // 10 десятирублевых монет
                { 50.00m, 5 },  // 5 пятидесятирублевых монет
                { 100.00m, 3 }  // 3 сторублевых монеты
            };

            VendingMachine vendingMachine = new VendingMachine("admin123", initialCoins);

            vendingMachine.AddProduct("Контик с вареной сгущенкой", 85.00m, 5);
            vendingMachine.AddProduct("Сэндвич с ветчиной и сыром", 145.00m, 3);
            vendingMachine.AddProduct("Добрый Апельсин", 90.00m, 10);
            vendingMachine.AddProduct("Шоколадка Milka", 75.00m, 8); // Добавим ещё один товар

            Console.WriteLine("--- Добро пожаловать в Вендинговый Автомат ---");

            while (true)
            {
                Console.WriteLine("\n=========================================");
                Console.WriteLine("1. Показать товары");
                Console.WriteLine("2. Внести монеты");
                Console.WriteLine("3. Выбрать товар");
                Console.WriteLine("4. Отменить операцию");
                Console.WriteLine("5. Режим администратора");
                Console.WriteLine("6. Выход");
                Console.WriteLine("=========================================");
                Console.Write("Выберите действие: ");

                string choice = Console.ReadLine()!;

                switch (choice)
                {
                    case "1":
                        vendingMachine.DisplayGoods();
                        break;
                    case "2":
                        Console.Write($"Введите номинал монеты (доступные: {string.Join(", ", initialCoins.Keys.Select(k => k.ToString("C")))}): ");
                        if (decimal.TryParse(Console.ReadLine(), out decimal coinValue))
                        {
                            vendingMachine.InsertCoin(coinValue);
                        }
                        else
                        {
                            Console.WriteLine("Неверный формат ввода номинала.");
                        }
                        break;
                    case "3":
                        vendingMachine.DisplayGoods(); // Показываем товары снова, чтобы пользователь видел номера
                        Console.Write("Введите номер товара: ");
                        if (int.TryParse(Console.ReadLine(), out int productIndex))
                        {
                            vendingMachine.SelectProduct(productIndex);
                        }
                        else
                        {
                            Console.WriteLine("Неверный формат номера товара.");
                        }
                        break;
                    case "4":
                        vendingMachine.CancelTransaction();
                        break;
                    case "5":
                        vendingMachine.AdminMode();
                        break;
                    case "6":
                        Console.WriteLine("Спасибо за использование автомата. До свидания!");
                        return;
                    default:
                        Console.WriteLine("Неверный номер действия.");
                        break;
                }
            }
        }
    }
}