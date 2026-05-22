using System.Windows.Controls;
using DokterspraktijkLib;

namespace WPFDokter
{
    /// <summary>
    /// Formulierpagina voor het aanmaken of bewerken van een patiëntdossier.
    /// Bij een nieuwe patiënt wordt een lege Patient meegegeven; bij bewerken de bestaande.
    /// </summary>
    public partial class PatientWijzigenPage : Page
    {
        private Patient _patient;

        /// <summary>
        /// Initialiseert de wijzigingspagina met de opgegeven patiënt.
        /// </summary>
        /// <param name="patient">Lege Patient voor nieuw dossier, of bestaande Patient voor bewerken.</param>
        public PatientWijzigenPage(Patient patient)
        {
            InitializeComponent();
            _patient = patient;
        }
    }
}
