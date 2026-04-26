using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Word = Microsoft.Office.Interop.Word; // підключення простору імен 

namespace laba12
{ 
    public partial class Form1 : Form
    {
      
        private Word.Application wordApp = null;
        private Word.Document doc = null;

        public Form1()
        {
            InitializeComponent();

            // красиві назви у списку шаблонів
            cmbTemplates.Items.Add("Заява на звільнення");
            cmbTemplates.Items.Add("Лист-подяка за рекомендацію");
            cmbTemplates.Items.Add("Запрошення до партнерства");

            if (cmbTemplates.Items.Count > 0)
                cmbTemplates.SelectedIndex = 0;
        }

        // кнопка створення та перегляду документа
        private void btnPreview_Click(object sender, EventArgs e)
        {
            if (cmbTemplates.SelectedIndex == -1)
            {
                MessageBox.Show("вибери шаблон зі списку");
                return;
            }

            try
            {
                wordApp = new Word.Application();

                string selectedTemplate = cmbTemplates.SelectedItem.ToString();
                string realFilePath = "";

                // шлях до шаблонів
                switch (selectedTemplate)
                {
                    case "Заява на звільнення":
                        realFilePath = @"D:\універ\2курс\Об'єктно-орієнтоване програмування\laba 12\Templates\ResignationLetter.dotx";
                        break;
                    case "Лист-подяка за рекомендацію":
                        realFilePath = @"D:\універ\2курс\Об'єктно-орієнтоване програмування\laba 12\Templates\RecommendationLetter.dotx";
                        break;
                    case "Запрошення до партнерства":
                        realFilePath = @"D:\універ\2курс\Об'єктно-орієнтоване програмування\laba 12\Templates\PartnershipLetter.dotx";
                        break;
                }

                Object templatePathObj = realFilePath;
                Object missingObj = System.Reflection.Missing.Value;

                // створення документу
                doc = wordApp.Documents.Add(ref templatePathObj, ref missingObj, ref missingObj, ref missingObj);
                doc.Activate();

                // заповнення закладок
                foreach (Word.FormField f in doc.FormFields)
                {
                    switch (f.Name)
                    {
                        case "ReceiverName":
                            f.Range.Text = txtReceiverName.Text;
                            break;
                        case "CompanyName":
                            f.Range.Text = txtCompanyName.Text;
                            break;
                        case "Position":
                            f.Range.Text = txtPosition.Text;
                            break;
                        case "SenderName":
                            f.Range.Text = txtSenderName.Text;
                            break;
                    }
                }

                wordApp.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка: " + ex.Message);
                CloseWordProcesses();
            }
        }

        //кнопка збереження документа
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (doc != null)
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Документ Word (*.docx)|*.docx";
                sfd.Title = "Зберегти документ";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    object filename = sfd.FileName;
                    object missingObj = System.Reflection.Missing.Value;

                    doc.SaveAs2(ref filename, ref missingObj, ref missingObj, ref missingObj, ref missingObj,
                                ref missingObj, ref missingObj, ref missingObj, ref missingObj, ref missingObj,
                                ref missingObj, ref missingObj, ref missingObj, ref missingObj, ref missingObj, ref missingObj);

                    MessageBox.Show("Документ успішно збережено!");
                }
            }
            else
            {
                MessageBox.Show("спочатку створи документ за допомогою кнопки Створити та переглянути");
            }
        }

        // кнопка пошуку та заміни тексту
        private void btnSearchReplace_Click(object sender, EventArgs e)
        {
            if (doc == null)
            {
                MessageBox.Show("спочатку створи документ за допомогою кнопки Створити");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSearch.Text) || string.IsNullOrWhiteSpace(txtReplace.Text))
            {
                MessageBox.Show("напиши текст для пошуку та текст для заміни");
                return;
            }

            try
            {
                if (doc.ProtectionType != Word.WdProtectionType.wdNoProtection)
                {
                    object noPassword = System.Reflection.Missing.Value;
                    doc.Unprotect(ref noPassword);
                }

                object findText = txtSearch.Text;
                object replaceText = txtReplace.Text;
                object replaceAll = Word.WdReplace.wdReplaceAll;

                object wrap = Word.WdFindWrap.wdFindContinue;
                object matchCase = false;
                object missingObj = System.Reflection.Missing.Value;

                Word.Find findObject = doc.Content.Find;
                findObject.ClearFormatting();
                findObject.Replacement.ClearFormatting();

                findObject.Execute(
                    ref findText, ref matchCase, ref missingObj, ref missingObj, ref missingObj,
                    ref missingObj, ref missingObj, ref wrap, ref missingObj, ref replaceText,
                    ref replaceAll, ref missingObj, ref missingObj, ref missingObj, ref missingObj);

                MessageBox.Show("заміну успішно виконано");
            }
            catch (Exception ex)
            {
                MessageBox.Show("появилась помилка під час заміни: " + ex.Message);
            }
        }

        // закриття процесів Word при закритті форми
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            CloseWordProcesses();
        }

        // метод для закриття процесів Word
        private void CloseWordProcesses()
        {
            if (doc != null)
            {
                object saveChanges = Word.WdSaveOptions.wdDoNotSaveChanges;
                object missingObj = System.Reflection.Missing.Value;
                doc.Close(ref saveChanges, ref missingObj, ref missingObj);
                doc = null;
            }
            if (wordApp != null)
            {
                object missingObj = System.Reflection.Missing.Value;
                wordApp.Quit(ref missingObj, ref missingObj, ref missingObj);
                wordApp = null;
            }
        }
    }
}

