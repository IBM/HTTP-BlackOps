using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Testing
{

    public enum AttackTargetStatus
    {
        Enabled,
        Disabled
    }

    /// <summary>
    /// 
    /// </summary>
    public class AttackTarget
    {
        private string _name;
        /// <summary>
        /// The name of the target
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        private AttackTargetStatus _status = AttackTargetStatus.Enabled;
        /// <summary>
        /// Whether the target is enable or disabled
        /// </summary>
        public AttackTargetStatus Status
        {
            get { return _status; }
            set { _status = value; }
        }

        private string _requestPattern = String.Empty;
        /// <summary>
        /// The pattern that a test request is identified with
        /// </summary>
        public string RequestPattern
        {
            get { return _requestPattern; }
            set { _requestPattern = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="statusString"></param>
        /// <param name="requestPattern"></param>
        public AttackTarget(string name, string statusString, string requestPattern)
        {
            _name = name;
            AttackTargetStatus statusVal;
            Enum.TryParse<AttackTargetStatus>(statusString, out statusVal);
            _status = statusVal;
            _requestPattern = requestPattern;

        }

        /// <summary>
        /// Returns the contents of the definition separated by \t
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0}\t{1}\t{2}", _name, _status, _requestPattern);
        }

    }
}
