using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace LapLich.Models
{
    public class GeneticAlgorithm
    {
        List<Doctor> doctors;
        List<Room> rooms;
        List<Day> days;
        int populationSize; // Kích thước của quần thể
        int generations; // Số lượng thế hệ
        double mutpb; // Xác suất đột biến
        Random random;

        public GeneticAlgorithm(List<Doctor> doctors, List<Room> rooms, List<Day> days,
                                int populationSize = 200, int generations = 2000, double mutpb = 0.2)
        {
            this.doctors = doctors;
            this.rooms = rooms;
            this.days = days;
            this.populationSize = populationSize;
            this.generations = generations;
            this.mutpb = mutpb;
            random = new Random();
        }

        public List<Schedule> CreateIndividual() // Tạo ra một cá thể
        {
            var individual = new List<Schedule>();

            foreach (var day in days)
            {
                //var doctorsAssignedToday = new HashSet<Doctor>();
                foreach (var room in rooms)
                {
                    var doctor = doctors[random.Next(doctors.Count)];
                    individual.Add(new Schedule(doctor, room, day));

                    //var availableDoctors = doctors
                    //    .Where(d => d.AllowedRooms.Contains(room) && !d.DaysOff.Contains(day) && !doctorsAssignedToday.Contains(d))
                    //    .ToList();

                    //if (availableDoctors.Any())
                    //{
                    //    var doctor = availableDoctors[random.Next(availableDoctors.Count)];
                    //    doctorsAssignedToday.Add(doctor);
                    //    individual.Add(new Schedule(doctor, room, day));
                    //}
                    //else
                    //    return null;
                }
            }
            return individual;
        }

        public List<List<Schedule>> CreatePopulation() // Tạo ra một quần thể
        {
            var population = new List<List<Schedule>>();

            for (int i = 0; i < populationSize; i++)
            {
                var individual = CreateIndividual();
                while (population.Any(p => p.SequenceEqual(individual)) || individual == null)
                    individual = CreateIndividual();
                population.Add(individual);
            }
            return population;
        }

        public int Fitness(List<Schedule> individual)
        {
            int violations = 0; // Vi phạm
            var doctorWorkCount = new Dictionary<int, int>();
            var doctorsAssignedToday = new Dictionary<int, HashSet<Day>>();

            foreach (var schedule in individual)
            {
                var doctor = schedule.Doctor;
                var day = schedule.Day;
                var room = schedule.Room;

                // Không xếp bác sĩ nghỉ phép, công tác phải trực
                if (doctor.DaysOff.Contains(day))
                    violations += 1000;

                // Mỗi bác sĩ chỉ được phép làm việc ở một số phòng nhất định
                if (!doctor.AllowedRooms.Contains(room))
                    violations += 1000;

                // Tại một thời điểm, một bác sĩ không được phép trực nhiều phòng
                if (!doctorsAssignedToday.ContainsKey(doctor.DoctorID))
                    doctorsAssignedToday[doctor.DoctorID] = new HashSet<Day>();

                if (!doctorsAssignedToday[doctor.DoctorID].Add(day))
                    violations += 1000;

                // Mỗi ngày, mỗi phòng đều phải có một bác sĩ trực
            }

            // Khoảng cách giữa 2 lần trực của bác sĩ càng xa nhau càng tốt
            foreach (var doctorGroup in individual.GroupBy(s => s.Doctor))
            {
                var sortedSchedules = doctorGroup.OrderBy(s => s.Day.Date).ToList();
                for (int i = 1; i < sortedSchedules.Count; i++)
                {
                    var dayDifference = sortedSchedules[i].Day.DayID - sortedSchedules[i - 1].Day.DayID;
                    int daysInMonth = DateTime.DaysInMonth(sortedSchedules[i].Day.Date.Year, sortedSchedules[i].Day.Date.Month);
                    violations += Convert.ToInt32(daysInMonth / (dayDifference != 0 ? dayDifference : 1));
                }

                // Đếm số lần trực của bác sĩ
                foreach (var schedule in doctorGroup)
                {
                    if (!doctorWorkCount.ContainsKey(schedule.Doctor.DoctorID))
                        doctorWorkCount[schedule.Doctor.DoctorID] = 0;
                    doctorWorkCount[schedule.Doctor.DoctorID]++;
                }
            }

            // Tổng thời gian trực của các bác sĩ phải tương đương nhau
            int avgWorkTime = Convert.ToInt32(doctorWorkCount.Values.Average());
            foreach (var doctor in doctorWorkCount)
                violations += Math.Abs(doctor.Value - avgWorkTime);

            return violations;
        }

        public List<List<Schedule>> Selection(List<List<Schedule>> population) // Chọn lọc
        {
            var sortedPopulation = population.OrderBy(individual => Fitness(individual)).ToList();
            var bestHalf = sortedPopulation.Take(population.Count / 2).ToList();
            return bestHalf;
        }

        public (List<Schedule>, List<Schedule>) Crossover(List<Schedule> parent1, List<Schedule> parent2) // Giao phối
        {
            int cutPoint = random.Next(1, Math.Min(parent1.Count, parent2.Count));

            List<Schedule> child1 = parent1.Take(cutPoint).ToList();
            child1.AddRange(parent2.Skip(cutPoint));

            List<Schedule> child2 = parent2.Take(cutPoint).ToList();
            child2.AddRange(parent1.Skip(cutPoint));

            return (child1, child2);
        }

        public List<Schedule> Mutate(List<Schedule> individual) // Đột biến
        {
            if (random.NextDouble() < mutpb)
            {
                var doctor = doctors[random.Next(doctors.Count)];
                int idx = random.Next(individual.Count);
                individual[idx] = new Schedule(doctor, individual[idx].Room, individual[idx].Day);
            }
            return individual;
        }

        public List<Schedule> Run()
        {
            var population = CreatePopulation();
            for (int generation = 0; generation < generations; generation++)
            {
                var fitnessValues = population.Select(individual => Fitness(individual)).ToList();
                Debug.WriteLine($"Generation {generation + 1}: Best Fitness = {fitnessValues.Min()}");

                if (fitnessValues.Min() == 0)
                {
                    Debug.WriteLine("Optimal solution found!");
                    break;
                }

                population = Selection(population);
                var offspring = new List<List<Schedule>>();
                while (offspring.Count < populationSize)
                {
                    var parent1 = population[random.Next(population.Count)];
                    var parent2 = population[random.Next(population.Count)];
                    var (child1, child2) = Crossover(parent1, parent2);
                    offspring.Add(Mutate(child1));
                    offspring.Add(Mutate(child2));
                }
                population = offspring;
            }
            List<Schedule> bestSchedule = population.OrderBy(individual => Fitness(individual)).First();
            return bestSchedule;
        }
    }
}