using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFDokter
{
    /// <summary>
    /// Detailpagina van een patiënt. Toont alle gegevens van het geselecteerde dossier.
    /// </summary>
    public partial class PatientDetailsPage : Page
    {
        private Patient _patient;

        /// <summary>
        /// Initialiseert de detailpagina voor de opgegeven patiënt.
        /// </summary>
        /// <param name="patient">De patiënt waarvan de details worden getoond.</param>
        public PatientDetailsPage(Patient patient)
        {
            InitializeComponent();
            _patient = patient;
        }
    }
}
