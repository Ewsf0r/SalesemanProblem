using System;
using System.Collections.Generic;

namespace TravellingSalesmanProblem
{
    class Locations
    {
        public class LocationValue
        {
            private Dictionary<string, double> otherLocationDistance = new Dictionary<string, double>();
            public double X { get; set; }
            public double Y { get; set; }
            public bool IsVisited { get; set; }
            public LocationValue(double x, double y)
            {
                X = x;
                Y = y;
                IsVisited = false;
            }
            public void AddDestination(string name, double distance)
            {
                otherLocationDistance.Add(name, distance);
            }
            public double GetOtherLocationDistance(string destinationName) { return otherLocationDistance[destinationName]; }
            public void SetOtherLocationDistance(string destinationName, double value) { otherLocationDistance[destinationName] = value; }
            public double CalculateDitance(double x, double y)
            {
                return Math.Pow(Math.Pow(X - x, 2) + Math.Pow(Y - y, 2), 0.5);
            }
        }
        private IDictionary<string, LocationValue> locations = new Dictionary<string, LocationValue>();
        public void AddLocation(string name, double x, double y)
        {
            locations.Add(name, new LocationValue(x , y));
        }
        public LocationValue this[string LocationName]
        {
            get { return locations[LocationName]; }
            set { locations[LocationName] = value; }
        }
        public Locations(string[] name, double[] x, double[] y)
        {
            for(int i=0; i < name.Length; i++)
            {
                AddLocation(name[i], x[i], y[i]);
                for (int j = 0; j < name.Length; j++)
                {
                    if (j != i)
                        this[name[i]].AddDestination(name[j], this[name[i]].CalculateDitance(x[j], y[j]));
                }
            }
        }
        public struct Result
        {
            public string ResultName { get; set; }
            public double ResultX { get; set; }
            public double ResultY { get; set; }
            public double ResultDistanceToPrevious { get; set; }
            public double ResultTotalDistance { get; set; }
            public Result(string name, double x, double y, double distance, double total)
            {
                ResultName = name;
                ResultX = x;
                ResultY = y;
                ResultDistanceToPrevious = distance;
                ResultTotalDistance = total;
            }
            public void TotalDistanceCalc(Result[] toCount)
            {
                ResultTotalDistance = 0;
                foreach (Result r in toCount)
                {
                    ResultTotalDistance += r.ResultDistanceToPrevious;
                }
            }
        }
        public Result[] BruteForce(string[] name)
        {
            Result[] result = new Result[locations.Count + 1];
            Result[] sub = new Result[locations.Count + 1];
            string lastLocationName = "";
            this[name[0]].IsVisited = true;
            result[0] = new Result(name[0], this[name[0]].X, this[name[0]].Y, 0, 0);
            sub[0] = result[0];
            for (int i = 1; i < locations.Count; i++)
            {
                result[i] = new Result(name[i], this[name[i]].X, this[name[i]].Y, this[name[i]].GetOtherLocationDistance(name[i - 1]), 0);
                this[name[i]].IsVisited = false;
                lastLocationName = name[i];
            }
            result[0].TotalDistanceCalc(result);
            lastLocationName = BruteForceRecursion(result, sub, name, name[0], 1);
            result[locations.Count] = new Result(name[0], this[name[0]].X, this[name[0]].Y,
                this[name[0]].GetOtherLocationDistance(lastLocationName), 0);
            result[0].TotalDistanceCalc(result);
            return result;
        }
        public string BruteForceRecursion(Result[] result, Result[] sub, string[] allLocatonsNames, string previousLocation, int index)
        {
            string lastLocation = "";
            if (index < allLocatonsNames.Length)
            {
                foreach (string name in allLocatonsNames)
                {
                    if (this[name].IsVisited) continue;
                    else
                    {
                        this[name].IsVisited = true;
                        sub[index] = new Result(name, this[name].X, this[name].Y, this[name].GetOtherLocationDistance(previousLocation), 0);
                        lastLocation = BruteForceRecursion(result, sub, allLocatonsNames, name, index + 1);
                        this[name].IsVisited = false;
                    }
                }
            }
            else
            {
                sub[0].TotalDistanceCalc(sub);
                lastLocation = result[index - 1].ResultName;
                if (sub[0].ResultTotalDistance < result[0].ResultTotalDistance)
                    for (int i = 0; i < sub.Length; i++)
                    {
                        result[i] = sub[i];
                    }
            }
            return lastLocation;
        }
        public string ShortestDistanceDestination(string[] allLocatonsNames, string currentLocationName)
        {
            string shortestDistanceDestination = "";
            double shortestDistance = new double();
            bool isAnyDestinationAvailable = false;
            foreach (string name in allLocatonsNames)
            {
                if (!this[name].IsVisited)
                {
                    shortestDistanceDestination = name;
                    shortestDistance = this[currentLocationName].GetOtherLocationDistance(name);
                    this[name].IsVisited = true;
                    isAnyDestinationAvailable = true;
                    break;
                }
            }
            if (isAnyDestinationAvailable)
            {
                foreach (string name in allLocatonsNames)
                {
                    if (!this[name].IsVisited)
                    {
                        if (this[currentLocationName].GetOtherLocationDistance(name) < shortestDistance)
                        {
                            this[shortestDistanceDestination].IsVisited = false;
                            shortestDistanceDestination = name;
                            shortestDistance = this[currentLocationName].GetOtherLocationDistance(name);
                            this[name].IsVisited = true;
                        }
                    }
                }
            }
            return shortestDistanceDestination;
        }
        public Result[] NearestNeighbour(string[] name)
        {
            Result[] result = new Result[locations.Count + 1];
            string lastlocationName = name[0];
            this[name[0]].IsVisited = true;
            result[0] = new Result(name[0], this[name[0]].X, this[name[0]].Y, 0, 0);
            for (int i = 1; i < locations.Count; i++)
            {
                string shortestDistanceDestination = ShortestDistanceDestination(name, lastlocationName);
                result[i] = new Result(shortestDistanceDestination, this[shortestDistanceDestination].X, this[shortestDistanceDestination].Y,
                    this[shortestDistanceDestination].GetOtherLocationDistance(lastlocationName), 0);
                lastlocationName = shortestDistanceDestination;
            }
            result[locations.Count] = new Result(name[0], this[name[0]].X, this[name[0]].Y, 
                this[name[0]].GetOtherLocationDistance(lastlocationName), 0);
            result[0].TotalDistanceCalc(result);
            return result;
        }
    }
}
