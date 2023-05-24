using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Plantvill;
using System.IO;
using Newtonsoft.Json;
using System.Reflection;
using System.Net.Http;

namespace week08
{
    public partial class Interface : Window
    {
        private string dataFilePath = "data.txt";
        private string username;

        public Interface(string username)
        {
            InitializeComponent();
            DataContext = LoadPlayerData();
            var playerdata = (Player)DataContext;
            playerdata.Username = username;
            SeedsEmporiumPage();
        }

        private Player LoadPlayerData()
        {
            if (File.Exists(dataFilePath))
            {
                string json = File.ReadAllText(dataFilePath);
                return JsonConvert.DeserializeObject<Player>(json);
            }
            else
            {
                Player playerData = Player.GetPlayerData();
                SavePlayerData(playerData);
                return playerData;
            }
        }

        private void SavePlayerData(Player playerData)
        {
            string json = JsonConvert.SerializeObject(playerData);
            File.WriteAllText(dataFilePath, json);
        }

        public List<Seed> getSeedAvailable()
        {
            List<Seed> seed_list = new List<Seed>();
            DefaultSeeds d = new DefaultSeeds();
            Seed defaultStrawberrySeed = d.StrawberrySeed;
            Seed defaultSpinachSeed = d.SpinachSeed;
            Seed defaultPearSeed = d.PearSeed;

            seed_list.Add(defaultStrawberrySeed);
            seed_list.Add(defaultSpinachSeed);
            seed_list.Add(defaultPearSeed);

            return seed_list;
        }
        public void SeedsEmporiumPage()
        {
            SetPage("Seeds Emporium", "What would you like to purchase ?", true, false, false, "", false);
            myListBox.SelectionChanged += SeedSelected;
            List<Seed> availableSeeds = getSeedAvailable();
            foreach (Seed s in availableSeeds)
            {
                myListBox.Items.Add(s.name + "  $" + s.price);
            }
        }
        public void ChatPage(object e, RoutedEventArgs r)
        {
            SetPage("Chat", "", true, true, true, "Send", false);
            BottomButton.Width = 80;
            BottomButton.Click += Send;
        }

        public void TradeMarketPage(object sender, RoutedEventArgs r)
        {
            SetPage("Trade Marketplace", "", true, false, true, "Accept Trade", false);
            // BottomButton.Click += Send;
        }

        public void ProposeTradePage(object sender, RoutedEventArgs r)
        {
            SetPage("Propose Trade", "", false, false, true, "Submit", true);
            // BottomButton.Click += Send;
        }


        public void displaySeedsPage(object sender, RoutedEventArgs e)
        {
            myListBox.Items.Clear();
            SeedsEmporiumPage();
        }
        private void SeedSelected(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (myListBox.SelectedItem != null)
                {
                    string selectedSeed = myListBox.SelectedItem.ToString();
                    string[] seedInfo = selectedSeed.Split('$');
                    string seedName = seedInfo[0].Trim();
                    int seedPrice = int.Parse(seedInfo[1].Trim());
                    PurchaseSeed(seedName, seedPrice);
                    myListBox.SelectedItem = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Selection Changed Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void PurchaseSeed(string seedName, int seedPrice)
        {
            Player playerData = (Player)DataContext;
            if (playerData.Money > seedPrice)
            {
                if (playerData.LandPots > 0)
                {
                    playerData.Money -= seedPrice;
                    playerData.LandPots -= 1;
                    MessageBox.Show($"Congratulations! You have successfully purchased {seedName} for ${seedPrice}.", "Purchase Confirmation");
                    AddToInventory(seedName);
                }
                else
                {
                    MessageBox.Show($"You don't have enough land pots to buy this item");

                }
            }
            else
            {
                MessageBox.Show($"You cannot afford this item !");
            }
            DataContext = playerData;
            SavePlayerData(playerData);

        }
        private void AddToInventory(string seedname)
        {
            Player playerData = (Player)DataContext;
            DefaultSeeds d = new DefaultSeeds();
            switch (seedname)
            {
                case "Strawberry":
                    Seed defaultStrawberrySeed = d.StrawberrySeed;
                    playerData.Myseed.Add(defaultStrawberrySeed);
                    break;
                case "Spinach":
                    Seed defaultSpinachSeed = d.SpinachSeed;
                    playerData.Myseed.Add(defaultSpinachSeed);
                    break;
                case "Pears":
                    Seed defaultPearSeed = d.PearSeed;
                    playerData.Myseed.Add(defaultPearSeed);
                    break;
            }
            DataContext = playerData;
            SavePlayerData(playerData);
        }
        public void GardenPage(object sender, RoutedEventArgs e)
        {
            try
            {
                SetPage("Garden", "What would you like to harvest ?", true, false, true, "Harvest all", false);
                BottomButton.Click += HarvestAll;
                BottomButton.Width = 80;
                myListBox.SelectionChanged += Harvest;
                Player playerdata = (Player)DataContext;
                myListBox.Items.Clear();
                foreach (Seed s in playerdata.Myseed)
                {
                    TimeSpan elapsed = DateTime.UtcNow - s.timePurchased;
                    TimeSpan timeLeftToHarvest = s.timeToHarvest - elapsed;
                    TimeSpan timeLeftBeforeSpoiled = s.timeBeforeSpoiled - elapsed;
                    if (timeLeftToHarvest > TimeSpan.Zero)
                    {
                        myListBox.Items.Add(s.name + " " + timeLeftToHarvest.ToString("mm':'ss") + " left (harvest)");
                    }
                    else if (timeLeftToHarvest <= TimeSpan.Zero)
                    {
                        if (timeLeftBeforeSpoiled <= TimeSpan.Zero)
                        {
                            myListBox.Items.Add(s.name + " " + "spoiled");
                        }
                        else
                        {
                            myListBox.Items.Add(s.name + " " + timeLeftBeforeSpoiled.ToString("mm':'ss") + " left (spoiled)");
                        }
                    }
                }
                if (playerdata.Myseed.Count == 0)
                {
                    myListBox.Items.Add("No plants in the garden.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Selection Changed Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        public void InventoryPage(object sender, RoutedEventArgs e)
        {
            try
            {
                SetPage("Inventory", "10$ for each trip to farmer's market", true, false, true, "Sell in farmer's market", false);
                BottomButton.Click += SellItems;
                BottomButton.Width = 150;
                Player playerdata = (Player)DataContext;
                myListBox.Items.Clear();
                var groupedSeeds = playerdata.SeedHarvested
                        .GroupBy(s => s.name)
                        .Select(g => new
                        {
                            Name = g.Key,
                            Count = g.Count(),
                            Price = g.First().reward
                        })
                        .ToList();
                foreach (var s in groupedSeeds)
                {
                    myListBox.Items.Add($"{s.Name} [{s.Count}] {s.Price}$ ");
                }
                if (playerdata.SeedHarvested.Count == 0)
                {
                    myListBox.Items.Add("No fruits or vegetables harvested.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Selection Changed Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void Harvest(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var playerData = (Player)DataContext;
                if (myListBox.SelectedItem != null && playerData.Myseed.Count > 0)
                {
                    string selectedSeed = myListBox.SelectedItem.ToString();
                    string[] seedInfo = selectedSeed.Split(' ');
                    string seedName = seedInfo[0].Trim();
                    string seedTime = seedInfo[1].Trim();
                    string seedState = seedInfo[3].Trim();
                    DefaultSeeds defaultSeeds = new DefaultSeeds();
                    Seed s = null;
                    switch (seedName)
                    {
                        case "Strawberry":
                            s = defaultSeeds.StrawberrySeed;
                            break;
                        case "Spinach":
                            s = defaultSeeds.SpinachSeed;
                            break;
                        case "Pears":
                            s = defaultSeeds.PearSeed;
                            break;
                    }

                    if (seedTime != "00:00" && seedState == "(spoiled)")
                    {
                        playerData.SeedHarvested.Add(s);
                        MessageBox.Show($"{s.name} harvested !");
                    }
                    else
                    {
                        MessageBox.Show($"{s.name} lost, not ready to be harvested yet !");
                    }
                    playerData.LandPots++;
                    int indexToRemove = playerData.Myseed.FindIndex(seed => seed.name == s.name);
                    playerData.Myseed.RemoveAt(indexToRemove);
                    myListBox.Items.Remove(myListBox.SelectedItem);
                    myListBox.SelectedItem = null;
                    if (playerData.Myseed.Count == 0)
                    {
                        myListBox.Items.Add("No plants in the garden");
                    }
                    playerData = (Player)DataContext;
                    SavePlayerData(playerData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Selection Changed Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
        private void HarvestAll(object sender, RoutedEventArgs e)
        {
            var playerData = (Player)DataContext;
            foreach (Seed s in playerData.Myseed)
            {
                TimeSpan elapsed = DateTime.UtcNow - s.timePurchased;
                TimeSpan timeLeftToHarvest = s.timeToHarvest - elapsed;
                TimeSpan timeLeftBeforeSpoiled = s.timeBeforeSpoiled - elapsed;
                if (timeLeftToHarvest <= TimeSpan.Zero && timeLeftBeforeSpoiled > TimeSpan.Zero)
                {
                    playerData.SeedHarvested.Add(s);
                    MessageBox.Show($"{s.name} harvested !");
                }
                else if (timeLeftToHarvest > TimeSpan.Zero)
                {
                    MessageBox.Show($"{s.name} lost, not ready to be harvested yet !");
                }
                else if (timeLeftBeforeSpoiled == TimeSpan.Zero)
                {
                    MessageBox.Show($"{s.name} spoiled !");
                }

            }
            if (playerData.Myseed.Count == 0)
            {
                MessageBox.Show("Nothing to harvest");
            }
            playerData.LandPots = 15;
            playerData.Myseed.Clear();
            myListBox.Items.Clear();
            myListBox.Items.Add("No plants in the garden.");
            DataContext = playerData;
            SavePlayerData(playerData);

        }
        public void SellItems(object sender, RoutedEventArgs r)
        {
            var playerData = (Player)DataContext;
            int currentMoney = playerData.Money;
            MessageBoxResult result = MessageBoxResult.No;
            if (playerData.SeedHarvested.Count == 0)
            {
                result = MessageBox.Show("You don't have any items in your inventory, do you still want to proceed ?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
            }

            if (playerData.SeedHarvested.Count > 0 || result == MessageBoxResult.Yes)
            {
                if (playerData.Money > 10)
                {
                    foreach (Seed s in playerData.SeedHarvested)
                    {
                        playerData.Money = playerData.Money + s.reward;
                    }
                    playerData.Money = playerData.Money - 10;
                    MessageBox.Show($"Clear inventory, you made {playerData.Money - currentMoney}$");
                    playerData.SeedHarvested.Clear();
                    myListBox.Items.Clear();
                    myListBox.Items.Add("No fruits or vegetables harvested.");
                    SavePlayerData(playerData);
                }
                else
                {
                    MessageBox.Show("Your credit is not sufficient to sell your items ($10)");
                }
            }

        }
        private void Send(object sender, RoutedEventArgs e)
        {
            var playerData = (Player)DataContext;
            var username = playerData.Username;
            var message = SendInput.Text;

            if (!string.IsNullOrEmpty(message))
            {
                PostChatMessage(username, message);
            }
            SendInput.Text = "";
        }
        private async void LoadChatMessages()
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync("http://plantville.herokuapp.com/");
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        var messages = JsonConvert.DeserializeObject<List<ChatMessage>>(json);
                        myListBox.ItemsSource = messages;
                    }
                    else
                    {
                        MessageBox.Show(response.StatusCode.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        private async void PostChatMessage(string username, string message)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        { "username", username },
                        { "message", message }
                    });

                    var response = await httpClient.PostAsync("http://plantville.herokuapp.com/", content);
                    if (response.IsSuccessStatusCode)
                    {
                        LoadChatMessages();
                    }
                    else
                    {
                        MessageBox.Show(response.ReasonPhrase.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }
        public static void RemoveRoutedEventHandlers(UIElement element, RoutedEvent routedEvent)
        {
            var eventHandlersStoreProperty = typeof(UIElement).GetProperty(
                "EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic);
            object eventHandlersStore = eventHandlersStoreProperty.GetValue(element, null);
            if (eventHandlersStore == null)
                return;
            var getRoutedEventHandlers = eventHandlersStore.GetType().GetMethod(
                "GetRoutedEventHandlers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var routedEventHandlers = (RoutedEventHandlerInfo[])getRoutedEventHandlers.Invoke(
                eventHandlersStore, new object[] { routedEvent });
            foreach (var routedEventHandler in routedEventHandlers)
                element.RemoveHandler(routedEvent, routedEventHandler.Handler);
        }

        public static void RemoveSelectionChangedEventHandlers(ListBox listBox)
        {
            var eventHandlersStoreProperty = typeof(UIElement).GetProperty(
                "EventHandlersStore", BindingFlags.Instance | BindingFlags.NonPublic);
            object eventHandlersStore = eventHandlersStoreProperty.GetValue(listBox, null);
            if (eventHandlersStore == null)
                return;
            var getRoutedEventHandlers = eventHandlersStore.GetType().GetMethod(
                "GetRoutedEventHandlers", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var selectionChangedEvent = ListBox.SelectionChangedEvent;
            var routedEventHandlers = (RoutedEventHandlerInfo[])getRoutedEventHandlers.Invoke(eventHandlersStore, new object[] { selectionChangedEvent });
            foreach (var routedEventHandler in routedEventHandlers)
                listBox.RemoveHandler(selectionChangedEvent, routedEventHandler.Handler);
        }

        private void SetPage(string Title, string Description, bool listVisibility, bool InputVisibility, bool buttonVisibility, string ButtonContent, bool tradeMenuVisibility)
        {
            TitleSection.Text = Title;
            DescriptionSection.Text = Description;
            RemoveRoutedEventHandlers(BottomButton, Button.ClickEvent);
            RemoveSelectionChangedEventHandlers(myListBox);
            myListBox.Visibility = listVisibility ? Visibility.Visible : Visibility.Collapsed;
            BottomButton.Visibility = buttonVisibility ? Visibility.Visible : Visibility.Collapsed;
            SendInput.Visibility = InputVisibility ? Visibility.Visible : Visibility.Collapsed;
            tradeExchange.Visibility = tradeMenuVisibility ? Visibility.Visible : Visibility.Collapsed;
            myListBox.Items.Clear();
            BottomButton.Content = ButtonContent;
        }
        // private void SimulateButtonClick(object o, RoutedEventArgs r)
        // {
        //     RoutedEventArgs eventArgs = new RoutedEventArgs(Button.ClickEvent);
        //     BottomButton.RaiseEvent(eventArgs);
        // }
        public class ChatMessage
        {
            public string Username { get; set; }
            public string Message { get; set; }
        }
    }
}
public class Player : INotifyPropertyChanged
{
    private string _username;
    private int _money = 100;
    private int _landpots = 15;

    private List<Seed> _mySeed = new List<Seed>();
    private List<Seed> _seedHarvested = new List<Seed>();
    public List<Seed> Myseed
    {
        get { return _mySeed; }
        set
        {
            _mySeed = value;
            OnPropertyChanged(nameof(Myseed));
        }
    }
    public List<Seed> SeedHarvested
    {
        get { return _seedHarvested; }
        set
        {
            _seedHarvested = value;
            OnPropertyChanged(nameof(SeedHarvested));
        }
    }
    public int Money
    {
        get { return _money; }
        set
        {
            _money = value;
            OnPropertyChanged(nameof(Money));
        }
    }

    public int LandPots
    {
        get { return _landpots; }
        set
        {
            _landpots = value;
            OnPropertyChanged(nameof(LandPots));
        }
    }
    public string Username
    {
        get { return _username; }
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    public static Player GetPlayerData()
    {
        return new Player();

    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}
