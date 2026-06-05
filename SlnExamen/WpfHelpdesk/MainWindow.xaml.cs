using System.Configuration;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CLHelpdesk;

namespace WpfHelpdesk;

/// <summary>
/// Code-behind voor het hoofdvenster van de helpdesk-applicatie.
/// Beheert de weergave, filtering en invoer van tickets.
/// </summary>
public partial class MainWindow : Window
{
    /// <summary>Volledige lijst van alle geladen tickets.</summary>
    private List<Ticket> _tickets = new List<Ticket>();

    /// <summary>Unieke lijst van melders opgebouwd uit de geladen tickets.</summary>
    private List<Medewerker> _medewerkers = new List<Medewerker>();

    /// <summary>Pad naar het CSV-bestand, gelezen uit App.config.</summary>
    private string _csvPad;

    /// <summary>
    /// Initialiseert het venster: laadt CSV, vult comboboxen en listbox.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        _csvPad = ConfigurationManager.AppSettings["CsvPad"] ?? "tickets.csv";

        try
        {
            _tickets = TicketRepository.LaadTickets(_csvPad);
        }
        catch (FileNotFoundException)
        {
            // Bestand bestaat nog niet bij eerste gebruik, start met lege lijst
            _tickets = new List<Ticket>();
        }
        catch (IOException ex)
        {
            MessageBox.Show($"Fout bij het laden van het bestand:\n{ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            _tickets = new List<Ticket>();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Onverwachte fout:\n{ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            _tickets = new List<Ticket>();
        }

        VulMelderComboBoxes();
        VulListBox();
        UpdateButtonStates();
    }

    /// <summary>
    /// Vult cmbMelder en cmbMelderNieuw met unieke melders uit de geladen tickets.
    /// Bouwt tegelijk de interne _medewerkers-lijst op voor later opzoeken.
    /// </summary>
    private void VulMelderComboBoxes()
    {
        _medewerkers.Clear();
        cmbMelder.Items.Clear();
        cmbMelder.Items.Add("Alle");
        cmbMelderNieuw.Items.Clear();

        foreach (Ticket ticket in _tickets)
        {
            // Controleer of de melder al in de lijst staat
            bool bestaatAl = false;
            foreach (Medewerker m in _medewerkers)
            {
                if (m.Voornaam == ticket.Melder.Voornaam && m.Achternaam == ticket.Melder.Achternaam)
                {
                    bestaatAl = true;
                    break;
                }
            }

            if (!bestaatAl)
            {
                _medewerkers.Add(ticket.Melder);
                string naam = $"{ticket.Melder.Voornaam} {ticket.Melder.Achternaam}";
                cmbMelder.Items.Add(naam);
                cmbMelderNieuw.Items.Add(naam);
            }
        }

        cmbMelder.SelectedIndex = 0;
        if (cmbMelderNieuw.Items.Count > 0)
            cmbMelderNieuw.SelectedIndex = 0;
    }

    /// <summary>
    /// Wist en herlaadt de listbox op basis van de actieve filterinstellingen.
    /// Filters: prioriteit, melder en alleen-open-tickets.
    /// </summary>
    private void VulListBox()
    {
        if (lbxTickets == null) return;

        string geselecteerdePrioriteit = (cmbPrioriteit.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Alle";
        string geselecteerdeMelder = cmbMelder.SelectedItem?.ToString() ?? "Alle";
        bool alleenOpen = chkAlleenOpen.IsChecked == true;

        lbxTickets.Items.Clear();

        foreach (Ticket ticket in _tickets)
        {
            // Filter op prioriteit (sla over als niet overeenkomt)
            if (geselecteerdePrioriteit != "Alle" && ticket.Prioriteit.ToString() != geselecteerdePrioriteit)
                continue;

            // Filter op melder (sla over als niet overeenkomt)
            if (geselecteerdeMelder != "Alle")
            {
                string melderNaam = $"{ticket.Melder.Voornaam} {ticket.Melder.Achternaam}";
                if (melderNaam != geselecteerdeMelder)
                    continue;
            }

            // Filter op open tickets (sla afgesloten tickets over indien aangevinkt)
            if (alleenOpen && ticket.IsAfgesloten)
                continue;

            lbxTickets.Items.Add(ticket);
        }
    }

    /// <summary>
    /// Beheert de ingeschakelde staat van btnToevoegen en btnAfsluiten.
    /// Wordt aangeroepen bij elke relevante wijziging in het venster.
    /// </summary>
    private void UpdateButtonStates()
    {
        if (btnToevoegen == null || btnAfsluiten == null) return;

        // btnToevoegen enkel actief als de titel ingevuld is
        btnToevoegen.IsEnabled = !string.IsNullOrWhiteSpace(txbTitel.Text);

        // btnAfsluiten enkel actief als een ticket geselecteerd én nog open is
        Ticket? geselecteerdTicket = lbxTickets.SelectedItem as Ticket;
        btnAfsluiten.IsEnabled = geselecteerdTicket != null && !geselecteerdTicket.IsAfgesloten;
    }

    /// <summary>
    /// Toont de details van het geselecteerde ticket in tbkDetails.
    /// </summary>
    private void lbxTickets_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Ticket? ticket = lbxTickets.SelectedItem as Ticket;

        if (ticket == null)
        {
            tbkDetails.Text = "";
            UpdateButtonStates();
            return;
        }

        tbkDetails.Text = ticket.GeefInfo();
        UpdateButtonStates();
    }

    /// <summary>
    /// Herlaadt de listbox wanneer de prioriteitsfilter wijzigt.
    /// </summary>
    private void cmbPrioriteit_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        VulListBox();
        UpdateButtonStates();
    }

    /// <summary>
    /// Herlaadt de listbox wanneer de melderfilter wijzigt.
    /// </summary>
    private void cmbMelder_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        VulListBox();
        UpdateButtonStates();
    }

    /// <summary>
    /// Herlaadt de listbox wanneer de checkbox "Alleen open tickets" wijzigt.
    /// </summary>
    private void chkAlleenOpen_Changed(object sender, RoutedEventArgs e)
    {
        VulListBox();
        UpdateButtonStates();
    }

    /// <summary>
    /// Schakelt btnToevoegen in of uit naargelang de titel ingevuld is.
    /// </summary>
    private void txbTitel_TextChanged(object sender, TextChangedEventArgs e)
    {
        UpdateButtonStates();
    }

    /// <summary>
    /// Valideert de invoervelden, maakt een nieuw ticket aan en slaat het op.
    /// </summary>
    private void btnToevoegen_Click(object sender, RoutedEventArgs e)
    {
        // Titel is al gevalideerd via btnToevoegen.IsEnabled, maar dubbele check is veilig
        if (string.IsNullOrWhiteSpace(txbTitel.Text))
        {
            MessageBox.Show("Geef een titel in.", "Validatiefout", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (cmbMelderNieuw.SelectedItem == null)
        {
            MessageBox.Show("Selecteer een melder.", "Validatiefout", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (cmbPrioriteitNieuw.SelectedItem == null)
        {
            MessageBox.Show("Selecteer een prioriteit.", "Validatiefout", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (cmbType.SelectedItem == null)
        {
            MessageBox.Show("Selecteer een type (Hardware of Software).", "Validatiefout", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(txbExtraInfo.Text))
        {
            MessageBox.Show("Geef een toestel of applicatie in.", "Validatiefout", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Zoek het Medewerker-object op dat overeenkomt met de geselecteerde naam
        Medewerker? melder = null;
        string geselecteerdeMelder = cmbMelderNieuw.SelectedItem.ToString() ?? "";
        foreach (Medewerker m in _medewerkers)
        {
            if ($"{m.Voornaam} {m.Achternaam}" == geselecteerdeMelder)
            {
                melder = m;
                break;
            }
        }

        if (melder == null)
        {
            MessageBox.Show("Melder niet gevonden.", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        Enum.TryParse((cmbPrioriteitNieuw.SelectedItem as ComboBoxItem)?.Content.ToString(), out TicketPrioriteit prioriteit);
        string type = (cmbType.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "";

        Ticket nieuwTicket;
        if (type == "Hardware")
        {
            nieuwTicket = new HardwareTicket
            {
                Id = _tickets.Count + 1,
                Titel = txbTitel.Text,
                Melder = melder,
                Prioriteit = prioriteit,
                IsAfgesloten = false,
                DatumAangemaakt = DateTime.Now,
                Toestel = txbExtraInfo.Text
            };
        }
        else
        {
            nieuwTicket = new SoftwareTicket
            {
                Id = _tickets.Count + 1,
                Titel = txbTitel.Text,
                Melder = melder,
                Prioriteit = prioriteit,
                IsAfgesloten = false,
                DatumAangemaakt = DateTime.Now,
                Applicatie = txbExtraInfo.Text
            };
        }

        _tickets.Add(nieuwTicket);

        try
        {
            TicketRepository.SlaTicketOp(_csvPad, nieuwTicket);
        }
        catch (IOException ex)
        {
            MessageBox.Show($"Fout bij het opslaan:\n{ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Onverwachte fout:\n{ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        VulListBox();

        // Wis invoervelden na succesvol toevoegen
        txbTitel.Text = "";
        cmbPrioriteitNieuw.SelectedIndex = -1;
        cmbType.SelectedIndex = -1;
        txbExtraInfo.Text = "";
    }

    /// <summary>
    /// Sluit het geselecteerde ticket af en herschrijft het volledige CSV-bestand.
    /// </summary>
    private void btnAfsluiten_Click(object sender, RoutedEventArgs e)
    {
        Ticket? ticket = lbxTickets.SelectedItem as Ticket;
        if (ticket == null) return;

        ticket.IsAfgesloten = true;
        ticket.DatumAfgesloten = DateTime.Now;

        try
        {
            // Volledig herschrijven zodat de gewijzigde status bewaard blijft
            TicketRepository.SlaAlleTicketsOp(_csvPad, _tickets);
        }
        catch (IOException ex)
        {
            MessageBox.Show($"Fout bij het opslaan:\n{ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Onverwachte fout:\n{ex.Message}", "Fout", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        VulListBox();
        UpdateButtonStates();
    }
}
