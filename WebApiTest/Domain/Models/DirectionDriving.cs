using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class DirectionDrivingRequest
    {
        public int uqId { get; set; }
        public int fId { get; set; }
        public int tId { get; set; }



        public string key { get; set; }
        public string origin { get; set; } = "";
        public string destination { get; set; } = "";
        public int strategy { get; set; } = 34;
        public string show_fields { get; set; } = "cost";
    }
    public class DirectionDrivingCost
    {
        public string duration { get; set; }
        public string tolls { get; set; }
        public string toll_distance { get; set; }
        public string toll_road { get; set; }
        public string traffic_lights { get; set; }
    }

    public class DirectionDrivingStep
    {

        public string instruction { get; set; }
        public string orientation { get; set; }
        public string step_distance { get; set; }
        public DirectionDrivingCost cost { get; set; }
    }


    public class DirectionDrivingPath
    {

        public string distance { get; set; }
        public string restriction { get; set; }
        public DirectionDrivingCost cost { get; set; }
        public List<DirectionDrivingStep> steps { get; set; }

    }

    public class DirectionDrivingRoute
    {
        public string origin { get; set; }
        public string destination { get; set; }
        public string taxi_cost { get; set; }
        public List<DirectionDrivingPath> paths { get; set; }


    }

    public class DirectionDrivingRes
    {
        public string status { get; set; }
        public string info { get; set; }
        public string infocode { get; set; }
        public string count { get; set; }
        public DirectionDrivingRoute route { get; set; }
    }
}
