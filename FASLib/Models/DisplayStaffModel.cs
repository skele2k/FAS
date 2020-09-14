using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FASLib.Models
{
    public class DisplayStaffModel
    {
        private string _firstName;
        private string _lastName;
        private string _branchName;
        private int _id;
        public int id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }
        public string firstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
            }
        }

        public string lastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
            }
        }
        public string branchName
        {
            get
            {
                return _branchName;
            }
            set
            {
                _branchName = value;
            }
        }
    }
}
