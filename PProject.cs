using System;
using Serilog;

namespace FoodOrderApp {
    class Program {
        static void Main(string[] args) {

            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File("food_order_app.log", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Log.Information("Додаток для замовлення їжі запущено.");

            try {

                var user = RegisterUser();
                Log.Information($"Користувач {user.Name} з номером {user.PhoneNumber} зареєстрований.");

                string foodType = ChooseFoodType();
                Log.Information($"Обрано тип їжі: {foodType}.");

                string selectedItem = ChooseMenuItem(foodType);
                Log.Information($"Обрано: {selectedItem}.");

                string address = EnterDeliveryAddress();
                Log.Information($"Адреса доставки: {address}.");

                var paymentResult = ProcessPayment();
                Log.Information($"Оплата на суму {paymentResult.Amount} за допомогою картки {paymentResult.CardNumber}. Залишок на карті: {paymentResult.Balance}.");

                ConfirmOrder(user, foodType, selectedItem, address, paymentResult);
                Log.Information($"Замовлення для {user.Name} успішно оформлено.");

            }

            catch (Exception ex) {

                Log.Error($"Помилка: {ex.Message}");
                Console.WriteLine($"Сталася помилка: {ex.Message}");

            }

            finally {

                Log.CloseAndFlush();

            }
        }

        static User RegisterUser() {

            Console.WriteLine("Введіть ваше ім'я:");
            string? userName = Console.ReadLine();

            Console.WriteLine("Введіть ваш номер телефону:");
            string? phoneNumber = Console.ReadLine();

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(phoneNumber)) {

                throw new Exception("Ім'я або номер телефону не можуть бути порожніми.");

            }

            return new User { Name = userName, PhoneNumber = phoneNumber };

        }

        static string ChooseFoodType() {

            string? foodType;
            while (true) {

                Console.WriteLine("Оберіть тип їжі (Піца, Бургер, Суші, Напої):");
                foodType = Console.ReadLine()?.ToLower();

                if (foodType == "Pizza" || foodType == "Burger" || foodType == "Sushi" || foodType == "Drinks") {

                    break;

                }

                else {

                    Console.WriteLine("Невірний тип їжі. Спробуйте ще раз.");

                }
            }

            return foodType!;
        }

        static string ChooseMenuItem(string foodType) {

            string[] menu = foodType switch {

                "Pizza" => new[] { "Маргарита", "Пепероні", "Гавайська", "Чотири сири", "М'ясна" },
                "Burger" => new[] { "Чізбургер", "Чікенбургер", "Вегетаріанський", "Біг Мак", "Дабл Чізбургер" },
                "Sushi" => new[] { "Філадельфія", "Каліфорнія", "Дракон", "Рол з лососем", "Рол з тунцем" },
                "Drinks" => new[] { "Кола", "Фанта", "Спрайт", "Чай", "Кава" },
                _ => throw new Exception("Невірний тип їжі.")

            };

            Console.WriteLine($"Оберіть страву з меню {foodType}:");
            for (int i = 0; i < menu.Length; i++) {

                Console.WriteLine($"{i + 1}. {menu[i]}");

            }

            string? input = Console.ReadLine();
            if (!int.TryParse(input, out int choice) || choice < 1 || choice > menu.Length) {

                throw new Exception("Невірний вибір страви.");

            }

            return menu[choice - 1];

        }

        static string EnterDeliveryAddress() {

            Console.WriteLine("Введіть вашу адресу доставки:");
            string? address = Console.ReadLine();

            if (string.IsNullOrEmpty(address)) {

                throw new Exception("Адреса не може бути порожньою.");

            }

            return address;
        }

        static PaymentResult ProcessPayment() {

            Console.WriteLine("Введіть суму до оплати:");
            string? amountInput = Console.ReadLine();
            if (!decimal.TryParse(amountInput, out decimal amount) || amount <= 0) {

                throw new Exception("Невірна сума до оплати.");

            }

            Console.WriteLine("Введіть номер картки для оплати:");
            string? cardNumber = Console.ReadLine();
            if (string.IsNullOrEmpty(cardNumber)) {

                throw new Exception("Номер картки не може бути порожнім.");

            }

            Console.WriteLine("Введіть баланс картки:");
            string? balanceInput = Console.ReadLine();
            if (!decimal.TryParse(balanceInput, out decimal balance) || balance < amount) {

                throw new Exception("Невірний баланс картки або недостатньо коштів.");

            }

            balance -= amount;

            return new PaymentResult { Amount = amount, CardNumber = cardNumber, Balance = balance };

        }

        static void ConfirmOrder(User user, string foodType, string selectedItem, string address, PaymentResult paymentResult) {

            Console.WriteLine("Дякуємо за ваше замовлення!");
            Console.WriteLine($"Деталі замовлення:");
            Console.WriteLine($"Ім'я: {user.Name}");
            Console.WriteLine($"Телефон: {user.PhoneNumber}");
            Console.WriteLine($"Тип їжі: {foodType}");
            Console.WriteLine($"Обрано: {selectedItem}");
            Console.WriteLine($"Адреса: {address}");
            Console.WriteLine($"Сума: {paymentResult.Amount}");
            Console.WriteLine($"Номер картки: {paymentResult.CardNumber}");

        }
    }

    class User {

        public string Name {get; set;} = string.Empty;
        public string PhoneNumber {get; set;} = string.Empty;

    }

    class PaymentResult {

        public decimal Amount {get; set;}
        public string CardNumber {get; set;} = string.Empty;
        public decimal Balance {get; set;}

    }
}
