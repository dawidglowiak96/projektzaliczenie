using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace projekt
{
    public enum AdminOperation
    {
        AddItem = 1,
        UpdateItem = 2,
        DisplayItem = 3,
        DisplayMoney = 4,
        Logout = 5
    }
    class Pos
    {
        public List<Item> Items;
        public Dictionary<int, BoughtItem> BoughtItems;
        public Dictionary<int, int> StockItem = new Dictionary<int, int>();
        public int Sum = 0;

        public string LoginChoicePrompt = "Wybierz opcję logowania:";
        public string LoginChoiceErrorPrompt = "Błąd. Spróbuj ponownie.";
        public string AdminChoiceErrorPrompt = "Błąd. Spróbuj ponownie.";
        public string AdminChoicePrompt = "Podaj wybór:";
        public string QuantityInputPrompt = "Wprowadź ilość";
        public string QuantityErrorPrompt = "Błąd. Spróbuj ponownie.";
        public string BuyPrompt = "Co chcesz zamówić? Podaj Id produktu:";
        public string BuyErrorPrompt = "Błąd. Spróbuj ponownie.";
        public string ViewCartPrompt = "Wprowadź 0, żeby sprawdzić zamówienie.";
        public string ItemNotFound = "Nie znaleziono produktu. Spróbuj ponownie.";
        

        public void Begin()
        {
            Console.WriteLine("Wprowadź 0, żeby zalogować się jako administrator. Wprowadź 1, żeby zalogować się jako kasjer");
            int loginChoice = TakeUserInput(LoginChoicePrompt, LoginChoiceErrorPrompt);
            switch (loginChoice)
            {
                case 0:
                    Console.WriteLine("Zalogowano jako administrator");
                    DisplayItem();
                    AdminOperation();
                    break;
                case 1:
                    Console.WriteLine("Zalogowano jako kasjer");
                    CustomerOperation();
                    break;
                default:
                    Begin();
                    break;
            }
        }
        public void CustomerOperation()
        {
            DisplayItem();
            Console.WriteLine(ViewCartPrompt);
            int choice = TakeUserInput(BuyPrompt, BuyErrorPrompt);

            switch (choice)
            {
                case 0:
                    DisplayCart(BoughtItems);
                    break;
                default:
                    Item getItem = GetItem(choice);
                    if (getItem == null)
                    {
                        Console.WriteLine(ItemNotFound);
                        CustomerOperation();
                    }
                    else
                    {
                        AddToCart(getItem);
                        DisplayItem();
                    }
                    break;
            }
        }
        public Item GetItem(int choice)
        {
            foreach (Item t in Items)
            {
                if (choice == t.Id)
                    return t;
            }
            return null;
        }
        public void AddToCart(Item item)
        {
            string itemName = item.ItemName;
            Console.Write($"Wybrano {itemName} ");
            int quantity = TakeUserInput(QuantityInputPrompt, QuantityErrorPrompt);

            if (item.ItemStock >= quantity)
            {
                Console.WriteLine("Dodano produkt.");
                StockCheck(item, quantity);
                item.ItemStock -= quantity;
                CustomerOperation();
            }
            else
            {
                Console.WriteLine($"{quantity}, {itemName} niedostępny");
                CustomerOperation();
            }
        }
        public void StockCheck(Item item, int quantity)
        {
            if (!BoughtItems.ContainsKey(item.Id))
            {
                BoughtItems.Add(item.Id, new BoughtItem { Id = item.Id, Quantity = quantity, Item = item });
            }
            else
            {
                BoughtItems[item.Id].Quantity += quantity;
            }
        }
        public void DisplayCart(Dictionary<int, BoughtItem> boughtItemList)
        {
            double total = 0;
            Console.WriteLine("\n-----------------------------------------Zamówienie---------------------------------------\n");
            Console.WriteLine("Przedmiot\t\tIlość\t\tCena jednostkowa\t\tSuma");
            foreach (var pair in boughtItemList)
            {
                Sum += pair.Value.Quantity;
                double price = pair.Value.Quantity * pair.Value.Item.ItemPrice;
                Console.WriteLine(pair.Value.Item.ItemName + "\t\t" + pair.Value.Quantity + "\t\t\t" + pair.Value.Item.ItemPrice + "\t\t\t" + price);
                total += price;
            }
            Console.WriteLine("--------------------------------------------------------------------------------------------");
            Console.WriteLine($"Do zapłaty\t\t\t\t\t\t\t{total}");

            Console.WriteLine("\nDo złożenia kolejnego zamówienia wprowadź 0, żeby się wylogować wprowadź 1");
            int choice = TakeUserInput("Podaj swój wybór", "Wprowadzona wartość jest nieprawidłowa");
            if (choice == 0)
            {
                DisplayItem();
                CustomerOperation();
            }
            else
            {
                Begin();
            }
        }
        public void AdminOperation()
        {
            Console.WriteLine("Wprowadź 1, żeby dodać nowy produkt.");
            Console.WriteLine("Wprowadź 2, żeby zmienić stan.");
            Console.WriteLine("Wprowadź 3, żeby wyświetlić listę produktów.");
            Console.WriteLine("Wprowadź 4, żeby sprawdzić utarg.");
            Console.WriteLine("Wprowadź 5, żeby się wylogować.");
            
            int adminChoice = TakeUserInput(AdminChoicePrompt, AdminChoiceErrorPrompt);
            switch (adminChoice)
            {
                case (int)projekt.AdminOperation.AddItem:
                    AddItem();
                    break;
                case (int)projekt.AdminOperation.UpdateItem:
                    UpdateItem();
                    break;
                case (int)projekt.AdminOperation.DisplayItem:
                    DisplayItem();
                    AdminOperation();
                    break;
                case (int)projekt.AdminOperation.DisplayMoney:
                    DisplayMoney();
                    AdminOperation();
                    break;
                case (int)projekt.AdminOperation.Logout:
                    Begin();
                    DisplayItem();
                    break;
                default:
                    Console.WriteLine(AdminChoiceErrorPrompt);
                    AdminOperation();
                    break;
            }
        }
        public void AddItem()
        {
            Console.WriteLine("Podaj nazwę produktu: ");
            string name = Console.ReadLine();
            int price = TakeUserInput("Podaj cenę", "Błąd! Wprowadź poprawną cenę.");
            int quantity = TakeUserInput("Wprowadź ilość", "Błąd! Wprowadź odpowiednią ilosć.");

            Items.Add(new Item { Id = Items.Count + 1, ItemName = name, ItemPrice = price, ItemStock = quantity });
            Console.WriteLine("Sukces.");
            AdminOperation();
        }
        public void UpdateItem()
        {
            var input = TakeUserInput("Wybierz przedmiot, żeby dodać do magazynu.", AdminChoicePrompt);
            if (input != 4)
                if (input <= Items.Count)
                {
                    int quantity = TakeUserInput(QuantityInputPrompt, QuantityErrorPrompt);
                    if (quantity > 0)
                        if (Items != null) Items[input - 1].ItemStock += quantity;
                    DisplayItem();
                    AdminOperation();
                    return;
                }
                else
                {
                    Console.WriteLine(AdminChoiceErrorPrompt);
                    AdminOperation();
                }
            else
                DisplayItem();
            AdminOperation();
        }
        public void DisplayMoney()
        {
            
        }
        public void DisplayItem()
        {
            Console.WriteLine("Produkty");
            Console.WriteLine("===========================");
            Console.WriteLine("Id\tPrzedmiot\tCena\t Ilość");
            Console.WriteLine("---------------------------------------------");
            foreach (var item in Items)
            {
                Console.WriteLine(item.Id + "\t" + item.ItemName + "\t\t" + item.ItemPrice + "\t   " + item.ItemStock);
            }
        }
        public void DefaultInvnt()
        {
            Items = new List<Item>
            {
                
                new Item{ Id = 1, ItemName = "Pizza", ItemPrice = 29.99, ItemStock = 10},
                new Item{ Id = 2, ItemName = "Schab", ItemPrice = 15.99, ItemStock = 15 },
                new Item{ Id = 3, ItemName = "Kasza", ItemPrice = 15.99, ItemStock = 20 },
                new Item{ Id = 4, ItemName = "Pepsi", ItemPrice =  4.99, ItemStock = 10 }
            };
            BoughtItems = new Dictionary<int, BoughtItem>();
        }
        public int TakeUserInput(string inputPrompt, string errorPrompt)
        {
            Console.WriteLine(inputPrompt);
            var input = Console.ReadLine();
            try
            {
                return Convert.ToInt32(input);
            }
            catch (Exception)
            {
                Console.WriteLine(errorPrompt);
                return TakeUserInput(inputPrompt, errorPrompt);
            }
        }
    }
}
