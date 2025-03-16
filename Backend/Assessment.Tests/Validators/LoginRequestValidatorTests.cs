using Assessment.Application.DTOs;
using Assessment.Application.Validators;
using FluentValidation.TestHelper;

namespace Assessment.Tests.Validators
{
    public class LoginRequestValidatorTests
    {
        private readonly LoginRequestValidator _validator;

        public LoginRequestValidatorTests()
        {
            _validator = new LoginRequestValidator();
        }

        [Fact]
        public void ShouldNotHaveValidationErrors_WhenValidModelIsProvided()
        {
            var model = new LoginRequestDTO("test@example.com", "StrongPass1!");
            var result = _validator.TestValidate(model);
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void ShouldHaveValidationError_WhenEmailIsEmpty()
        {
            var model = new LoginRequestDTO("", "ValidPass1!");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Email is required.");
        }

        [Fact]
        public void ShouldHaveValidationError_WhenEmailIsInvalid()
        {
            var model = new LoginRequestDTO("invalid-email", "ValidPass1!");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Invalid email format.");
        }

        [Fact]
        public void ShouldHaveValidationError_WhenEmailContainsSpaces()
        {
            var model = new LoginRequestDTO("test @email.com", "ValidPass1!");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Email cannot contain spaces.");
        }

        [Fact]
        public void ShouldHaveValidationError_WhenEmailExceedsMaxLength()
        {
            var longEmail = new string('a', 101) + "@example.com";
            var model = new LoginRequestDTO(longEmail, "ValidPass1!");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Email)
                  .WithErrorMessage("Email must be at most 100 characters.");
        }

        [Fact]
        public void ShouldHaveValidationError_WhenPasswordIsEmpty()
        {
            var model = new LoginRequestDTO("test@example.com", "");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password is required.");
        }

        [Fact]
        public void ShouldHaveValidationError_WhenPasswordIsTooShort()
        {
            var model = new LoginRequestDTO("test@example.com", "12345");
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password must be at least 6 characters.");
        }

        [Fact]
        public void ShouldHaveValidationError_WhenPasswordExceedsMaxLength()
        {
            var longPassword = new string('a', 51);
            var model = new LoginRequestDTO("test@example.com", longPassword);
            var result = _validator.TestValidate(model);
            result.ShouldHaveValidationErrorFor(x => x.Password)
                  .WithErrorMessage("Password must be at most 50 characters.");
        }
    }
}