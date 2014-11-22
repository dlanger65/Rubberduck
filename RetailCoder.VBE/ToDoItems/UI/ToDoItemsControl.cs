﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Vbe.Interop;
using System.Collections.Generic;
using Rubberduck.Extensions;

namespace Rubberduck.ToDoItems
{
    public partial class ToDoItemsControl : UserControl
    {
        private VBE vbe;
        private ToDoList todoList;

        public ToDoItemsControl(VBE vbe, List<Config.ToDoMarker> markers)
        {
            this.vbe = vbe;
            this.todoList = new ToDoList(vbe, markers);
            InitializeComponent();
            InitializeGrid();
        }

        private void InitializeGrid()
        {
            todoItemsGridView.DataSource = this.todoList;
            var descriptionColumn = todoItemsGridView.Columns["Description"];
            if (descriptionColumn != null)
            {
                descriptionColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            }

            todoItemsGridView.CellDoubleClick += taskListGridView_CellDoubleClick;
            refreshButton.Click += refreshButton_Click;

        }

        void taskListGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ToDoItem task = this.todoList.ElementAt(e.RowIndex);
            VBComponent component = vbe.ActiveVBProject.VBComponents.Item(task.Module);

            component.CodeModule.CodePane.Show();
            component.CodeModule.CodePane.SetSelection(task.LineNumber);
        }

        private void RefreshGridView()
        {
            this.todoList.Refresh();
            todoItemsGridView.Refresh();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            RefreshGridView();
        }
    }
}
