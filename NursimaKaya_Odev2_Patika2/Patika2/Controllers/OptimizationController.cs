﻿using Data;
using Data.Uow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Patika2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OptimizationController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        private readonly ILogger<OptimizationController> _logger;

        public OptimizationController(ILogger<OptimizationController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            this.unitOfWork = unitOfWork;
        }

        public class Point
        {
            public Point(decimal x, decimal y)
            {
                X = x;
                Y = y;
            }
            public decimal X { get; set; }
            public decimal Y { get; set; }

        }

        private double GetSquaredDistance(Point p, Point q)
        {
            return Math.Pow((double)(p.X - q.X), 2) + Math.Pow((double)(p.Y - q.Y), 2);
        }

        // assign each point to the closest cluster
        private List<int> AssignPointsToClusters(List<Point> points, List<Point> centroids, int n)
        {
            var assignment = new List<int>(points.Count());
            //double cost = 0;
            for(var i = 0; i < points.Count(); i++)
            {
                var point = points.ElementAt(i);

                // find the closest one among centroids for a point

                double minDist = 0;

                for (var j = 0; j < centroids.Count(); j++)
                {
                    var centroid = centroids.ElementAt(j);

                    double dist = GetSquaredDistance(point, centroid);
                    if (j == 0 || dist < minDist)
                    {
                        minDist = dist;
                        if(assignment.Count() == i + 1)
                            assignment[i] = j;
                        else
                            assignment.Add(j);
                    }
                }
                //cost += minDist;
            }
            return assignment;
        }

        private List<Point> UpdateCentroids(List<Point> points, List<int> assignment, int n)
        {
            // mapping from a cluster to its mean (x, y)
            var centroids = new List<Point>();
            var clusterPopulations = new List<int>();
            for (var i = 0; i < n; i++)
            {
                centroids.Add(new Point(0, 0));
                clusterPopulations.Add(0);
            }
            for (var i = 0; i < points.Count(); i++)
            {
                int clusterNo = assignment[i];
                clusterPopulations[clusterNo]++;
                centroids[clusterNo].X += points.ElementAt(i).X;
                centroids[clusterNo].Y += points.ElementAt(i).Y;
            }
            for (var i = 0; i < n; i++)
            {
                var count = clusterPopulations[i];
                centroids.ElementAt(i).X /= count;
                centroids.ElementAt(i).Y /= count;
            }

            return centroids;
        }

        private List<int> kmeans(List<Point> points, int n)
        {
            // select n initial centroids
            List<Point> centroids = points.Take(n).ToList();

            // assign each point to the closest cluster
            List<int> prevAssignment = AssignPointsToClusters(points, centroids, n);
            
            while (true)
            {
                centroids = UpdateCentroids(points, prevAssignment, n);
                List<int> assignment = AssignPointsToClusters(points, centroids, n);
                if(Enumerable.SequenceEqual(assignment, prevAssignment))
                {
                    break;
                }
                prevAssignment = assignment;
            }

            return prevAssignment;
        }

        [HttpGet("{id}/{n}")]
        public async Task<IActionResult> GetRoutes([FromRoute] long id, [FromRoute] int n)
        {
            var vehicle = await unitOfWork.Vehicle.GetById(id);

            if (vehicle is null)
            {
                return NotFound();
            }

            var containerList = await unitOfWork.Vehicle.GetContainers(id);

            // split into n batches using kmeans algorithm

            var points = new List<Point>();
            foreach (var container in containerList)
            {
                points.Add(new Point(container.Latitude, container.Longitude));
            }

            List<int> assignment = kmeans(points, n);

            List<List<Container>> batches = new List<List<Container>>();
            for(int i = 0; i < n; i++)
            {
                batches.Add(new List<Container>());
            }

            for(int i = 0; i < assignment.Count(); i++)
            {
                var clusterNo = assignment[i];
                batches[clusterNo].Add(containerList.ElementAt(i));
            }

            return Ok(batches);
        }

    }

}
