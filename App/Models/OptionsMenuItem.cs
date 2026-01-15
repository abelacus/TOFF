using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui.Views;

namespace TOFF.Models
{
    internal class OptionsMenuItem : TreeNode
    {
        public override string Text { get; set; }
        public string[]? currentValues { get; set; }

        public override IList<ITreeNode> Children => Array.ConvertAll(currentValues ?? [], item => new TreeNode { Text = item });
    }
}
