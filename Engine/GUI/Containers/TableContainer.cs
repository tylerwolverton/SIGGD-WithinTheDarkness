using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Engine.GUI
{
    public class TableContainer : ListContainer
    {
        public ListContainer[] rows;
        public Vector2 limits;                  // X is left most column Y is right most column
        public GUIItem[,] allChildren;
        public int cols { get; set; }
        public TableContainer(GUIComponent theInterface, int row, int col) : this(theInterface, row, col, col) { }
        public TableContainer(GUIComponent theInterface,  int row, int col, int limit)
            : base(theInterface)
        {
            this.orientation = Orientation.VERTICAL;
            rows = new ListContainer[row];
            allChildren = new GUIItem[row, col];
            limits = new Vector2(0, limit);     //This assumes that limit is in bounds
            for (int i = 0; i < row; i++)
            {
                rows[i] = new ListContainer(theInterface);
                rows[i].orientation = Orientation.HORIZONTAL;
                this.children.Add(rows[i]);
            }
            cols = col;
        }
        public override void Add(GUIItem it)
         {
           for(int i = 0; i< rows.Length; i++)
             {
                 for (int j = 0; j < cols; j++) 
                 {
                     if (allChildren[i,j]==null)
                     {
                         allChildren[i,j]=it;
                         return;
                     }  
                 }
             }
         }
        public void right()
        {
            if (limits.Y < cols - 1)
            {
                limits.Y = limits.Y + 1;
                limits.X = limits.X + 1;
                pack();
            }            
        }
        public void left()
        {
            if (limits.X > 0)
            {
                limits.Y = limits.Y - 1;
                limits.X = limits.X - 1;
                pack();
            }
        }
         public void Replace(GUIItem it,int r, int c)
         {
             if (c < cols && r < rows.Length && allChildren[r,c]!=null) rows[r].children[c] = it; return;
         }
         public override void performLayout()
         {
             for (int i = 0; i < rows.Length; i++)
             {
                 rows[i].children = new List<GUIItem>();
                  for (int j = 0; j < (int)(limits.Y-limits.X); j++)
                 {
                     rows[i].children.Add(allChildren[i,j+(int)limits.X]);
                 }
             }
             base.performLayout();
         }
    }
}
