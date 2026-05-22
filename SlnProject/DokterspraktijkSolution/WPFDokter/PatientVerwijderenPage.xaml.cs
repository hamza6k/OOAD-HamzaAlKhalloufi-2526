using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFDokter
{
    /// <summary>
    /// Bevestigingspagina voor het verwijderen van een patiëntdossier inclusief alle afspraken.
    /// </summary>
    public partial class PatientVerwijderenPage : Page
    {
        private Patient _patient;

        /// <summary>
        /// Initialiseert de verwijderingspagina voor de opgegeven patiënt.
        /// </summary>
        /// <param name="patient">De patiënt die verwijderd zal worden.</param>
        public PatientVerwijderenPage(Patient patient)
        {
            InitializeComponent();
            _patient = patient;
        }
    }
}
