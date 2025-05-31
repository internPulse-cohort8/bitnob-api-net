﻿using InternPulse4.Core.Application.Interfaces.Repositories;
using InternPulse4.Core.Application.Interfaces.Services;
using InternPulse4.Core.Domain.Entities;
using InternPulse4.Models.UserModel;
using InternPulse4.Models;
using static InternPulse4.Models.UserModel.UserRequest;
using System.Security.Claims;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;

namespace InternPulse4.Core.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IHttpContextAccessor httpContext, IConfiguration configuration, IEmailService emailService)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _httpContext = httpContext;
            _configuration = configuration;
            _emailService = emailService;
        }
        public async Task<BaseResponse<UserResponse>> CreateUser(UserRequest request)
        {
            // Check if email is null or empty or whitespace
            if (string.IsNullOrWhiteSpace(request.Email))
            {
                return new BaseResponse<UserResponse>
                {
                    Message = "Email cannot be empty",
                    IsSuccessful = false
                };
            }

            int randomCode = new Random().Next(100000, 999999); // 6-digit token

            if (await _userRepository.ExistsAsync(request.Email))
            {
                return new BaseResponse<UserResponse>
                {
                    Message = "Email already exists!!!",
                    IsSuccessful = false
                };
            }

            if (request.Password != request.ConfirmPassword)
            {
                return new BaseResponse<UserResponse>
                {
                    Message = "Password does not match",
                    IsSuccessful = false
                };
            }

            var user = new User
            {
                Email = request.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateCreated = DateTime.UtcNow,
                IsDeleted = false,
                Role = Domain.Enums.Role.User,
                CreatedBy = "ManualRegistration",
                EmailConfirmationToken = randomCode.ToString(),
                TokenExpiry = DateTime.UtcNow.AddMinutes(15),
                IsEmailConfirmed = false
            };

            var newUser = await _userRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();

            // Send confirmation email
            string subject = "Your Email Confirmation Code";

            string body = $@"
                       Dear {user.FirstName},
                   Thank you for registering with us.

               Your email confirmation code is: {randomCode}

                     Please enter this code to verify your email address.  
                      Note: This code will expire in 10 minutes.

                 If you did not request this registration, please ignore this message.

                        Best regards,  
                     ";

          await _emailService.SendEmailAsync(user.Email, subject, body);


            return new BaseResponse<UserResponse>
            {
                Message = $"Registration successful. Check your email for the confirmation code.",
                IsSuccessful = true,
                Value = new UserResponse
                {
                    Id = user.Id,
                    FullName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    RoleName = user.Role.ToString(),
                }
            };
        }



        /*public async Task<BaseResponse<UserResponse>> GoogleLogin(string tokenId)
        {
            if (!tokenId.Contains("."))
            {
                return new BaseResponse<UserResponse>
                {
                    Message = "Invalid Google token format",
                    IsSuccessful = false
                };
            }

            var validationSettings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] { "237085518851-jofqas7qcl591ba3kqcj09dl177ikbh1.apps.googleusercontent.com" }
            };

            try
            {
                var googlePayload = await GoogleJsonWebSignature.ValidateAsync(tokenId, validationSettings);

                if (googlePayload == null)
                {
                    return new BaseResponse<UserResponse>
                    {
                        Message = "Invalid Google token",
                        IsSuccessful = false
                    };
                }

                var user = await _userRepository.GetAsync(u => u.Email == googlePayload.Email);

                if (user != null)
                {
                    var existingUserResponse = new UserResponse
                    {
                        Id = user.Id,
                        FullName = user.FirstName + " " + user.LastName,
                        Email = user.Email,
                        RoleName = user.Role.ToString()
                    };

                    return new BaseResponse<UserResponse>
                    {
                        Message = "User logged in successfully",
                        IsSuccessful = true,
                        Value = existingUserResponse
                    };
                }
                else if (restaurant)
                {
                    return new BaseResponse<UserResponse>
                    {
                        Message = "A restaurant with this email already exists. Please use a different email for registration.",
                        IsSuccessful = false
                    };
                }
                else
                {
                    

                    user = new User
                    {
                        FirstName = googlePayload.GivenName,
                        LastName = googlePayload.FamilyName,
                        Email = googlePayload.Email,
                        DateCreated = DateTime.UtcNow,
                        IsDeleted = false,
                        Role = Domain.Enums.Role.User,
                        CreatedBy = "GoogleOAuth",
                    };
                    var newUser = await _userRepository.AddAsync(user);
                    await _unitOfWork.SaveAsync();

                    var newUserResponse = new UserResponse
                    {
                        Id = user.Id,
                        FullName = user.FirstName + " " + user.LastName,
                        Email = user.Email,
                        RoleName = user.Role.ToString()
                    };

                    return new BaseResponse<UserResponse>
                    {
                        Message = "Google user created successfully",
                        IsSuccessful = true,
                        Value = newUserResponse
                    };
                }
            }
            catch (InvalidJwtException ex)
            {
                return new BaseResponse<UserResponse>
                {
                    Message = $"JWT validation failed: {ex.Message}",
                    IsSuccessful = false
                };
            }
        }*/

        public async Task<BaseResponse<ICollection<UserResponse>>> GetAllUsers()
        {
            var users = await _userRepository.GetAllAsync();

            return new BaseResponse<ICollection<UserResponse>>
            {
                Message = "List of users",
                IsSuccessful = true,
                Value = users.Select(user => new UserResponse
                {
                    Id = user.Id,
                    FullName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    RoleName = user.Role.ToString(),
                }).ToList(),
            };
        }

        public async Task<BaseResponse<UserResponse>> GetUser(int id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return new BaseResponse<UserResponse>
                {
                    Message = "User not found",
                    IsSuccessful = false
                };
            }
            return new BaseResponse<UserResponse>
            {
                Message = "User successfully found",
                IsSuccessful = true,
                Value = new UserResponse
                {
                    Id = user.Id,
                    FullName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    RoleName = user.Role.ToString(),
                }
            };
        }

        public async Task<BaseResponse> RemoveUser(int id)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return new BaseResponse
                {
                    Message = "User does not exist",
                    IsSuccessful = false
                };
            }

            _userRepository.Remove(user);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "User deleted successfully",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse> UpdateUser(int id, UserRequest request)
        {
            var user = await _userRepository.GetAsync(id);
            if (user == null)
            {
                return new BaseResponse
                {
                    Message = "User does not exist",
                    IsSuccessful = false
                };
            }

            var exists = await _userRepository.ExistsAsync(request.Email, id);
            if (exists)
            {
                return new BaseResponse
                {
                    Message = "Email already exists!!!",
                    IsSuccessful = false
                };
            }

            if (request.Password != request.ConfirmPassword)
            {
                return new BaseResponse
                {
                    Message = "Password does not match",
                    IsSuccessful = false
                };
            }

            var loginUserId = _httpContext.HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier).Value;
            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.Email = request.Email;
            user.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.IsDeleted = false;
            user.Role = Domain.Enums.Role.User;
            user.DateModified = DateTime.UtcNow;
            user.ModifiedBy = loginUserId;
            _userRepository.Update(user);
            await _unitOfWork.SaveAsync();

            return new BaseResponse
            {
                Message = "User updated successfully",
                IsSuccessful = true
            };
        }

        public async Task<BaseResponse<UserResponse>> Login(LoginRequestModel model)
        {
            var user = await _userRepository.GetAsync(model.Email);

            if (user == null)
            {
                return new BaseResponse<UserResponse>
                {
                    Message = "Invalid Credentials",
                    IsSuccessful = false
                };
            }

            if (user.Email == model.Email && BCrypt.Net.BCrypt.Verify(model.Password, user.Password))
            {
                return new BaseResponse<UserResponse>
                {
                    Message = "Login Successful",
                    IsSuccessful = true,
                    Value = new UserResponse
                    {
                        Id = user.Id,
                        FullName = user.FirstName + " " + user.LastName,
                        Email = user.Email,
                        RoleName = user.Role.ToString(),
                    }
                };
            }

            return new BaseResponse<UserResponse>
            {
                Message = "Invalid Credentials",
                IsSuccessful = false
            };
        }


        public async Task<BaseResponse<UserResponse>> ConfirmEmailAsync(ConfirmEmail request)
        {
            var user = await _userRepository.GetAsync(request.Email);
            if (user == null)
            {
                return new BaseResponse<UserResponse>
                {
                    IsSuccessful = false,
                    Message = "User not found"
                };
            }

            if (user.IsEmailConfirmed)
            {
                return new BaseResponse<UserResponse>
                {
                    IsSuccessful = false,
                    Message = "Email already confirmed"
                };
            }

            if (user.EmailConfirmationToken != request.Token)
            {
                return new BaseResponse<UserResponse>
                {
                    IsSuccessful = false,
                    Message = "Invalid confirmation token"
                };
            }

            if (user.TokenExpiry < DateTime.UtcNow)
            {
                return new BaseResponse<UserResponse>
                {
                    IsSuccessful = false,
                    Message = "Token has expired"
                };
            }

            user.IsEmailConfirmed = true;
            user.EmailConfirmationToken = string.Empty;
            user.TokenExpiry = null;

            _userRepository.Update(user);
            await _unitOfWork.SaveAsync();

            return new BaseResponse<UserResponse>
            {
                IsSuccessful = true,
                Message = "Email confirmed successfully",
                Value = new UserResponse
                {
                    Id = user.Id,
                    FullName = user.FirstName + " " + user.LastName,
                    Email = user.Email,
                    RoleName = user.Role.ToString(),
                }
            };


        }

      
    }
}
