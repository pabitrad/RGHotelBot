using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BOTManager.Entities.Utility
{
    public class Indicator
    {
        #region Code to Check the Merchant Rate
        #endregion
        /// <summary>
        /// This is Code is added after discussion with Awneesh sir
        /// </summary>
        /// <param name="ReponseBlock"></param>
        /// <returns></returns> 
        public bool IsMerChantRatePromotion(string ReponseBlock, List<string> lstKeyword)
        {
            //string KeyWord = Config.Keyword;
            bool flag = false;
            if (lstKeyword != null && lstKeyword.Count != 0)
            {
                foreach (string Key in lstKeyword)
                {
                    if (ReponseBlock.ToLower().IndexOf(Key.ToLower()) >= 0)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            return flag;
        }
        #region Getting List
        ///<Summary>
        /// This Function is used to generate the list
        /// Corrosponding to the Data. Whether it is From 
        /// Config or Database. IF it is coming from Database
        /// then We will copy this function in the Database related file 
        /// and from there we will retrun a list.       
        /// </Summary>
        public List<string> GetListOfKeyword(string KeyWord)
        {

            List<string> listKeyword = new List<string>();
            if (!string.IsNullOrEmpty(KeyWord))
            {
                string[] splitter = KeyWord.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                //splitter = KeyWord.Split(',', StringSplitOptions.RemoveEmptyEntries);
                foreach (string Key in splitter)
                {
                    listKeyword.Add(Key);
                }
            }
            return listKeyword;
        }
        #endregion
    }
}
