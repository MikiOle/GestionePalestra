   M S B A M L   `   `   `  ����G  CPalestraTest, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:PalestraTestPalestraTest  5   	   | xSystem.Windows.Controls.DataVisualization.Toolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35�{clr-namespace:System.Windows.Controls.DataVisualization.Charting;assembly=System.Windows.Controls.DataVisualization.Toolkit2System.Windows.Controls.DataVisualization.Charting 5   	       PalestraTest.MainWindow   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35U QWPFToolkit, Version=3.5.40128.1, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35H 9http://schemas.microsoft.com/winfx/2006/xaml/presentation      R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   ,localclr-namespace:PalestraTest     5   	   �chartingToolkit{clr-namespace:System.Windows.Controls.DataVisualization.Charting;assembly=System.Windows.Controls.DataVisualization.Toolkit     5   	   -   5          � Title$  Gestione palestra��5	   	    � Icon$ /Immagini/Icon.ico��6#   $��709.705��6=   $��1007.671��6N    � 	Resources 5
      ��%    5      .��5      � .+�5   
   �� -   5         ��Name$
 Menu��5      $
��Left=�6#   $	��663��6>   $��	10,10,0,0q�6K   $	��Top=�6^   $	��984��6v   .@�5      �� -   5         ��Name$ 	tabUtenti��5      $��Utenti��6)   .��5      � ���	#FFE5E5E5$��10,-4,-16,4q�6.   .+�5      &   System.Windows.Controls.DataGrid  -   5          Name$ dtUtenti��5      $
��Left=�61   $��	10,10,0,0q�6L   $	��Top=�6_   $	��611��6w   $	��663��6�   6�   �� 5      -   5         ��Name$ grpControllo��5      $��	Controlli��65   $
��Left=�6H   $��
678,10,0,0q�6c   $	��Top=�6w   $	��288��6�   $	��286��6�   .��5      � $
��Left=�6   $	��261��6:   $��	10,0,-2,0q�6G   $	��Top=�6Z   $	��266��6r   .+�5      �� -   5         ��Name$ btnControllaAbbonamenti��5   %   $��Controlla abbonamenti��6F   $
��Left=�6f   $��0,10,0,0q�6�   $	��Top=�6�   $	��255��6�   $��52��6�   6�   �� 5      -   5         ��Name$ label2��5   $   $����Il controllo abbonamenti disabilita gli utenti
che:
●    hanno l'abbonamento scaduto.
●    hanno terminato le entrate previste.
●    hanno l'iscrizione annuale scaduta.
●    hanno la visita medica scaduta.
●    hanno dei debiti in sospeso.
●    non hanno alcun iscrizione attiva.
●    hanno 6 mesi trascorsi dal rinnovo dell 
      abbonamento a lezioni.
��64   $
��Left=�6�  $��0,62,0,0q�6�  $	��Top=�6�  $	��189��6  $	��245��6  5      5      �� 5      -   5        	 ��Name$	 imgLogo��5      $
��Left=�6-   $	��144��6H   $��820,441,0,0q�6U   $	��Top=�6j   $	��141��6�   $��Immagini/logopal.jpg��6�   �� 5      -	   5        $ lblPalestra��5      $D��>Palestra VILLAGE GYM
p.zza indipendenza n.20
Borello di Cesena��61   $
��Left=�6�   $��818,561,0,0q�6�   $	��Top=�6�   $	��146��6�   $��60��6�   $
��Bold"�6�   $��Italic$�6�   �� 5      -
   5        $ btnApriTornello��5      $��Apri tornello��66   $
��Left=�6N   $��678,303,0,0q�6i   $	��Top=�6~   $	��286��6�   $��91��6�   6�   5      5      �� 5      -   5        $ tabInserisci��5      $��Dati utente��6,   .��5      � -   5        
 �Name$
 grdInserisci��5      ���	#FFE5E5E5.+�5      �� -   5        $ dati_utenti��5      $��Dati utente��64   $
��Left=�6I   $��
10,105,0,0q�6d   $	��Top=�6x   $	��256��6�   $	��610��6�   .��5       � $
��Left=�6   $	��243��6:   $��
4,-6,-2,-3q�6G   $	��Top=�6[   $	��596��6s   .+�5!      �� -   5        $ lblID��5!   $   $��ID��63   $
��Left=�6@   $��	10,16,0,0q�6[   $	��Top=�6n   $��26��6�   �� 5"      -   5        $ lblNome��5"   $   $
��Nome��65   $
��Left=�6D   $��	10,79,0,0q�6_   $	��Top=�6r   $��45��6�   �� 5#      -   5        $ 
lblCognome��5#   $   $��Cognome��68   $
��Left=�6J   $��
198,79,0,0q�6e   $	��Top=�6y   $��63��6�   �� 5$      -   5        $ lblIndirizzo��5$   $   $��	Indirizzo��6:   $
��Left=�6N   $��
10,119,0,0q�6i   $	��Top=�6}   $��56��6�   �� 5%      -   5        $ lblCitta��5%   $   $��Città��66   $
��Left=�6F   $��222,119,0,0q�6a   $	��Top=�6v   $��39��6�   �� 5&      -   5        $ lblProvincia��5&   $   $��	Provincia��6:   $
��Left=�6N   $��378,119,-113,0q�6i   $	��Top=�6�   $��59��6�   �� 5'      -   5        $ lblTelefono��5'   $   $��Telefono��69   $
��Left=�6L   $��
10,166,0,0q�6g   $	��Top=�6{   $	��101��6�   �� 5(      -   5        $ lblPubblicita��5(   $   $��
Pubblicita��6;   $
��Left=�6P   $��
10,194,0,0q�6k   $	��Top=�6   $	��101��6�   �� 5)      -   5         ��Name$ txtID��5)   &   $
��Left=�65   $��23��6P   $��	36,19,0,0q�6\    �� TextWrapping$
 Wrap=�6o   $	��Top=�6�   $��75��6�   �� 5*      -   5        $ txtNome��5*   &   $
��Left=�67   $��23��6R   $��	71,82,0,0q�6^   $
 Wrap=�6q   $	��Top=�6�   $	��120��6�   �� 5+      -   5        $ 
txtCognome��5+   &   $
��Left=�6:   $��23��6U   $��266,82,-52,0q�6a   $
 Wrap=�6w   $	��Top=�6�   $	��110��6�   �� 5,      -   5        $ txtIndirizzo��5,   &   $
��Left=�6<   $��23��6W   $��
71,122,0,0q�6c   $
 Wrap=�6w   $	��Top=�6�   $	��120��6�   �� 5-      -   5        $ txtCitta��5-   &   $
��Left=�68   $��23��6S   $��266,122,-52,0q�6_   $
 Wrap=�6v   $	��Top=�6�   $	��110��6�   �� 5.      -   5        $ txtTelefono��5.   &   $
��Left=�6;   $��23��6V   $��116,169,0,0q�6b   $
 Wrap=�6w   $	��Top=�6�   $	��120��6�   �� 5/      -   5        $ txtPubblicita��5/   &   $
��Left=�6=   $��23��6X   $��116,197,0,0q�6d   $
 Wrap=�6y   $	��Top=�6�   $	��120��6�   �� 50      -   5        $ txtProvincia��50   &   $
��Left=�6<   $��23��6W   $��437,122,-153,0q�6c   $
 Wrap=�6{   $	��Top=�6�   $��40��6�   �� 51      -   5        $ lblTelefono_Copy��51   $   $��Data di nascita��6>   $
��Left=�6X   $��266,166,-43,0q�6s   $	��Top=�6�   $	��101��6�   (  "System.Windows.Controls.DatePicker  52      -   5          Name$ dateNascita��52   )   $
��Left=�6>   $��378,166,-198,0q�6Y   $	��Top=�6q   6�   �� 53      -    5         ��Name$ chkCompleanno��53   '   $��Festeggia compleanno��6?   $
��Left=�6^   $��266,200,0,0q�6y   $	��Top=�6�    d� 	IsChecked$
 TrueY�6�   55      56      �� 57      -!   5        $ Rapporto_utenti��57      $��Rapporto utente��68   $
��Left=�6Q   $��
10,366,0,0q�6l   $	��Top=�6�   $	��256��6�   $	��610��6�   .��58      � $
��Left=�6   $	��252��6:   $��4,-6,-2,-12q�6G   $	��Top=�6\   $	��596��6t   .+�59      �� -"   5        $ btnAnnullaAbbon��59   %   $��252,28,296,202q�6>   $��Annulla��6V   6h   �� 5:      -#   5        $ label��5:   $   $��Data pagamento��63   $
��Left=�6L   $��	10,77,0,0q�6g   $	��Top=�6z   $	��105��6�     5;      -$   5        $ datePagamento��5;   )   $
��Left=�6@   $��
120,78,0,0q�6[   $	��Top=�6o   6�   �� 5<      -%   5        $ 
label_Copy��5<   $   $��Data scadenza iscrizione��68   $
��Left=�6[   $��
290,77,0,0q�6v   $	��Top=�6�   $	��149��6�     5=      -&   5        $ dateScadenza��5=   )   $
��Left=�6?   $��
439,78,0,0q�6Z   $	��Top=�6n   �� 5>      -'   5        $ label_Copy1��5>   $   $��Data certificato
medico��69   $
��Left=�6_   $��
10,107,0,0q�6z   $	��Top=�6�   $��93��6�     5?      -(   5        $ dateCertificato��5?   )   $
��Left=�6B   $��120,116,0,0q�6]   $	��Top=�6r   6�   �� 5@      -)   5        $ label_Copy2��5@   $   $&�� Data scadenza certificato
medico��69   $
��Left=�6h   $��290,107,0,0q�6�   $	��Top=�6�   $	��149��6�     5A      -*   5        $ dateScadCertificato��5A   )   $
��Left=�6F   $��439,116,0,0q�6a   $	��Top=�6v   �� 5B      -+   5        $ label_Copy3��5B   $   $
��Note��69   $
��Left=�6H   $��
10,196,0,0q�6c   $	��Top=�6w   $��88��6�   �� 5C      -,   5        $ txtNote��5C   &   $
��Left=�67   $��46��6R   $��120,196,0,0q�6^   $
 Wrap=�6s   $	��Top=�6�   $	��144��6�   �� 5D      --   5        $ label_Copy4��5D   $   $��Tipo iscrizione��69   $
��Left=�6S   $��	10,28,0,0q�6n   $	��Top=�6�   $��88��6�   �� 5E      -.   5         ��Name$ cmbTipoIscrizione��5E   '   $
��Left=�6B   $��
103,28,0,0q�6]   $	��Top=�6q   $	��144��6�   6�   �� 5F      -/   5        $ label_Copy5��5F   $   $��Extra��69   $
��Left=�6I   $��300,196,0,0q�6d   $	��Top=�6y   $��46��6�   �� 5G      -0   5        $ txtExtra��5G   &   $
��Left=�68   $��26��6S   $��351,196,0,0q�6_   $
 Wrap=�6t   $	��Top=�6�   $��50��6�   �� 5H      -1   5        $ label_Copy6��5H   $   $��Data iscrizione��69   $
��Left=�6S   $��
313,25,0,0q�6n   $	��Top=�6�   $��88��6�     5I      -2   5        $ dateIscrizione��5I   )   $
��Left=�6A   $��
406,26,0,0q�6\   $	��Top=�6p   �� 5J      -3   5        $ lblInfo��5J   $   $
��Left=�65   $��439,196,0,0q�6P   $	��Top=�6e   $	��111��6}   $��46��6�   $�� ��6�   �� 5K      -4   5        $ label_Copy7��5K   $   $��Data iscrizione 
annuale��69   $
��Left=�6`   $��
10,145,0,0q�6{   $	��Top=�6�   $��93��6�   $��42��6�     5L      -5   5        $ dateAnnuale��5L   )   $
��Left=�6>   $��120,154,0,0q�6Y   $	��Top=�6n   6�   �� 5M      -6   5        $ label_Copy8��5M   $   $'��!Data scadenza iscrizione 
annuale��69   $
��Left=�6i   $��290,145,0,0q�6�   $	��Top=�6�   $	��149��6�     5N      -7   5        $ dateScadAnnuale��5N   )   $
��Left=�6B   $��439,154,0,0q�6]   $	��Top=�6r   �� 5O      -8   5        $ txtGiornateRimanenti��5O   &   $
��Left=�6D   $��26��6_   $��555,196,0,0q�6k   $
 Wrap=�6�   $	��Top=�6�   $��28��6�   5P      5Q      �� 5R      -9   5        $ btnInserisci��5R      $��	Inserisci��63   $
��Left=�6G   $��
360,10,0,0q�6b   $	��Top=�6v   $��65��6�   $��40��6�   6�   �� 5S      -:   5        $ Cerca��5S      $��Cerca��6.   $
��Left=�6=   $��10,7,0,0q�6X   $	��Top=�6j   $��93��6�   $	��335��6�   .��5T      � -;   5        $
 grdCerca��5T      $
��Left=�61   $��83��6L   $	��Top=�6X   $	��325��6p   $��
0,0,-2,-12q�6|   .+�5U      �� -<   5        $ txtCercaNome��5U   &   $
��Left=�6<   $��23��6W   $��	77,10,0,0q�6c   $
 Wrap=�6v   $	��Top=�6�   $	��138��6�   �� 5V      -=   5        $ label1��5V   $   $
��Nome��64   $
��Left=�6C   $��10,7,0,0q�6^   $	��Top=�6p   �� 5W      ->   5        $ txtCercaCognome��5W   &   $
��Left=�6?   $��23��6Z   $��	77,41,0,0q�6f   $
 Wrap=�6y   $	��Top=�6�   $	��138��6�   �� 5X      -?   5        $ btnCerca��5X   %   $��Cerca��67   $
��Left=�6G   $��220,7,-91,0q�6b   $	��Top=�6w   $��96��6�   $��57��6�   6�   �� 5Y      -@   5        $ label1_Copy��5Y   $   $��Cognome��69   $
��Left=�6K   $��	10,38,0,0q�6f   $	��Top=�6y   5Z      5[      �� 5\      -A   5        $ 
btnAnnulla��5\      $��Annulla��61   $
��Left=�6C   $��
500,10,0,0q�6^   $	��Top=�6r   $��62��6�   $��85��6�   6�   �� 5]      -B   5        $ btnModifica��5]      $��Modifica��62   $
��Left=�6E   $��
360,55,0,0q�6`   $	��Top=�6t   $	��135��6�   $��40��6�   6�   �� 5^      -C   5        $ btnFatto��5^      $��Fatto��6/   $
��Left=�6?   $��
430,10,0,0q�6Z   $	��Top=�6n   $��65��6�   $��40��6�   6�   �� 5_      -D   5        $ 
btnElimina��5_      $��Elimina��61   $
��Left=�6C   $��
567,10,0,0q�6^   $	��Top=�6r   $��53��6�   $��85��6�   6�   �� 5`      -E   5        $ 
btnRinnova��5`      $'��!Rinnova abbonamento / certificato��61   $
��Left=�6]   $��
625,10,0,0q�6x   $	��Top=�6�   $	��219��6�   $��85��6�   6�   �� 5a      -F   5        $ grpDisattiva��5a      $��Disattiva
��65   $
��Left=�6R   $��627,105,0,0q�6m   $	��Top=�6�   $	��230��6�   $	��217��6�   .��5b      � -G   5        $
 grdDisattiva��5b      $
��Left=�65   $	��185��6P   $��
0,10,-2,-3q�6]   $	��Top=�6q   $	��207��6�   .+�5c      �� -H   5        $ btnDisattiva��5c   %   $��Disattiva manuale��6;   $
��Left=�6W   $��	10,10,0,0q�6r   $	��Top=�6�   $	��187��6�   $��78��6�   6�     5d      -I   5        $ dateDisattiva��5d   )   $
��Left=�6@   $��	10,93,0,0q�6[   $	��Top=�6n   $	��187��6�   $��26��6�   �� 5e      -J   5        $ lblDisattiva��5e   $   $�� ��6:   $
��Left=�6E   $��
10,149,0,0q�6`   $	��Top=�6t   $	��187��6�   5f      5g      5i      5j      �� 5k      -K   5        $ tabAvanzate��5k      $��Avanzate��6+   .��5l      � -L   5        $
 grdAvanzate��5l      ���	#FFE5E5E5$��
10,0,-10,0q�6C   .+�5m      �� -M   5        $ grpGestioneAbbonamenti��5m      $��Gestione abbonamenti��6?   $
��Left=�6]   $��	10,10,0,0q�6x   $	��Top=�6�   $	��611��6�   $	��384��6�   .��5n      � $
��Left=�6   $	��591��6:   $��10,10,-2,-12q�6G   $	��Top=�6]   $	��364��6u   .+�5o      �� -N   5        $ label4��5o   $   $��Nome ��64   $
��Left=�6D   $��	10,10,0,0q�6_   $	��Top=�6r   �� 5p      -O   5        $ label4_Copy��5p   $   $��Prezzo��69   $
��Left=�6J   $��	10,41,0,0q�6e   $	��Top=�6x   �� 5q      -P   5        $ label4_Copy2��5q   $   $��Giorni validità��6:   $
��Left=�6T   $��
10,103,0,0q�6o   $	��Top=�6�   �� 5r      -Q   5        $ label4_Copy3��5r   $   $��Mesi validità��6:   $
��Left=�6R   $��
10,134,0,0q�6m   $	��Top=�6�   �� 5s      -R   5        $ 
txtAbbNome��5s   &   $
��Left=�6:   $��23��6U   $��
121,13,0,0q�6a   $
 Wrap=�6u   $	��Top=�6�   $	��233��6�   �� 5t      -S   5        $ txtAbbPrezzo��5t   &   $
��Left=�6<   $��23��6W   $��
121,44,0,0q�6c   $
 Wrap=�6w   $	��Top=�6�   $	��138��6�   �� 5u      -T   5        $ txtAbbGiorniVal��5u   &   $
��Left=�6?   $��23��6Z   $��121,103,0,0q�6f   $
 Wrap=�6{   $	��Top=�6�   $	��138��6�   �� 5v      -U   5        $ txtAbbMesiVal��5v   &   $
��Left=�6=   $��23��6X   $��121,137,0,0q�6d   $
 Wrap=�6y   $	��Top=�6�   $	��138��6�   �� 5w      -V   5        $ chkAbbPeriodico��5w   '   $��E' periodico��6A   $
��Left=�6X   $��
121,78,0,0q�6s   $	��Top=�6�   $
 TrueY�6�   6�     5x      -W   5        $ dtAbbonamenti��5x   '   $
��Left=�6>   $��
10,177,0,0q�6Y   $	��Top=�6m   $	��349��6�   $	��344��6�   �� 5y      -X   5        $ btnAbbAggiungi��5y   %   $��Aggiungi��6=   $
��Left=�6P   $��
15,531,0,0q�6k   $	��Top=�6   $��78��6�   $��34��6�   6�   �� 5z      -Y   5        $ btnAbbElimina��5z   %   $��Elimina��6<   $
��Left=�6N   $��276,531,0,0q�6i   $	��Top=�6~   $��78��6�   $��34��6�   6�   5{      5|      �� 5}      -Z   5        $ grpStat��5}      $��Statistiche��60   $
��Left=�6E   $��
399,10,0,0q�6`   $	��Top=�6t   $	��611��6�   $	��565��6�   .��5~      � $
��Left=�6   $	��601��6:   $��10,0,-2,-12q�6G   $	��Top=�6\   $	��545��6t   .+�5        -[   5        $ dateDa��5   )   $
��Left=�69   $��	41,11,0,0q�6T   $	��Top=�6g     5�      -\   5        $ dateFino��5�   )   $
��Left=�6;   $��	41,38,0,0q�6V   $	��Top=�6i   �� 5�      -]   5        $ label3��5�   $   $	��Da:��64   $
��Left=�6B   $��	10,10,0,0q�6]   $	��Top=�6p   $��26��6�   �� 5�      -^   5        $ label3_Copy��5�   $   $��A:��69   $
��Left=�6F   $��	10,37,0,0q�6a   $	��Top=�6t   $��26��6�   �� 5�      -_   5        $ 
btnCalcola��5�   %   $��Calcola��69   $
��Left=�6K   $	��Top=�6f   $��75��6~   $��
190,10,0,0q�6�   $��53��6�   6�   >  8System.Windows.Controls.DataVisualization.Charting.Chart  5�      -`   5          Name$ 	chartDati��5�   4   $
��Left=�6G   $��	10,68,0,0q�6b      Title$ Abbonamenti effettuati: ��6u   $	��Top=�6�   $	��525��6�   $	��278��6�      Series. 5�   "   E  ?System.Windows.Controls.DataVisualization.Charting.ColumnSeries  ?  9System.Windows.Controls.DataVisualization.Charting.Series   Title$ Abbonamenti��6?   H  BSystem.Windows.Controls.DataVisualization.Charting.DataPointSeries   DependentValuePath$ Value��6T      IndependentValuePath$	 Key��6o      ItemsSource 6�   �� 5�        5�      -a   5        $ chartIncassi��5�   4   $
��Left=�6J   $��
10,351,0,0q�6e   $ Incassi Totali: ��6y   $	��Top=�6�   $	��229��6�   $	��525��6�   . 5�   "     $ Incassi��6?   $ Value��6O   $	 Key��6j    6�   �� 5�      5�      5�      5�      5�      5�      5�      5�      