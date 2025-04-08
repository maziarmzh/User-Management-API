using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private static List<User> Users = new List<User>();

    [HttpGet]
    public IActionResult GetAllUsers()
    {
        return Ok(Users);
    }

    [HttpGet("{id}")]
    public IActionResult GetUserById(int id)
    {
        try
        {
            var user = Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            return Ok(user);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    public IActionResult CreateUser([FromBody] User newUser)
    {
        if (string.IsNullOrWhiteSpace(newUser.Name))
        {
            return BadRequest("Invalid input: Name is required.");
        }

        if (newUser.Email == null || !IsValidEmail(newUser.Email))
        {
            return BadRequest("Invalid input: A valid email is required.");
        }

        var existingUser = Users.FirstOrDefault(user => user.Id == newUser.Id || user.Email == newUser.Email);

        if (existingUser != null)
        {
            var conflictField = existingUser.Id == newUser.Id ? $"Id {newUser.Id}" : $"Email {newUser.Email}";
            return Conflict($"Error: A user with {conflictField} already exists.");
        }

        Users.Add(newUser);
        return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
    }


    private bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }


    //[HttpPut("{id}")]
    //public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
    //{
    //    var user = Users.FirstOrDefault(u => u.Id == id);
    //    if (user == null)
    //    {
    //        return NotFound();
    //    }
    //    user.Name = updatedUser.Name;
    //    user.Email = updatedUser.Email;
    //    return NoContent();
    //}


    [HttpPut("{id}")]
    public IActionResult UpdateUser(int id, [FromBody] User updatedUser)
    {
        // Find the user to update
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        user.Id= id;

        // Validate input
        if (string.IsNullOrWhiteSpace(updatedUser.Name))
        {
            return BadRequest("Invalid input: Name is required.");
        }

        if (string.IsNullOrEmpty(updatedUser.Email) || !IsValidEmail(updatedUser.Email))
        {
            return BadRequest("Invalid input: A valid email is required.");
        }

        // Check for conflicts excluding the current user being updated
        var existingUser = Users.FirstOrDefault(u => u.Email == updatedUser.Email && u.Id != id);
        if (existingUser != null)
        {
            return Conflict($"Error: A user with Email {updatedUser.Email} already exists.");
        }

        // Warn if the ID is being changed
        if (updatedUser.Id != id)
        {
            return BadRequest($"Warning: ID cannot be changed. The current user's ID is {id}.");
        }

        // Update user details
        user.Name = updatedUser.Name;
        user.Email = updatedUser.Email;

        return NoContent();
    }


    [HttpDelete("{id}")]
    public IActionResult DeleteUser(int id)
    {
        var user = Users.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }
        Users.Remove(user);
        return NoContent();
    }
}

public class User
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
}