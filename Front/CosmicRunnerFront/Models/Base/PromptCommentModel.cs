using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace CosmicRunnerFront.Models;

public class PromptComment
{
  public int commentId { get; set; }
  public int commentPromptId { get; set; }
  public int commentUserId { get; set; }
  public string commentContent { get; set; }
}
