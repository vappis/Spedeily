using System;
using System.Collections.Generic;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

namespace Spedeily
{
    public partial class Spedeily : Form
    {
        private List<int> litButton = new List<int>();                  // lista syttyneille napeille
        private List<int> pushedButton = new List<int>();               // lista painetuille napeille
        private Random rnd = new Random();
        private static System.Timers.Timer lightTimer;                  // pelin pääajastin
        private static System.Timers.Timer redTimer;                    // nappien ajastimet
        private static System.Timers.Timer yellowTimer;
        private static System.Timers.Timer greenTimer;
        private int mainDelay = 1250;                                   // päälaskurin aloitusintervalli millisekunteina
        private int buttonLightDelay = 1000;                            // nappien aloitusintervalli millisekunteina
        private int marker = 0;                                         // painettujen nappien indeksinumerona toimiva muuttuja
        private int points = 0;                                         // pistelaskuri
        private bool redOn = false;                                     // boolean-muuttujat palaville valoille, ettei jo palavaa valoa sytytetä uudestaan (ei vielä käytössä)
        private bool yellowOn = false;
        private bool greenOn = false;
                
        public Spedeily()
        {
            InitializeComponent();
            lightTimer = new System.Timers.Timer(mainDelay);
            redTimer = new System.Timers.Timer(buttonLightDelay);
            yellowTimer = new System.Timers.Timer(buttonLightDelay);
            greenTimer = new System.Timers.Timer(buttonLightDelay);
        }

        //
        // Kaikissa seuraavissa kolmessa napissa sama toiminto, lisää elementin (napin mukaan numeroituna) pushedButton-listaan ja kasvattaa marker-muuttujaa yhdellä.
        // Lisäksi napin väri muuttuu takaisin tummemmaksi.
        //
        private void redButton_Click(object sender, EventArgs e)        
        {
            pushedButton.Add(1);
            marker++;
            redButton.BackColor = Color.DarkRed;
            redOn = false;
        }
    

        private void yellowButton_Click(object sender, EventArgs e)
        {
            pushedButton.Add(2);
            marker++;
            yellowButton.BackColor = Color.Gold;
            yellowOn = false;
        }
    

        private void greenButton_Click(object sender, EventArgs e)
        {
            pushedButton.Add(3);
            marker++;
            greenButton.BackColor = Color.DarkGreen;
            greenOn = false;
        }

        /*
            Metodi sytyttää valoja satunnaisesti, ja käynnistää aina valon kohdalla ajastimen, joka Elapsed-eventillä sammuttaa valon taas. Ajastin tuottaa vain yhden eventin.
        */
        private void lightButton(int delay)                             
        {
            int lit = rnd.Next(1, 4);
            litButton.Add(lit);
            switch (lit)
            {
                case 1:
                    {
                        redButton.BackColor = Color.Red;
                        redOn = true;
                        redTimer.Elapsed += redTimedEvent;
                        redTimer.AutoReset = false;
                        redTimer.Enabled = true;
                        break;
                    }
                case 2:
                    {
                        yellowButton.BackColor = Color.Yellow;
                        yellowOn = true;
                        yellowTimer.Elapsed += yellowTimedEvent;
                        yellowTimer.AutoReset = false;
                        yellowTimer.Enabled = true;
                        break;
                    }
                case 3:
                    {
                        greenButton.BackColor = Color.SpringGreen;
                        greenOn = true;
                        greenTimer.Elapsed += greenTimedEvent;
                        greenTimer.AutoReset = false;
                        greenTimer.Enabled = true;
                        break;
                    }
            }
        }

        private void redTimedEvent(Object source, ElapsedEventArgs e)
        {
            redButton.BackColor = Color.DarkRed;
            redOn = false;
        }

        private void yellowTimedEvent(Object source, ElapsedEventArgs e)
        {
            yellowButton.BackColor = Color.Gold;
            yellowOn = false;
        }

        private void greenTimedEvent(Object source, ElapsedEventArgs e)
        {
            greenButton.BackColor = Color.DarkGreen;
            greenOn = false;
        }
        // 
        // Metodi, jolla verrataan painettujen ja syttyneiden nappien listojen alkioita keskenään. Palauttaa "true", mikäli oikein (ja ylimääräisiä lyöntejä ei ole tehty), tai jos mitään 
        // värillistä nappia ei ole vielä painettu. 
        //
        private bool compareLists()
        {
            bool match = false;
            if (pushedButton.Count == 0)
                match = true;
            else if (pushedButton.Count > litButton.Count)
                match = false;
            else if (pushedButton[marker - 1] == litButton[marker - 1])
                match = true;
            return match;
        }

        /*
            Metodi aloittaa pelin käynnistämällä ajastimen. Se myös nollaa listat ja marker-muuttujan aluksi varmuuden vuoksi, mikäli 
            värinappeja on paineltu pelin olematta vielä käynnissä.
        */
        private void startButton_Click(object sender, EventArgs e)
        {
            if (lightTimer.Enabled == false)
            {
                litButton.Clear();
                pushedButton.Clear();
                lightTimer = new System.Timers.Timer(mainDelay);
                lightTimer.Elapsed += lightTimedEvent;
                lightTimer.AutoReset = true;
                lightTimer.Enabled = true;
            }
        }
        
        /*
            Metodi toimii ajastimen kutsumana. Se tekee vertailun litButton- ja pushedButton-listojen välillä, jatkaa peliä ja lisää marker-muuttujaan yhden, mikäli virhelyöntiä tai 
            ylimääräisiä lyöntejä ei ole tehty, tai litButton-listassa ei ole yli neljää alkiota enempää kuin pushedButton-listassa. Mikäli ehdot eivät täyty, lightTimer-ajastin 
            pysäytetään, nappien värit muutetaan takaisin tummiksi, ja litButton- ja pushedButton-listat sekä marker- ja points-muuttujat nollataan. Metodi myös nopeuttaa peliä jokaisella 
            kutsulla puolen sekunnin verran.
        */
        private void lightTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (compareLists() == true & litButton.Count - pushedButton.Count < 5)
            {
                points = marker;
                if (lightTimer.Interval > 100)
                {
                    lightTimer.Interval = lightTimer.Interval - 50;
                }
                if (buttonLightDelay > 100)
                {
                    buttonLightDelay = buttonLightDelay - 50;
                }
                lightButton(buttonLightDelay);
            }

            else
            {
                lightTimer.Enabled = false;
                redButton.BackColor = Color.DarkRed;
                yellowButton.BackColor = Color.Gold;
                greenButton.BackColor = Color.DarkGreen;
                litButton.Clear();
                pushedButton.Clear();
                buttonLightDelay = 1000;
                string message = "You got " + points + " points.";
                string caption = "GAME OVER!";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                MessageBox.Show(message, caption, buttons);
                marker = 0;
                points = 0;
            }
        }

        //
        // Ohjelman lopetusnappi
        //
        private void closeButton_Click(object sender, EventArgs e)
        {
            lightTimer.Stop();
            lightTimer.Dispose();
            redTimer.Stop();
            redTimer.Dispose();
            yellowTimer.Stop();
            yellowTimer.Dispose();
            greenTimer.Stop();
            greenTimer.Dispose();
            Close();
        }
    }
}
