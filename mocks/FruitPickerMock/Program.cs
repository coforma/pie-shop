var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// In-memory job storage
var jobs = new Dictionary<string, JobInfo>();
var random = new Random();

app.MapPost("/api/v1/pick-fruit", (PickFruitRequest request) =>
{
    var jobId = $"pick_{Guid.NewGuid():N}";
    
    // Simulate 10% failure rate
    var willFail = random.Next(100) < 10;
    
    // Simulate 30-60 second delay
    var delaySeconds = random.Next(30, 61);
    var estimatedCompletion = DateTime.UtcNow.AddSeconds(delaySeconds);
    
    var job = new JobInfo
    {
        JobId = jobId,
        Status = "PENDING",
        EstimatedCompletion = estimatedCompletion,
        WillFail = willFail,
        CompletionTime = estimatedCompletion
    };
    
    jobs[jobId] = job;
    
    // Simulate async processing
    _ = Task.Run(async () =>
    {
        await Task.Delay(delaySeconds * 1000);
        
        if (job.WillFail)
        {
            job.Status = "FAILED";
            job.ErrorMessage = "Fruit picker robot malfunction - low battery";
        }
        else
        {
            job.Status = "COMPLETED";
            job.Fruits = new List<FruitInfo>
            {
                new() { Type = request.FruitType, Quantity = request.Quantity, Quality = request.Quality }
            };
        }
    });
    
    return Results.Ok(new PickFruitResponse
    {
        JobId = jobId,
        EstimatedCompletion = estimatedCompletion
    });
});

app.MapGet("/api/v1/jobs/{jobId}", (string jobId) =>
{
    if (!jobs.TryGetValue(jobId, out var job))
    {
        return Results.NotFound(new { error = "Job not found" });
    }
    
    return Results.Ok(new JobStatusResponse
    {
        JobId = job.JobId,
        Status = job.Status,
        EstimatedCompletion = job.EstimatedCompletion,
        Fruits = job.Fruits,
        ErrorMessage = job.ErrorMessage
    });
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "fruit-picker" }));

Console.WriteLine("Fruit Picker Mock Service starting on port 8081");
Console.WriteLine("Simulating 30-60 second delays with 10% failure rate");
app.Run("http://0.0.0.0:8081");

// Models
record PickFruitRequest(string FruitType, int Quantity, string Quality);
record PickFruitResponse(string JobId, DateTime EstimatedCompletion);
record JobStatusResponse(string JobId, string Status, DateTime? EstimatedCompletion, List<FruitInfo>? Fruits, string? ErrorMessage);
record FruitInfo(string Type, int Quantity, string Quality);

class JobInfo
{
    public string JobId { get; set; } = "";
    public string Status { get; set; } = "";
    public DateTime? EstimatedCompletion { get; set; }
    public DateTime CompletionTime { get; set; }
    public bool WillFail { get; set; }
    public List<FruitInfo>? Fruits { get; set; }
    public string? ErrorMessage { get; set; }
}
