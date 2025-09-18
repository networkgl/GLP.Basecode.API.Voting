using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GLP.Basecode.API.Helper
{
    public class ExcelHelper
    {
        //public List<ImportArrears> ReadExcelFile(string fileLocation, string fileExtension)
        //{
        //    using (var stream = File.Open(fileLocation, FileMode.Open, FileAccess.Read))
        //    {
        //        var items = new List<ImportArrears>();
        //        IExcelDataReader excelReader;
        //        if (fileExtension.ToLower() == ".xls")
        //        {
        //            //1. Reading from a binary Excel file ('97-2003 format; *.xls)
        //            excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
        //        }
        //        else
        //        {
        //            // Reading from a OpenXml Excel file (2007 format; *.xlsx)
        //            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        //        }

        //        int rowNumber = 0;
        //        bool extract = false;
        //        while (excelReader.Read())
        //        {
        //            rowNumber++;
        //            if (rowNumber == 11)
        //                extract = true;
        //            if (extract)
        //            {
        //                try
        //                {
        //                    // 1 - get required fields
        //                    string name = excelReader.GetString(0);
        //                    string address = excelReader.GetString(0);

        //                    var i = new ImportArrears()
        //                    {
        //                        Name = excelReader.GetString(0),
        //                        Address = excelReader.GetString(1),
        //                        DebitsuccessID = excelReader.GetString(3),
        //                        BusinessID = excelReader.GetString(4),
        //                        ReversalReason = excelReader.GetString(7),
        //                        Overdue = excelReader.GetString(10)
        //                    };
        //                    try
        //                    {
        //                        i.ContactDetail = excelReader.GetString(2);
        //                    }
        //                    catch { }
        //                    try
        //                    {
        //                        //i.Reversal = excelReader.GetDateTime(8);
        //                        i.Reversal = excelReader.IsDBNull(8) ? (DateTime?)null : excelReader.GetDateTime(8);
        //                    }
        //                    catch { }
        //                    //var installment = excelReader.GetString(6);
        //                    //var remainingOS = excelReader.GetString(11);
        //                    //if (!string.IsNullOrEmpty(installment))
        //                    //{
        //                    //    i.Installment = excelReader.GetDouble(6);
        //                    //}
        //                    //if (!string.IsNullOrEmpty(remainingOS))
        //                    //{
        //                    //    i.RemainingOS = Convert.ToDecimal(remainingOS);
        //                    //}
        //                    i.Installment = excelReader.GetDouble(6);
        //                    i.RemainingOS = excelReader.GetDouble(11);
        //                    i.StartDate = excelReader.GetDateTime(5);
        //                    i.ODStatus = excelReader.GetDouble(12);
        //                    items.Add(i);
        //                }
        //                catch (Exception e)
        //                {
        //                    var me = e.Message;
        //                }
        //            }

        //        }
        //        return items;
        //    }
        //}
        //protected int TryGetInt(string value)
        //{
        //    if (!string.IsNullOrEmpty(value))
        //    {
        //        return Convert.ToInt16(value);
        //    }
        //    return 0;
        //}

        //public List<Member> ReadExcelFileForCancelledMembers(string fileLocation, string fileExtension)
        //{
        //    using (var stream = File.Open(fileLocation, FileMode.Open, FileAccess.Read))
        //    {
        //        var items = new List<Member>();
        //        IExcelDataReader excelReader;
        //        if (fileExtension.ToLower() == ".xls")
        //        {
        //            //1. Reading from a binary Excel file ('97-2003 format; *.xls)
        //            excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
        //        }
        //        else
        //        {
        //            // Reading from a OpenXml Excel file (2007 format; *.xlsx)
        //            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        //        }


        //        bool isFirstRow = true;
        //        while (excelReader.Read())
        //        {
        //            if (isFirstRow)
        //            {
        //                isFirstRow = false;
        //                continue;
        //            }

        //            if (excelReader.GetString(0) == null)
        //            {
        //                break;
        //            }

        //            var mFullName = excelReader.GetString(0).ToString()?.Split(' ') ?? Array.Empty<string>();
        //            var lastName = mFullName.LastOrDefault() ?? "";
        //            var firstName = string.Empty;
        //            for (int i = 0; i < mFullName.Count() - 1; i++)
        //            {
        //                if (i != 0)
        //                    firstName += " " + mFullName[i];
        //                else firstName = mFullName[i];
        //            }

        //            var memberNum = excelReader.GetString(1).ToString() ?? "";
        //            var membershipType = excelReader.GetString(6).ToString() ?? "";

        //            var newCancelledMember = new Member
        //            {
        //                FirstName = firstName,
        //                LastName = lastName,
        //                MemberNumber = memberNum,
        //                MembershipType = membershipType,
        //                Status = Model.Enum.MemberStatus.Cancelled
        //            };

        //            items.Add(newCancelledMember);
        //        }
        //        return items;
        //    }
        //}

    }
}
