﻿namespace Application.DTOs.UserDtos
{
    public class UserDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}
