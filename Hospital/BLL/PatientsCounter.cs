using Hospital.DataAccess.Models;
using Hospital.Models;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hospital.BLL
{
    public class PatientsCounter
    {
        /// <summary>
        /// calculates number of doctors' patients and set it to DoctorItemViewModel's property
        /// </summary>
        /// <param name="doctors"></param>
        /// <param name="treatments"></param>
        /// <returns></returns>
        public static List<DoctorItemViewModel> CalculatePatientsNumber(
            IEnumerable<Doctor> doctors,IEnumerable<Treatment> treatments)
        {
            var logger = LogManager.GetCurrentClassLogger();
            logger.Info("Counting patients number");

            //maps doctor ids to set of patients
            Dictionary<string, HashSet<string>> doctorPatients = new Dictionary<string, HashSet<string>>();

            //initialize doctors patients
            foreach(var d in doctors)
            {
                doctorPatients[d.UserId] = new HashSet<string>();
            }

            //fill their sets using treatments
            foreach(var t in treatments)
            {
                doctorPatients[t.DoctorId].Add(t.PatientId);
            }

            //build answer acording to doctor data and their set size
            List<DoctorItemViewModel> ans = new List<DoctorItemViewModel>();
            foreach(var d in doctors)
            {
                ans.Add(new DoctorItemViewModel()
                {
                    FullName = d.FullName,
                    Position = d.Position,
                    UserId = d.UserId,
                    PatientsNumber = doctorPatients[d.UserId].Count
                });
            }

            return ans;
        }
    }
}