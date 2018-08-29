//se non scrivo niente nella textbox dell ID non segna alcun errore !
using System;
using System.Collections.Generic;
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
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Threading;
using PalestraTest.App_Data;
using System.Data.Entity;
using RawInputProcessor;
using System.Diagnostics;

namespace PalestraTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Libreria gestione DB25
        #region lib
        [DllImport("inpout32.dll")]
        private static extern UInt32 IsInpOutDriverOpen();
        [DllImport("inpout32.dll")]
        private static extern void Out32(short PortAddress, short Data);
        [DllImport("inpout32.dll")]
        private static extern char Inp32(short PortAddress);

        [DllImport("inpout32.dll")]
        private static extern void DlPortWritePortUshort(short PortAddress, ushort Data);
        [DllImport("inpout32.dll")]
        private static extern ushort DlPortReadPortUshort(short PortAddress);

        [DllImport("inpout32.dll")]
        private static extern void DlPortWritePortUlong(int PortAddress, uint Data);
        [DllImport("inpout32.dll")]
        private static extern uint DlPortReadPortUlong(int PortAddress);

        [DllImport("inpoutx64.dll")]
        private static extern bool GetPhysLong(ref int PortAddress, ref uint Data);
        [DllImport("inpoutx64.dll")]
        private static extern bool SetPhysLong(ref int PortAddress, ref uint Data);


        [DllImport("inpoutx64.dll", EntryPoint = "IsInpOutDriverOpen")]
        private static extern UInt32 IsInpOutDriverOpen_x64();
        [DllImport("inpoutx64.dll", EntryPoint = "Out32")]
        private static extern void Out32_x64(short PortAddress, short Data);
        [DllImport("inpoutx64.dll", EntryPoint = "Inp32")]
        private static extern char Inp32_x64(short PortAddress);

        [DllImport("inpoutx64.dll", EntryPoint = "DlPortWritePortUshort")]
        private static extern void DlPortWritePortUshort_x64(short PortAddress, ushort Data);
        [DllImport("inpoutx64.dll", EntryPoint = "DlPortReadPortUshort")]
        private static extern ushort DlPortReadPortUshort_x64(short PortAddress);

        [DllImport("inpoutx64.dll", EntryPoint = "DlPortWritePortUlong")]
        private static extern void DlPortWritePortUlong_x64(int PortAddress, uint Data);
        [DllImport("inpoutx64.dll", EntryPoint = "DlPortReadPortUlong")]
        private static extern uint DlPortReadPortUlong_x64(int PortAddress);

        [DllImport("inpoutx64.dll", EntryPoint = "GetPhysLong")]
        private static extern bool GetPhysLong_x64(ref int PortAddress, ref uint Data);
        [DllImport("inpoutx64.dll", EntryPoint = "SetPhysLong")]
        private static extern bool SetPhysLong_x64(ref int PortAddress, ref uint Data);
        #endregion

        private static MainWindow instance = null;

        //flag per evitare conflitti di comportamento a causa dei comandi comuni
        //Nella modalita INSERIMENTO i comandi delle date si autoaggiornano in base alle esigenze
        //Nella modalita RICERCA no.
        bool flagInserimento = false;
        bool flagRicerca = false;
        bool flagRinnovo = false;

        //variabili per gestione porta
        int decData = 0;
        int decAdd = 888; // 378h Selected Default


        private RawInputEventArgs _event;
        private RawPresentationInput _rawInput;
        string barcodeDeviceName = ConfigurationManager.AppSettings["barcodeDeviceName"].Replace(@"\\", @"\");
        string barcodeBuffer;
        GestioneIngressi gestione = new GestioneIngressi();

        public static MainWindow getInstance()
        {
            if (instance == null)
            {
                instance = new MainWindow();
            }
            return instance;
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LogInfo("Avvio applicazione");
            Login();
            FinestraGestioneIngressi();
            InitPorta();
            AggiornaAbbonamentiCorrenti();
            AbbonamentoPeriodicoCheck();
            //cn = new SqlConnection(strConn);
            // * * * * *se cambiano le offerte della palestra ricorda di cambiarle anche negli altri punti del codice!! 
            //offerte = new string[6] { "Nessuna", "Lezione singola (10,00 €)", "10 lezioni (65,00 €)", "Mensile (55,00 €)", "Trimestrale (145,00 €)", "Annuale (395,00 €)" };
            //cmbTipoIscrizione.ItemsSource = offerte;


            //LIMITAZIONE ERRORI: i comandi vengono attivati solo se strettamente necessario
            Standby();
            dateIscrizione.IsEnabled = false; //  *** La data di iscrizione sarà sempre il giorno attuale per comodità, quindi non sarà MAI MODIFICABILE
            dateScadAnnuale.IsEnabled = false; // -- 

            //cmbTipoIscrizione.SelectedIndex = 0; appena tolto
            dateScadenza.IsEnabled = false;
            dateScadCertificato.IsEnabled = false; //Non è possibile in ALCUN CASO modificare la data di scadenza del certificato, poichè non dipende 
            AggiornaTabella();
            // dtUtenti.IsReadOnly = true; //la tabella non è modificabile PS NON POSSO PERCHE DISATTIVA IL CONTROLLO DI DOPPIO CLICK (?)

            //estetica
            lblPalestra.Foreground = Brushes.DimGray;
            btnInserisci.Background = Brushes.LightGreen;
            btnFatto.Background = Brushes.LightGreen;
            btnElimina.Background = Brushes.DarkGray;
            btnAnnulla.Background = Brushes.DarkSalmon;
            btnCerca.Background = Brushes.LightSkyBlue;
            btnModifica.Background = Brushes.PaleGoldenrod;

            /*
            PalestraTest.UtentiPalestraDataSet utentiPalestraDataSet = ((PalestraTest.UtentiPalestraDataSet)(this.FindResource("utentiPalestraDataSet")));
            // Carica i dati nella tabella RapportoClienti. Se necessario, è possibile modificare questo codice.
            PalestraTest.UtentiPalestraDataSetTableAdapters.RapportoClientiTableAdapter utentiPalestraDataSetRapportoClientiTableAdapter = new PalestraTest.UtentiPalestraDataSetTableAdapters.RapportoClientiTableAdapter();
            utentiPalestraDataSetRapportoClientiTableAdapter.Fill(utentiPalestraDataSet.RapportoClienti);
            System.Windows.Data.CollectionViewSource rapportoClientiViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("rapportoClientiViewSource")));
            rapportoClientiViewSource.View.MoveCurrentToFirst();
            // Carica i dati nella tabella Utenti. Se necessario, è possibile modificare questo codice.
            PalestraTest.UtentiPalestraDataSetTableAdapters.UtentiTableAdapter utentiPalestraDataSetUtentiTableAdapter = new PalestraTest.UtentiPalestraDataSetTableAdapters.UtentiTableAdapter();
            utentiPalestraDataSetUtentiTableAdapter.Fill(utentiPalestraDataSet.Utenti);
            System.Windows.Data.CollectionViewSource utentiViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("utentiViewSource")));
            utentiViewSource.View.MoveCurrentToFirst();
            */

        }
        
        //Funzioni
        #region Inserimento utenti OK

        private void btnInserisci_Click(object sender, RoutedEventArgs e)
        {
            //Inizia la modalita di inserimento
            ModalitaIns();
            //Standby();
        }

        private void Inserimento()
        {
            try
            {
                UtentiPalestraEntities ent = new UtentiPalestraEntities();

                Utente ut = new Utente();
                PrezziAbbonamenti iscrizioneSel = (int?)cmbTipoIscrizione.SelectedValue != null ? ent.PrezziAbbonamenti.FirstOrDefault(a => a.id == (int)cmbTipoIscrizione.SelectedValue) : null;
                ut.ID_Tessera = txtID.Text.Trim();
                if(txtID.Text.Trim() == "")
                {
                    alertWarning("Inserire l'ID del cliente");
                    return;
                }
                ut.Nome = txtNome.Text.Trim();
                ut.Cognome = txtCognome.Text.Trim();
                ut.Indirizzo = txtIndirizzo.Text.Trim();
                ut.Citta = txtCitta.Text.Trim();
                ut.Provincia = txtProvincia.Text.Trim();
                ut.Telefono = txtTelefono.Text.Trim();
                ut.Pubblicita = txtPubblicita.Text.Trim();
                ut.DataNascita = dateNascita.SelectedDate;
                ut.FesteggiaCompleanno = (bool)chkCompleanno.IsChecked;

                RapportoCliente rp = new RapportoCliente();
                rp.ID_Tessera = txtID.Text.Trim();
                rp.Data_Annuale = dateAnnuale.SelectedDate;
                rp.Data_AnnualeScadenza = dateScadAnnuale.SelectedDate;
                rp.Note = txtNote.Text.Trim();
                rp.Data_Iscrizione = dateIscrizione.SelectedDate;
                rp.Extra = txtExtra.Text.Trim();

                //gestione abbonamento
                if (iscrizioneSel != null)
                {
                    rp.Tipo_Iscrizione = iscrizioneSel.id;
                    if (iscrizioneSel.is_periodico)
                        rp.Data_Scadenza = dateScadenza.SelectedDate;
                    else
                    {
                        rp.N_EntrateRimanenti = iscrizioneSel.validita_giorni;
                    }
                    rp.Abilitato = true;

                    if (datePagamento.SelectedDate != null)
                        rp.Data_Pagamento = datePagamento.SelectedDate;
                }
                else
                {
                    rp.Abilitato = false;
                }
                

                //gestione certifiato
                if (dateCertificato.SelectedDate != null)
                    rp.Data_Certificato = dateCertificato.SelectedDate;
                if (dateScadCertificato.SelectedDate != null)
                    rp.Data_Scadenza_Certificato = dateScadCertificato.SelectedDate;
               
                ent.Utenti.Add(ut);
                ent.RapportoClienti.Add(rp);
                LogInfo("Inserimento utente '" + ut.Nome.Trim() + " " + ut.Cognome.Trim() + "'");
                ent.SaveChanges();

                MessageBox.Show(ut.Nome + " " + ut.Cognome + " è stato aggiunto al database!", "Inserimento eseguito!", MessageBoxButton.OK, MessageBoxImage.Information);

                Pulizia();

            }
            catch (Exception ecc)
            {
                LogError(getInnerException(ecc).Message);
                MessageBox.Show(ecc.Message, "ERRORE!", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void btnFatto_Click(object sender, RoutedEventArgs e)
        {
            Inserimento();

            //Finisce la modalita di inserimento
            Standby();
        }

        #endregion

        #region Ricerca OK

        private void btnCerca_Click(object sender, RoutedEventArgs e)
        {
            CercaUtente();
        }

        private void CercaUtente()
        {
            try
            {
                LogInfo("Ricerca utente '" + txtCercaNome.Text.Trim() + " " + txtCercaCognome.Text.Trim() + "'");
                UtentiPalestraEntities ent = new UtentiPalestraEntities();
                var utente = ent.Utenti.Where(p => p.Nome == txtCercaNome.Text && p.Cognome == txtCercaCognome.Text).FirstOrDefault();

                //string cmdCerca = "SELECT * FROM Utenti INNER JOIN RapportoClienti ON Utenti.ID_Tessera = RapportoClienti.ID_Tessera WHERE(Nome = @Nome AND Cognome = @Cognome)";
                //SqlCommand cmd = new SqlCommand(cmdCerca, cn);
                //cmd.Parameters.AddWithValue("@Nome", txtCercaNome.Text);
                //cmd.Parameters.AddWithValue("@Cognome", txtCercaCognome.Text);

                //SqlDataReader dr = cmd.ExecuteReader();

                //effettua la lettura ed inserisce i dati della riga corrente nelle varie textbox
                //*** nel caso ci siano DIVERSE PERSONE CON LO STESSO NOME, è un PROBLEMA, verra inserita solo l'ultima
                if (utente == null)
                {
                    MessageBox.Show("Nessuna persona trovata!", "Attenzione!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    return;
                }

                txtID.Text = utente.ID_Tessera.Trim();
                txtNome.Text = utente.Nome.Trim();
                txtCognome.Text = utente.Cognome.Trim();
                txtIndirizzo.Text = utente.Indirizzo.Trim();
                txtCitta.Text = utente.Citta.Trim();
                txtProvincia.Text = utente.Provincia.Trim();
                txtTelefono.Text = utente.Telefono.Trim();
                txtPubblicita.Text = utente.Pubblicita.Trim();
                dateNascita.SelectedDate = utente.DataNascita;
                chkCompleanno.IsChecked = utente.FesteggiaCompleanno;

                dateIscrizione.SelectedDate = utente.RapportoClienti.Data_Iscrizione;
                datePagamento.SelectedDate = utente.RapportoClienti.Data_Pagamento;
                dateScadenza.SelectedDate = utente.RapportoClienti.Data_Scadenza;

                dateCertificato.SelectedDate = utente.RapportoClienti.Data_Certificato;
                dateScadCertificato.SelectedDate = utente.RapportoClienti.Data_Scadenza_Certificato;

                dateAnnuale.SelectedDate = utente.RapportoClienti.Data_Annuale;
                dateScadAnnuale.SelectedDate = utente.RapportoClienti.Data_AnnualeScadenza; 

                txtNote.Text = utente.RapportoClienti.Note.Trim();
                txtExtra.Text = utente.RapportoClienti.Extra.Trim();

                dateDisattiva.SelectedDate = utente.RapportoClienti.Data_Disattivazione;
                lblDisattiva.Content = dateDisattiva.SelectedDate == null ? "L'utente è ATTIVO" : "L'utente è DISATTIVATO";

                //mostra in base al booleano se il cliente è abilitato o no
                if (utente.RapportoClienti.Abilitato == true)
                {
                    //lblInfo.Foreground = Brushes.LightGreen;                        
                    if (utente.RapportoClienti.N_EntrateRimanenti != null)
                    {
                        lblInfo.Content = "ABILITATO! \nEntrate rimaste: ";
                        txtGiornateRimanenti.Text = utente.RapportoClienti.N_EntrateRimanenti.ToString();
                    }
                    else
                        lblInfo.Content = "ABILITATO!";

                }
                else
                {
                    //lblInfo.Foreground = Brushes.DarkSalmon;
                    lblInfo.Content = "NON ABILITATO!";
                }

                //"Nessuna", "Lezione singola (10,00 €)", "10 lezioni (65,00 €)", "Mensile (55,00 €)", "Trimestrale (145,00 €)", "Annuale (395,00 €)" 
                if(utente.RapportoClienti.PrezziAbbonamenti != null)
                {
                    cmbTipoIscrizione.SelectedValue = utente.RapportoClienti.PrezziAbbonamenti.id;
                }
                else
                {
                    cmbTipoIscrizione.SelectedIndex = -1;
                }
                //switch (utente.RapportoClienti.Tipo_Iscrizione.ToString().Trim())
                //{
                //    case "Nessuna":
                //        cmbTipoIscrizione.SelectedIndex = 0;
                //        break;
                //    case "Lezione singola (10,00 €)":
                //        cmbTipoIscrizione.SelectedIndex = 1;
                //        break;
                //    case "10 lezioni (65,00 €)":
                //        cmbTipoIscrizione.SelectedIndex = 2;
                //        break;
                //    case "Mensile (55,00 €)":
                //        cmbTipoIscrizione.SelectedIndex = 3;
                //        break;
                //    case "Trimestrale (145,00 €)":
                //        cmbTipoIscrizione.SelectedIndex = 4;
                //        break;
                //    case "Annuale (395,00 €)":
                //        cmbTipoIscrizione.SelectedIndex = 5;
                //        break;
                //}
                Ricerca();
            }
            catch (Exception ecc)
            {
                LogError(getInnerException(ecc).Message);
                MessageBox.Show(ecc.Message, "ERRORE!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {

            }
        }

        #endregion

        #region Annulla

        private void btnAnnulla_Click(object sender, RoutedEventArgs e)
        {
            Pulizia();
            Standby();
        }

        #endregion

        #region Modifica OK

        private void Aggiorna()
        {
            try
            {
                LogInfo("Modifica utente ID: '" + txtID.Text.Trim() + "'");
                UtentiPalestraEntities ent = new UtentiPalestraEntities();
                var utente = ent.Utenti.FirstOrDefault(a => a.ID_Tessera == txtID.Text.Trim());
                PrezziAbbonamenti iscrizioneSel = (int?)cmbTipoIscrizione.SelectedValue != null ? ent.PrezziAbbonamenti.FirstOrDefault(a => a.id == (int)cmbTipoIscrizione.SelectedValue) : null;

                utente.Nome = txtNome.Text.Trim();
                utente.Cognome = txtCognome.Text.Trim();
                utente.Indirizzo = txtIndirizzo.Text.Trim();
                utente.Citta = txtCitta.Text.Trim();
                utente.Provincia = txtProvincia.Text.Trim();
                utente.Telefono = txtTelefono.Text.Trim();
                utente.Pubblicita = txtPubblicita.Text.Trim();
                utente.Nome = txtNome.Text.Trim();
                utente.DataNascita = dateNascita.SelectedDate;
                utente.FesteggiaCompleanno = (bool)chkCompleanno.IsChecked;

                //aggiorna le giornate rimanenti nel caso la data pagamento vari da quella precedente 
                if (iscrizioneSel != null)
                {
                    utente.RapportoClienti.Tipo_Iscrizione = iscrizioneSel.id;
                    utente.RapportoClienti.Data_Pagamento = datePagamento.SelectedDate;
                    utente.RapportoClienti.Abilitato = true;

                    //se è stato cambiato manualmente il numero di entrate
                    if(int.TryParse(txtGiornateRimanenti.Text, out int giornateRim) && utente.RapportoClienti.N_EntrateRimanenti != null && utente.RapportoClienti.N_EntrateRimanenti != giornateRim )
                    {
                        utente.RapportoClienti.N_EntrateRimanenti = giornateRim;
                    }
                    else if (!iscrizioneSel.is_periodico) {
                        utente.RapportoClienti.N_EntrateRimanenti = iscrizioneSel.validita_giorni;
                        utente.RapportoClienti.Data_Scadenza = null;
                    }    
                    else {
                        utente.RapportoClienti.N_EntrateRimanenti = null;
                        utente.RapportoClienti.Data_Scadenza = dateScadenza.SelectedDate;
                    }
                }
                else
                {
                    utente.RapportoClienti.Tipo_Iscrizione = null;
                    utente.RapportoClienti.Abilitato = false;
                }

                //la data di iscrizione non può essere modificata
                //utente.RapportoClienti.Data_Iscrizione = dateIscrizione.SelectedDate; 

                utente.RapportoClienti.Data_Annuale = dateAnnuale.SelectedDate;
                utente.RapportoClienti.Data_AnnualeScadenza = dateScadAnnuale.SelectedDate;

                utente.RapportoClienti.Data_Certificato = dateCertificato.SelectedDate;
                utente.RapportoClienti.Data_Scadenza_Certificato = dateScadCertificato.SelectedDate;

                utente.RapportoClienti.Note = txtNote.Text.Trim();
                utente.RapportoClienti.Extra = txtExtra.Text.Trim();

                ent.SaveChanges();

                MessageBox.Show(txtNome.Text + " " + txtCognome.Text + " è stato modificato con successo!", "Modifica eseguita!", MessageBoxButton.OK, MessageBoxImage.Information);
                Pulizia();

            }
            catch (Exception ecc)
            {
                LogError(getInnerException(ecc).Message);
                MessageBox.Show(ecc.Message, "ERRORE!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                //chiude infine la connessione

            }
        }

        private void btnModifica_Click(object sender, RoutedEventArgs e)
        {
            //if (flagRicerca)
            //{
                Aggiorna();
                flagRicerca = false;
                Standby();
            //}
            //else if (flagRinnovo)
            //{
            //    RinnovaAbbonamento();
            //    Pulizia();
            //    Standby();
            //}
        }

        #endregion

        #region Elimina OK

        private void btnElimina_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Sei sicuro di voler eliminare " + txtNome.Text + " " + txtCognome.Text + "?", "Sei sicuro?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Elimina();
                Pulizia();
                Standby();
            }
        }

        private void Elimina()
        {
            try
            {
                LogInfo("Eliminazione utente ID: '" + txtID.Text.Trim() + "'");
                UtentiPalestraEntities ent = new UtentiPalestraEntities();
                var utente = ent.Utenti.FirstOrDefault(a => a.ID_Tessera == txtID.Text.Trim());
                ent.Utenti.Remove(utente);
                ent.SaveChanges();

                MessageBox.Show(txtNome.Text + " " + txtCognome.Text + "è stato eliminato!", "Eliminazione eseguita!", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ecc)
            {
                LogError(getInnerException(ecc).Message);
                MessageBox.Show(ecc.Message, "ERRORE!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                //chiude infine la connessione

            }

        }

        #endregion

        #region Rinnovo abbonamento OK

        private void btnRinnova_Click(object sender, RoutedEventArgs e)
        {
            Rinnovo();
        }

        private void RinnovaAbbonamento()
        {
            //try
            //{
            //    LogInfo("Modifica dati abbonamento utente ID: '" + txtID.Text.Trim() + "'");
            //    UtentiPalestraEntities ent = new UtentiPalestraEntities();
            //    var rapporto = ent.RapportoClienti.FirstOrDefault(a => a.ID_Tessera == txtID.Text.Trim());
            //    PrezziAbbonamenti iscrizioneSel = ent.PrezziAbbonamenti.FirstOrDefault(a => a.id == (int)cmbTipoIscrizione.SelectedValue);
            //    //aggiorna le giornate solo se viene selezionata un'opzione diversa da quella attuale
            //    //quindi se per esempio ha 10 giornate, l'utente effettua 2 entrate, poi l'admin entra per modificare il certificato medico, non si avrà la modifica delle giornate
            //    if (iscrizioneSel.id != rapporto.PrezziAbbonamenti.id)
            //    {
            //        if (!iscrizioneSel.is_periodico)
            //            rapporto.N_EntrateRimanenti = iscrizioneSel.validita_giorni;
            //        else
            //            rapporto.N_EntrateRimanenti = null;
            //    }

            //    rapporto.Data_Iscrizione = dateIscrizione.SelectedDate;
            //    rapporto.Data_Pagamento = datePagamento.SelectedDate;
            //    rapporto.Data_Scadenza = dateScadenza.SelectedDate;
            //    rapporto.Data_Annuale = dateAnnuale.SelectedDate;
            //    rapporto.Data_AnnualeScadenza = dateScadAnnuale.SelectedDate;
            //    rapporto.Data_Certificato = dateCertificato.SelectedDate;
            //    rapporto.Data_Scadenza_Certificato = dateScadCertificato.SelectedDate;
            //    rapporto.Note = txtNote.Text.Trim();
            //    rapporto.Tipo_Iscrizione = iscrizioneSel.id;
            //    rapporto.Abilitato = iscrizioneSel.titolo != "Nessuno" ? true : false;
            //    rapporto.Extra = txtExtra.Text.Trim();

            //    //se viene selezionata una opzione PERIODICA, il numero di lezioni si azzera perchè si passa ad un abbonamento PERIODICO
            //    if (iscrizioneSel.is_periodico)
            //    {
            //        rapporto.Data_Scadenza = dateScadenza.SelectedDate;
            //        rapporto.N_EntrateRimanenti = null;
            //    }

            //    ent.SaveChanges();
            //}
            //catch (Exception ecc)
            //{
            //    LogError(getInnerException(ecc).Message);
            //    MessageBox.Show(ecc.Message, "ERRORE!", MessageBoxButton.OK, MessageBoxImage.Error);
            //}
            //finally
            //{
            //    //chiude infine la connessione

            //}
        }

        #endregion

        #region Controllo Abbonamenti OK

        private void btnControllaAbbonamenti_Click(object sender, RoutedEventArgs e)
        {
            ControlloAbbonamenti();
        }

        public void ControlloAbbonamenti()
        {
            try
            {
                LogInfo("Controllo abbonamenti ... ");
                //vengono controllati tutti gli abbonamenti
                UtentiPalestraEntities ent = new UtentiPalestraEntities();
                ent.RapportoClienti.ToList().ForEach(a => { a.Codice_disattivazione = null; a.Abilitato = true; });

                ent.RapportoClienti.Where(a => DbFunctions.DiffDays(a.Data_AnnualeScadenza, DateTime.Today) > 0).ToList().ForEach(a => { a.Codice_disattivazione = 1; a.Abilitato = false; });
                ent.RapportoClienti.Where(a => a.Data_AnnualeScadenza == null).ToList().ForEach(a => { a.Codice_disattivazione = 2; a.Abilitato = false; });
                ent.RapportoClienti.Where(a => a.N_EntrateRimanenti == 0).ToList().ForEach(a => { a.Codice_disattivazione = 3; a.Abilitato = false; });
                ent.RapportoClienti.Where(a => DbFunctions.DiffDays(a.Data_Scadenza, DateTime.Today) > 0).ToList().ForEach(a => { a.Codice_disattivazione = 4; a.Abilitato = false; });
                ent.RapportoClienti.Where(a => a.Tipo_Iscrizione == null).ToList().ForEach(a => { a.Codice_disattivazione = 5; a.Abilitato = false; });
                ent.RapportoClienti.Where(a => DbFunctions.DiffDays(a.Data_Scadenza_Certificato, DateTime.Today) > 0).ToList().ForEach(a => { a.Codice_disattivazione = 6; a.Abilitato = false; });
                ent.RapportoClienti.Where(a => a.Data_Scadenza_Certificato == null).ToList().ForEach(a => { a.Codice_disattivazione = 7; a.Abilitato = false; });
                ent.RapportoClienti.Where(a => a.PrezziAbbonamenti.titolo.Equals("Nessuno")).ToList().ForEach(a => { a.Codice_disattivazione = 8; a.Abilitato = false; });
                ent.RapportoClienti.Where(a => a.Extra != "       ").ToList().ForEach(a => { a.Codice_disattivazione = 9; a.Abilitato = false; });
                ent.RapportoClienti.Where(a => DbFunctions.DiffMonths(a.Data_Pagamento, DateTime.Today) > 6 && (!a.PrezziAbbonamenti.is_periodico)).ToList().ForEach(a => { a.Codice_disattivazione = 10; a.Abilitato = false; });
                ent.RapportoClienti.Where(a => a.Data_Disattivazione != null).ToList().ForEach(a => { a.Codice_disattivazione = 11; a.Abilitato = false; });

                ent.SaveChanges();



                //string sqlAttivaTutti = "UPDATE RapportoClienti SET Abilitato=1, Codice_disattivazione = NULL";
                //SqlCommand attiva = new SqlCommand(sqlAttivaTutti, cn);
                //attiva.ExecuteNonQuery();
                //SqlCommand cmd = new SqlCommand("", cn);

                // ** BISOGNA SISTEMARE IL FATTO CHE SE UNO HA NULL IN QUALCHE DATA BYPASSA IL CONTROLLO DELLE DATE !!! *** 
                /*string sqlcontrol = "UPDATE RapportoClienti SET Abilitato=0 WHERE " +
                    "DATEDIFF(DAY, Data_AnnualeScadenza, GETDATE()) > 0 " +
                    "OR DATEDIFF(DAY, Data_AnnualeScadenza, GETDATE()) IS NULL " +
                    "OR N_EntrateRimanenti = 0 " +
                    "OR DATEDIFF(DAY, Data_Scadenza, GETDATE()) > 0 " +
                    "OR DATEDIFF(DAY, Data_Scadenza, GETDATE()) IS NULL " +
                    "OR DATEDIFF(DAY, Data_Scadenza_Certificato, GETDATE()) > 0 " +
                    "OR DATEDIFF(DAY, Data_Scadenza_Certificato, GETDATE()) IS NULL " +
                    "OR Tipo_Iscrizione = 'Nessuna' " +
                    "OR Extra != '       ' " +
                    "OR (DATEDIFF(MONTH, Data_Pagamento, GETDATE()) > 6)";

                cmd.ExecuteNonQuery();*/

                // scadenza annuale scaduta COD: 0 *** 
                //string sqlcontrol0 = "UPDATE RapportoClienti SET Abilitato=0, Codice_disattivazione = 0 WHERE " +
                //    "DATEDIFF(DAY, Data_AnnualeScadenza, GETDATE()) > 0 ";
                //SqlCommand cmd0 = new SqlCommand(sqlcontrol0, cn);
                //cmd0.ExecuteNonQuery();

                // scadenza annuale nulla COD: 1 *** 
                //string sqlcontrol1 = "UPDATE RapportoClienti SET Abilitato=0, Codice_disattivazione = 1 WHERE " +
                //    "DATEDIFF(DAY, Data_AnnualeScadenza, GETDATE()) IS NULL ";
                //SqlCommand cmd1 = new SqlCommand(sqlcontrol1, cn);
                //cmd1.ExecuteNonQuery();

                // entrate terminate COD: 2 *** 
                //string sqlcontrol2 = "UPDATE RapportoClienti SET Abilitato=0, Codice_disattivazione = 2 WHERE " +
                //    " N_EntrateRimanenti = 0 ";
                //SqlCommand cmd2 = new SqlCommand(sqlcontrol2, cn);
                //cmd2.ExecuteNonQuery();

                // abbonamento scaduto COD: 3 *** 
                //string sqlcontrol3 = "UPDATE RapportoClienti SET Abilitato=0, Codice_disattivazione = 3 WHERE " +
                //    " DATEDIFF(DAY, Data_Scadenza, GETDATE()) > 0 ";
                //SqlCommand cmd3 = new SqlCommand(sqlcontrol3, cn);
                //cmd3.ExecuteNonQuery();

                // abbonamento nullo COD: 4 *** 
                //string sqlcontrol4 = "UPDATE RapportoClienti SET Abilitato=0, Codice_disattivazione = 4 WHERE " +
                //    " DATEDIFF(DAY, Data_Scadenza, GETDATE()) IS NULL ";
                //SqlCommand cmd4 = new SqlCommand(sqlcontrol4, cn);
                //cmd4.ExecuteNonQuery();

                // certificato scaduto COD: 5 *** 
                //string sqlcontrol5 = "UPDATE RapportoClienti SET Abilitato=0, Codice_disattivazione = 5 WHERE " +
                //    " DATEDIFF(DAY, Data_Scadenza_Certificato, GETDATE()) > 0 ";
                //SqlCommand cmd5 = new SqlCommand(sqlcontrol5, cn);
                //cmd5.ExecuteNonQuery();

                // certificato nullo COD: 6 *** 
                //string sqlcontrol6 = "UPDATE RapportoClienti SET Abilitato=0, Codice_disattivazione = 6 WHERE " +
                //    " DATEDIFF(DAY, Data_Scadenza_Certificato, GETDATE()) IS NULL ";
                //SqlCommand cmd6 = new SqlCommand(sqlcontrol6, cn);
                //cmd6.ExecuteNonQuery();

                // nessuna iscrizione COD: 7 *** 
                //string sqlcontrol7 = "UPDATE RapportoClienti SET Abilitato=0, Codice_disattivazione = 7 WHERE " +
                //    " Tipo_Iscrizione = 'Nessuna' ";
                //SqlCommand cmd7 = new SqlCommand(sqlcontrol7, cn);
                //cmd7.ExecuteNonQuery();

                // pagamento in sospeso COD: 8 *** 
                //string sqlcontrol8 = "UPDATE RapportoClienti SET Abilitato=0, Codice_disattivazione = 8 WHERE " +
                //    " Extra != '       ' ";
                //SqlCommand cmd8 = new SqlCommand(sqlcontrol8, cn);
                //cmd8.ExecuteNonQuery();

                // 6 mesi trascorsi da pagamento a lezioni COD: 9 *** 
                //string sqlcontrol9 = "UPDATE RapportoClienti SET Abilitato=0, Codice_disattivazione = 9 WHERE " +
                //    " (DATEDIFF(MONTH, Data_Pagamento, GETDATE()) > 6) AND (Tipo_Iscrizione = 'Lezione singola (10,00 €)' OR Tipo_Iscrizione = '10 lezioni (65,00 €)' )";
                //SqlCommand cmd9 = new SqlCommand(sqlcontrol9, cn);
                //cmd9.ExecuteNonQuery();

                //DEBUG
                //cmd.CommandText = "SELECT COUNT(*) FROM RapportoClienti WHERE Abilitato = 0";
                //MessageBox.Show("Gli utenti che da oggi non possono frequentare la palestra sono: " + cmd.ExecuteScalar(), "Riepilogo", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (Exception ecc)
            {
                LogError(getInnerException(ecc).Message);
                MessageBox.Show(ecc.Message, "ERRORE!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                //chiude infine la connessione

                AggiornaTabella();
            }

        }

        #endregion

        #region Lettura dati
        private void AggiornaTabella()
        {
            try
            {
                UtentiPalestraEntities ent = new UtentiPalestraEntities();
                var test = ent.Utenti.Select(a => new { Nome = a.Nome, Cognome = a.Cognome, Pagamento = a.RapportoClienti.Data_Pagamento, Abilitato = a.RapportoClienti.Abilitato, Messaggio = a.RapportoClienti.Casi_disattivazione_abbonamento.motivo });

                dtUtenti.ItemsSource = test.ToArray();
                //ColoraTabella();
            }
            catch (Exception ecc)
            {
                LogError(getInnerException(ecc).Message);
                MessageBox.Show(ecc.Message, "ERRORE!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        

        private void ColoraTabella()
        {

        }

        #endregion

        #region Login

        void Login()
        {
            Login log = new Login();
            if ((bool)log.ShowDialog())
                grpStat.Visibility = Visibility.Visible;
            else
                grpStat.Visibility = Visibility.Hidden;
        }

        #endregion

        #region Finestra benvenuto

        public void FinestraGestioneIngressi()
        {
            gestione.Show();
        }

        #endregion


        //Sistema
        #region Modalità

        private void ModalitaIns()
        {
            dati_utenti.IsEnabled = true;
            Rapporto_utenti.IsEnabled = true;
            Cerca.IsEnabled = false;

            btnModifica.IsEnabled = false;
            btnCerca.IsEnabled = false;
            btnInserisci.IsEnabled = false;

            btnFatto.IsEnabled = true;
            flagInserimento = true;

            grpDisattiva.IsEnabled = false;


            //mentre inserisci, ATTIVA TUTTO
            cmbTipoIscrizione.IsEnabled = true;
            datePagamento.IsEnabled = true;
            dateCertificato.IsEnabled = true;
            dateAnnuale.IsEnabled = true;
        }

        private void Standby()
        {
            ControlloAbbonamenti();

            dati_utenti.IsEnabled = false;
            Rapporto_utenti.IsEnabled = false;
            Cerca.IsEnabled = true;

            btnModifica.IsEnabled = false;
            btnCerca.IsEnabled = true;
            btnInserisci.IsEnabled = true;
            btnElimina.IsEnabled = false;
            btnFatto.IsEnabled = false;
            btnRinnova.IsEnabled = false;

            grpDisattiva.IsEnabled = false;

            flagInserimento = false;
            flagRicerca = false;
            flagRinnovo = false;
            AggiornaTabella();

            cmbTipoIscrizione.SelectedIndex = -1;

            //velocità
            datePagamento.SelectedDate = DateTime.Today;
            dateIscrizione.SelectedDate = DateTime.Today;
            dateAnnuale.SelectedDate = DateTime.Today;
            dateScadAnnuale.SelectedDate = DateTime.Today.AddMonths(12);
            lblInfo.Content = "";
        }

        private void Ricerca()
        {
            dati_utenti.IsEnabled = true;
            Rapporto_utenti.IsEnabled = true;
            Cerca.IsEnabled = false;

            btnModifica.IsEnabled = true;
            btnCerca.IsEnabled = false;
            btnInserisci.IsEnabled = false;
            btnElimina.IsEnabled = true;
            btnRinnova.IsEnabled = true;

            grpDisattiva.IsEnabled = true;
            dateDisattiva.IsEnabled = false;

            btnFatto.IsEnabled = false;
            flagInserimento = false;
            flagRicerca = true;
            flagRinnovo = false;

            // mentre cerca, DISATTIVA TUTTO
            datePagamento.IsEnabled = false;
            dateCertificato.IsEnabled = false;
            cmbTipoIscrizione.IsEnabled = false;
            dateAnnuale.IsEnabled = false;
            txtGiornateRimanenti.IsEnabled = false;
        }

        private void Rinnovo()
        {
            dati_utenti.IsEnabled = false;
            Rapporto_utenti.IsEnabled = true;
            Cerca.IsEnabled = false;

            btnModifica.IsEnabled = true;
            btnCerca.IsEnabled = false;
            btnInserisci.IsEnabled = false;
            btnElimina.IsEnabled = false;
            btnRinnova.IsEnabled = false;

            grpDisattiva.IsEnabled = true;
            dateDisattiva.IsEnabled = false;

            btnFatto.IsEnabled = false;
            flagInserimento = true;//per automatizzare le date nonostante non sia la modalità di inserimento
            flagRicerca = false;
            flagRinnovo = true;

            //mentre rinnova, ATTIVA TUTTO
            datePagamento.IsEnabled = true;
            dateCertificato.IsEnabled = true;
            cmbTipoIscrizione.IsEnabled = true;
            dateAnnuale.IsEnabled = true;
            txtGiornateRimanenti.IsEnabled = true;
        }

        #endregion

        #region Velocità

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            //velocità
            if (e.Key == Key.Enter && Menu.SelectedIndex == 1)
                Inserimento();
        }

        private void OnKeyPressed(object sender, RawInputEventArgs e)
        {
            if (e.Device.Name.Contains(barcodeDeviceName) && e.KeyPressState == KeyPressState.Down)
            {
                int n;
                if (int.TryParse(e.Key.ToString().Replace("D", ""), out n))
                {
                    barcodeBuffer += e.Key.ToString().Replace("D", "");
                    e.Handled = true;

                    if (barcodeBuffer.Length == 8)
                    {
                        barcodeBuffer = barcodeBuffer.Replace("Return", "");
                        gestione.cercaUtente(barcodeBuffer);
                        barcodeBuffer = "";
                    }
                }
            }

        }


        protected override void OnSourceInitialized(EventArgs e)
        {
            StartWndProcHandler();
            base.OnSourceInitialized(e);
        }

        private void StartWndProcHandler()
        {
            _rawInput = new RawPresentationInput(this, RawInputCaptureMode.Foreground);
            _rawInput.KeyPressed += OnKeyPressed;
        }

        private void Pulizia()
        {
            //pulizia
            txtID.Clear();
            txtNome.Clear();
            txtCognome.Clear();
            txtIndirizzo.Clear();
            txtCitta.Clear();
            txtProvincia.Text = "FC";
            txtTelefono.Clear();
            txtPubblicita.Clear();
            cmbTipoIscrizione.SelectedIndex = 0;
            datePagamento.SelectedDate = DateTime.Today;
            dateIscrizione.SelectedDate = DateTime.Today;
            dateAnnuale.SelectedDate = DateTime.Today;
            dateScadAnnuale.SelectedDate = DateTime.Today.AddMonths(12);
            dateScadenza.SelectedDate = null;
            dateCertificato.SelectedDate = null;
            dateScadCertificato.SelectedDate = null;
            txtNote.Clear();
            txtExtra.Clear();
            txtGiornateRimanenti.Clear();

            // flagInserimento = true; //ogni volta che viene effettuata la pulizia il comandi sono pronti per un altro inserimento
        }

        private void chkAbbPeriodico_Click(object sender, RoutedEventArgs e)
        {
            if (chkAbbPeriodico.IsChecked == true)
            {
                AbbonamentoPeriodicoCheck();
            }
            else
            {
                AbbonamentoNonPeriodicoCheck();
            }
        }

        private void PulisciFormAbbonamenti()
        {
            txtAbbNome.Text = "";
            txtAbbPrezzo.Text = "";
            txtAbbMesiVal.Text = "";
            txtAbbGiorniVal.Text = "";
        }

        private void btnAnnullaAbbon_Click(object sender, RoutedEventArgs e)
        {
            cmbTipoIscrizione.SelectedIndex = -1;
        }

        #endregion

        #region Automatizzazione controlli


        private void dtUtenti_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            tabInserisci.IsSelected = true;

            var source = ((DataGrid)sender).SelectedItem;
            if (source == null) return;
            var nome = source.GetType().GetProperty("Nome").GetValue(source, null);
            var cognome = source.GetType().GetProperty("Cognome").GetValue(source, null);

            txtCercaNome.Text = nome.ToString();
            txtCercaCognome.Text = cognome.ToString();

            flagRicerca = true;
            CercaUtente();
        }

        private void cmbTipoIscrizione_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //se stiamo inserendo un nuovo elemento, i controlli si autoaggiornano
            if (flagInserimento)
                AggiornaDataScadenza();
        }

        private void dateCertificato_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flagInserimento)
                if (dateCertificato.SelectedDate != null)
                {
                    dateScadCertificato.SelectedDate = ((DateTime)dateCertificato.SelectedDate).AddMonths(12);
                }
        }

        private void datePagamento_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flagInserimento)
                AggiornaDataScadenza();
            //impostazioni per la correttezza dei pagamenti, *** CHIEDI CON DEAN COSA NE PENSA, LE POLITICHE SONO SUE ****
            //if (datePagamento.SelectedDate > DateTime.Now)
            //    lblInfo.Content = "Visto che la data del pagamento è ancora da raggiungersi, l'utente non sarà abilitato a frequentare la palestra.";
        }

        private void dateAnnuale_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (flagInserimento)
                if (dateAnnuale.SelectedDate != null)
                {
                    dateScadAnnuale.SelectedDate = ((DateTime)dateAnnuale.SelectedDate).AddMonths(12);
                }
        }

        private void AggiornaDataScadenza()
        {
            //*** MODALITA CREAZIONE ***            PER LA MODIFICA DEI DATI BISOGNERA' OPERARE DIVERSAMENTE
            //I dati che indicano le varie date, una volta inseriti la prima volta diventano inaccessibili( IsENABLED = FALSE ), si potrà operare soltanto sulla DATA DI SCADENZA 
            //tramite l'apposito form ( *** DA FARE *** ).
            //
            //Automatizzazione selezione, aggiorna la data di scadenza in base alla iscrizione scelta.
            //Se vengono selezionate le lezioni singole la data di scadenza si disabilita.
            UtentiPalestraEntities ent = new UtentiPalestraEntities();
            PrezziAbbonamenti iscrizioneSel = (int?)cmbTipoIscrizione.SelectedValue != null ? ent.PrezziAbbonamenti.FirstOrDefault(a => a.id == (int)cmbTipoIscrizione.SelectedValue) : null;

            if(iscrizioneSel == null)
            {
                datePagamento.SelectedDate = null;
                datePagamento.IsEnabled = false;
                //disattiva la data di scadenza e la setta a NULL.
                dateScadenza.SelectedDate = null;
                dateScadenza.IsEnabled = false;
            } else
            {
                if (!iscrizioneSel.is_periodico)
                {
                    datePagamento.IsEnabled = true;
                    //datePagamento.SelectedDate = DateTime.Today;//data di pagamento impostata ad oggi

                    //essendo a lezioni non ha data di scadenza, quindi viene disattivata e messa a NULL.
                    dateScadenza.SelectedDate = null;
                    dateScadenza.IsEnabled = false;
                }
                else
                {
                    datePagamento.IsEnabled = true;
                    //datePagamento.SelectedDate = DateTime.Today;//data di pagamento impostata ad oggi
                    dateScadenza.IsEnabled = false;
                    if (datePagamento.SelectedDate != null && iscrizioneSel.validita_mesi != null)//precauzione
                        dateScadenza.SelectedDate = ((DateTime)datePagamento.SelectedDate).AddMonths((int)iscrizioneSel.validita_mesi);
                }
            }
     
            

            //switch (cmbTipoIscrizione.SelectedIndex)
            //{
            //    case 0://nel caso sia selezionata "Nessuna iscrizione".
            //           //disattiva la data di pagamento e la imposta a NULL.
            //        datePagamento.SelectedDate = null;
            //        datePagamento.IsEnabled = false;
            //        //disattiva la data di scadenza e la setta a NULL.
            //        dateScadenza.SelectedDate = null;
            //        dateScadenza.IsEnabled = false;
            //        break;

            //    case 1://nel caso sia selezionata "1 Lezione".
            //        datePagamento.IsEnabled = true;
            //        //datePagamento.SelectedDate = DateTime.Today;//data di pagamento impostata ad oggi

            //        //essendo a lezioni non ha data di scadenza, quindi viene disattivata e messa a NULL.
            //        dateScadenza.SelectedDate = null;
            //        dateScadenza.IsEnabled = false;
            //        break;

            //    case 2://nel caso sia selezionata "10 Lezioni".
            //        datePagamento.IsEnabled = true;
            //        //datePagamento.SelectedDate = DateTime.Today;//data di pagamento impostata ad oggi

            //        //essendo a lezioni non ha data di scadenza, quindi viene disattivata e messa a NULL.
            //        dateScadenza.SelectedDate = null;
            //        dateScadenza.IsEnabled = false;
            //        break;

            //    case 3://nel caso sia selezionata "1 Mese"
            //        datePagamento.IsEnabled = true;
            //        //datePagamento.SelectedDate = DateTime.Today;//data di pagamento impostata ad oggi
            //        dateScadenza.IsEnabled = false;
            //        if (datePagamento.SelectedDate != null)//precauzione
            //            dateScadenza.SelectedDate = ((DateTime)datePagamento.SelectedDate).AddMonths(1);
            //        break;

            //    case 4://nel caso sia selezionata "3 Mesi"
            //        datePagamento.IsEnabled = true;
            //        //datePagamento.SelectedDate = DateTime.Today;//data di pagamento impostata ad oggi
            //        dateScadenza.IsEnabled = false;
            //        if (datePagamento.SelectedDate != null)//precauzione
            //            dateScadenza.SelectedDate = ((DateTime)datePagamento.SelectedDate).AddMonths(3);
            //        break;

            //    case 5://nel caso sia selezionata "1 Anno"
            //        datePagamento.IsEnabled = true;
            //        //datePagamento.SelectedDate = DateTime.Today;//data di pagamento impostata ad oggi
            //        dateScadenza.IsEnabled = false;
            //        if (datePagamento.SelectedDate != null)//precauzione
            //            dateScadenza.SelectedDate = ((DateTime)datePagamento.SelectedDate).AddMonths(12);
            //        break;
            //}
        }

        #endregion

        #region Avanzate

        private void btnQuery_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //vengono controllati tutti gli abbonamenti

                //string sqlDebug = txtQuery.Text;
                //UtentiPalestraEntities ent = new UtentiPalestraEntities();
                //var query = ent.Database.ExecuteSqlCommand(sqlDebug);

                //dtQuery.ItemsSource = query;
            }
            catch (Exception ecc)
            {
                LogError(getInnerException(ecc).Message);
                MessageBox.Show(ecc.Message, "ERRORE!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                //chiude infine la connessione

            }
        }

        private void btnCalcola_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LogInfo("Calcolo statistiche periodo " + dateDa.SelectedDate + " - " + dateFino.SelectedDate);
                //vengono controllati tutti gli abbonamenti
                UtentiPalestraEntities ent = new UtentiPalestraEntities();
                IDictionary<int, decimal> prezzi = new Dictionary<int, decimal>();
                var prezzi_correnti = ent.PrezziAbbonamenti.ToList();

                foreach (var prezzo in prezzi_correnti)
                {
                    prezzi[prezzo.id] = prezzo.prezzo != null ? (decimal)prezzo.prezzo : 0;
                }               

                var abbonamenti = ent.RapportoClienti.Where(a =>
                DbFunctions.DiffDays(a.Data_Pagamento, dateDa.SelectedDate) < 0
                && DbFunctions.DiffDays(a.Data_Pagamento, dateFino.SelectedDate) > 0)
                .GroupBy(a => a.Tipo_Iscrizione)
                .Select(g => new { id = g.Key, count = g.Count() })
                .ToList();

                int abbonamentiTot = 0;
                List<KeyValuePair<string, int>> numAbbonamenti = new List<KeyValuePair<string, int>>();

                decimal incassiTot = 0;
                List<KeyValuePair<string, decimal>> incassi = new List<KeyValuePair<string, decimal>>();

                foreach (var abbonamento in abbonamenti)
                {
                    var abbonamento_corr = ent.PrezziAbbonamenti.FirstOrDefault(a => a.id == abbonamento.id);
                    numAbbonamenti.Add(new KeyValuePair<string, int>(abbonamento_corr.titolo + "(" + abbonamento.count + ")", abbonamento.count));
                    abbonamentiTot += abbonamento.count;
                   
                    incassi.Add(new KeyValuePair<string, decimal>(abbonamento_corr.titolo + "(" + abbonamento.count * prezzi[(int)abbonamento.id] + ")", abbonamento.count * prezzi[(int)abbonamento.id]));
                    incassiTot += abbonamento.count * prezzi[(int)abbonamento.id];
                }

                //Setting data for column chart
                System.Windows.Controls.DataVisualization.Charting.ISeries s = chartDati.Series[0];
                chartDati.Series.Clear();
                chartDati.DataContext = numAbbonamenti;
                chartDati.Series.Add(s);
                chartDati.Title = "Abbonamenti effettuati: " + abbonamentiTot;


                //Setting data for column chart
                System.Windows.Controls.DataVisualization.Charting.ISeries d = chartIncassi.Series[0];
                chartIncassi.Series.Clear();
                chartIncassi.DataContext = incassi;
                chartIncassi.Series.Add(d);
                chartIncassi.Title = "Incassi Totali: " + incassiTot + "€";

            }
            catch (Exception ecc)
            {
                LogError(getInnerException(ecc).Message);
                MessageBox.Show(ecc.Message, "ERRORE!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                //chiude infine la connessione

            }
        }

        #endregion

        #region Controllo DB25

        private void btnApriTornello_Click(object sender, RoutedEventArgs e)
        {
            Tornello(true);
        }

        bool m_bX64 = false;

        public void InitPorta()
        {
            try
            {
                uint nResult = 0;
                //decide a quanti bit è il sistema operativo

                try
                {
                    nResult = IsInpOutDriverOpen();
                }
                catch (BadImageFormatException)
                {
                    nResult = IsInpOutDriverOpen_x64();
                    if (nResult != 0)
                        m_bX64 = true;
                }

                if (nResult == 0)
                {
                    MessageBox.Show("Unable to open InpOut32 driver");
                }
            }
            catch (DllNotFoundException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                MessageBox.Show("Unable to find InpOut32.dll");
            }

            Tornello(false);
        }

        //gestione tornello, se var è FALSE, inizializza soltanto il tornello
        //se è TRUE, scrive il valore di apertura e dopo 1 secondo riscrive il valore iniziale
        public bool Tornello(bool var)
        {
            int num = var == true ? 222 : 218;
            try
            {
                short iPort = Convert.ToInt16(888);
                short iData = Convert.ToInt16(num);
                if (m_bX64)
                    Out32_x64(iPort, iData);
                else
                    Out32(iPort, iData);

                //se è una
                if (var == true)
                {
                    Thread.Sleep(1000);
                    iData = Convert.ToInt16(218);
                    if (m_bX64)
                        Out32_x64(iPort, iData);
                    else
                        Out32(iPort, iData);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occured:\n" + ex.Message);
                return false;
            }
            return true;
        }
















        #endregion

        #region Disattiva

        private void btnDisattiva_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UtentiPalestraEntities ent = new UtentiPalestraEntities();
                RapportoCliente rp = ent.RapportoClienti.Where(a => a.ID_Tessera == txtID.Text.Trim()).FirstOrDefault();

                if(dateScadenza.SelectedDate == null)
                {
                    alertWarning("E' possibile disattivare solo gli utenti con un abbonamento a periodi");
                    return;
                }

                string msg = "";

                //FUNZIONAMENTO ON/OFF CHE SI ALTERNA
                if (dateDisattiva.SelectedDate == null)
                {
                    //DISATTIVA ------------- se non c'è nessuna data di disattivazione bisogna disattivarlo 
                    rp.Data_Disattivazione = DateTime.Today;
                    msg = txtNome.Text.Trim() + " " + txtCognome.Text.Trim() + " è stato disattivato con successo!";
                    LogInfo("Disattivazione utente '" + txtNome.Text.Trim() + " " + txtCognome.Text.Trim() + "'");
                }
                else
                {
                    if (MessageBox.Show("Vuoi che i giorni di inattività vengano aggiungi alla data di scadenza?", "Opzione", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        DateTime inizio_disattivazione = (DateTime)dateDisattiva.SelectedDate;
                        TimeSpan tmp = (DateTime.Today - inizio_disattivazione);
                        int giorni_disattivazione = tmp.Days;
                        DateTime fsd = ((DateTime)dateScadenza.SelectedDate).AddDays(giorni_disattivazione);

                        rp.Data_Scadenza = fsd;
                    }
                    //ATTIVA ------------------ se c'è una data la cancella e quindi l'utente risulta attivato
                    rp.Data_Disattivazione = null;
                    msg = txtNome.Text.Trim() + " " + txtCognome.Text.Trim() + " è stato attivato con successo!";
                    LogInfo("Attivazione utente '" + txtNome.Text.Trim() + " " + txtCognome.Text.Trim() + "'");
                }


                //apre la connessione ed esegue l'inserimento    
                    
                ent.SaveChanges();
                MessageBox.Show(msg, "Operazione eseguita!", MessageBoxButton.OK, MessageBoxImage.Information);
                Pulizia();
                Standby();
            }
            catch (Exception ecc)
            {
                LogError(getInnerException(ecc).Message);
                MessageBox.Show(ecc.Message, "ERRORE!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                //chiude infine la connessione
                AggiornaTabella();
            }

        }




        #endregion

        #region Logging

        public void LogInfo(string msg)
        {
            Trace.TraceInformation(DateTime.Now + " || " + msg);
            Trace.Flush();
        }

        public void LogError(string msg)
        {
            Trace.TraceError(DateTime.Now + " || " + msg);
            Trace.Flush();
        }

        public void LogWarning(string msg)
        {
            Trace.TraceWarning(DateTime.Now + " || " + msg);
            Trace.Flush();
        }

        public Exception getInnerException(Exception e)
        {
            while (e.InnerException != null) e = e.InnerException;
            return e;
        }

        public void alertWarning(string msg)
        {
            LogWarning(msg);
            MessageBox.Show(msg);
        }
        #endregion

        #region Gestione abbonamenti
        private void btnAbbAggiungi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UtentiPalestraEntities ent = new UtentiPalestraEntities();

                PrezziAbbonamenti prezzo = new PrezziAbbonamenti();

                prezzo.titolo = txtAbbNome.Text.Trim();
                prezzo.prezzo = Convert.ToDecimal(txtAbbPrezzo.Text.Trim());
                prezzo.is_periodico = (bool)chkAbbPeriodico.IsChecked;
                prezzo.validita_giorni = txtAbbGiorniVal.Text.Trim() != "" ? Convert.ToInt32(txtAbbGiorniVal.Text.Trim()) : (int?)null;
                prezzo.validita_mesi = txtAbbMesiVal.Text.Trim() != "" ? Convert.ToInt32(txtAbbMesiVal.Text.Trim()) : (int?)null;

                ent.PrezziAbbonamenti.Add(prezzo);

                LogInfo("Inserimento abbonamento '" + prezzo.titolo.Trim());
                ent.SaveChanges();

                MessageBox.Show("Abbonamento " + prezzo.titolo.Trim() + " inserito nel database.", "Inserimento eseguito!", MessageBoxButton.OK, MessageBoxImage.Information);

                Pulizia();
                Standby();
                AggiornaAbbonamentiCorrenti();
                PulisciFormAbbonamenti();
            }
            catch (Exception ecc)
            {
                LogError(getInnerException(ecc).Message);
                MessageBox.Show(ecc.Message, "ERRORE!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AggiornaAbbonamentiCorrenti()
        {
            try
            {
                UtentiPalestraEntities ent = new UtentiPalestraEntities();
                var test = ent.PrezziAbbonamenti.Select(a => new { id = a.id, Nome = a.titolo, Prezzo = a.prezzo, Periodico = a.is_periodico, Giorni = a.validita_giorni, Mesi = a.validita_mesi });

                dtAbbonamenti.ItemsSource = test.ToArray();

                List<PrezziAbbonamenti> offerte = ent.PrezziAbbonamenti.ToList();

                cmbTipoIscrizione.ItemsSource = offerte;
                cmbTipoIscrizione.DisplayMemberPath = "titolo";
                cmbTipoIscrizione.SelectedValuePath = "id";

                //ColoraTabella();
            }
            catch (Exception ecc)
            {
                LogError(getInnerException(ecc).Message);
                MessageBox.Show(ecc.Message, "ERRORE!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void btnAbbElimina_Click(object sender, RoutedEventArgs e)
        {
            UtentiPalestraEntities ent = new UtentiPalestraEntities();
            var selezioneAbbonamenti = dtAbbonamenti.SelectedItem;
            int id = (int)selezioneAbbonamenti.GetType().GetProperty("id").GetValue(selezioneAbbonamenti, null);
            if (selezioneAbbonamenti == null) return;

            List<Utente> utenteAbbonamentoSelezionato = ent.Utenti.Where(a => a.RapportoClienti.PrezziAbbonamenti.id == id).ToList();
            if(utenteAbbonamentoSelezionato.Count > 0)
            {
                string utentiAbb = "";
                foreach(Utente u in utenteAbbonamentoSelezionato)
                {
                    utentiAbb += u.Nome.Trim() + " " + u.Cognome.Trim() + "\n";
                }
                MessageBox.Show("Non è possibile eliminare l'abbonamento poichè i seguenti utenti hanno sottoscritto questo abbonamento: \n\n" + utentiAbb, "Operazione negata");
                return;
            }

            if (MessageBox.Show("Sei sicuro di voler eliminare l'abbonamento selezionato?", "Sei sicuro?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {  
                PrezziAbbonamenti abbElim = ent.PrezziAbbonamenti.FirstOrDefault(a => a.id == id);

                ent.PrezziAbbonamenti.Remove(abbElim);
                ent.SaveChanges();
                Pulizia();
                Standby();
                AggiornaAbbonamentiCorrenti();
                PulisciFormAbbonamenti();
            }
        }

        private void AbbonamentoPeriodicoCheck()
        {
            txtAbbGiorniVal.IsEnabled = false;
            txtAbbMesiVal.IsEnabled = true;
        }

        private void AbbonamentoNonPeriodicoCheck()
        {
            txtAbbGiorniVal.IsEnabled = true;
            txtAbbMesiVal.IsEnabled = false;
        }


        #endregion

       
    }
}
