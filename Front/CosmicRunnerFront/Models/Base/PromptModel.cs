namespace CosmicRunnerFront.Models;

public class PromptModel
{
    public int promptId { get; set; }
    public int promptUserId { get; set; }
    public string promptCategory { get; set; }
    public int promptCategoryId { get; set; }
    public string promptDepartment { get; set; }
    public int promptDepartmentId { get; set; }
    public string promptTitle { get; set; }
    public string promptDescription { get; set; }

    public DateTime promptCreatedAt { get; set; }
    public bool IsSaved { get; set; }
    public int Likes { get; set; }
    public int Dislikes { get; set; }
    public int CurrentUserRating { get; set; }
    public List<PromptComment> Comments { get; set; } = new List<PromptComment>();
}