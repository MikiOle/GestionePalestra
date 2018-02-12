using PalestraTest.App_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace PalestraTest
{
    /// <summary>
    /// Logica di interazione per GestioneIngressi.xaml
    /// </summary>
    public partial class GestioneIngressi : Window
    {

        DispatcherTimer timer = new DispatcherTimer();

        public GestioneIngressi()
        {
            InitializeComponent();
            Standyby();
        }

        public void cercaUtente(string pk)
        {
            try
            {
                UtentiPalestraEntities ent = new UtentiPalestraEntities();
                Utente utente = ent.Utenti
                    .Where(a => a.ID_Tessera == pk)
                    .FirstOrDefault();
                if (utente == null)
                {
                    MessageBox.Show("Non esiste l'utente specificato! PK:" + pk);
                    return;
                }

                Entrata();
                MainWindow.getInstance().ControlloAbbonamenti();
                MainWindow.getInstance().LogInfo("Entrata utente '" + utente.Nome.Trim() + " " + utente.Cognome.Trim() + "'");
                lblBenvenuto.Content = "Benvenuto " + utente.Nome.Trim() + " " + utente.Cognome.Trim();
                if (utente.RapportoClienti.Abilitato)
                {
                    if (!utente.RapportoClienti.PrezziAbbonamenti.is_periodico)
                    {
                        utente.RapportoClienti.N_EntrateRimanenti = utente.RapportoClienti.N_EntrateRimanenti - 1;
                        lblDettagli.Content = "Entrate rimaste: " + utente.RapportoClienti.N_EntrateRimanenti;
                        ent.SaveChanges();
                    }
                    else
                    {
                        lblDettagli.Content = "Data scadenza abbonamento: " + (utente.RapportoClienti.Data_Scadenza).Value.ToString("dd/MM/yyyy");
                    }

                    if (utente.DataNascita.Value.Day == DateTime.Now.Day && utente.DataNascita.Value.Month == DateTime.Now.Month && utente.FesteggiaCompleanno)
                    {
                        imgCompleanno.Visibility = Visibility.Visible;
                        lblDettagli.Content += "\n\n Buon compleanno " + utente.Nome.Trim() + " e buon allenamento!";
                    }
                    else
                    {
                        lblDettagli.Content += "\n\n Buon allenamento!";
                    }
                    MainWindow.getInstance().Tornello(true);
                }
                else
                {
                    lblDettagli.Content = "L'utente non è abilitato: \n " + utente.RapportoClienti.Casi_disattivazione_abbonamento.motivo;
                }

                timer.Interval = TimeSpan.FromSeconds(5);
                timer.Tick += timer_Tick;
                timer.Start();
            }
            catch (Exception ex)
            {
                MainWindow.getInstance().LogError(MainWindow.getInstance().getInnerException(ex).Message);
                MessageBox.Show(ex.Message);
            }
        }

        public void InfoUtente(Utente ut)
        {
            
        }


        void timer_Tick(object sender, EventArgs e)
        {
            ClearAll();
            timer.Stop();
            Standyby();
        }

        public void ClearAll()
        {
            lblDettagli.Content = "";
            lblBenvenuto.Content = "";
        }

        public void Standyby()
        {
            gridBenvenuto.Visibility = Visibility.Visible;
            gridDettagliEntrata.Visibility = Visibility.Hidden;
            imgCompleanno.Visibility = Visibility.Hidden;
        }

        public void Entrata()
        {
            gridBenvenuto.Visibility = Visibility.Hidden;
            gridDettagliEntrata.Visibility = Visibility.Visible;
        }
    }
}
