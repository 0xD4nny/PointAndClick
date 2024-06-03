using System.Drawing.Drawing2D;
using System.Xml.Linq;


namespace PointAndClick0xDanny
{
    public partial class LeaderBoardForm : Form
    {
        private readonly RoundButton button1, button2;
        private readonly GraphicsPath _leaderboardGuiEdgePath;
        private readonly Pen _myPen = new Pen(Color.Black, 2);
        public LeaderBoardForm(List<Entry> entries)
        {
            InitializeComponent();
            InitializeListView();
            PopulateListView(entries);

            button1 = new RoundButton
            {
                Text = "Play Again",
                Font = new Font("Comic San MS", 9, FontStyle.Regular),
                ForeColor = Color.Black,
                EdgeColor = Color.DarkRed,
                ArcLength = 40,
                EdgeSize = 2,
                DialogResult = DialogResult.OK
            };
            button2 = new RoundButton
            {
                Text = "Close Game",
                Font = new Font("Comic San MS", 9, FontStyle.Regular),
                ForeColor = Color.Black,
                EdgeColor = Color.DarkRed,
                ArcLength = 40,
                EdgeSize = 2,
                DialogResult = DialogResult.Cancel,
                
            };

            button1.SetBounds(120, 380, 160, 40);
            button2.SetBounds(420, 380, 160, 40);

            AcceptButton = button1;
            CancelButton = button2;
            Controls.Add(button1);
            Controls.Add(button2);

            Region = new Region(MainForm.GetRectWithRoundCornor(0, 0, Width, Height, 52));
            _leaderboardGuiEdgePath = MainForm.GetRectWithRoundCornor(1, 1, Width - 1, Height - 1, 50);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            e.Graphics.DrawPath(_myPen, _leaderboardGuiEdgePath);
        }

        private void InitializeListView()
        {
            listView1.View = View.Details;
            listView1.Columns.Add("Player Name");
            listView1.Columns.Add("Score", -2, HorizontalAlignment.Right);
            listView1.Columns.Add("Miss Hits", -2, HorizontalAlignment.Right);
            listView1.Columns.Add("Accuracy", -2, HorizontalAlignment.Right);
            listView1.Columns.Add("Level", -2, HorizontalAlignment.Right);
            listView1.Columns.Add("Game Time", -2, HorizontalAlignment.Right);
            listView1.Columns.Add("Entry Date", -2, HorizontalAlignment.Right);
        }

        private void PopulateListView(List<Entry> entries)
        {
            listView1.BeginUpdate();
            foreach (var entry in entries)
            {
                ListViewItem item = new ListViewItem(entry.PlayerName);
                item.SubItems.Add(entry.Score.ToString());
                item.SubItems.Add(entry.MissHits.ToString());
                item.SubItems.Add(entry.Accuracy.ToString());
                item.SubItems.Add(entry.Level.ToString());
                item.SubItems.Add(entry.GameTime.ToString());
                item.SubItems.Add(entry.EntryCreateTime.ToString("dd-MM-yyyy"));
                listView1.Items.Add(item);
            }
            listView1.EndUpdate();
        }


        private const string _path = "leaderboard.xml";

        public static void AddEntry(Entry newEntry)
        {
            if (newEntry is null)
                throw new Exception("the Leaderboard entry can't be null.");

            using (FileStream stream = File.Open(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                XDocument doc;
                if (stream.Length == 0)
                    doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), new XElement("Entrys"));
                else
                    doc = XDocument.Load(stream);

                XElement? root = doc.Element("Entrys");
                root?.Add(new XElement("Entry",
                    new XElement("Score", newEntry.Score),
                    new XElement("MissHits", newEntry.MissHits),
                    new XElement("Accuracy", newEntry.Accuracy),
                    new XElement("Level", newEntry.Level),
                    new XElement("GameTime", newEntry.GameTime),
                    new XElement("PlayerName", newEntry.PlayerName),
                    new XElement("EntryCreateTime", newEntry.EntryCreateTime)));

                stream.SetLength(0);
                stream.Position = 0;
                doc.Save(stream);
            }
        }

        public static List<Entry> GetLeaderboardData()
        {
            if (!File.Exists(_path))
                throw new Exception("Missing leaderboard.xml file.");

            using (FileStream stream = File.Open(_path, FileMode.Open, FileAccess.Read))
            {
                XDocument XML = XDocument.Load(stream);
                if (XML is null)
                    throw new Exception("Missing leaderboard.xml file.");

                List<Entry> entries = XML.Descendants("Entry")
                    .Select(entry => new Entry(
                    int.Parse(entry.Element("Score").Value),
                    int.Parse(entry.Element("MissHits").Value),
                    int.Parse(entry.Element("Accuracy").Value),
                    int.Parse(entry.Element("Level").Value),
                    int.Parse(entry.Element("GameTime").Value),
                    entry.Element("PlayerName").Value,
                    DateTime.Parse(entry.Element("EntryCreateTime").Value)))
                    .OrderByDescending(e => e.Score).ToList();

                return entries;
            }
        }

    }
}
