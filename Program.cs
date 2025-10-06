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
            {
                products.Add(new Product(name, price, quantity));
            }
        }

        public void DisplayGoods()
        {
            Console.WriteLine("\n--- Доступные товары ---");
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
                Console.WriteLine($"Внесено: {amount:C}. Всего для этой операции: {currentInsertedAmount:C}");
            }
            else
            {
                Console.WriteLine($"Недопустимый номинал монеты");
            }
        }

        public Product? SelectProduct(int productIndex)
        {
            if (productIndex <= 0 || productIndex > products.Count)
            {
                Console.WriteLine("Неверный номер товара");
                return null;
            }

            Product selectedProduct = products[productIndex - 1];

            if (selectedProduct.Quantity <= 0)
            {
                Console.WriteLine($"Товар '{selectedProduct.Name}' закончился");
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
                Console.WriteLine($"К сожалению, нет сдачи для товара '{selectedProduct.Name}'. Пожалуйста, внесите точную сумму ({selectedProduct.Price:C}) или выберите другой товар");
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
                Console.WriteLine("Нет активной операции для отмены");
            }
        }

        public void AdminMode()
        {
            Console.Write("\nВведите пароль администратора: ");
            string enteredPassword = Console.ReadLine()!;
            if (enteredPassword == adminPassword)
            {
                Console.WriteLine("\nВход в режим администратора.");
                AdminMenu();
            }
            else
            {
                Console.WriteLine("\nНеверный пароль");
            }
        }

        private void AdminMenu()
        {
            while (true)
            {
                Console.WriteLine("\n------ Меню администратора ------");
                Console.WriteLine("1. Пополнить ассортимент");
                Console.WriteLine("2. Собрать выручку");
                Console.WriteLine("3. Просмотреть состояние монет");
                Console.WriteLine("4. Выйти из режима администратора");
                Console.WriteLine("-----------------------------------");
                Console.Write("\nВыберите действие: ");

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
                        Console.WriteLine("Неверный выбор действия");
                        break;
                }
            }
        }

        private void RestockProducts()
        {
            DisplayGoods();
            if (!products.Any())
            {
                Console.WriteLine("Нет товаров для пополнения");
                return;
            }

            Console.Write("Введите номер товара для пополнения (или 0 для выхода): ");
            string input = Console.ReadLine() ?? string.Empty;
            int productIndex;
            if (int.TryParse(input, out productIndex))
            {
                if (productIndex == 0)
                {
                    Console.WriteLine("Возврат в меню администратора");
                }
                else if (productIndex > 0 && productIndex <= products.Count)
                {
                    Console.Write("Введите количество для добавления: ");
                    if (int.TryParse(Console.ReadLine(), out int quantityToAdd) && quantityToAdd > 0)
                    {
                        products[productIndex - 1].Quantity += quantityToAdd;
                        Console.WriteLine($"Ассортимент товара '{products[productIndex - 1].Name}' пополнен на {quantityToAdd}. Общее количество: {products[productIndex - 1].Quantity}");
                    }
                    else
                    {
                        Console.WriteLine("Неверное количество. Пожалуйста, введите положительное число");
                    }
                }
                else
                {
                    Console.WriteLine("Неверный номер товара");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат ввода");
            }
        }

        private void CollectMoney()
        {
            if (profit > 0)
            {
                Console.WriteLine($"--- Сбор выручки ---");
                Console.WriteLine($"Накопленной прибыли собрано: {profit:C}");

                coinSupply = new Dictionary<decimal, int>(initialCoinSupply);
                totalBalance = CalculateTotalBalance();
                profit = 0;
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

        private bool CanProvideChange(decimal change)
        {
            if (change == 0) return true;
            Dictionary<decimal, int> tempCoinSupply = new Dictionary<decimal, int>(coinSupply);
            decimal remainingChange = change;

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

        private void ProvideChange(decimal change)
        {
            if (change == 0)
            {
                Console.WriteLine("Сдача не требуется");
                return;
            }

            Console.WriteLine($"Ваша сдача: {change:C}");
            decimal remainingChange = change;

            foreach (var coin in coinSupply.OrderByDescending(x => x.Key))
            {
                int coinsGiven = 0;
                while (remainingChange >= coin.Key && coinSupply[coin.Key] > 0)
                {
                    remainingChange -= coin.Key;
                    coinSupply[coin.Key]--;
                    totalBalance -= coin.Key;
                    coinsGiven++;
                }
                if (coinsGiven > 0)
                {
                    Console.WriteLine($"{coinsGiven} x {coin.Key:C}");
                }
            }
        }

        private void ReturnCurrentInsertedCoins()
        {
            if (currentInsertedAmount == 0) return;
            Console.WriteLine("Возврат внесенных средств:");
            decimal amountToReturn = currentInsertedAmount;
            List<decimal> denominations = coinSupply.Keys.OrderByDescending(d => d).ToList();
            foreach (var denom in denominations)
            {
                while (amountToReturn >= denom && coinSupply[denom] < initialCoinSupply[denom] + (int)currentInsertedAmount / denom) 
                {
                    Dictionary<decimal, int> returnedCoins = new Dictionary<decimal, int>();
                    decimal tempAmount = currentInsertedAmount;
                    foreach (var coinDenomination in coinSupply.OrderByDescending(c => c.Key))
                    {
                        while (tempAmount >= coinDenomination.Key)
                        {
                            if (coinSupply[coinDenomination.Key] < initialCoinSupply[coinDenomination.Key] * 2)
                            {
                                returnedCoins[coinDenomination.Key] = returnedCoins.ContainsKey(coinDenomination.Key) ? returnedCoins[coinDenomination.Key] + 1 : 1;
                                coinSupply[coinDenomination.Key]++;
                                totalBalance += coinDenomination.Key;
                                tempAmount -= coinDenomination.Key;
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (tempAmount == 0) break;
                    }

                    decimal amountToDistribute = currentInsertedAmount;
                    List<decimal> denominationsToReturn = coinSupply.Keys.OrderByDescending(d => d).ToList();
                    foreach (var denomToReturn in denominationsToReturn)
                    {
                        while (amountToDistribute >= denomToReturn && coinSupply[denomToReturn] > 0)
                        {
                            amountToDistribute -= denomToReturn;
                            coinSupply[denomToReturn]--;
                            totalBalance -= denomToReturn;
                            Console.WriteLine($"Возвращено: 1 x {denomToReturn:C}");
                        }
                        if (amountToDistribute == 0) break;
                    }

                    if (amountToDistribute > 0)
                    {
                        Console.WriteLine($"Внимание: Не удалось полностью вернуть {amountToDistribute:C} из-за нехватки монет");
                    }

                    currentInsertedAmount = 0;
                }
            }
        }

        class Program
        {
            static void Main(string[] args)
            {
                Console.OutputEncoding = Encoding.UTF8;

                var initialCoins = new Dictionary<decimal, int>()
            {
                { 1.00m, 200 },
                { 5.00m, 100 },
                { 10.00m, 100 },
                { 50.00m, 50 },
                { 100.00m, 30 }
            };

                VendingMachine vendingMachine = new VendingMachine("ЯлюблюКонтики", initialCoins);
                vendingMachine.AddProduct("Контик с вареной сгущенкой", 85.00m, 5);
                vendingMachine.AddProduct("Сэндвич с ветчиной и сыром", 145.00m, 3);
                vendingMachine.AddProduct("Добрый Апельсин", 90.00m, 10);
                Console.WriteLine("\n--- Добро пожаловать в Вендинговый Автомат ---");

                while (true)
                {
                    Console.WriteLine("\n=========================================");
                    Console.WriteLine("1. Показать товары");
                    Console.WriteLine("2. Внести монеты");
                    Console.WriteLine("3. Выбрать товар");
                    Console.WriteLine("4. Отменить операцию");
                    Console.WriteLine("5. Режим администратора");
                    Console.WriteLine("=========================================");
                    Console.Write("\nВыберите действие: ");

                    string choice = Console.ReadLine()!;

                    switch (choice)
                    {
                        case "1":
                            vendingMachine.DisplayGoods();
                            break;
                        case "2":
                            Console.Write($"\nВведите номинал монеты (доступные: {string.Join(", ", initialCoins.Keys.Select(k => k.ToString("C")))}): ");
                            if (decimal.TryParse(Console.ReadLine(), out decimal coinValue))
                            {
                                vendingMachine.InsertCoin(coinValue);
                            }
                            else
                            {
                                Console.WriteLine("Неверный формат ввода номинала");
                            }
                            break;
                        case "3":
                            vendingMachine.DisplayGoods();
                            Console.Write("Введите номер товара: ");
                            if (int.TryParse(Console.ReadLine(), out int productIndex))
                            {
                                vendingMachine.SelectProduct(productIndex);
                            }
                            else
                            {
                                Console.WriteLine("Неверный формат номера товара");
                            }
                            break;
                        case "4":
                            vendingMachine.CancelTransaction();
                            break;
                        case "5":
                            vendingMachine.AdminMode();
                            break;
                        default:
                            Console.WriteLine("Неверный номер действия");
                            break;
                    }
                }
            }
        }
    }
}