using System;

namespace PhotoCostCalculator
{
    /**
     * Assignment - ReadMe
     *
     * The assignment is a very open one; How would you as a developer improve this program if you
     * were to take over the development of the program?
     *
     * There is unfortunately no documentation about the program, except the code itself...
     * 
     * The other developers are complaining about the fragility of the program, and that the code is too hard to read. How could you help with this?
     * 
     * There is also probably one or two bugs hidden in the code that needs to be fixed!
     * Make any changes you see fit, fix any bugs you find, and think about how to ensure that you don't break anything in the process.
     * 
     * Return the updated project to jonathan@lvira.dev through a file sharing service of your choice
     */
    public class PhotographCostCalculator
    {
        /**
            * Calculates the total cost of a photographer for one day
            *
            * If two jobs are within 90 minutes the photographer gets an
            * extra 10% of the cost of the least expensive of the two jobs
            *           
            * The maximum cost for one day is 5000
            *
            * @param dates - date and time of all photo shoots in one day
            * @return - the total cost of the photographer for that day
            */
        public int GetPhotoCost(Booking[] bookings)
        {
            DateTime intervalStart = bookings[0].Date;
            double totalCost = 0;
            for (int i = 0; i < bookings.Length; i++)
            {
                Booking b = bookings[i];
                DateTime date = bookings[i].Date;
                DateTime previousDate = bookings[i-1].Date;
                double extraCostFactor = 1.1;
                int nextCost = GetPhotoCost(date, b.Job);
                int previousCost = GetPhotoCost(previousDate, b.Job);

                long diffInMillies = date.Ticks - intervalStart.Ticks;
                var minutes = TimeSpan.FromTicks(diffInMillies).TotalMinutes;

                if (minutes < 90)
                {
                    if (nextCost >= previousCost) totalCost += nextCost + ((previousCost*extraCostFactor) - previousCost);
                    else totalCost += nextCost*extraCostFactor;
                }
                else
                {
                    totalCost += nextCost;
                }
            }
            if (totalCost > 5000) totalCost = 5000;
            return Convert.ToInt32(totalCost);
        }

        private Boolean IsCostFreeJob(PhotoJob photoJob)
        {
            if (photoJob == null) return false;
            String jobType = photoJob.GetType();
            return jobType.Equals(JobType.PICKUPKEY.GetType()) ||
                    jobType.Equals(JobType.PLANMEASUREMENT.GetType()) ||
                    jobType.Equals(JobType.TRANSPORT.GetType());
        }

        public int GetPhotoCost(DateTime date, PhotoJob j)
        {
            if (IsCostFreeJob(j)) return 0;
                
            int hour = date.Hour;
            int minute = date.Minute;
            int cost;
            if (hour == 6 && minute >= 0 && minute <= 59) cost = 1300;
            else if (hour == 7 && minute >= 0 && minute <= 59) cost = 1200;
            else if (hour == 8 && minute >= 0 && minute <= 29) cost = 1100;
            else if (((hour > 8 && hour < 17) || (hour == 8 && minute >= 30))) cost = 1000;
            else if (hour < 17) cost = 1000;
            else if (hour == 17 && minute >= 0 && minute <= 59) cost = 1100;
            else if (hour == 18 && minute >= 0 && minute <= 59) cost = 1200;
            else if (hour == 19 && minute >= 0 && minute <= 30) cost = 1300;
            else return 1500;

            if (IsDoubleCostDate(date)) return cost * 2;
            else return cost;
        }

        private Boolean IsDoubleCostDate(DateTime date)
        {
            int year = date.Year;
            int month = date.Month;
            int day = date.Day;
            int hour = date.Hour;
            DayOfWeek dayOfWeek = date.DayOfWeek;
            if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday) return true;
            //if is holiday    
            if (year == 2013)
            {
                if (month == (int)Months.JANUARY && day == 1 ||
                    month == (int)Months.MARCH && (day == 28 || day == 29) ||
                    month == (int)Months.APRIL && (day == 1 || day == 30) ||
                    month == (int)Months.MAY && (day == 1 || day == 8 || day == 9) ||
                    month == (int)Months.JUNE && (day == 5 || day == 6 || day == 21) ||
                    month == (int)Months.NOVEMBER && day == 1 ||
                    month == (int)Months.DECEMBER && (day == 24 || day == 25 || day == 26 || day == 31))
                {
                    return true;
                }
            }
            return false;
        }

        private enum JobType
        {
            TRANSPORT,
            PHOTO,
            VIDEO,
            PLANMEASUREMENT,
            VR,
            DRONE,
            PICKUPKEY
        }

        private enum Months
        {
            JANUARY = 1,
            FEBRUARY = 2,
            MARCH = 3,
            APRIL = 4,
            MAY = 5,
            JUNE = 6,
            JULY = 7,
            AUGUST = 8,
            SEPTEMBER= 9,
            OCTOBER = 10,
            NOVEMBER = 11,
            DECEMBER = 12
        }
    }

    public class Booking
    {
        public PhotoJob Job { get; set; }
        public DateTime Date { get; set; }
        //TODO Use
        public DateTime EndDate { get; set; }
    }

    public interface PhotoJob
    {
        string GetType();
    }

    public class Photo : PhotoJob
    {
        public string GetType()
        {
            return "Photo";
        }
    }

    public class Video : PhotoJob
    {
        public string GetType()
        {
            return "Video";
        }
    }
}