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
        private int buttonLightDelay = 750;
        private int mainDelay = 1250;
        private int marker = 0;                                         // painettujen nappien indeksinumerona toimiva muuttuja
        private int points = 0;                                         // pistelaskuri
                
        public Spedeily()
        {
            InitializeComponent();
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
        }
    

        private void yellowButton_Click(object sender, EventArgs e)
        {
            pushedButton.Add(2);
            marker++;
            yellowButton.BackColor = Color.Gold;
        }
    

        private void greenButton_Click(object sender, EventArgs e)
        {
            pushedButton.Add(3);
            marker++;
            greenButton.BackColor = Color.DarkGreen;
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
                        redTimer = new System.Timers.Timer(delay);
                        redTimer.Elapsed += redTimedEvent;
                        redTimer.AutoReset = false;
                        redTimer.Enabled = true;
                        break;
                    }
                case 2:
                    {
                        yellowButton.BackColor = Color.Yellow;
                        yellowTimer = new System.Timers.Timer(delay);
                        yellowTimer.Elapsed += yellowTimedEvent;
                        yellowTimer.AutoReset = false;
                        yellowTimer.Enabled = true;
                        break;
                    }
                case 3:
                    {
                        greenButton.BackColor = Color.SpringGreen;
                        greenTimer = new System.Timers.Timer(delay);
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
        }

        private void yellowTimedEvent(Object source, ElapsedEventArgs e)
        {
            yellowButton.BackColor = Color.Gold;
        }

        private void greenTimedEvent(Object source, ElapsedEventArgs e)
        {
            greenButton.BackColor = Color.DarkGreen;
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
            Metodi aloittaa pelin käynnistämällä ajastimen. Ajastimen aika on tällä hetkellä vielä kiinteä. Se myös nollaa listat ja marker-muuttujan aluksi varmuuden vuoksi, mikäli 
            värinappeja on paineltu pelin olematta vielä käynnissä.
        */
        private void startButton_Click(object sender, EventArgs e)
        {
            litButton.Clear();
            pushedButton.Clear();
            int delay = 1250;
            lightTimer = new System.Timers.Timer(delay);
            lightTimer.Elapsed += lightTimedEvent;
            lightTimer.AutoReset = true;
            lightTimer.Enabled = true;
        }
        
        /*
            Metodi toimii ajastimen kutsumana. Se tekee vertailun litButton- ja pushedButton-listojen välillä, jatkaa peliä ja lisää marker-muuttujaan yhden, mikäli virhelyöntiä tai 
            ylimääräisiä lyöntejä ei ole tehty, tai litButton-listassa ei ole yli neljää alkiota enempää kuin pushedButton-listassa. Mikäli ehdot eivät täyty, lightTimer-ajastin 
            pysäytetään, nappien värit muutetaan takaisin tummiksi, ja litButton- ja pushedButton-listat sekä marker- ja points-muuttujat nollataan. 
        */
        private void lightTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (compareLists() == true & litButton.Count - pushedButton.Count < 5)
            {
                points = marker;
                lightTimer.Interval = lightTimer.Interval - 50;
                lightButton(buttonLightDelay);
                redTimer.Interval = redTimer.Interval - 50;
                yellowTimer.Interval = yellowTimer.Interval - 50;
                greenTimer.Interval = greenTimer.Interval - 50;
                
            }

            else
            {
                lightTimer.Enabled = false;
                redButton.BackColor = Color.DarkRed;
                yellowButton.BackColor = Color.Gold;
                greenButton.BackColor = Color.DarkGreen;
                litButton.Clear();
                pushedButton.Clear();
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
