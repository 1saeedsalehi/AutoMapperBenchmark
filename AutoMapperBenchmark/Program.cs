using AutoMapper;
using AutoMapper.Configuration;
using AutoMapper.QueryableExtensions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public class CustomersDbContext : DbContext
    {
        public CustomersDbContext()
            : base(new SqlConnection("Data Source=.;Initial Catalog=CustomersDb;Integrated Security=True"), contextOwnsConnection: true)
        {

        }

        public virtual DbSet<Customer> Customers { get; set; }
    }
    public class Customer
    {
        public virtual int Id { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

    }
    public class CustomerDto
    {
        public virtual int Id { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string LastName { get; set; }

    }
    public class AutoMapperTest
    {
        CustomersDbContext context;
        List<Customer> customers = new List<Customer>();
        public AutoMapperTest()
        {
            var cfg = new MapperConfigurationExpression();
            cfg.CreateMap<Customer, CustomerDto>();

            Mapper.Initialize(cfg);
            for (int i = 0; i < 1000; i++)
            {
                customers.Add(new Customer
                {
                    FirstName = i.ToString(),
                    LastName = i.ToString(),
                    Id = i,
                });
            }

            context = new CustomersDbContext();
        }

        [Benchmark]
        public List<CustomerDto> CustomerToCustomerDtoInMemory()
        {
            var ret = new List<CustomerDto>();
            foreach (var customer in customers)
            {
                ret.Add(Mapper.Map<Customer, CustomerDto>(customer));
            }
            return ret;
        }

        [Benchmark]
        public List<CustomerDto> CustomerToCustomerDtoUsingProjectTo()
        {

            return context.Customers.ProjectTo<CustomerDto>().ToList();
        }
        [Benchmark]
        public List<CustomerDto> CustomerToCustomerDtoUsingCustomMapping()
        {
            var ret = new List<CustomerDto>();
            var allCustomers = context.Customers;
            foreach (var customer in allCustomers)
            {
                ret.Add(Mapper.Map<Customer, CustomerDto>(customer));
            }
            return ret;
        }


    }

    public class Program
    {


        static void Main(string[] args)
        {
            
            var summary = BenchmarkRunner.Run<AutoMapperTest>();

        }
    }
}
