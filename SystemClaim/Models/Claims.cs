namespace SystemClaim.Models
{
    public class Claims
    {
        public int Id { get; set; }



        public string? WorkerUserId { get; set; }



        public string? Name { get; set; }



        public string? Surname { get; set; }


        public string? Department { get; set; }



        public decimal RatePerJob { get; set; }


        public int NumberOfJobs { get; set; }


        public decimal TotalAmount { get; set; }



        public string? Status { get; set; }



        public string? RejectReason { get; set; }
        public bool ReasonRequired { get; set; } = false;


        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}

