using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;
using static CookCuisine.principalForm;

namespace CookCuisine
{
    public partial class principalForm : Form
    {

        [Serializable]
        public class CuisineState
        {
            public Size CuisineSize { get; set; }
            public List<MeubleState> Meubles { get; set; } = new List<MeubleState>();
        }

        [Serializable]
        public class MeubleState
        {
            public string Type { get; set; }
            public string Nom { get; set; }
            public Size Taille { get; set; }
            public Point Position { get; set; }
            public float Rotation { get; set; } = 0f;
        }

        // Variables et objets utiles
        private int buttonCounter = 1;
        private bool isDragging = false;
        private Button draggedButton;
        private Point offset;
        private bool isResizing = false;
        private Point lastMousePos;
        private Size zoneCuisineBaseSize = new Size(467, 361);
        private Point zoneCuisineBaseLocation = new Point(8,8);
        private Boolean showCat = true;  // Pour afficher ou pas les categories de meuble

        // Dictionnaire des categories de meuble
        private List<CategorieMeuble> categoriesMeubles = new List<CategorieMeuble>()
        {
            new CategorieMeuble("Chaises", new Size(60, 80), "chaises_modele.jpg"),
            new CategorieMeuble("Tables", new Size(120, 70), "table.png"),
            //new CategorieMeuble("Canapés", new Size(80, 120), "canape_blanc.png"),
            //new CategorieMeuble("Microondes", new Size(80, 120), "microondes_modele.jpg"),
            //new CategorieMeuble("Plan_de_travail", new Size(80, 120), ""),
            new CategorieMeuble("Armoires", new Size(80, 120), "armoires_modele.jpg"),
            new CategorieMeuble("Eviers", new Size(80, 120), "evier_00.png"),
            new CategorieMeuble("Cuisinières", new Size(80, 76), "cuisiniere_abc.png"),
            new CategorieMeuble("Fours", new Size(80, 76), "cuisinière_modele.jpg"),
            new CategorieMeuble("Tiroirs", new Size(198, 50), "commode.png"),
            new CategorieMeuble("Plan de travail", new Size(300, 50), "PlanTravail.png"),
            new CategorieMeuble("Frigos", new Size(80, 120), "frigo_modele.jpg")
        };

        // Dictionnaire des meubles avec nom, taille et image
        private Dictionary<string, Meuble> meubles = new Dictionary<string, Meuble>()
        {
            {"Chaise", new Meuble("Chaises", "Chaise", new Size(60, 80), new Point(20, 20), "chair.png")},
            {"Chaise2", new Meuble("Chaises", "Chaise", new Size(60, 80), new Point(20, 20), "chaises_modele.jpg")},
            {"Table", new Meuble("Tables", "Table", new Size(120, 70), new Point(20, 20), "table.png")},
            //{"sofa", new Meuble("Canapés", "sofa", new Size(150, 80), new Point(20, 20), "sofa.png")},
            {"Armoire", new Meuble("Armoires", "Armoire", new Size(80, 120), new Point(20, 20), "armoires_modele.jpg")},
            {"Armoire2", new Meuble("Armoires", "Armoire", new Size(80, 120), new Point(20, 20), "armoires_modele.jpg")},
            {"Armoire3", new Meuble("Armoires", "Armoire", new Size(80, 120), new Point(20, 20), "armoires_modele.jpg")},
            {"Armoire4", new Meuble("Armoires", "Armoire", new Size(80, 120), new Point(20, 20), "armoires_modele.jpg")},
            {"Armoire5", new Meuble("Armoires", "Armoire", new Size(80, 120), new Point(20, 20), "armoires_modele.jpg")},
            {"Armoire6", new Meuble("Armoires", "Armoire", new Size(80, 120), new Point(20, 20), "armoires_modele.jpg")},
            {"Four", new Meuble("Fours", "Fours", new Size(80, 80), new Point(20, 20), "cuisinière_modele.jpg")},
            {"evier_00", new Meuble("Eviers", "evier_00", new Size(73, 41), new Point(20, 20), "evier_00.png")},
            {"Tiroirs", new Meuble("Tiroirs", "Tiroirs", new Size(73, 41), new Point(20, 20), "commode.png")},
            {"Frigo 1", new Meuble("Frigos", "Frigos", new Size(80, 80), new Point(20, 20), "frigo_modele.jpg")},
            {"Plan de travail 1", new Meuble("Plan de travail", "Plan de travail", new Size(150, 50), new Point(20, 20), "PlanTravail.png")},
            {"cuisiniere_abc", new Meuble("Cuisinières", "cuisiniere_abc", new Size(80, 76), new Point(20, 20), "cuisiniere_abc.png")},
            //{"Canape_blanc", new Meuble("Canapés", "Canape_blanc", new Size(158, 86), new Point(20, 20), "canape_blanc.png")}
        };

        // Dictionnaire pour stocker les images de meubles chargées
        private Dictionary<string, Image> meubleImages = new Dictionary<string, Image>();
        // Dictionnaire pour stocker les images de categorie de meubles chargées
        private Dictionary<string, Image> catMeubleImages = new Dictionary<string, Image>();
        // Dictionnaire pour stocker les position des meubles
        private Dictionary<String, Point> meublesPositions = new Dictionary<String, Point>();

        public principalForm()
        {
            InitializeComponent();
            this.backButton.Image = Image.FromFile(Path.Combine(Application.StartupPath, "Images", "backBTN.png"));
            this.undoToolStripMenuItem.Image = Image.FromFile(Path.Combine(Application.StartupPath, "Images", "undo.png"));
            this.redoToolStripMenuItem.Image = Image.FromFile(Path.Combine(Application.StartupPath, "Images", "redo.png"));
            SetupUI();
            LoadImages();
            CreateFurniturePanel();
            //CreateAddFurnitureButtons();
            CreateMeubleCategories();

        }

        private void SetupUI()
        {
            // Abonnement aux événements de redimensionnement de la fenêtre
            this.Resize += new EventHandler(principalForm_Resize);
            // Abonnement aux événements de redimensionnement de la zoneCuisine
            this.zoneCuisine.Resize += new EventHandler(zoneCuisine_Resize);
            // Gestion des événements pour le redimensionnement
            gripItem.MouseDown += GripItem_MouseDown;
            gripItem.MouseMove += GripItem_MouseMove;
            gripItem.MouseUp += GripItem_MouseUp;
            // Ajout des évènements pour empêcher la selection des labels w et h
            width_label.GotFocus += (s, e) => menuBar.Focus();
            height_label.GotFocus += (s, e) => menuBar.Focus();
            // les texbox w et h recupèrent la taille de la cuisine
            width_texbox.Text = zoneCuisine.Width.ToString("F0");
            height_texbox.Text += zoneCuisine.Height.ToString("F0");
            // On abonne les texbox de dimension de la cuisine aux évènements nécéssaires
            width_texbox.KeyDown += textBoxDimension_KeyDown;
            height_texbox.KeyDown += textBoxDimension_KeyDown;
            width_texbox.Leave += textBoxDimension_Leave;
            height_texbox.Leave += textBoxDimension_Leave;
            // Evènement pour vérifier la saisie avant d'appliquer*
            width_texbox.TextChanged += width_texbox_Textchanged;
            height_texbox.TextChanged += height_texbox_Textchanged;
        }

        private void Width_texbox_MouseDown(object sender, MouseEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void InitializeComponent()
        {
            this.mainTLPanel = new System.Windows.Forms.TableLayoutPanel();
            this.menuFLPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.menuBar = new System.Windows.Forms.MenuStrip();
            this.fichier = new System.Windows.Forms.ToolStripMenuItem();
            this.load = new System.Windows.Forms.ToolStripMenuItem();
            this.save = new System.Windows.Forms.ToolStripMenuItem();
            this.quitter = new System.Windows.Forms.ToolStripMenuItem();
            this.edition = new System.Windows.Forms.ToolStripMenuItem();
            this.aide = new System.Windows.Forms.ToolStripMenuItem();
            this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.redoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.width_label = new System.Windows.Forms.ToolStripTextBox();
            this.width_texbox = new System.Windows.Forms.ToolStripTextBox();
            this.height_label = new System.Windows.Forms.ToolStripTextBox();
            this.height_texbox = new System.Windows.Forms.ToolStripTextBox();
            this.meubleFLPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.searchPanel = new System.Windows.Forms.Panel();
            this.backButton = new System.Windows.Forms.Button();
            this.searchMeubles = new System.Windows.Forms.TextBox();
            this.meublesPanel = new System.Windows.Forms.Panel();
            this.cuisinePanel = new System.Windows.Forms.Panel();
            this.zoneCuisine = new System.Windows.Forms.Panel();
            this.gripItem = new System.Windows.Forms.Panel();
            this.mainTLPanel.SuspendLayout();
            this.menuFLPanel.SuspendLayout();
            this.menuBar.SuspendLayout();
            this.meubleFLPanel.SuspendLayout();
            this.searchPanel.SuspendLayout();
            this.cuisinePanel.SuspendLayout();
            this.zoneCuisine.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTLPanel
            // 
            this.mainTLPanel.BackColor = System.Drawing.Color.Transparent;
            this.mainTLPanel.ColumnCount = 2;
            this.mainTLPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16F));
            this.mainTLPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 84F));
            this.mainTLPanel.Controls.Add(this.menuFLPanel, 0, 0);
            this.mainTLPanel.Controls.Add(this.meubleFLPanel, 0, 1);
            this.mainTLPanel.Controls.Add(this.cuisinePanel, 1, 1);
            this.mainTLPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTLPanel.Location = new System.Drawing.Point(0, 0);
            this.mainTLPanel.MinimumSize = new System.Drawing.Size(900, 600);
            this.mainTLPanel.Name = "mainTLPanel";
            this.mainTLPanel.RowCount = 2;
            this.mainTLPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 6F));
            this.mainTLPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 94F));
            this.mainTLPanel.Size = new System.Drawing.Size(900, 600);
            this.mainTLPanel.TabIndex = 0;
            // 
            // menuFLPanel
            // 
            this.menuFLPanel.BackColor = System.Drawing.Color.Transparent;
            this.mainTLPanel.SetColumnSpan(this.menuFLPanel, 2);
            this.menuFLPanel.Controls.Add(this.menuBar);
            this.menuFLPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.menuFLPanel.Location = new System.Drawing.Point(0, 0);
            this.menuFLPanel.Margin = new System.Windows.Forms.Padding(0);
            this.menuFLPanel.Name = "menuFLPanel";
            this.menuFLPanel.Size = new System.Drawing.Size(900, 36);
            this.menuFLPanel.TabIndex = 2;
            // 
            // menuBar
            // 
            this.menuBar.AutoSize = false;
            this.menuBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(201)))), ((int)(((byte)(170)))));
            this.menuBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fichier,
            this.edition,
            this.aide,
            this.undoToolStripMenuItem,
            this.redoToolStripMenuItem,
            this.width_label,
            this.width_texbox,
            this.height_label,
            this.height_texbox});
            this.menuBar.Location = new System.Drawing.Point(0, 0);
            this.menuBar.Name = "menuBar";
            this.menuBar.Size = new System.Drawing.Size(900, 36);
            this.menuBar.TabIndex = 0;
            // 
            // fichier
            // 
            this.fichier.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.load,
            this.save,
            this.quitter});
            this.fichier.Name = "fichier";
            this.fichier.Size = new System.Drawing.Size(54, 32);
            this.fichier.Text = "Fichier";
            // 
            // load
            // 
            this.load.Name = "load";
            this.load.Size = new System.Drawing.Size(139, 22);
            this.load.Text = "Charger";
            this.load.Click += new System.EventHandler(this.load_Click);
            // 
            // save
            // 
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(139, 22);
            this.save.Text = "Sauvegarder";
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // quitter
            // 
            this.quitter.Name = "quitter";
            this.quitter.Size = new System.Drawing.Size(139, 22);
            this.quitter.Text = "Quitter";
            this.quitter.Click += new System.EventHandler(this.quitter_Click);
            // 
            // edition
            // 
            this.edition.Name = "edition";
            this.edition.Size = new System.Drawing.Size(56, 32);
            this.edition.Text = "Edition";
            // 
            // aide
            // 
            this.aide.Name = "aide";
            this.aide.Size = new System.Drawing.Size(43, 32);
            this.aide.Text = "Aide";
            // 
            // undoToolStripMenuItem
            // 
            this.undoToolStripMenuItem.Margin = new System.Windows.Forms.Padding(80, 0, 0, 0);
            this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            this.undoToolStripMenuItem.Size = new System.Drawing.Size(12, 32);
            // 
            // redoToolStripMenuItem
            // 
            this.redoToolStripMenuItem.Margin = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            this.redoToolStripMenuItem.Size = new System.Drawing.Size(12, 32);
            // 
            // width_label
            // 
            this.width_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(201)))), ((int)(((byte)(170)))));
            this.width_label.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.width_label.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.width_label.Margin = new System.Windows.Forms.Padding(50, 0, 1, 0);
            this.width_label.Name = "width_label";
            this.width_label.ReadOnly = true;
            this.width_label.Size = new System.Drawing.Size(15, 32);
            this.width_label.Text = "w";
            // 
            // width_texbox
            // 
            this.width_texbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(201)))), ((int)(((byte)(170)))));
            this.width_texbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.width_texbox.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.width_texbox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.width_texbox.Name = "width_texbox";
            this.width_texbox.Size = new System.Drawing.Size(50, 32);
            this.width_texbox.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // height_label
            // 
            this.height_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(201)))), ((int)(((byte)(170)))));
            this.height_label.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.height_label.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.height_label.Margin = new System.Windows.Forms.Padding(20, 0, 1, 0);
            this.height_label.Name = "height_label";
            this.height_label.ReadOnly = true;
            this.height_label.Size = new System.Drawing.Size(15, 32);
            this.height_label.Text = "h";
            // 
            // height_texbox
            // 
            this.height_texbox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(201)))), ((int)(((byte)(170)))));
            this.height_texbox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.height_texbox.Font = new System.Drawing.Font("Segoe UI Semibold", 9F);
            this.height_texbox.Name = "height_texbox";
            this.height_texbox.Size = new System.Drawing.Size(50, 32);
            this.height_texbox.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // meubleFLPanel
            // 
            this.meubleFLPanel.AutoScroll = true;
            this.meubleFLPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(217)))), ((int)(((byte)(161)))), ((int)(((byte)(128)))));
            this.meubleFLPanel.Controls.Add(this.searchPanel);
            this.meubleFLPanel.Controls.Add(this.meublesPanel);
            this.meubleFLPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.meubleFLPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.meubleFLPanel.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.meubleFLPanel.Location = new System.Drawing.Point(0, 36);
            this.meubleFLPanel.Margin = new System.Windows.Forms.Padding(0);
            this.meubleFLPanel.Name = "meubleFLPanel";
            this.meubleFLPanel.Size = new System.Drawing.Size(144, 564);
            this.meubleFLPanel.TabIndex = 0;
            this.meubleFLPanel.WrapContents = false;
            // 
            // searchPanel
            // 
            this.searchPanel.BackColor = System.Drawing.Color.Transparent;
            this.searchPanel.Controls.Add(this.backButton);
            this.searchPanel.Controls.Add(this.searchMeubles);
            this.searchPanel.Location = new System.Drawing.Point(10, 6);
            this.searchPanel.Margin = new System.Windows.Forms.Padding(10, 6, 10, 3);
            this.searchPanel.Name = "searchPanel";
            this.searchPanel.Size = new System.Drawing.Size(124, 25);
            this.searchPanel.TabIndex = 0;
            // 
            // backButton
            // 
            this.backButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.backButton.Location = new System.Drawing.Point(0, 0);
            this.backButton.Name = "backButton";
            this.backButton.Size = new System.Drawing.Size(44, 25);
            this.backButton.TabIndex = 1;
            this.backButton.UseVisualStyleBackColor = true;
            this.backButton.Click += new System.EventHandler(this.backButton_Click_1);
            // 
            // searchMeubles
            // 
            this.searchMeubles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.searchMeubles.Location = new System.Drawing.Point(50, 0);
            this.searchMeubles.Multiline = true;
            this.searchMeubles.Name = "searchMeubles";
            this.searchMeubles.Size = new System.Drawing.Size(74, 25);
            this.searchMeubles.TabIndex = 0;
            // 
            // meublesPanel
            // 
            this.meublesPanel.AccessibleName = "furniturePanel";
            this.meublesPanel.BackColor = System.Drawing.Color.Transparent;
            this.meublesPanel.Location = new System.Drawing.Point(10, 37);
            this.meublesPanel.Margin = new System.Windows.Forms.Padding(10, 3, 10, 8);
            this.meublesPanel.Name = "meublesPanel";
            this.meublesPanel.Size = new System.Drawing.Size(124, 519);
            this.meublesPanel.TabIndex = 0;
            // 
            // cuisinePanel
            // 
            this.cuisinePanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(246)))), ((int)(((byte)(241)))));
            this.cuisinePanel.Controls.Add(this.zoneCuisine);
            this.cuisinePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cuisinePanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cuisinePanel.Location = new System.Drawing.Point(147, 39);
            this.cuisinePanel.Name = "cuisinePanel";
            this.cuisinePanel.Size = new System.Drawing.Size(750, 558);
            this.cuisinePanel.TabIndex = 3;
            // 
            // zoneCuisine
            // 
            this.zoneCuisine.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.zoneCuisine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.zoneCuisine.Controls.Add(this.gripItem);
            this.zoneCuisine.Location = new System.Drawing.Point(8, 8);
            this.zoneCuisine.Margin = new System.Windows.Forms.Padding(8);
            this.zoneCuisine.MaximumSize = new System.Drawing.Size(734, 542);
            this.zoneCuisine.Name = "zoneCuisine";
            this.zoneCuisine.Size = new System.Drawing.Size(467, 361);
            this.zoneCuisine.TabIndex = 1;
            // 
            // gripItem
            // 
            this.gripItem.BackColor = System.Drawing.Color.Gray;
            this.gripItem.Cursor = System.Windows.Forms.Cursors.SizeNWSE;
            this.gripItem.Location = new System.Drawing.Point(455, 349);
            this.gripItem.Margin = new System.Windows.Forms.Padding(0);
            this.gripItem.Name = "gripItem";
            this.gripItem.Size = new System.Drawing.Size(10, 10);
            this.gripItem.TabIndex = 0;
            // 
            // principalForm
            // 
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Controls.Add(this.mainTLPanel);
            this.DoubleBuffered = true;
            this.MainMenuStrip = this.menuBar;
            this.MinimumSize = new System.Drawing.Size(916, 639);
            this.Name = "principalForm";
            this.Text = "CookCuisine";
            this.mainTLPanel.ResumeLayout(false);
            this.menuFLPanel.ResumeLayout(false);
            this.menuBar.ResumeLayout(false);
            this.menuBar.PerformLayout();
            this.meubleFLPanel.ResumeLayout(false);
            this.searchPanel.ResumeLayout(false);
            this.searchPanel.PerformLayout();
            this.cuisinePanel.ResumeLayout(false);
            this.zoneCuisine.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void GripItem_MouseDown(object sender, MouseEventArgs e)
        {
            isResizing = true;
            lastMousePos = this.PointToClient(Control.MousePosition);
        }

        private void GripItem_MouseMove(object sender, MouseEventArgs e)
        {
            if (isResizing)
            {
                Point currentMousePos = this.PointToClient(Control.MousePosition);
                int dx = currentMousePos.X - lastMousePos.X;
                int dy = currentMousePos.Y - lastMousePos.Y;
                int newWidth = Math.Max(50, zoneCuisine.Width + dx);
                int newHeight = Math.Max(50, zoneCuisine.Height + dy);

                lastMousePos = currentMousePos;
                zoneCuisine.Size = new Size(newWidth, newHeight);
                zoneCuisineBaseSize = new Size(newWidth, newHeight);
                gripItem.Location = new Point(zoneCuisine.Width - gripItem.Width, zoneCuisine.Height - gripItem.Height);
            }
        }
        private void GripItem_MouseUp(object sender, MouseEventArgs e)
        {
            isResizing = false;
        }

        private void CreateFurniturePanel()
        {
            searchMeubles.TextChanged += SearchBox_TextChanged;
        }

        // Méthode pour calculer les ratios de redimensionnement
        private (float ratioW, float ratioH) GetCuisineResizeRatios()
        {
            return ((float)cuisinePanel.Width / 756f, (float)cuisinePanel.Height / 564f);
        }

        // Méthode pour réagir au redimensionnement de la fenêtre
        private void principalForm_Resize(object sender, EventArgs e)
        {
            // Mettre à jour la taille de searchPanel et meublesPanel 
            searchPanel.Width = meubleFLPanel.Width - 20; // marges de 20
            searchPanel.Height = Math.Min((int)(meubleFLPanel.Height / 22.56),35); // hauteur max de 35
            meublesPanel.Width = meubleFLPanel.Width - 20; // marges de 20
            meublesPanel.Height = meubleFLPanel.Height - searchPanel.Height - 20; // marges de 20
            // Mettre à jour la taille de texte de la barre de recherche
            searchMeubles.Font = new System.Drawing.Font("Segoe UI", Math.Min(searchMeubles.Height/2.58f,15));
            // Mettre à jour la taille du menu et de ses éléments
            menuBar.Size = menuFLPanel.Size;
            // Mettre à jour la taille de la police du menu avec echelle de 5.33 par rapport à la hauteur
            menuBar.Font = new Font(menuBar.Font.FontFamily, Math.Min(menuFLPanel.Height / 4.33f,12));
            width_label.Font = new Font(menuBar.Font.FontFamily, Math.Min(menuFLPanel.Height / 4.33f, 12));
            height_label.Font = new Font(menuBar.Font.FontFamily, Math.Min(menuFLPanel.Height / 4.33f, 12));
            width_texbox.Font = new Font("Segoe UI Semibold", Math.Min(menuFLPanel.Height / 4.33f, 12));
            height_texbox.Font = new Font("Segoe UI Semibold", Math.Min(menuFLPanel.Height / 4.33f, 12));
            width_texbox.Size = new Size((int)(menuFLPanel.Width / 18), menuFLPanel.Height - 10);
            height_texbox.Size = new Size((int)(menuFLPanel.Width / 18), menuFLPanel.Height - 10);
            // Calculons le ratio de redimentionnement dans la zone de cuisine
            var (ratioZC_W, ratioZC_H) = GetCuisineResizeRatios();
            //Console.WriteLine($"{ratioZC_W} ... {ratioZC_H} + ************ + {(int)Math.Round(ratioZC_W)} ... {(int)ratioZC_H}");

            // Redimentionner zoneCuisine et mettre à jour sa taille maximale
            zoneCuisine.MaximumSize = new Size(  // Ici on ajuste la taille max pour la zoneCuisine
                (int)Math.Round(cuisinePanel.Width - 2 * zoneCuisineBaseLocation.X * ratioZC_W),
                (int)Math.Round(cuisinePanel.Height - 2 * zoneCuisineBaseLocation.Y * ratioZC_H)
            );
            zoneCuisine.Size = new Size(
                (int)Math.Round(zoneCuisineBaseSize.Width * ratioZC_W),
                (int)Math.Round(zoneCuisineBaseSize.Height * ratioZC_H)
            );
            zoneCuisine.Location = new Point((int)(zoneCuisineBaseLocation.X*ratioZC_W), (int)(zoneCuisineBaseLocation.Y*ratioZC_H));
            // Mettre à jour la position du gripItem (carré pour redimentionner la cuisine)
            gripItem.Location = new Point(zoneCuisine.Width - gripItem.Width, zoneCuisine.Height - gripItem.Height);
            // Redimentionner les boutons de categorie de meubles
            ResizeCategoriesMeubles();
            // Redimentionner les boutons d'ajout de meubles
            ResizeFurnitureButtons();
            // Redimentionner les meubles selon la taille de la fenetre
            ResizeFurniture(ratioZC_W,ratioZC_H);
        }
        // Méthode pour réagir au redimentionnement de la zone de cuisine
        private void zoneCuisine_Resize(object sender, EventArgs e)
        {
            foreach (Control ctrl in cuisinePanel.Controls)
            {
                if (ctrl is Button btn && btn.Name.StartsWith("btn_"))
                {
                    // Vérifier si les meubles sont toujours dans la cuisine
                    checkMeubleDansCuisine(btn);
                }
            }
            // les texbox w et h recupèrent la taille de la cuisine
            var (ratioZC_W, ratioZC_H) = GetCuisineResizeRatios();
            width_texbox.Text = (zoneCuisine.Width / ratioZC_W).ToString("F0");
            height_texbox.Text = (zoneCuisine.Height / ratioZC_H).ToString("F0");

        }

        // Méthode pour la barre de recherche
        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            // Récupérer le texte de la barre de recherche et le convertir en minuscules
            string searchText = searchMeubles.Text.ToLower();
            int yPosition = 10;
            // Filtrer les boutons en fonction du texte de recherche
            var buttons = meublesPanel.Controls.OfType<Button>()
                              .Where(b => b.Tag != null)
                              .OrderBy(b => b.Tag.ToString());
            foreach (var btn in buttons)
            {
                bool shouldShow = btn.Tag.ToString().ToLower().Contains(searchText) ||
                                 string.IsNullOrEmpty(searchText);
                btn.Visible = shouldShow;
                if (shouldShow)
                {
                    btn.Location = new Point(10, yPosition);
                    yPosition += btn.Height + 5;
                }
            }
            // Mettre à jour la taille minimale de défilement
            meublesPanel.AutoScrollMinSize = new Size(0, yPosition);
        }

        // Charger les images
        private void LoadImages()
        {
            string imagePath = Path.Combine(Application.StartupPath, "Images");

            foreach (var meuble in meubles)
            {
                try
                {
                    string fullPath = Path.Combine(imagePath, meuble.Value.ImageName);
                    if (File.Exists(fullPath))
                    {
                        meubleImages[meuble.Key] = Image.FromFile(fullPath);
                    }
                    else
                    {
                        meubleImages[meuble.Key] = CreateDefaultImage(meuble.Value.Size, meuble.Key);
                    }
                }
                catch
                {
                    meubleImages[meuble.Key] = CreateDefaultImage(meuble.Value.Size, meuble.Key);
                }
            }
            foreach (var categorie in categoriesMeubles)
            {
                try
                {
                    string fullPath = Path.Combine(imagePath, categorie.ImageName);
                    if (File.Exists(fullPath))
                    {
                        catMeubleImages[categorie.Type] = Image.FromFile(fullPath);
                    }
                    else
                    {
                        catMeubleImages[categorie.Type] = CreateDefaultImage(categorie.Taille, categorie.Type);
                    }
                }
                catch
                {
                    catMeubleImages[categorie.Type] = CreateDefaultImage(categorie.Taille, categorie.Type);
                }
            }
        }

        private Image CreateDefaultImage(Size size, string text)
        {
            Bitmap bmp = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.LightGray);
                using (Font font = new Font("Arial", 8))
                {
                    g.DrawString(text, font, Brushes.Black,
                        new PointF(5, (size.Height - font.Height) / 2));
                }
                g.DrawRectangle(Pens.Black, 0, 0, size.Width - 1, size.Height - 1);
            }
            return bmp;
        }

        // Méthode pour créer les boutons de catégories de meubles
        private void CreateMeubleCategories()
        {
            backButton.Enabled = false;
            meublesPanel.Controls.Clear(); // Vider le menu
            showCat = true; // On affiche les categories
            meublesPanel.Controls.Clear();
            meublesPanel.AutoScroll = true;
            meublesPanel.VerticalScroll.Enabled = true;
            meublesPanel.VerticalScroll.Visible = true;
            int yPosition = 10;
            int xPosition = meublesPanel.Width / 20;
            int buttonHeight = (meublesPanel.Height - 40) / 10;
            int buttonWidth = meublesPanel.Width - (2 * xPosition);

            foreach (var categorie in categoriesMeubles.OrderBy(m => m.Type))
            {
                // Vérifier si une image existe pour cette catégorie
                Image image = catMeubleImages[categorie.Type];

                // Créer le bouton
                Button meubleCatBtn = new Button
                {
                    Name = $"cat_meuble_btn_{categorie.Type}",
                    Text = " " + categorie.Type,
                    Location = new Point(xPosition, yPosition),
                    Size = new Size(buttonWidth, buttonHeight),
                    Tag = categorie.Type,
                    Font = new Font("Arial", Math.Min(buttonHeight * 0.175f, 11)),
                    Image = new Bitmap(image, new Size(buttonHeight, buttonHeight)),
                    TextImageRelation = TextImageRelation.ImageBeforeText,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ImageAlign = ContentAlignment.TopLeft
                };
                // Ajouter l'événement de clic
                meubleCatBtn.Click += (sender, e) => CreateAddFurnitureButtons(categorie.Type);
                // Ajouter le bouton au panel
                meublesPanel.Controls.Add(meubleCatBtn);
                yPosition += meubleCatBtn.Height + meubleCatBtn.Height / 5;
            }

            // Mettre à jour la taille minimale de défilement
            meublesPanel.AutoScrollMinSize = new Size(0, yPosition);
        }

        // Méthode pour redimentionner les boutons de categorie d'image
        private void ResizeCategoriesMeubles()
        {
            int yPosition = 10;
            int xPosition = meublesPanel.Width / 20;
            int buttonHeight = (meublesPanel.Height - 40) / 10;
            int buttonWidth = meublesPanel.Width - (2 * xPosition);

            foreach (Control ctrl in meublesPanel.Controls)
            {
                if (ctrl is Button btn && btn.Name.StartsWith("cat_meuble_btn_"))
                {
                    btn.Size = new Size(buttonWidth, buttonHeight);
                    btn.Location = new Point(xPosition, yPosition);
                    // Ajuster taille police
                    float newFontSize = Math.Min(btn.Height * 0.175f, 11);
                    btn.Font = new Font(btn.Font.FontFamily, newFontSize, FontStyle.Regular);
                    // Ajuster la taille de l'image du bouton
                    btn.Image = ResizeImage(catMeubleImages[(string)btn.Tag], new Size(btn.Height, btn.Height));
                    btn.TextImageRelation = TextImageRelation.ImageBeforeText;
                    btn.TextAlign = ContentAlignment.MiddleCenter;
                    btn.ImageAlign = ContentAlignment.TopLeft;
                    yPosition += btn.Height + btn.Height / 5;
                }
            }
        }


        // Méthode pour créer les boutons d'ajout de Meubles
        private void CreateAddFurnitureButtons(String categorie)
        {
            backButton.Enabled = true;
            meublesPanel.Controls.Clear(); // Vider le menu
            showCat = false; // On affiche les meubles
            meublesPanel.AutoScroll = true;
            meublesPanel.VerticalScroll.Enabled = true;
            meublesPanel.VerticalScroll.Visible = true;
            int yPosition = 10;
            int xPosition = meublesPanel.Width / 20;
            int buttonHeight = (meublesPanel.Height - 40) / 10;
            int buttonWidth = meublesPanel.Width - (2*xPosition + 15);

            foreach (var meuble in meubles.OrderBy(m => m.Key))
            {
                if (meubleImages.TryGetValue(meuble.Key, out var image) && meubles[meuble.Key].Type == categorie
                    && meubles.TryGetValue(meuble.Key, out var meubleData) && meubleData.Type == categorie)
                {
                    // Créer le bouton
                    Button addButton = new Button
                    {
                        Name = $"add_Button_{meuble.Key}",
                        Text = " " + meuble.Key,
                        Location = new Point(xPosition, yPosition),
                        Size = new Size(buttonWidth, buttonHeight),
                        Tag = meuble.Key,
                        Font = new Font("Arial", Math.Min(buttonHeight * 0.175f, 11)),
                        Image = new Bitmap(image, new Size(buttonHeight, buttonHeight)),
                        TextImageRelation = TextImageRelation.ImageBeforeText,
                        TextAlign = ContentAlignment.MiddleCenter,
                        ImageAlign = ContentAlignment.TopLeft
                    };
                    // Ajouter l'événement de clic
                    addButton.Click += AddFurnitureButton_Click;
                    // Ajouter le bouton au panel
                    meublesPanel.Controls.Add(addButton);
                    yPosition += addButton.Height + addButton.Height/5;
                }
            }

            // Mettre à jour la taille minimale de défilement
            meublesPanel.AutoScrollMinSize = new Size(0, yPosition);
        }

        // Méthode pour redimentionner les boutons d'ajout de meubles
        private void ResizeFurnitureButtons()
        {
            int yPosition = 10;
            int xPosition = meublesPanel.Width/20;
            int buttonHeight = (meublesPanel.Height - 40) / 10;
            int buttonWidth = meublesPanel.Width - (2*xPosition + 15);

            foreach (Control ctrl in meublesPanel.Controls)
            {
                if (ctrl is Button retourB && retourB.Name.StartsWith("retourBTN")) {
                    retourB.Size = new Size(50, buttonWidth);
                }
                else if (ctrl is Button btn && btn.Name.StartsWith("add_Button_"))
                {
                    btn.Size = new Size( buttonWidth, buttonHeight);
                    btn.Location = new Point(xPosition, yPosition);
                    // Ajuster taille police
                    float newFontSize = Math.Min(btn.Height * 0.175f, 11);
                    btn.Font = new Font(btn.Font.FontFamily, newFontSize, FontStyle.Regular);
                    // Ajuster la taille de l'image du bouton
                    btn.Image = ResizeImage(meubleImages[(string)btn.Tag], new Size(btn.Height, btn.Height));
                    btn.TextImageRelation = TextImageRelation.ImageBeforeText;
                    btn.TextAlign = ContentAlignment.MiddleCenter;
                    btn.ImageAlign = ContentAlignment.TopLeft;
                    yPosition += btn.Height + btn.Height/5;
                }
            }
        }

        // Méthode pour redimentionner les meubles quand la fenetre s'agrandit
        private void ResizeFurniture(float ratio_W, float ratio_H)
        {
            foreach (Control ctrl in cuisinePanel.Controls)
            {
                if (ctrl is Button btn && btn.Name.StartsWith("btn_"))
                {
                    // Récupérer les dimensions et positions originales
                    Size originalSize = meubles[(string)btn.Tag].Size;
                    Point originalPosition = meublesPositions[btn.Name];

                    // Calculer la nouvelle taille et position
                    int newWidth = Math.Max(10, (int)Math.Round(originalSize.Width * ratio_W));
                    int newHeight = Math.Max(10, (int)Math.Round(originalSize.Height * ratio_H));
                    int newX = Math.Max(0, (int)Math.Round(originalPosition.X * ratio_W));
                    int newY = Math.Max(0, (int)Math.Round(originalPosition.Y * ratio_H));

                    // Appliquer les nouvelles dimensions et positions
                    btn.Size = new Size(newWidth, newHeight);
                    btn.Location = new Point(newX, newY);

                    // Ajuster l'image du meuble pour correspondre à la nouvelle taille
                    btn.Image = ResizeImage(meubleImages[(string)btn.Tag], btn.Size);

                    // Vérifier si le meuble reste dans les limites de zoneCuisine
                    checkMeubleDansCuisine(btn);
                }
            }
        }

        // Méthode pour redimentionner nos images proprement
        public static Bitmap ResizeImage(Image image, Size newSize)
        {
            var resized = new Bitmap(newSize.Width, newSize.Height);
            using (var graphics = Graphics.FromImage(resized))
            {
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                graphics.DrawImage(image, 0, 0, newSize.Width, newSize.Height);
            }
            return resized;
        }

        // Méthode pour créer un meuble quand on clique sur un bouton
        private void AddFurnitureButton_Click(object sender, EventArgs e)
        {
            Button addButton = (Button)sender;
            string meubleType = (string)addButton.Tag;
            var (ratioZC_W, ratioZC_H) =  GetCuisineResizeRatios();
            createMeuble(meubleType,ratioZC_W,ratioZC_H);
        }

        // Méthode pour générer le meuble
        private void createMeuble(string meubleType, float ratio_W, float ratio_H)
        {
            if (meubles.TryGetValue(meubleType, out var properties) &&
                meubleImages.TryGetValue(meubleType, out var image))
            {
                Size b_size = new Size((int)Math.Round(properties.Size.Width * ratio_W), (int)Math.Round(properties.Size.Height * ratio_H));
                Point b_position = new Point((int)Math.Round(properties.Location.X*ratio_W), (int)Math.Round(properties.Location.Y*ratio_H));
                Image resizedImage = new Bitmap(image, b_size);
                Button newButton = new Button
                {
                    Name = "btn_" + meubleType + "_" + (buttonCounter++),
                    Text = "",
                    Size = b_size,
                    Location = b_position,
                    FlatStyle = FlatStyle.Flat,
                    Tag = meubleType,
                    Image = resizedImage,
                    BackgroundImageLayout = ImageLayout.Stretch,         
                    BackColor = Color.Transparent
                };
                meublesPositions[newButton.Name] = b_position;
                Console.WriteLine(meublesPositions[newButton.Name]);

                SetupButton(newButton);
                this.cuisinePanel.Controls.Add(newButton);
                newButton.BringToFront();
            }
        }

        private void SetupButton(Button btn)
        {
            btn.MouseDown += Button_MouseDown;
            btn.MouseUp += Button_MouseUp;
            btn.MouseMove += Button_MouseMove;
            ContextMenuStrip contextMenu = new ContextMenuStrip();
            ToolStripMenuItem rotateItem = new ToolStripMenuItem("Rotation 90°");
            rotateItem.Click += (sender, e) => RotateButton(btn);
            contextMenu.Items.Add(rotateItem);
            ToolStripMenuItem duplicateItem = new ToolStripMenuItem("Dupliquer");
            duplicateItem.Click += (sender, e) => DuplicateButton(btn);
            contextMenu.Items.Add(duplicateItem);
            ToolStripMenuItem deleteItem = new ToolStripMenuItem("Supprimer");
            deleteItem.Click += (sender, e) => DeleteButton(btn);
            contextMenu.Items.Add(deleteItem);
            btn.ContextMenuStrip = contextMenu;
            btn.BringToFront();
        }


        private void Button_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isDragging = true;
                draggedButton = (Button)sender;
                offset = e.Location;
                draggedButton.Capture = true;
                draggedButton.BringToFront();
            }
            else if (e.Button == MouseButtons.Right)
            {
                ((Button)sender).ContextMenuStrip?.Show((Button)sender, e.Location);
            }
        }

        private void Button_MouseUp(object sender, MouseEventArgs e)
        {
            isDragging = false;
            if (draggedButton != null)
            {
                draggedButton.Capture = false;
                draggedButton = null;
            }
        }

        // Méthode pour déplacer le meuble
        private void Button_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && draggedButton != null)
            {
                // Obtenir la position de la souris par rapport à cuisinePanel
                Point mousePosInZone = cuisinePanel.PointToClient(Control.MousePosition);
                Point newLocation = new Point(mousePosInZone.X - offset.X, mousePosInZone.Y - offset.Y);
                draggedButton.Location = newLocation;
                //meubles[(string)draggedButton.Tag].Location = newLocation;
                meublesPositions[draggedButton.Name] = newLocation;

                // Vérifier si le bouton est entièrement dans cuisinePanel
                checkMeubleDansCuisine(draggedButton);
            }
        }

        // Méthode pour vérifier si un meuble est dans la cuisine
        private void checkMeubleDansCuisine (Control meuble)
        {
            if (objetEstDansContainer(zoneCuisine, meuble))
            {
                meuble.ForeColor = Color.Black; // le contour devient noir
            }
            else
            {
                meuble.ForeColor = Color.Red; // le contour devient rouge
            }

        }

        // Méthode pour vérifier si un objet est dans un autre (comme un meuble dans une zone)
        private bool objetEstDansContainer(Control container, Control objet)
        {
            if (objet == null) return false;
            Rectangle containerRect = container.RectangleToScreen(container.ClientRectangle);
            Rectangle controlRect = objet.RectangleToScreen(objet.ClientRectangle);
            return containerRect.Contains(controlRect);
        }

        // Méthode pour la rotation
        private void RotateButton(Button btn)
        {
            Image originalImage = btn.Image;
            Bitmap rotatedImage = new Bitmap(originalImage.Height, originalImage.Width);
            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                g.TranslateTransform(rotatedImage.Width / 2, rotatedImage.Height / 2);
                g.RotateTransform(90);
                g.TranslateTransform(-originalImage.Width / 2, -originalImage.Height / 2);
                g.DrawImage(originalImage, Point.Empty);
            }

            btn.Size = new Size(btn.Height, btn.Width);
            btn.Image = rotatedImage;
        }

        // Méthode pour dupliquer le meuble
        private void DuplicateButton(Button originalBtn)
        {
            if (originalBtn.Tag is string meubleType)
            {
                Button newButton = new Button
                {
                    Name = "btn_" + meubleType + "_" + (buttonCounter++),
                    Text = originalBtn.Text,
                    Size = originalBtn.Size,
                    Location = new Point(originalBtn.Right + 10, originalBtn.Top),
                    FlatStyle = originalBtn.FlatStyle,
                    Tag = originalBtn.Tag,
                    Image = (Image)originalBtn.Image.Clone(),
                    BackgroundImageLayout = originalBtn.BackgroundImageLayout,
                    BackColor = originalBtn.BackColor
                };

                SetupButton(newButton);
                this.cuisinePanel.Controls.Add(newButton);
                newButton.BringToFront();
            }
        }

        // Méthode pour supprimer le meuble
        private void DeleteButton(Button btn)
        {
            DialogResult result = MessageBox.Show("Supprimer ce meuble ?", "Confirmation",
                MessageBoxButtons.YesNo,MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                this.Controls.Remove(btn);
                btn.Dispose();
            }
        }


        // Bouton pour quitter l'application
        private void quitter_Click(object sender, EventArgs e)
        {
            // Quitter l'application
            Application.Exit();
        }

        // Commentaire .... ?
        private void button1_Click(object sender, EventArgs e)
        {
            meublesPanel.Visible = !meublesPanel.Visible;
        }

        // Méthodes vides nécessaires
        private void button2_Click(object sender, EventArgs e) { }
        private void panel1_Paint(object sender, PaintEventArgs e) { }

        private void backButton_Click(object sender, EventArgs e)
        {
            CreateMeubleCategories();
        }

        private void backButton_Click_1(object sender, EventArgs e)
        {
            CreateMeubleCategories();
        }

        // Pour gerer les dimensions de la zone de cuisine avec les texbox
        private void textBoxDimension_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) 
            {
                if (int.TryParse(width_texbox.Text, out int newWidth) && int.TryParse(height_texbox.Text, out int newHeight))
                {
                    Size newSize = new Size(newWidth, newHeight);
                    this.zoneCuisine.Size = newSize;
                    // Mettre à jour la position du gripItem
                    gripItem.Location = new Point(zoneCuisine.Width - gripItem.Width, zoneCuisine.Height - gripItem.Height);
                    zoneCuisineBaseSize = zoneCuisine.Size;
                    menuBar.Focus();
                }
            }
        }

        private void save_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Fichier Cuisine (*.ccs)|*.ccs";
            saveFileDialog.Title = "Sauvegarder la cuisine";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    CuisineState etatCuisine = new CuisineState
                    {
                        CuisineSize = zoneCuisine.Size
                    };

                    // Récupérer tous les meubles dans la zone cuisine
                    foreach (Control ctrl in cuisinePanel.Controls)
                    {
                        if (ctrl is Button btn && btn.Name.StartsWith("btn_"))
                        {
                            etatCuisine.Meubles.Add(new MeubleState
                            {
                                Type = (string)btn.Tag,
                                Nom = btn.Name,
                                Taille = btn.Size,
                                Position = btn.Location
                                // Note: Vous devrez ajouter la rotation si vous l'implémentez
                            });
                        }
                    }

                    // Sérialiser et sauvegarder
                    using (FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        formatter.Serialize(fs, etatCuisine);
                    }

                    MessageBox.Show("Cuisine sauvegardée avec succès!", "Sauvegarde", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors de la sauvegarde: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void load_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Fichier Cuisine (*.ccs)|*.ccs";
            openFileDialog.Title = "Charger une cuisine";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    CuisineState etatCuisine;

                    // Désérialiser le fichier
                    using (FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        etatCuisine = (CuisineState)formatter.Deserialize(fs);
                    }

                    // Appliquer l'état chargé
                    zoneCuisine.Size = etatCuisine.CuisineSize;
                    zoneCuisineBaseSize = zoneCuisine.Size;
                    gripItem.Location = new Point(zoneCuisine.Width - gripItem.Width, zoneCuisine.Height - gripItem.Height);

                    // Supprimer les meubles existants
                    List<Control> controlsASupprimer = new List<Control>();
                    foreach (Control ctrl in cuisinePanel.Controls)
                    {
                        if (ctrl is Button btn && btn.Name.StartsWith("btn_"))
                        {
                            controlsASupprimer.Add(ctrl);
                        }
                    }
                    foreach (Control ctrl in controlsASupprimer)
                    {
                        cuisinePanel.Controls.Remove(ctrl);
                        ctrl.Dispose();
                    }

                    // Recréer les meubles sauvegardés
                    foreach (var meubleState in etatCuisine.Meubles)
                    {
                        if (meubles.TryGetValue(meubleState.Type, out var properties) &&
                            meubleImages.TryGetValue(meubleState.Type, out var image))
                        {
                            Button newButton = new Button
                            {
                                Name = meubleState.Nom,
                                Text = "",
                                Size = meubleState.Taille,
                                Location = meubleState.Position,
                                FlatStyle = FlatStyle.Flat,
                                Tag = meubleState.Type,
                                Image = ResizeImage(image, meubleState.Taille),
                                BackgroundImageLayout = ImageLayout.Stretch,
                                BackColor = Color.Transparent
                            };

                            SetupButton(newButton);
                            cuisinePanel.Controls.Add(newButton);
                            newButton.BringToFront();

                            // Mettre à jour le dictionnaire de positions
                            meublesPositions[newButton.Name] = meubleState.Position;

                            // Si vous avez implémenté la rotation, l'appliquer ici
                        }
                    }

                    MessageBox.Show("Cuisine chargée avec succès!", "Chargement", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erreur lors du chargement: {ex.Message}", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void textBoxDimension_Leave(object sender, EventArgs e)
        {
            if (int.TryParse(width_texbox.Text, out int newWidth) && int.TryParse(height_texbox.Text, out int newHeight))
            {
                Size newSize = new Size(newWidth,newHeight);
                this.zoneCuisine.Size = newSize;
                // Mettre à jour la position du gripItem
                gripItem.Location = new Point(zoneCuisine.Width - gripItem.Width, zoneCuisine.Height - gripItem.Height);
                zoneCuisineBaseSize = zoneCuisine.Size;
                menuBar.Focus();
            }
        }

        // On vérifie si la saisie est un entier avant d'appliquer
        private void width_texbox_Textchanged(object sender, EventArgs e)
        {
            if (!int.TryParse(width_texbox.Text, out _))
            {
                width_texbox.Text = string.Concat(width_texbox.Text.Where(char.IsDigit));
                width_texbox.SelectionStart = width_texbox.TextLength;
            }
        }
        private void height_texbox_Textchanged(object sender, EventArgs e)
        {
            if (!int.TryParse(width_texbox.Text, out _))
            {
               height_texbox.Text = string.Concat(width_texbox.Text.Where(char.IsDigit));
               height_texbox.SelectionStart = height_texbox.TextLength;
            }
        }
    }
    // Classe pour représenter un meuble
    public class Meuble
    {
        public String Type { get; set; }
        public string Text { get; set; }
        public Size Size { get; set; }
        public Point Location { get; set; }
        public string ImageName { get; set; }
        public Meuble (String type_m, string text, Size size, Point location, string imageName) //  Constructeur
        {
            this.Type = type_m; // Type de meuble
            Text = text; // Texte du meuble
            Size = size; // Taille du meuble
            Location = location; // Position du meuble
            ImageName = imageName; // Nom de l'image du meuble
        }
    };

    // Classe pour representer un type de meuble
    public class CategorieMeuble
    {
        public String Type { get; set; }
        public Size Taille { get; set; }
        public String ImageName { get; set; }
        public CategorieMeuble(String t,Size ta, String im){
            Type = t;
            Taille = ta;
            ImageName = im;
        }
    }
}