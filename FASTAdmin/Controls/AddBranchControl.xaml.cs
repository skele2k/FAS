﻿using FASLib.Models;
using FASLib.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FASLib.Helpers;

namespace FASTAdmin.Controls
{
    /// <summary>
    /// Interaction logic for AddBranchControl.xaml
    /// </summary>
    public partial class AddBranchControl : UserControl
    {
        public AddBranchControl()
        {
            InitializeComponent();
        }
        private async void AddBranchToDatabase()
        {
            var form = Validate();
            
            if (form.isValid == false)
            {
                MessageBox.Show("Форм бөглөхөд алдаа гарлаа. Та формоо зөв бөглөнө үү?");
                return;
            }

            var t = await ApiProcessor.SaveBranch(form.model);
            if (t == "success")
            {
                MessageBox.Show("Амжилттай нэмэгдлээ.");
            }
            else
            {
                MessageBox.Show("Алдаа гарлаа. Та дахин оролдоно уу?");
            }
        }

        private void ResetForm()
        {
            BranchNameTextBox.Text = "";
        }

        private (bool isValid, BranchModel model) Validate()
        {
            bool isValid = true;
            BranchModel model = new BranchModel();
            try
            {
                model.name = BranchNameTextBox.Text;
            }
            catch
            {
                isValid = false;
            }
            return (isValid, model);
        }

        private void addBranchButton_Click(object sender, RoutedEventArgs e)
        {
            if (BranchNameTextBox.Text == "")
            {
                MessageBox.Show("Та шинэ тасгийн нэрээ өгнө үү?");
                return;
            }
            AddBranchToDatabase();
            ResetForm();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            Back.Visibility = Visibility.Collapsed;
            addBranchTextBlock.Visibility = Visibility.Collapsed;
            addBranchNameTextBox.Visibility = Visibility.Collapsed;
            addBranchButton.Visibility = Visibility.Collapsed;
            BackControl.Content = new AdminControl();
        }
    }
}