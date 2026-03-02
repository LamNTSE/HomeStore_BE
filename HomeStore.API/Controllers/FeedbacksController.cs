using System.Security.Claims;
using HomeStore.Domain.DTOs.Feedbacks;
using HomeStore.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ApiExplorerSettings(GroupName = "09. Feedbacks")]
public class FeedbacksController : ControllerBase
{
    private readonly IFeedbackService _feedbackService;

    public FeedbacksController(IFeedbackService feedbackService) => _feedbackService = feedbackService;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private bool IsAdmin => User.IsInRole("Admin");

    /// <summary>Admin: get all feedbacks</summary>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _feedbackService.GetAllFeedbacksAsync();
        return Ok(result);
    }

    /// <summary>Public: get feedbacks of a product (includes rating)</summary>
    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetByProduct(int productId)
    {
        var result = await _feedbackService.GetFeedbacksByProductAsync(productId);
        return Ok(result);
    }

    /// <summary>Customer: get my own feedbacks</summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetMyFeedbacks()
    {
        var result = await _feedbackService.GetMyFeedbacksAsync(UserId);
        return Ok(result);
    }

    /// <summary>Admin/Customer: get single feedback by id</summary>
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _feedbackService.GetFeedbackByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    /// <summary>Customer: submit feedback/rating for a product</summary>
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFeedbackRequest request)
    {
        var result = await _feedbackService.CreateFeedbackAsync(UserId, request);
        return result.Success ? CreatedAtAction(nameof(GetById), new { id = result.Data!.FeedbackId }, result) : BadRequest(result);
    }

    /// <summary>Customer: update own feedback</summary>
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateFeedbackRequest request)
    {
        var result = await _feedbackService.UpdateFeedbackAsync(UserId, id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    /// <summary>Customer: delete own feedback. Admin: delete any feedback.</summary>
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _feedbackService.DeleteFeedbackAsync(UserId, id, IsAdmin);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
