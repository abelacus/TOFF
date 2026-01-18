using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terminal.Gui.Views;

namespace TOFF.Models
{
    /// <summary>
    /// Defines a tree node with dynamic children. tree.RebuildTree() must be called after data has changed to refresh the 
    /// [Children] takes a function that returns an array of strings
    /// </summary>
    internal class DataSourceTreeNode : TreeNode
    {
        public override string Text { get; set; }
        public Func<string[]>? DataDataSource { get; set; }

        public override IList<ITreeNode> Children => Array.ConvertAll((DataDataSource?.Invoke() ?? []), item => new TreeNode { Text = item });
    }
}
