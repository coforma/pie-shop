var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// In-memory job storage
var jobs = new Dictionary<string, BakeJobInfo>();
var random = new Random();

app.MapPost("/api/v1/bake", (BakeRequest request) =>
{
    var jobId = $"bake_{Guid.NewGuid():N}";
    
    // Simulate 5% failure rate
    var willFail = random.Next(100) < 5;
    
    // Simulate 15-20 minute baking time (accelerated to 15-20 seconds for testing)
    var delaySeconds = random.Next(15, 21);
    var estimatedCompletion = DateTime.UtcNow.AddSeconds(delaySeconds);
    var ovenId = $"oven-{random.Next(1, 6)}";
    
    var job = new BakeJobInfo
    {
        JobId = jobId,
        Status = "IN_PROGRESS",
        OvenId = ovenId,
        EstimatedCompletion = estimatedCompletion,
        Progress = 0,
        WillFail = willFail,
        CompletionTime = estimatedCompletion
    };
    
    jobs[jobId] = job;
    
    // Simulate async baking with progress updates
    _ = Task.Run(async () =>
    {
        var totalSeconds = delaySeconds;
        for (int i = 0; i <= 10; i++)
        {
            await Task.Delay(totalSeconds * 100);
            job.Progress = i * 10;
            
            if (i == 10)
            {
                if (job.WillFail)
                {
                    job.Status = "FAILED";
                    job.ErrorMessage = "Oven malfunction - temperature control failure";
                }
                else
                {
                    job.Status = "COMPLETED";
                    job.Progress = 100;
                }
            }
        }
    });
    
    return Results.Ok(new BakeResponse
    {
        JobId = jobId,
        OvenId = ovenId,
        EstimatedCompletion = estimatedCompletion
    });
});

app.MapGet("/api/v1/jobs/{jobId}", (string jobId) =>
{
    if (!jobs.TryGetValue(jobId, out var job))
    {
        return Results.NotFound(new { error = "Job not found" });
    }
    
    return Results.Ok(new BakeJobStatusResponse
    {
        JobId = job.JobId,
        Status = job.Status,
        OvenId = job.OvenId,
        Progress = job.Progress,
        EstimatedCompletion = job.EstimatedCompletion,
        ErrorMessage = job.ErrorMessage
    });
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "baker" }));

Console.WriteLine("Baker Mock Service starting on port 8082");
Console.WriteLine("Simulating 15-20 second baking with 5% failure rate");
app.Run("http://0.0.0.0:8082");

// Models
record BakeRequest(string PieType, int Temperature, int Duration);
record BakeResponse(string JobId, string OvenId, DateTime EstimatedCompletion);
record BakeJobStatusResponse(string JobId, string Status, string OvenId, int Progress, DateTime? EstimatedCompletion, string? ErrorMessage);

class BakeJobInfo
{
    public string JobId { get; set; } = "";
    public string Status { get; set; } = "";
    public string OvenId { get; set; } = "";
    public int Progress { get; set; }
    public DateTime? EstimatedCompletion { get; set; }
    public DateTime CompletionTime { get; set; }
    public bool WillFail { get; set; }
    public string? ErrorMessage { get; set; }
}
