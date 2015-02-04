using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CollaboratR.Models
{
    /// <summary>
    /// General object that contains a message and success value for when a model interacts with the database,
    /// Cannot be instantiated as it is abstract...
    /// </summary>
    public abstract class BusinessObject
    {
        protected String message;
        protected Boolean success;

        public String Message
        {
            get
            {
                return message;
            }
            set
            {
                message = value;
            }
        }
        public Boolean Success
        {
            get
            {
                return success;
            }
            set
            {
                success = value;
            }
        }

 
    }
}