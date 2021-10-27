using System.Collections;
using System.Windows.Forms;

namespace FFRK_LabMem.Config.UI
{
    class Sorters
    {

        public class PaintingSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                ListViewItem listviewX, listviewY;

                // Cast the objects to be compared to ListViewItem objects
                listviewX = (ListViewItem)x;
                listviewY = (ListViewItem)y;

                // Get values
                int priorityX = int.Parse(listviewX.Text);
                int priorityY = int.Parse(listviewY.Text);

                return priorityX.CompareTo(priorityY);

            }
        }

        public class TreasureSorter : IComparer
        {
            public int Compare(object x, object y)
            {
                ListViewItem listviewX, listviewY;

                // Cast the objects to be compared to ListViewItem objects
                listviewX = (ListViewItem)x;
                listviewY = (ListViewItem)y;

                // Get values
                int priorityX = int.Parse(listviewX.Text);
                int priorityY = int.Parse(listviewY.Text);
                int keysX = int.Parse(listviewX.SubItems[1].Text);
                int keysY = int.Parse(listviewY.SubItems[1].Text);

                if (priorityX == 0) priorityX = 1000;
                if (priorityY == 0) priorityY = 1000;

                var cmp1 = priorityX.CompareTo(priorityY);
                if (cmp1 == 0)
                {
                    return keysX.CompareTo(keysY);
                }
                else
                {
                    return cmp1;
                }

            }
        }

    }
}
