using System.Windows;
using System.Windows.Controls;
using EscapeGame.Classes;

namespace EscapeGame
{
    public partial class MainWindow : Window
    {
        Room currentRoom;

        public MainWindow()
        {
            InitializeComponent();

            Room room1 = new Room()
            {
                Name = "slaapkamer",
                Description = "Ik bevind me in een middelgrote slaapkamer. Er is een locker aan de linkerkant, een mooi tapijt op de vloer en een bed aan de rechterkant. "
            };

            Item key1 = new Item()
            {
                Name = "kleine zilveren sleutel",
                Description = "Een kleine zilveren sleutel, doet me denken aan eentje die ik had op school."
            };

            Item key2 = new Item()
            {
                Name = "grote sleutel",
                Description = "Een grote sleutel. Zou dit mijn uitweg zijn?"
            };

            Item locker = new Item()
            {
                Name = "locker",
                Description = "Een locker. Ik vraag me af wat er in zit.",
                IsPortable = false
            };
            locker.HiddenItem = key2;
            locker.IsLocked = true;
            locker.Key = key1;

            Item bed = new Item()
            {
                Name = "bed",
                Description = "Gewoon een bed. Ik ben nu niet moe.",
                IsPortable = false
            };

            room1.Items.Add(new Item()
            {
                Name = "stoel",
                Description = "Een gewone houten stoel. Niet erg comfortabel.",
                IsPortable = false
            });
            room1.Items.Add(new Item()
            {
                Name = "poster",
                Description = "Een poster van een zonsondergang. Prachtig."
            });
            room1.Items.Add(bed);
            room1.Items.Add(locker);

            currentRoom = room1;
            txtMessage.Text = "Ik ben wakker, maar kan me niet herinneren wie ik ben!? Dat moet een wilde nacht geweest zijn... ";
            txtRoomDesc.Text = currentRoom.Description;
            UpdateUI();
        }

        private void UpdateUI()
        {
            lstRoomItems.Items.Clear();
            foreach (Item itm in currentRoom.Items)
            {
                lstRoomItems.Items.Add(itm);
            }
        }

        private void LstItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnCheck.IsEnabled = lstRoomItems.SelectedValue != null;
            btnPickUp.IsEnabled = lstRoomItems.SelectedValue != null;
            btnUseOn.IsEnabled = lstRoomItems.SelectedValue != null && lstMyItems.SelectedValue != null;
            btnDrop.IsEnabled = lstMyItems.SelectedValue != null;
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            Item roomItem = (Item)lstRoomItems.SelectedItem;

            if (roomItem.IsLocked)
            {
                txtMessage.Text = $"{roomItem.Description}. Het is stevig afgesloten. ";
                return;
            }

            Item foundItem = roomItem.HiddenItem;
            if (foundItem != null)
            {
                txtMessage.Text = $"Oh, kijk, ik heb een {foundItem.Name} gevonden! ";
                lstMyItems.Items.Add(foundItem);
                roomItem.HiddenItem = null;
                return;
            }

            txtMessage.Text = roomItem.Description;
        }

        private void BtnUseOn_Click(object sender, RoutedEventArgs e)
        {
            Item myItem = (Item)lstMyItems.SelectedItem;
            Item roomItem = (Item)lstRoomItems.SelectedItem;


            if (roomItem.Key != myItem)
            {
                txtMessage.Text = "Dat lijkt niet te werken. ";
                return;
            }

            roomItem.IsLocked = false;
            roomItem.Key = null;
            lstMyItems.Items.Remove(myItem);
            txtMessage.Text = $"Ik heb de {roomItem.Name} zojuist ontgrendeld!";
        }

        private void BtnPickUp_Click(object sender, RoutedEventArgs e)
        {
            Item selItem = (Item)lstRoomItems.SelectedItem;
            if (!selItem.IsPortable)
            {
                txtMessage.Text = $"Ik kan de {selItem.Name} niet oppakken. ";
                return;
            }

            txtMessage.Text = $"Ik heb de {selItem.Name} opgepakt. ";
            lstMyItems.Items.Add(selItem);
            lstRoomItems.Items.Remove(selItem);
            currentRoom.Items.Remove(selItem);
        }
        private void BtnDrop_Click(object sender, RoutedEventArgs e)
        {
            Item selItem = (Item)lstMyItems.SelectedItem;

            txtMessage.Text = $"Ik heb de {selItem.Name} teruggelegd. ";
            currentRoom.Items.Add(selItem);
            lstMyItems.Items.Remove(selItem);
            UpdateUI();
        }
    }
}