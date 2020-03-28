using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aspose.Cells;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;

namespace JiraTask.Business
{
    public static class ExcelHelper
    {
        public static void SaveQuestions(List<UserQuestionMode> questions, string exportingDateTime)
        {
            if (GetExcelTemplatePath(out var excelPath))
            {
                try
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    string strDesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    sfd.InitialDirectory = strDesktopPath; //设置初始路径
                    sfd.Filter = "Excel文件(*.xlsx)|*.xlsx|Excel文件(*.xls)|*.xls|Csv文件(*.csv)|*.csv|所有文件(*.*)|*.*"; //设置“另存为文件类型”或“文件类型”框中出现的选择内容
                    sfd.FilterIndex = 1; //设置默认显示文件类型为Csv文件(*.csv)|*.csv
                    sfd.FileName = $"{Path.GetFileNameWithoutExtension(excelPath)}_{exportingDateTime}";//设置默认文件名
                    sfd.DefaultExt = "xlsx";//设置默认格式（可以不设）
                    sfd.AddExtension = true;//设置自动在文件名中添加扩展名
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        Workbook workbook = new Workbook(excelPath);
                        var workbookWorksheet = workbook.Worksheets[0];

                        Cells cells = workbookWorksheet.Cells;
                        cells[0, 0].Value = exportingDateTime;
                        var startRow = cells.MaxDataRow + 1;

                        foreach (var question in questions)
                        {
                            SaveUserQuestion(question, cells, startRow++);
                        }
                        workbookWorksheet.AutoFitRows();
                        workbook.Save(sfd.FileName, SaveFormat.Auto);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show($"{e.Message}\r\n导出到Excel异常，请确认是Excel状态是否正常！");
                }
            }
        }

        private static void SaveUserQuestion(UserQuestionMode question, Cells cells, int rowIndex)
        {
            int columnIndex = 0;
            var questionInfoFromDescription = JiraHelper.AnalysisDescriptionData(question.Description);
            var questionInfoFromComment = JiraHelper.AnalysisCommentData(question.Comment);

            //级别
            cells[rowIndex, columnIndex++].Value = JiraHelper.GetPorityName(question.Priority);
            //用户联系方式
            cells[rowIndex, columnIndex++].Value = questionInfoFromDescription.ContactInformation;
            //反馈时间
            cells[rowIndex, columnIndex++].Value = question.CreateTime;
            //问题
            cells[rowIndex, columnIndex++].Value = question.Summary;
            //问题描述
            cells[rowIndex, columnIndex++].Value = questionInfoFromDescription.Description;
            //原因
            cells[rowIndex, columnIndex++].Value = questionInfoFromComment.Reason;
            //解决方案
            cells[rowIndex, columnIndex++].Value = questionInfoFromComment.Solution;
            //影响范围
            cells[rowIndex, columnIndex++].Value = questionInfoFromComment.ScopeInfluence;
            //状态
            cells[rowIndex, columnIndex++].Value = question.Status;
            //第一负责人 固定为洪哥
            cells[rowIndex, columnIndex++].Value = "叶洪";
        }

        private static bool GetExcelTemplatePath(out string excelPath)
        {
            var baseDirectory = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            excelPath = Path.Combine(baseDirectory, @"Resources\用户反馈.xlsx");
            if (!File.Exists(excelPath))
            {
                MessageBox.Show($"文件“{excelPath}”未找到！");
                return false;
            }

            return true;
        }
    }
}
